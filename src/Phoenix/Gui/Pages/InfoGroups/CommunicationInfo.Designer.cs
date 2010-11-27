namespace Phoenix.Gui.Pages.InfoGroups
{
    partial class CommunicationInfo
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
            this.pingBox = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.upTotalBox = new System.Windows.Forms.Label();
            this.downTotalBox = new System.Windows.Forms.Label();
            this.timeConnectedBox = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.averageUpBandBox = new System.Windows.Forms.Label();
            this.averageDownBandBox = new System.Windows.Forms.Label();
            this.upBandBox = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.downBandBox = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.latencyTestTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // pingBox
            // 
            this.pingBox.AutoSize = true;
            this.pingBox.Location = new System.Drawing.Point(214, 82);
            this.pingBox.Name = "pingBox";
            this.pingBox.Size = new System.Drawing.Size(26, 13);
            this.pingBox.TabIndex = 44;
            this.pingBox.Text = "0ms";
            this.pingBox.DoubleClick += new System.EventHandler(this.pingBox_DoubleClick);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(160, 82);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 13);
            this.label16.TabIndex = 43;
            this.label16.Text = "Latency:";
            this.label16.DoubleClick += new System.EventHandler(this.pingBox_DoubleClick);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 60);
            this.label20.Margin = new System.Windows.Forms.Padding(3);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(34, 13);
            this.label20.TabIndex = 42;
            this.label20.Text = "Total:";
            // 
            // upTotalBox
            // 
            this.upTotalBox.Location = new System.Drawing.Point(160, 60);
            this.upTotalBox.Name = "upTotalBox";
            this.upTotalBox.Size = new System.Drawing.Size(59, 13);
            this.upTotalBox.TabIndex = 41;
            this.upTotalBox.Text = "unknown";
            this.upTotalBox.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // downTotalBox
            // 
            this.downTotalBox.Location = new System.Drawing.Point(62, 60);
            this.downTotalBox.Name = "downTotalBox";
            this.downTotalBox.Size = new System.Drawing.Size(59, 13);
            this.downTotalBox.TabIndex = 40;
            this.downTotalBox.Text = "unknown";
            this.downTotalBox.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // timeConnectedBox
            // 
            this.timeConnectedBox.AutoSize = true;
            this.timeConnectedBox.Location = new System.Drawing.Point(93, 82);
            this.timeConnectedBox.Name = "timeConnectedBox";
            this.timeConnectedBox.Size = new System.Drawing.Size(28, 13);
            this.timeConnectedBox.TabIndex = 39;
            this.timeConnectedBox.Text = "0:00";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(4, 82);
            this.label21.Margin = new System.Windows.Forms.Padding(3);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(87, 13);
            this.label21.TabIndex = 38;
            this.label21.Text = "Time connected:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(3, 41);
            this.label22.Margin = new System.Windows.Forms.Padding(3);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(50, 13);
            this.label22.TabIndex = 37;
            this.label22.Text = "Average:";
            // 
            // averageUpBandBox
            // 
            this.averageUpBandBox.Location = new System.Drawing.Point(160, 41);
            this.averageUpBandBox.Name = "averageUpBandBox";
            this.averageUpBandBox.Size = new System.Drawing.Size(59, 13);
            this.averageUpBandBox.TabIndex = 36;
            this.averageUpBandBox.Text = "unknown";
            this.averageUpBandBox.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // averageDownBandBox
            // 
            this.averageDownBandBox.Location = new System.Drawing.Point(62, 41);
            this.averageDownBandBox.Name = "averageDownBandBox";
            this.averageDownBandBox.Size = new System.Drawing.Size(59, 13);
            this.averageDownBandBox.TabIndex = 35;
            this.averageDownBandBox.Text = "unknown";
            this.averageDownBandBox.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // upBandBox
            // 
            this.upBandBox.Location = new System.Drawing.Point(160, 22);
            this.upBandBox.Name = "upBandBox";
            this.upBandBox.Size = new System.Drawing.Size(59, 13);
            this.upBandBox.TabIndex = 34;
            this.upBandBox.Text = "unknown";
            this.upBandBox.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label19.Location = new System.Drawing.Point(168, 3);
            this.label19.Margin = new System.Windows.Forms.Padding(3);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(47, 13);
            this.label19.TabIndex = 33;
            this.label19.Text = "Upload";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(3, 22);
            this.label15.Margin = new System.Windows.Forms.Padding(3);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(60, 13);
            this.label15.TabIndex = 32;
            this.label15.Text = "Bandwidth:";
            // 
            // downBandBox
            // 
            this.downBandBox.Location = new System.Drawing.Point(62, 22);
            this.downBandBox.Name = "downBandBox";
            this.downBandBox.Size = new System.Drawing.Size(59, 13);
            this.downBandBox.TabIndex = 31;
            this.downBandBox.Text = "unknown";
            this.downBandBox.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label11.Location = new System.Drawing.Point(60, 3);
            this.label11.Margin = new System.Windows.Forms.Padding(3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 30;
            this.label11.Text = "Download";
            // 
            // refreshTimer
            // 
            this.refreshTimer.Enabled = true;
            this.refreshTimer.Interval = 200;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // latencyTestTimer
            // 
            this.latencyTestTimer.Interval = 1000;
            this.latencyTestTimer.Tick += new System.EventHandler(this.latencyTestTimer_Tick);
            // 
            // CommunicationInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.pingBox);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.upTotalBox);
            this.Controls.Add(this.downTotalBox);
            this.Controls.Add(this.timeConnectedBox);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.averageUpBandBox);
            this.Controls.Add(this.averageDownBandBox);
            this.Controls.Add(this.upBandBox);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.downBandBox);
            this.Controls.Add(this.label11);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "CommunicationInfo";
            this.Size = new System.Drawing.Size(308, 101);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label pingBox;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label upTotalBox;
        private System.Windows.Forms.Label downTotalBox;
        private System.Windows.Forms.Label timeConnectedBox;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label averageUpBandBox;
        private System.Windows.Forms.Label averageDownBandBox;
        private System.Windows.Forms.Label upBandBox;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label downBandBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Timer latencyTestTimer;
    }
}
