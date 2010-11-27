namespace Phoenix.Gui.Pages._Journal
{
    partial class JournalPage
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
            this.richTextBox = new Phoenix.Gui.Controls.RichTextBoxEx();
            this.dummyMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox.ContextMenuStrip = this.dummyMenuStrip;
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(150, 150);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            this.richTextBox.WordWrap = false;
            this.richTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox_LinkClicked);
            // 
            // dummyMenuStrip
            // 
            this.dummyMenuStrip.Enabled = true;
            this.dummyMenuStrip.GripMargin = new System.Windows.Forms.Padding(2);
            this.dummyMenuStrip.Location = new System.Drawing.Point(21, 36);
            this.dummyMenuStrip.Name = "dummyMenuStrip";
            this.dummyMenuStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dummyMenuStrip.Size = new System.Drawing.Size(61, 4);
            this.dummyMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.dummyMenuStrip_Opening);
            // 
            // colorDialog
            // 
            this.colorDialog.AnyColor = true;
            this.colorDialog.SolidColorOnly = true;
            // 
            // JournalPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBox);
            this.Name = "JournalPage";
            this.ResumeLayout(false);

        }

        #endregion

        private Phoenix.Gui.Controls.RichTextBoxEx richTextBox;
        private System.Windows.Forms.ContextMenuStrip dummyMenuStrip;
        private System.Windows.Forms.ColorDialog colorDialog;
    }
}
