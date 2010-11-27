using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Gui.Pages
{
    public partial class InfoPage : UserControl, IAssemblyObjectList
    {
        #region Group class

        private class Group : IAssemblyObject
        {
            public readonly string Title;
            public readonly Control Control;
            public readonly Control GroupControl;

            public Group(Control control, string title)
            {
                this.Title = title;
                this.Control = control;
                this.GroupControl = new GroupBox();

                GroupControl.Text = title;
                GroupControl.Size = new Size(64, 64);
                GroupControl.Height = GroupControl.Height - GroupControl.DisplayRectangle.Height + control.Height;

                control.Dock = DockStyle.Fill;
                GroupControl.Controls.Add(control);
            }

            public System.Reflection.Assembly Assembly
            {
                get { return Control.GetType().Assembly; }
            }
        }

        #endregion

        private List<Group> groups = new List<Group>();

        public InfoPage()
        {
            InitializeComponent();

            groupsPanel.Location = new Point();
            groupsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupsPanel.AutoSize = true;
        }

        public void AddGroup(Control control, string title)
        {
            Group group = new Group(control, title);
            RuntimeCore.AddAssemblyObject(group, this);
            groups.Add(group);
            group.GroupControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupsPanel.Controls.Add(group.GroupControl);

            SortGroups();
        }

        public void RemoveGroup(Control control)
        {
            Group group = null;
            foreach (Group g in groups)
            {
                if (g.Control == control)
                {
                    group = g;
                    break;
                }
            }

            if (group != null)
            {
                groupsPanel.Controls.Remove(group.GroupControl);
                groups.Remove(group);
                RuntimeCore.RemoveAssemblyObject(group);

                SortGroups();
            }
        }

        void IAssemblyObjectList.Remove(IAssemblyObject obj)
        {
            Group g = obj as Group;
            if (g != null)
            {
                groupsPanel.Controls.Remove(g.GroupControl);
                groups.Remove(g);

                SortGroups();
            }
        }

        public void SortGroups()
        {
            groupsPanel.SuspendLayout();

            Point pt = new Point(3, 0);

            foreach (Control c in groupsPanel.Controls)
            {
                c.Location = pt;
                c.Width = groupsPanel.ClientSize.Width - 6;
                pt.Offset(0, c.Height + 3);
            }

            groupsPanel.ResumeLayout();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            groupsPanel.Width = this.ClientSize.Width;

            base.OnSizeChanged(e);
        }
    }
}
