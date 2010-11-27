namespace Phoenix.Gui.Pages
{
    partial class SettingsPage
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
            this.categoryControl = new Phoenix.Gui.Controls.CategoryControl();
            this.SuspendLayout();
            // 
            // categoryControl
            // 
            this.categoryControl.BackColor = System.Drawing.SystemColors.Control;
            this.categoryControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.categoryControl.Location = new System.Drawing.Point(0, 0);
            this.categoryControl.MinimumSize = new System.Drawing.Size(180, 0);
            this.categoryControl.Name = "categoryControl";
            this.categoryControl.Padding = new System.Windows.Forms.Padding(3);
            this.categoryControl.Size = new System.Drawing.Size(430, 337);
            this.categoryControl.TabIndex = 0;
            // 
            // SettingsPage
            // 
            this.Controls.Add(this.categoryControl);
            this.Name = "SettingsPage";
            this.Size = new System.Drawing.Size(430, 337);
            this.ResumeLayout(false);

        }

        #endregion

        private Phoenix.Gui.Controls.CategoryControl categoryControl;

    }
}
