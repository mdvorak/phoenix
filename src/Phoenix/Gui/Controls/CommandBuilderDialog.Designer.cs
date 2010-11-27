namespace Phoenix.Gui.Controls
{
    partial class CommandBuilderDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.graphicButton = new System.Windows.Forms.Button();
            this.colorButton = new System.Windows.Forms.Button();
            this.graphicColorButton = new System.Windows.Forms.Button();
            this.serialButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // graphicButton
            // 
            this.graphicButton.Location = new System.Drawing.Point(6, 48);
            this.graphicButton.Name = "graphicButton";
            this.graphicButton.Size = new System.Drawing.Size(54, 23);
            this.graphicButton.TabIndex = 0;
            this.graphicButton.Text = "Graphic";
            this.graphicButton.UseVisualStyleBackColor = true;
            this.graphicButton.Click += new System.EventHandler(this.graphicButton_Click);
            // 
            // colorButton
            // 
            this.colorButton.Location = new System.Drawing.Point(66, 48);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(54, 23);
            this.colorButton.TabIndex = 1;
            this.colorButton.Text = "Color";
            this.colorButton.UseVisualStyleBackColor = true;
            this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // graphicColorButton
            // 
            this.graphicColorButton.Location = new System.Drawing.Point(6, 77);
            this.graphicColorButton.Name = "graphicColorButton";
            this.graphicColorButton.Size = new System.Drawing.Size(114, 23);
            this.graphicColorButton.TabIndex = 2;
            this.graphicColorButton.Text = "Graphic && Color";
            this.graphicColorButton.UseVisualStyleBackColor = true;
            this.graphicColorButton.Click += new System.EventHandler(this.graphicColorButton_Click);
            // 
            // serialButton
            // 
            this.serialButton.Location = new System.Drawing.Point(6, 19);
            this.serialButton.Name = "serialButton";
            this.serialButton.Size = new System.Drawing.Size(114, 23);
            this.serialButton.TabIndex = 3;
            this.serialButton.Text = "Serial";
            this.serialButton.UseVisualStyleBackColor = true;
            this.serialButton.Click += new System.EventHandler(this.serialButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.graphicButton);
            this.groupBox1.Controls.Add(this.serialButton);
            this.groupBox1.Controls.Add(this.colorButton);
            this.groupBox1.Controls.Add(this.graphicColorButton);
            this.groupBox1.Location = new System.Drawing.Point(4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(127, 106);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Get from client";
            // 
            // CommandBuilderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(138, 117);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CommandBuilderDialog";
            this.Text = "Command Builder";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button graphicButton;
        private System.Windows.Forms.Button colorButton;
        private System.Windows.Forms.Button graphicColorButton;
        private System.Windows.Forms.Button serialButton;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}