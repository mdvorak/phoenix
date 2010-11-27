namespace Phoenix.Gui.Controls
{
    partial class ColorNumBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorNumBox));
            this.brosweButton = new System.Windows.Forms.Button();
            this.colorBox = new Phoenix.Gui.Controls.NumTextBox();
            this.SuspendLayout();
            // 
            // brosweButton
            // 
            this.brosweButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.brosweButton.Image = ((System.Drawing.Image)(resources.GetObject("brosweButton.Image")));
            this.brosweButton.Location = new System.Drawing.Point(55, 0);
            this.brosweButton.Margin = new System.Windows.Forms.Padding(0);
            this.brosweButton.Name = "brosweButton";
            this.brosweButton.Size = new System.Drawing.Size(20, 20);
            this.brosweButton.TabIndex = 1;
            this.brosweButton.Click += new System.EventHandler(this.brosweButton_Click);
            // 
            // colorBox
            // 
            this.colorBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.colorBox.FormatString = "X4";
            this.colorBox.Location = new System.Drawing.Point(0, 0);
            this.colorBox.Margin = new System.Windows.Forms.Padding(0);
            this.colorBox.MaximumSize = new System.Drawing.Size(52, 20);
            this.colorBox.MaxValue = 3000;
            this.colorBox.MinValue = 1;
            this.colorBox.Name = "colorBox";
            this.colorBox.ShowHexPrefix = true;
            this.colorBox.Size = new System.Drawing.Size(52, 20);
            this.colorBox.TabIndex = 0;
            this.colorBox.Text = "0x0001";
            this.colorBox.Value = 1;
            this.colorBox.ValueChanged += new System.EventHandler(this.colorBox_ValueChanged);
            // 
            // ColorNumBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.brosweButton);
            this.Controls.Add(this.colorBox);
            this.Name = "ColorNumBox";
            this.Size = new System.Drawing.Size(75, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button brosweButton;
        private NumTextBox colorBox;
    }
}
