using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Security.Permissions;

namespace Phoenix.Collections
{
    static class SynchronizedTypeBuilder
    {
        class CachedType
        {
            public readonly object SyncRoot = new object();
            public Type Type;
        }

        private static readonly MethodInfo MonitorEnterMethod = typeof(System.Threading.Monitor).GetMethod("Enter", new Type[] { typeof(Object) });
        private static readonly MethodInfo MonitorExitMethod = typeof(System.Threading.Monitor).GetMethod("Exit", new Type[] { typeof(Object) });

        private static readonly Dictionary<Type, CachedType> typeCache = new Dictionary<Type, CachedType>();

        /// <summary>
        /// Deletes all created types from cache.
        /// </summary>
        public static void ClearCache()
        {
            lock (typeCache) {
                typeCache.Clear();
            }
        }

        /// <summary>
        /// Creates class inherited from type, with all virtual methods overriden.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        /// <remarks>Created types are cached so they are built only once.</remarks>
        public static Type Get(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            if (sourceType.IsSealed)
                throw new ArgumentException("Cannot create synchronized wrapper around sealed type (" + sourceType.Name + ").", "sourceType");

            if (sourceType.ContainsGenericParameters)
                throw new NotSupportedException("Type " + sourceType.Name + " contains generic parameters.");

            CachedType cachedType = null;

            // Look for type in cache
            lock (typeCache) {
                if (!typeCache.TryGetValue(sourceType, out cachedType)) {
                    cachedType = new CachedType();
                    typeCache.Add(sourceType, cachedType);
                }
            }

            Debug.Assert(cachedType != null);

            lock (cachedType.SyncRoot) {
                // If type wasn't created yet build it
                if (cachedType.Type == null) {
                    cachedType.Type = BuildType(sourceType);
                }

                return cachedType.Type;
            }
        }

