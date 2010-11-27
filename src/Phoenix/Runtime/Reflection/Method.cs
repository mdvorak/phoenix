using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace Phoenix.Runtime.Reflection
{
    public class Method : IAssemblyObject
    {
        private MethodInfo method;
        private Object target;
        private InputParameter[] parameters;
        private int minParamCount;
        private int maxParamCount;

        public Method(MethodInfo method, Object target)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (!method.IsStatic && target == null)
                throw new ArgumentNullException("target", "Target cannot be null when non-static method specified.");

            if (method.IsConstructor)
                throw new ArgumentException("Cannot use class constructor as method.", "method");

            this.method = method;
            this.target = target;

            ParameterInfo[] paramList = method.GetParameters();
            parameters = new InputParameter[paramList.Length];

            for (int i = 0; i < paramList.Length; i++)
            {
                parameters[i] = new InputParameter(paramList[i]);

                if (!parameters[i].IsOptional && !parameters[i].IsParamArray)
                    minParamCount++;

                maxParamCount++;
            }

            if (parameters.Length > 0 && parameters[parameters.Length - 1].IsParamArray)
                maxParamCount = Int32.MaxValue;
        }

        public MethodInfo MethodInfo
        {
            get { return method; }
        }

        public Object Target
        {
            get { return target; }
        }

        public Assembly Assembly
        {
            get { return method.DeclaringType.Assembly; }
        }

        public virtual string Name
        {
            get { return method.Name; }
        }

        public InputParameter[] Parameters
        {
            get { return parameters; }
        }

        public int MinParameterCount
        {
            get { return minParamCount; }
        }

        public int MaxParameterCount
        {
            get { return maxParamCount; }
        }

        public object Invoke(ParameterData[] parametersData)
        {
            if (parametersData.Length < minParamCount || parametersData.Length > maxParamCount)
                throw new ParameterException("Invalid number of paramters.");

            // Parameters in exact types passed to Invoke method
            object[] valuesArray = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (i < parametersData.Length)
                {
                    if (parametersData.Length < parameters.Length - 1)
                    {
                        valuesArray[i] = parametersData[i].ConvertTo(parameters[i]);
                    }
                    else
                    {
                        if (parameters[i].IsParamArray)
                        {
                            Debug.Assert(i == parameters.Length - 1);

                            int variableArgumentsCount = parametersData.Length - i;

                            Type argumentsType = parameters[i].Type.GetElementType();
                            TypeClass argumentsClass = TypeHelper.GetTypeClass(argumentsType);

                            IList variableArgs = Array.CreateInstance(argumentsType, variableArgumentsCount);

                            for (int vi = 0; vi < variableArgumentsCount; vi++)
                            {
                                variableArgs[vi] = parametersData[i + vi].ConvertTo(argumentsType, argumentsClass);
                            }

                            valuesArray[i] = variableArgs;
                        }
                        else
                        {
                            valuesArray[i] = parametersData[i].ConvertTo(parameters[i]);
                        }
                    }
                }
                else
                {
                    // Parameter is not specified
                    if (parameters[i].IsParamArray)
                    {
                        valuesArray[i] = Array.CreateInstance(parameters[i].Type.GetElementType(), 0);
                    }
                    else
                    {
                        Debug.Assert(parameters[i].IsOptional, "Parameter is not optional. Internal error.");
                        valuesArray[i] = Missing.Value;
                    }
                }
            }

            return method.Invoke(target, valuesArray);
        }

        public override string ToString()
        {
            return String.Format("Name=\"{0}\" Method=\"{1}.{2}()\"", Name, MethodInfo.DeclaringType, MethodInfo.Name);
        }
    }
}
