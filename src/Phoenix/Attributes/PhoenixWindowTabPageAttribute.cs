using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Drawing;
using System.IO;
using Crownwood.Magic.Controls;
using Phoenix.Runtime;

namespace Phoenix
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PhoenixWindowTabPageAttribute : RuntimeAttribute
    {
        private string title;
        private int position;
        private string icon;
        private bool titleIsResource;

        public PhoenixWindowTabPageAttribute(string title)
        {
            this.title = title;
            this.position = -1;
            this.icon = null;
            this.titleIsResource = false;
        }

        /// <summary>
        /// Gets tab title.
        /// </summary>
        public string Title
        {
            get { return title; }
        }

        /// <summary>
        /// Gets or sets position of tab in window. Less than zero mean last. Default value is -1.
        /// </summary>
        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets tab icon. It could be both resource or file.
        /// </summary>
        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        /// <summary>
        /// Gets or sets whether title string is path to resource, or it is title itself.
        /// </summary>
        public bool TitleIsResource
        {
            get { return titleIsResource; }
            set { titleIsResource = value; }
        }

        private string TitleInternal
        {
            get
            {
                if (titleIsResource)
                {
                    Regex typeRegex = new Regex(@"\A[a-zA-Z0-9_.]+\z");
                    if (typeRegex.IsMatch(title))
                    {
                        string containingType = title.Remove(title.LastIndexOf('.'));
                        Type type = Type.GetType(containingType);

                        if (type != null)
                        {
                            string name = title.Substring(title.LastIndexOf('.') + 1);

                            PropertyInfo pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            if (pi != null)
                            {
                                string realTitle = pi.GetValue(null, new object[0]) as string;
                                if (realTitle != null) return realTitle;
                            }

                            FieldInfo fi = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                            if (fi != null)
                            {
                                string realTitle = fi.GetValue(null) as string;
                                if (realTitle != null) return realTitle;
                            }
                        }
                    }
                }

                return title;
            }
        }

        private Icon FindIcon()
        {
            if (icon == null || icon.Length == 0)
                return null;

            Regex typeRegex = new Regex(@"\A[a-zA-Z0-9_.]+\z");
            if (typeRegex.IsMatch(icon))
            {
                // Could be object
                string parentType = icon.Remove(icon.LastIndexOf('.'));
                Type type = Type.GetType(parentType, false);

                Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                int i = 0;
                while (type == null && i < loadedAssemblies.Length)
                {
                    type = loadedAssemblies[i++].GetType(parentType);
                }

                if (type != null)
                {
                    string name = icon.Substring(icon.LastIndexOf('.') + 1);

                    PropertyInfo pi = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (pi != null)
                    {
                        Icon iconObject = pi.GetValue(null, new object[0]) as Icon;
                        if (iconObject != null) return iconObject;
                    }

                    FieldInfo fi = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if (fi != null)
                    {
                        Icon iconObject = fi.GetValue(null) as Icon;
                        if (iconObject != null) return iconObject;
                    }
                }
            }

            // Try to find it as file
            if (File.Exists(icon))
                return new Icon(icon);

            DirectoryInfo phoenixDir = new DirectoryInfo(Core.Directory);

            if (File.Exists(Path.Combine(phoenixDir.FullName, icon)))
                return new Icon(Path.Combine(phoenixDir.FullName, icon));

            DirectoryInfo[] dirs = phoenixDir.GetDirectories();
            foreach (DirectoryInfo d in dirs)
            {
                if (File.Exists(Path.Combine(d.FullName, icon)))
                    return new Icon(Path.Combine(d.FullName, icon));
            }

            return null;
        }

        protected override string Register(MemberInfo mi, object target)
        {
            if (target.GetType().IsSubclassOf(typeof(System.Windows.Forms.Control)))
            {
                Icon iconObject = FindIcon();
                string realTitle = TitleInternal;
                if (realTitle.Length > 12) realTitle = realTitle.Remove(12);

                TabPage page = new TabPage(realTitle, (System.Windows.Forms.Control)target, iconObject);
                Core.GuiThread.InsertTab(position, page);

                return String.Format("Window tab '{0}' added.", title);
            }
            else
            {
                throw new RuntimeException("PhoenixWindowTabPageAttribute can be used only on types inherited from System.Windows.Forms.Control.");
            }
        }
    }
}
