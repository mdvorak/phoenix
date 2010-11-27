namespace Phoenix.Gui.Pages
{
    partial class ScriptsPage
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
            this.label1 = new System.Windows.Forms.Label();
            this.compileButton = new System.Windows.Forms.Button();
            this.showReportsButton = new System.Windows.Forms.Button();
            this.fileList = new Phoenix.Gui.Editor.FileList();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Scripts:";
            // 
            // compileButton
            // 
            this.compileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.compileButton.Location = new System.Drawing.Point(212, 243);
            this.compileButton.Name = "compileButton";
            this.compileButton.Size = new System.Drawing.Size(75, 23);
            this.compileButton.TabIndex = 4;
            this.compileButton.Text = "Compile";
            this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
            // 
            // showReportsButton
            // 
            this.showReportsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.showReportsButton.Location = new System.Drawing.Point(3, 243);
            this.showReportsButton.Name = "showReportsButton";
            this.showReportsButton.Size = new System.Drawing.Size(91, 23);
            this.showReportsButton.TabIndex = 5;
            this.showReportsButton.Text = "Show Reports";
            this.showReportsButton.UseVisualStyleBackColor = true;
            this.showReportsButton.Click += new System.EventHandler(this.showResultsButton_Click);
            // 
            // fileList
            // 
            this.fileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileList.Location = new System.Drawing.Point(4, 20);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(283, 217);
            this.fileList.TabIndex = 3;
            // 
            // ScriptsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.showReportsButton);
            this.Controls.Add(this.compileButton);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.label1);
            this.Name = "ScriptsPage";
            this.Size = new System.Drawing.Size(290, 269);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button compileButton;
        private Phoenix.Gui.Editor.FileList fileList;
        private System.Windows.Forms.Button showReportsButton;
    }
}
