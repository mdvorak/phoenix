namespace Phoenix.Gui.Pages.InfoGroups
{
    partial class ServerInfo
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
            this.shardBox = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.addressBox = new System.Windows.Forms.Label();
            this.encryptionBox = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // shardBox
            // 
            this.shardBox.AutoSize = true;
            this.shardBox.Location = new System.Drawing.Point(65, 41);
            this.shardBox.Name = "shardBox";
            this.shardBox.Size = new System.Drawing.Size(51, 13);
            this.shardBox.TabIndex = 12;
            this.shardBox.Text = "unknown";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 41);
            this.label13.Margin = new System.Windows.Forms.Padding(3);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(38, 13);
            this.label13.TabIndex = 11;
            this.label13.Text = "Name:";
            // 
            // addressBox
            // 
            this.addressBox.AutoSize = true;
            this.addressBox.Location = new System.Drawing.Point(65, 3);
            this.addressBox.Name = "addressBox";
            this.addressBox.Size = new System.Drawing.Size(51, 13);
            this.addressBox.TabIndex = 10;
            this.addressBox.Text = "unknown";
            // 
            // encryptionBox
            // 
            this.encryptionBox.AutoSize = true;
            this.encryptionBox.Location = new System.Drawing.Point(65, 22);
            this.encryptionBox.Name = "encryptionBox";
            this.encryptionBox.Size = new System.Drawing.Size(51, 13);
            this.encryptionBox.TabIndex = 9;
            this.encryptionBox.Text = "unknown";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 22);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Encryption:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Address:";
            // 
            // ServerInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.shardBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.addressBox);
            this.Controls.Add(this.encryptionBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "ServerInfo";
            this.Size = new System.Drawing.Size(211, 61);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label shardBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label addressBox;
        private System.Windows.Forms.Label encryptionBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
