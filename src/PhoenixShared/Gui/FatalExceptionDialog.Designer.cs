namespace Phoenix.Gui
{
    public partial class FatalExceptionDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FatalExceptionDialog));
            this.label1 = new System.Windows.Forms.Label();
            this.exitButton = new System.Windows.Forms.Button();
            this.continueButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.iconBox1 = new Phoenix.Gui.Controls.IconBox();
            this.exceptionControl = new Phoenix.Gui.Controls.ExceptionControl();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(50, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(356, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Unhandled exception occured in Phoenix and program should be terminated.";
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exitButton.Location = new System.Drawing.Point(212, 298);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 5;
            this.exitButton.Text = "Exit";
            this.toolTip.SetToolTip(this.exitButton, "Terminates client.");
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // continueButton
            // 
            this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.continueButton.Location = new System.Drawing.Point(131, 298);
            this.continueButton.Name = "continueButton";
            this.continueButton.Size = new System.Drawing.Size(75, 23);
            this.continueButton.TabIndex = 6;
            this.continueButton.Text = "Try Continue";
            this.toolTip.SetToolTip(this.continueButton, "Phoenix will try to continue. Use at your own risk.");
            this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
            // 
            // iconBox1
            // 
            this.iconBox1.BackColor = System.Drawing.Color.Transparent;
            this.iconBox1.BackgroundImage = null;
            this.iconBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.iconBox1.ForeColor = System.Drawing.Color.Transparent;
            this.iconBox1.Icon = ((System.Drawing.Icon)(resources.GetObject("iconBox1.Icon")));
            this.iconBox1.Location = new System.Drawing.Point(12, 9);
            this.iconBox1.Name = "iconBox1";
            this.iconBox1.Size = new System.Drawing.Size(32, 32);
            this.iconBox1.TabIndex = 1;
            this.iconBox1.Text = "IconBox";
            // 
            // exceptionControl
            // 
            this.exceptionControl.Exception = null;
            this.exceptionControl.Location = new System.Drawing.Point(12, 47);
            this.exceptionControl.Name = "exceptionControl";
            this.exceptionControl.Size = new System.Drawing.Size(394, 242);
            this.exceptionControl.TabIndex = 7;
            // 
            // FatalExceptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(418, 333);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.iconBox1);
            this.Controls.Add(this.exceptionControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FatalExceptionDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phoenix Fatal Error";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button continueButton;
        private System.Windows.Forms.ToolTip toolTip;
        private Phoenix.Gui.Controls.IconBox iconBox1;
        private Phoenix.Gui.Controls.ExceptionControl exceptionControl;
    }
}
