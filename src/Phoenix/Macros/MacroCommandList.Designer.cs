namespace Phoenix.Macros
{
    partial class MacroCommandList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.table = new XPTable.Models.Table();
            this.columnModel = new XPTable.Models.ColumnModel();
            this.textColumn1 = new XPTable.Models.TextColumn();
            this.tableModel = new XPTable.Models.TableModel();
            this.dummyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.ColumnModel = this.columnModel;
            this.table.ColumnResizing = false;
            this.table.ContextMenuStrip = this.dummyContextMenu;
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.EnableHeaderContextMenu = false;
            this.table.EnableToolTips = true;
            this.table.FullRowSelect = true;
            this.table.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.table.Location = new System.Drawing.Point(0, 0);
            this.table.Name = "table";
            this.table.NoItemsText = "";
            this.table.Size = new System.Drawing.Size(204, 153);
            this.table.TabIndex = 0;
            this.table.TableModel = this.tableModel;
            this.table.Text = "table";
            this.table.SizeChanged += new System.EventHandler(this.table1_SizeChanged);
            // 
            // columnModel
            // 
            this.columnModel.Columns.AddRange(new XPTable.Models.Column[] {
            this.textColumn1});
            // 
            // textColumn1
            // 
            this.textColumn1.Width = 50;
            // 
            // dummyContextMenu
            // 
            this.dummyContextMenu.Name = "dummyContextMenu";
            this.dummyContextMenu.Size = new System.Drawing.Size(153, 26);
            this.dummyContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.dummyContextMenu_Opening);
            // 
            // MacroCommandList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.table);
            this.Name = "MacroCommandList";
            this.Size = new System.Drawing.Size(204, 153);
            ((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XPTable.Models.Table table;
        private XPTable.Models.TableModel tableModel;
        private XPTable.Models.ColumnModel columnModel;
        private XPTable.Models.TextColumn textColumn1;
        private System.Windows.Forms.ContextMenuStrip dummyContextMenu;
    }
}