        private static Type BuildType(Type sourceType)
        {
            AssemblyName asmName = new AssemblyName("Sync" + sourceType.Name);
            AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("SynchronizedTypeModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(asmName.Name, TypeAttributes.Public | TypeAttributes.Class, sourceType);

            // Find SyncRoot property
            PropertyInfo syncRootProperty = sourceType.GetProperty("SyncRoot", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (syncRootProperty == null)
                throw new ArgumentException("Base type doesn't contain SyncRoot property.");

            Debug.WriteLineIf(syncRootProperty.CanWrite, "SyncRoot property has set accessor, which can lead to synchronization failure.", "Warning");

            MethodInfo syncRootGetter = syncRootProperty.GetGetMethod();
            if (syncRootGetter == null)
                throw new ArgumentException("Base type doesn't contain SyncRoot get accessor.");

            // Define constructors
            ConstructorInfo[] constructors = sourceType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (ConstructorInfo ci in constructors) {
                Type[] paramTypes = ConvertParamsToTypes(ci.GetParameters());
                ConstructorBuilder ctorBuilder = typeBuilder.DefineConstructor(ci.Attributes, ci.CallingConvention, paramTypes);
                ILGenerator IL = ctorBuilder.GetILGenerator();

                // Push 'this' to stack
                IL.Emit(OpCodes.Ldarg_0);
                // Push parameters to stack
                for (short i = 1; i <= paramTypes.Length; i++) {
                    IL.Emit(OpCodes.Ldarg, i);
                }

                // Call base constructor
                IL.Emit(OpCodes.Call, ci);
                IL.Emit(OpCodes.Ret);
            }

            // Synchronize methods
            MethodInfo[] methods = sourceType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo mi in methods) {
                if (!mi.IsSpecialName && IsMethodOverridable(mi)) {
                    SynchronizeAttribute syncAttr = (SynchronizeAttribute)Attribute.GetCustomAttribute(mi, typeof(SynchronizeAttribute));

                    if (syncAttr == null || syncAttr.Synchronize) {
                        // Synchronize method
                        SynchronizeMethod(syncRootGetter, typeBuilder, mi);
                    }
                }
            }

            // Synchronize properties
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (PropertyInfo pi in properties) {
                SynchronizeAttribute syncAttr = (SynchronizeAttribute)Attribute.GetCustomAttribute(pi, typeof(SynchronizeAttribute));

                // Special case
                if (pi.Name == "IsSynchronized" && pi.CanRead && !pi.CanWrite && pi.GetIndexParameters().Length == 0) {
                    MethodInfo getMI = pi.GetGetMethod(true);

                    if (getMI != null && IsMethodOverridable(getMI)) {
                        MethodBuilder methodBuilder = typeBuilder.DefineMethod(getMI.Name, getMI.Attributes, getMI.CallingConvention, getMI.ReturnType, Type.EmptyTypes);

                        ILGenerator IL = methodBuilder.GetILGenerator();
                        IL.Emit(OpCodes.Ldc_I4_1);
                        IL.Emit(OpCodes.Ret);

                        typeBuilder.DefineMethodOverride(methodBuilder, getMI);
                    }

                    continue;
                }

                if (syncAttr == null || syncAttr.Synchronize) {
                    MethodInfo getMI = pi.GetGetMethod(true);
                    MethodInfo setMI = pi.GetSetMethod(true);

                    if ((getMI == null || IsMethodOverridable(getMI)) && (setMI == null || IsMethodOverridable(setMI))) {
                        if (getMI == syncRootGetter) {
                            Debug.Print("SyncRoot property in base type is virtual and not marked with SynchronizeAttribute(false). Ignoring..");
                            continue;
                        }

                        /*
                        // Redefinition of property is not needed,
                        // because base property methods are virtual and called late-bound.

                        //  Create parameters array
                        Type[] paramTypes = ConvertParamsToTypes(pi.GetIndexParameters());
                        PropertyBuilder propBuilder = typeBuilder.DefineProperty(pi.Name, pi.Attributes, pi.PropertyType, paramTypes);
                        */

                        // Synchronize methods
                        if (getMI != null) {
                            MethodBuilder getBuilder = SynchronizeMethod(syncRootGetter, typeBuilder, getMI);
                            // propBuilder.SetGetMethod(getBuilder);
                        }

                        if (setMI != null) {
                            MethodBuilder setBuilder = SynchronizeMethod(syncRootGetter, typeBuilder, setMI);
                            // propBuilder.SetSetMethod(setBuilder);
                        }
                    }
                }
            }

            // Create type
            new ReflectionPermission(PermissionState.Unrestricted).Demand();
            return typeBuilder.CreateType();
        }

        private static bool IsMethodOverridable(MethodInfo mi)
        {
            return mi.IsVirtual && !mi.IsFinal && mi.DeclaringType != typeof(object) && mi.Name != "Finalizer";
        }

        /// <summary>
        /// Converts array of ParameterInfos to array of Types.
        /// </summary>
        /// <param name="paramInfos">Source parameter imfos array.</param>
        /// <returns>New array of parameter types.</returns>
        private static Type[] ConvertParamsToTypes(ParameterInfo[] paramInfos)
        {
            Type[] paramTypes = new Type[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; i++) {
                paramTypes[i] = paramInfos[i].ParameterType;
            }
            return paramTypes;
        }

        private static MethodBuilder SynchronizeMethod(MethodInfo syncRootGetter, TypeBuilder typeBuilder, MethodInfo sourceMethod)
        {
            if (sourceMethod.ContainsGenericParameters)
                throw new NotSupportedException("Method " + sourceMethod.Name + " contains generic parameters.");

            Type[] paramTypes = ConvertParamsToTypes(sourceMethod.GetParameters());

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(sourceMethod.Name, sourceMethod.Attributes, sourceMethod.CallingConvention, sourceMethod.ReturnType, paramTypes);

            ILGenerator IL = methodBuilder.GetILGenerator();
            LocalBuilder syncRoot = IL.DeclareLocal(typeof(Object));

            Label returnLabel = IL.DefineLabel();
            LocalBuilder returnValue = null;

            if (sourceMethod.ReturnType != null && sourceMethod.ReturnType != typeof(void)) {
                returnValue = IL.DeclareLocal(sourceMethod.ReturnType);
            }

            IL.BeginExceptionBlock();

            // Get synchronization object
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Callvirt, syncRootGetter);
            IL.Emit(OpCodes.Stloc, syncRoot);

            // Lock
            IL.Emit(OpCodes.Ldloc, syncRoot);
            IL.Emit(OpCodes.Call, MonitorEnterMethod);

            // Push 'this' to stack
            IL.Emit(OpCodes.Ldarg_0);
            // Push parameters to stack
            for (short i = 1; i <= paramTypes.Length; i++) {
                IL.Emit(OpCodes.Ldarg, i);
            }

            // Call base method
            IL.Emit(OpCodes.Call, sourceMethod);

            // Store return value, if any
            if (returnValue != null)
                IL.Emit(OpCodes.Stloc, returnValue);

            IL.Emit(OpCodes.Leave_S, returnLabel);

            IL.BeginFinallyBlock();

            // Unlock
            IL.Emit(OpCodes.Ldloc, syncRoot);
            IL.Emit(OpCodes.Call, MonitorExitMethod);

            IL.EndExceptionBlock();

            IL.MarkLabel(returnLabel);

            // Return return value, if exists
            if (returnValue != null)
                IL.Emit(OpCodes.Ldloc, returnValue);
            IL.Emit(OpCodes.Ret);

            // Override method
            typeBuilder.DefineMethodOverride(methodBuilder, sourceMethod);

            return methodBuilder;
        }
    }
}
