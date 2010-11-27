using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Runtime;

namespace Phoenix
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InfoGroupAttribute : RuntimeAttribute
    {
        private string title;

        public InfoGroupAttribute(string title)
        {
            this.title = title;
        }

        public string Title
        {
            get { return title; }
        }

        protected override string Register(System.Reflection.MemberInfo mi, object target)
        {
            if (target.GetType().IsSubclassOf(typeof(System.Windows.Forms.Control)))
            {
                Core.GuiThread.AddInfoGroup((System.Windows.Forms.Control)target, title);

                return String.Format("Info group '{0}' added.", title);
            }
            else
            {
                throw new RuntimeException("InfoGroupAttribute can be used only on types inherited from System.Windows.Forms.Control.");
            }
        }
    }
}
