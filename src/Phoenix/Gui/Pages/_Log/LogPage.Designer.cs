namespace Phoenix.Gui.Pages._Log
{
    partial class LogPage
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
            if (disposing && (components != null))
            {
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
            this.dummyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.logBox = new Phoenix.Gui.Pages._Log.LogBox();
            this.SuspendLayout();
            // 
            // dummyContextMenu
            // 
            this.dummyContextMenu.Enabled = true;
            this.dummyContextMenu.GripMargin = new System.Windows.Forms.Padding(2);
            this.dummyContextMenu.Location = new System.Drawing.Point(0, 0);
            this.dummyContextMenu.Name = "dummyContextMenu";
            this.dummyContextMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dummyContextMenu.Size = new System.Drawing.Size(61, 4);
            this.dummyContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.dummyContextMenu_Opening);
            // 
            // fontDialog
            // 
            this.fontDialog.FontMustExist = true;
            this.fontDialog.MaxSize = 24;
            this.fontDialog.ShowEffects = false;
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.Window;
            this.logBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.logBox.ContextMenuStrip = this.dummyContextMenu;
            this.logBox.DetectUrls = false;
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(271, 224);
            this.logBox.TabIndex = 0;
            this.logBox.Text = "";
            this.logBox.WordWrap = false;
            // 
            // LogPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.logBox);
            this.Name = "LogPage";
            this.Size = new System.Drawing.Size(271, 224);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip dummyContextMenu;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.FontDialog fontDialog;
        private LogBox logBox;

    }
}
