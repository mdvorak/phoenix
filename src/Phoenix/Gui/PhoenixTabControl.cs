using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Crownwood.Magic;
using Crownwood.Magic.Docking;
using Crownwood.Magic.Menus;
using Phoenix.Runtime;

namespace Phoenix.Gui
{
    [ToolboxBitmap(typeof(TabControl))]
    class PhoenixTabControl : Control, IAssemblyObjectList
    {
        class TabPageInfo : IAssemblyObject
        {
            private Crownwood.Magic.Controls.TabPage page;
            private int index;
            public event EventHandler IndexChanged;

            public TabPageInfo(int position, Crownwood.Magic.Controls.TabPage page)
            {
                this.page = page;
                index = position < 0 ? int.MaxValue : position;
            }

            public Crownwood.Magic.Controls.TabPage TabPage
            {
                get { return page; }
            }

            public int Index
            {
                get { return index; }
                set
                {
                    if (value != index) {
                        index = value;
                        SyncEvent.Invoke(IndexChanged, this, EventArgs.Empty);
                    }
                }
            }

            public System.Reflection.Assembly Assembly
            {
                get { return page.Control.GetType().Assembly; }
            }

            public static int Compare(TabPageInfo tr1, TabPageInfo tr2)
            {
                return tr1.index.CompareTo(tr2.index);
            }
        }

        private Crownwood.Magic.Controls.TabControl tabControl;
        private List<TabPageInfo> tabList;
        private List<string> hiddenTabList;

        private PopupMenu popupMenu;
        private MenuCommand shrinkMenuCommand;
        private MenuCommand multilineMenuCommand;
        private MenuCommand hideTextMenuCommand;

        public PhoenixTabControl()
        {
            tabList = new List<TabPageInfo>();
            hiddenTabList = new List<string>();

            Config.Profile.InternalSettings.Loaded += new EventHandler(Settings_Loaded);
            Config.Profile.InternalSettings.Saving += new EventHandler(Settings_Saving);

            tabControl = new Crownwood.Magic.Controls.TabControl();
            tabControl.Style = Crownwood.Magic.Common.VisualStyle.IDE;
            tabControl.Appearance = Crownwood.Magic.Controls.TabControl.VisualAppearance.MultiDocument;
            tabControl.IDEPixelArea = false;
            tabControl.IDEPixelBorder = false;
            tabControl.Dock = DockStyle.Fill;
            tabControl.ClosePressed += new EventHandler(tabControl_ClosePressed);

            popupMenu = new PopupMenu();
            popupMenu.Animate = Animate.System;
            popupMenu.AnimateStyle = Animation.Blend;
            popupMenu.Style = Crownwood.Magic.Common.VisualStyle.IDE;

            shrinkMenuCommand = new MenuCommand("Shrink Labels");
            shrinkMenuCommand.Click += new EventHandler(shrinkMenuCommand_Click);

            multilineMenuCommand = new MenuCommand("Multiline");
            multilineMenuCommand.Click += new EventHandler(multilineMenuCommand_Click);

            hideTextMenuCommand = new MenuCommand("Hide Text");
            hideTextMenuCommand.Click += new EventHandler(hideTextMenuCommand_Click);

            tabControl.ContextPopupMenu = popupMenu;

            Controls.Add(tabControl);

            UpdateTabs();
        }

        public bool HideText
        {
            get { return tabControl.SelectedTextOnly; }
            set
            {
                hideTextMenuCommand.Checked = value;
                tabControl.SelectedTextOnly = value;
            }
        }

        void Settings_Loaded(object sender, EventArgs e)
        {
            hiddenTabList.Clear();

            string text = Config.Profile.InternalSettings.GetElement("", "Config", "Window", "HiddenTabs");
            hiddenTabList.AddRange(text.Split(';'));

            shrinkMenuCommand.Checked = Config.Profile.InternalSettings.GetAttribute(false, "ShrinkPages", "Config", "Window");
            multilineMenuCommand.Checked = Config.Profile.InternalSettings.GetAttribute(false, "MultiLine", "Config", "Window");

            HideText = Config.Profile.InternalSettings.GetAttribute(false, "TextOnly", "Config", "Window");

            UpdateTabs();
        }

        void Settings_Saving(object sender, EventArgs e)
        {
            string text = "";
            for (int i = 0; i < hiddenTabList.Count; i++) {
                if (hiddenTabList[i].Length > 0) {
                    text += hiddenTabList[i] + ";";
                }
            }
            if (text.EndsWith(";"))
                text.Remove(text.Length - 1);

            Config.Profile.InternalSettings.SetElement(text, "Config", "Window", "HiddenTabs");

            Config.Profile.InternalSettings.SetAttribute(tabControl.ShrinkPagesToFit, "ShrinkPages", "Config", "Window");
            Config.Profile.InternalSettings.SetAttribute(tabControl.Multiline, "MultiLine", "Config", "Window");

            Config.Profile.InternalSettings.SetAttribute(HideText, "TextOnly", "Config", "Window");
        }

