using System;
using System.Collections.Generic;
using System.Text;
using Crownwood.Magic.Controls;
using Phoenix.Runtime;
using Phoenix.Gui.Controls;

namespace Phoenix
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SettingsCategoryAttribute : RuntimeAttribute
    {
        private string title;

        public SettingsCategoryAttribute(string title)
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
                CategoryData data = new CategoryData(title, (System.Windows.Forms.Control)target);
                Core.GuiThread.AddSettingsPage(data);

                return String.Format("Settings category '{0}' added.", title);
            }
            else
            {
                throw new RuntimeException("SettingsCategoryAttribute can be used only on types inherited from System.Windows.Forms.Control.");
            }
        }
    }
}
