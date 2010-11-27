namespace Phoenix.Gui
{
    public partial class ExceptionDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialog));
            this.iconBox1 = new Phoenix.Gui.Controls.IconBox();
            this.exceptionControl = new Phoenix.Gui.Controls.ExceptionControl();
            this.okButton = new System.Windows.Forms.Button();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // iconBox1
            // 
            this.iconBox1.BackColor = System.Drawing.Color.Transparent;
            this.iconBox1.BackgroundImage = null;
            this.iconBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.iconBox1.ForeColor = System.Drawing.Color.Transparent;
            this.iconBox1.Icon = ((System.Drawing.Icon)(resources.GetObject("iconBox1.Icon")));
            this.iconBox1.Location = new System.Drawing.Point(12, 12);
            this.iconBox1.Name = "iconBox1";
            this.iconBox1.Size = new System.Drawing.Size(32, 32);
            this.iconBox1.TabIndex = 1;
            this.iconBox1.Text = "IconBox";
            // 
            // exceptionControl
            // 
            this.exceptionControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.exceptionControl.Exception = null;
            this.exceptionControl.Location = new System.Drawing.Point(12, 50);
            this.exceptionControl.Name = "exceptionControl";
            this.exceptionControl.Size = new System.Drawing.Size(386, 215);
            this.exceptionControl.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.okButton.Location = new System.Drawing.Point(168, 271);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // messageBox
            // 
            this.messageBox.BackColor = System.Drawing.SystemColors.Control;
            this.messageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messageBox.Location = new System.Drawing.Point(50, 12);
            this.messageBox.Multiline = true;
            this.messageBox.Name = "messageBox";
            this.messageBox.ReadOnly = true;
            this.messageBox.Size = new System.Drawing.Size(348, 32);
            this.messageBox.TabIndex = 3;
            // 
            // ExceptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 306);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.iconBox1);
            this.Controls.Add(this.exceptionControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phoenix Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Phoenix.Gui.Controls.IconBox iconBox1;
        private Phoenix.Gui.Controls.ExceptionControl exceptionControl;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox messageBox;


    }
}
