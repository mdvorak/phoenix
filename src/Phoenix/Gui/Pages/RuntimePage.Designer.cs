namespace Phoenix.Gui.Pages
{
    partial class RuntimePage
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
            this.terminateButton = new System.Windows.Forms.Button();
            this.terminateAllButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.runningListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.commandBox = new Phoenix.Gui.Controls.CommandLineBox();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // terminateButton
            // 
            this.terminateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.terminateButton.Enabled = false;
            this.terminateButton.Location = new System.Drawing.Point(104, 209);
            this.terminateButton.Name = "terminateButton";
            this.terminateButton.Size = new System.Drawing.Size(75, 23);
            this.terminateButton.TabIndex = 2;
            this.terminateButton.Text = "Terminate";
            this.terminateButton.Click += new System.EventHandler(this.terminateButton_Click);
            // 
            // terminateAllButton
            // 
            this.terminateAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.terminateAllButton.Location = new System.Drawing.Point(185, 209);
            this.terminateAllButton.Name = "terminateAllButton";
            this.terminateAllButton.Size = new System.Drawing.Size(79, 23);
            this.terminateAllButton.TabIndex = 3;
            this.terminateAllButton.Text = "Terminate All";
            this.terminateAllButton.Click += new System.EventHandler(this.terminateAllButton_Click);
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Location = new System.Drawing.Point(228, 3);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(36, 20);
            this.runButton.TabIndex = 6;
            this.runButton.Text = "Run";
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.runningListBox);
            this.splitContainer.Panel1.Controls.Add(this.label1);
            this.splitContainer.Panel1.Controls.Add(this.terminateButton);
            this.splitContainer.Panel1.Controls.Add(this.terminateAllButton);
            this.splitContainer.Panel1MinSize = 96;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.commandBox);
            this.splitContainer.Panel2.Controls.Add(this.runButton);
            this.splitContainer.Panel2MinSize = 26;
            this.splitContainer.Size = new System.Drawing.Size(267, 265);
            this.splitContainer.SplitterDistance = 235;
            this.splitContainer.TabIndex = 8;
            // 
            // runningListBox
            // 
            this.runningListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.runningListBox.FormattingEnabled = true;
            this.runningListBox.IntegralHeight = false;
            this.runningListBox.Location = new System.Drawing.Point(3, 19);
            this.runningListBox.Name = "runningListBox";
            this.runningListBox.Size = new System.Drawing.Size(261, 184);
            this.runningListBox.TabIndex = 5;
            this.runningListBox.SelectedIndexChanged += new System.EventHandler(this.runningListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Running functions:";
            // 
            // commandBox
            // 
            this.commandBox.AcceptsTab = true;
            this.commandBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.commandBox.AutoWordSelection = true;
            this.commandBox.DetectUrls = false;
            this.commandBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.commandBox.HideSelection = false;
            this.commandBox.Location = new System.Drawing.Point(3, 3);
            this.commandBox.MaxLength = 65535;
            this.commandBox.Name = "commandBox";
            this.commandBox.Size = new System.Drawing.Size(219, 20);
            this.commandBox.TabIndex = 7;
            this.commandBox.Text = "";
            this.commandBox.WordWrap = false;
            // 
            // RuntimePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "RuntimePage";
            this.Size = new System.Drawing.Size(267, 265);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button terminateButton;
        private System.Windows.Forms.Button terminateAllButton;
        private System.Windows.Forms.Button runButton;
        private Phoenix.Gui.Controls.CommandLineBox commandBox;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListBox runningListBox;
        private System.Windows.Forms.Label label1;
    }
}