        void shrinkMenuCommand_Click(object sender, EventArgs e)
        {
            shrinkMenuCommand.Checked = !shrinkMenuCommand.Checked;
            multilineMenuCommand.Checked = false;

            UpdateSettings();
        }

        void multilineMenuCommand_Click(object sender, EventArgs e)
        {
            multilineMenuCommand.Checked = !multilineMenuCommand.Checked;
            shrinkMenuCommand.Checked = false;

            UpdateSettings();
        }

        void hideTextMenuCommand_Click(object sender, EventArgs e)
        {
            HideText = !HideText;
        }

        private void UpdateSettings()
        {
            tabControl.ShrinkPagesToFit = shrinkMenuCommand.Checked;
            tabControl.Multiline = multilineMenuCommand.Checked;

            tabControl.ShowArrows = !tabControl.ShrinkPagesToFit && !tabControl.Multiline;
        }

        void tabControl_ClosePressed(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab != null) {
                hiddenTabList.Add(tabControl.SelectedTab.Title);
                UpdateTabs();
            }
        }

        public void AddTab(Crownwood.Magic.Controls.TabPage page)
        {
            InsertTab(-1, page);
        }

        public void InsertTab(int index, Crownwood.Magic.Controls.TabPage page)
        {
            TabPageInfo tabInfo = new TabPageInfo(index, page);
            tabInfo.IndexChanged += new EventHandler(tabInfo_IndexChanged);

            RuntimeCore.AddAssemblyObject(tabInfo, this);
            tabList.Add(tabInfo);

            Debug.WriteLine("TabPage \"" + page.Title + "\" added with index " + index.ToString() + ".", "Gui");

            page.CreateControl();
            page.Control.CreateControl();

            UpdateTabs();
        }

        public void RemoveTab(Crownwood.Magic.Controls.TabPage page)
        {
            foreach (TabPageInfo info in tabList) {
                if (info.TabPage == page) {
                    info.IndexChanged -= tabInfo_IndexChanged;
                    tabList.Remove(info);
                    RuntimeCore.RemoveAssemblyObject(info);

                    Debug.WriteLine("TabPage \"" + page.Title + "\" removed.", "Gui");

                    UpdateTabs();
                    return;
                }
            }
        }

        void tabInfo_IndexChanged(object sender, EventArgs e)
        {
            UpdateTabs();
        }

        private void UpdateTabs()
        {
            tabControl.TabPages.Clear();
            popupMenu.MenuCommands.Clear();

            popupMenu.MenuCommands.Add(shrinkMenuCommand);
            popupMenu.MenuCommands.Add(multilineMenuCommand);
            popupMenu.MenuCommands.Add(hideTextMenuCommand);
            popupMenu.MenuCommands.Add(new MenuCommand("-"));

            tabList.Sort(new Comparison<TabPageInfo>(TabPageInfo.Compare));

            foreach (TabPageInfo tabRef in tabList) {
                bool visible = !hiddenTabList.Contains(tabRef.TabPage.Title);

                if (visible)
                    tabControl.TabPages.Add(tabRef.TabPage);

                MenuCommand cmd = new MenuCommand(tabRef.TabPage.Title);
                cmd.Checked = visible;
                cmd.Click += new EventHandler(cmd_Click);
                popupMenu.MenuCommands.Add(cmd);
            }

            if (tabControl.TabPages.Count > 0) {
                tabControl.SelectedIndex = 0;
            }
        }

        void cmd_Click(object sender, EventArgs e)
        {
            MenuCommand cmd = (MenuCommand)sender;

            if (cmd.Checked)
                hiddenTabList.Add(cmd.Text);
            else
                hiddenTabList.Remove(cmd.Text);

            UpdateTabs();
        }

        #region IAssemblyObjectList Members

        void IAssemblyObjectList.Remove(IAssemblyObject obj)
        {
            TabPageInfo info = obj as TabPageInfo;
            if (info != null) {
                info.IndexChanged -= tabInfo_IndexChanged;
                tabList.Remove(info);
                RuntimeCore.RemoveAssemblyObject(info);

                Debug.WriteLine("TabPage \"" + info.TabPage.Title + "\" removed.", "Gui");

                UpdateTabs();
            }
        }

        #endregion
    }
}
