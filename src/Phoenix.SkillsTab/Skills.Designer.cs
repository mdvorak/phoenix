namespace Phoenix.SkillsTab
{
    partial class Skills
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
            this.table = new XPTable.Models.Table();
            this.columnModel = new XPTable.Models.ColumnModel();
            this.buttonColumn = new XPTable.Models.ButtonColumn();
            this.nameColumn = new XPTable.Models.TextColumn();
            this.realValueColumn = new XPTable.Models.NumberColumn();
            this.valueColumn = new XPTable.Models.NumberColumn();
            //this.lockColumn = new XPTable.Models.TextColumn();
            this.gainColumn = new XPTable.Models.NumberColumn();
            this.tableModel = new XPTable.Models.TableModel();
            ((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.ColumnModel = this.columnModel;
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.EnableToolTips = true;
            this.table.FullRowSelect = true;
            this.table.GridColor = System.Drawing.SystemColors.ControlLight;
            this.table.GridLines = XPTable.Models.GridLines.Both;
            this.table.Location = new System.Drawing.Point(0, 0);
            this.table.Name = "table";
            this.table.NoItemsText = "";
            this.table.Size = new System.Drawing.Size(384, 252);
            this.table.TabIndex = 1;
            this.table.TableModel = this.tableModel;
            this.table.Text = "Skills list";
            this.table.CellButtonClicked += new XPTable.Events.CellButtonEventHandler(this.table_CellButtonClicked);
            // 
            // columnModel
            // 
            this.columnModel.Columns.AddRange(new XPTable.Models.Column[] {
            this.buttonColumn,
            this.nameColumn,
            this.realValueColumn,
            this.valueColumn,
            //this.lockColumn,
			this.gainColumn});
            // 
            // buttonColumn
            // 
            this.buttonColumn.Sortable = false;
            this.buttonColumn.Text = "Action";
            this.buttonColumn.Width = 20;
            // 
            // nameColumn
            // 
            this.nameColumn.Editable = false;
            this.nameColumn.Text = "Skill";
            this.nameColumn.ToolTipText = "Name of skill";
            this.nameColumn.Width = 90;
            // 
            // realValueColumn
            // 
            this.realValueColumn.Editable = false;
            this.realValueColumn.Format = "";
            this.realValueColumn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.realValueColumn.Text = "Real Value";
            this.realValueColumn.Width = 70;
            // 
            // valueColumn
            // 
            this.valueColumn.Editable = false;
            this.valueColumn.Format = "";
            this.valueColumn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 70;
            // 
            // lockColumn
            // 
            //this.lockColumn.Editable = false;
            //this.lockColumn.Text = "Lock";
            //this.lockColumn.ToolTipText = "Skill lock status";
            //this.lockColumn.Width = 50;
            // 
            // gainColumn
            // 
            this.gainColumn.Editable = false;
            this.gainColumn.Format = "";
            this.gainColumn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.gainColumn.Text = "Gain";
            this.gainColumn.Width = 50;
            // 
            // Skills
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.table);
            this.Name = "Skills";
            this.Size = new System.Drawing.Size(384, 252);
            ((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XPTable.Models.Table table;
        private XPTable.Models.ColumnModel columnModel;
        private XPTable.Models.TableModel tableModel;
        private XPTable.Models.TextColumn nameColumn;
        private XPTable.Models.NumberColumn realValueColumn;
        private XPTable.Models.NumberColumn valueColumn;
        private XPTable.Models.ButtonColumn buttonColumn;
        // private XPTable.Models.TextColumn lockColumn;
        private XPTable.Models.NumberColumn gainColumn;
    }
}
