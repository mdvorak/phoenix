using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XPTable.Models;
using Phoenix.Collections;

namespace Phoenix.Macros
{
    public partial class MacroCommandList : UserControl
    {
        private class MacroCommandRow : Row
        {
            private IMacroCommand macro;

            public MacroCommandRow(IMacroCommand macro)
            {
                if (macro == null)
                    throw new ArgumentNullException("macro");

                this.macro = macro;

                Editable = false;
                Enabled = true;

                Cell cell = new Cell(macro.ToString());
                Cells.Add(cell);

                macro.CommandChanged += new EventHandler(macro_CommandChanged);
            }

            void macro_CommandChanged(object sender, EventArgs e)
            {
                Cells[0].Text = macro.ToString();
            }

            public IMacroCommand MacroCommand
            {
                get { return macro; }
            }
        }

        private readonly object syncRoot = new object();
        private Macro commandList = null;
        private DefaultPublicEvent macroChanged = new DefaultPublicEvent();

        public event EventHandler MacroChanged
        {
            add { macroChanged.AddHandler(value); }
            remove { macroChanged.RemoveHandler(value); }
        }

        public MacroCommandList()
        {
            InitializeComponent();
        }

        public Macro Macro
        {
            get { return commandList; }
            set
            {
                lock (syncRoot) {
                    if (value != commandList) {
                        OnMacroChanging(EventArgs.Empty);
                        commandList = value;
                        OnMacroChanged(EventArgs.Empty);
                    }
                }
            }
        }

        protected virtual void OnMacroChanging(EventArgs e)
        {
            lock (syncRoot) {
                if (commandList != null) {
                    commandList.ItemInserted -= commandList_ItemInserted;
                    commandList.ItemRemoved -= commandList_ItemRemoved;
                    commandList.ItemUpdated -= commandList_ItemUpdated;
                    commandList.ListCleared -= commandList_ListCleared;
                }

                tableModel.Rows.Clear();
            }
        }

        protected virtual void OnMacroChanged(EventArgs e)
        {
            lock (syncRoot) {
                if (commandList != null) {
                    lock (commandList.SyncRoot) {
                        commandList.ItemInserted += new ListItemChangeEventHandler<IMacroCommand>(commandList_ItemInserted);
                        commandList.ItemRemoved += new ListItemChangeEventHandler<IMacroCommand>(commandList_ItemRemoved);
                        commandList.ItemUpdated += new ListItemUpdateEventHandler<IMacroCommand>(commandList_ItemUpdated);
                        commandList.ListCleared += new EventHandler(commandList_ListCleared);

                        foreach (IMacroCommand m in commandList) {
                            tableModel.Rows.Add(new MacroCommandRow(m));
                        }
                    }
                }

                macroChanged.Invoke(this, EventArgs.Empty);
            }
        }

        void commandList_ListCleared(object sender, EventArgs e)
        {
            lock (syncRoot) {
                tableModel.Rows.Clear();
            }
        }

        void commandList_ItemUpdated(object sender, ListItemUpdateEventArgs<IMacroCommand> e)
        {
            lock (syncRoot) {
                tableModel.Rows.RemoveAt(e.Index);
                tableModel.Rows.Insert(e.Index, new MacroCommandRow(e.Item));
            }
        }

        void commandList_ItemRemoved(object sender, ListItemChangeEventArgs<IMacroCommand> e)
        {
            lock (syncRoot) {
                tableModel.Rows.RemoveAt(e.Index);
            }
        }

        void commandList_ItemInserted(object sender, ListItemChangeEventArgs<IMacroCommand> e)
        {
            lock (syncRoot) {
                MacroCommandRow row = new MacroCommandRow(e.Item);

                if (e.Index < 0)
                    tableModel.Rows.Add(row);
                else
                    tableModel.Rows.Insert(e.Index, row);
            }
        }

        private void table1_SizeChanged(object sender, EventArgs e)
        {
            columnModel.Columns[0].Width = table.ClientSize.Width - SystemInformation.Border3DSize.Width * 2;
        }

        void insertWaitMenu_Click(object sender, EventArgs e)
        {
            if (commandList != null) {
                lock (commandList.SyncRoot) {
                    for (int i = table.SelectedIndicies.Length - 1; i >= 0; i--) {
                        WaitMacroCommand cmd = WaitMacroCommand.PromptUser(500);

                        if (cmd != null) {
                            Macro.Insert(table.SelectedIndicies[i], cmd);
                        }
                    }
                }
            }
        }

        void removeMenu_Click(object sender, EventArgs e)
        {
            if (commandList != null) {
                lock (commandList.SyncRoot) {
                    List<IMacroCommand> removeList = new List<IMacroCommand>();

                    foreach (int index in table.SelectedIndicies) {
                        removeList.Add(commandList[index]);
                    }

                    for (int i = 0; i < removeList.Count; i++) {
                        commandList.Remove(removeList[i]);
                    }
                }
            }
        }

        private void dummyContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            if (table.SelectedItems.Length > 0) {
                MacroCommandRow row = null;

                if (table.SelectedItems.Length == 1)
                    row = (MacroCommandRow)table.SelectedItems[0];

                ContextMenuStrip menu = new ContextMenuStrip();

                ToolStripMenuItem insertWaitMenu = new ToolStripMenuItem("Insert &Wait");
                ToolStripMenuItem removeMenu = new ToolStripMenuItem("&Remove");

                insertWaitMenu.Click += new EventHandler(insertWaitMenu_Click);
                removeMenu.Click += new EventHandler(removeMenu_Click);

                menu.Items.AddRange(new ToolStripItem[] {
                        insertWaitMenu,
                        removeMenu });

                if (row != null) {
                    ToolStripDropDownItem[] customItems = row.MacroCommand.CreateCustomMenu();

                    if (customItems != null && customItems.Length > 0) {
                        menu.Items.Add(new ToolStripSeparator());

                        menu.Items.AddRange(customItems);
                    }
                }

                menu.Show(Control.MousePosition);
            }
        }
    }
}
