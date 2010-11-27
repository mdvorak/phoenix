namespace Phoenix.Gui.Pages._Log
{
    partial class LogFilter
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.uiManagerCateory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.categoryControl1 = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.warningCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.informationCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.phoenixCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.otherCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.journalCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.communicationCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.worldCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.runtimeCategory = new Phoenix.Gui.Pages._Log.LogCategoriesControl();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(93, 310);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(12, 310);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // uiManagerCateory
            // 
            this.uiManagerCateory.CategoryColor = System.Drawing.Color.SteelBlue;
            this.uiManagerCateory.CategoryName = "UIManager";
            this.uiManagerCateory.Location = new System.Drawing.Point(12, 244);
            this.uiManagerCateory.MinimumSize = new System.Drawing.Size(100, 23);
            this.uiManagerCateory.Name = "uiManagerCateory";
            this.uiManagerCateory.Size = new System.Drawing.Size(156, 23);
            this.uiManagerCateory.TabIndex = 11;
            // 
            // categoryControl1
            // 
            this.categoryControl1.CategoryColor = System.Drawing.Color.Red;
            this.categoryControl1.CategoryName = "Error";
            this.categoryControl1.Location = new System.Drawing.Point(12, 99);
            this.categoryControl1.MinimumSize = new System.Drawing.Size(100, 23);
            this.categoryControl1.Name = "categoryControl1";
            this.categoryControl1.Size = new System.Drawing.Size(156, 23);
            this.categoryControl1.TabIndex = 10;
            // 
            // warningCategory
            // 
            this.warningCategory.CategoryColor = System.Drawing.Color.Orange;
            this.warningCategory.CategoryName = "Warning";
            this.warningCategory.Location = new System.Drawing.Point(12, 70);
            this.warningCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.warningCategory.Name = "warningCategory";
            this.warningCategory.Size = new System.Drawing.Size(156, 23);
            this.warningCategory.TabIndex = 9;
            // 
            // informationCategory
            // 
            this.informationCategory.CategoryColor = System.Drawing.Color.LimeGreen;
            this.informationCategory.CategoryName = "Information";
            this.informationCategory.Location = new System.Drawing.Point(12, 41);
            this.informationCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.informationCategory.Name = "informationCategory";
            this.informationCategory.Size = new System.Drawing.Size(156, 23);
            this.informationCategory.TabIndex = 8;
            // 
            // phoenixCategory
            // 
            this.phoenixCategory.CategoryColor = System.Drawing.Color.DarkRed;
            this.phoenixCategory.CategoryName = "Phoenix";
            this.phoenixCategory.Location = new System.Drawing.Point(12, 12);
            this.phoenixCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.phoenixCategory.Name = "phoenixCategory";
            this.phoenixCategory.Size = new System.Drawing.Size(156, 23);
            this.phoenixCategory.TabIndex = 7;
            // 
            // otherCategory
            // 
            this.otherCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.otherCategory.CategoryColor = System.Drawing.Color.Green;
            this.otherCategory.CategoryName = "Other";
            this.otherCategory.Location = new System.Drawing.Point(12, 281);
            this.otherCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.otherCategory.Name = "otherCategory";
            this.otherCategory.Size = new System.Drawing.Size(156, 23);
            this.otherCategory.TabIndex = 6;
            // 
            // journalCategory
            // 
            this.journalCategory.CategoryColor = System.Drawing.Color.Gold;
            this.journalCategory.CategoryName = "Journal";
            this.journalCategory.Location = new System.Drawing.Point(12, 215);
            this.journalCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.journalCategory.Name = "journalCategory";
            this.journalCategory.Size = new System.Drawing.Size(156, 23);
            this.journalCategory.TabIndex = 5;
            // 
            // communicationCategory
            // 
            this.communicationCategory.CategoryColor = System.Drawing.Color.Teal;
            this.communicationCategory.CategoryName = "Communication";
            this.communicationCategory.Location = new System.Drawing.Point(12, 186);
            this.communicationCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.communicationCategory.Name = "communicationCategory";
            this.communicationCategory.Size = new System.Drawing.Size(156, 23);
            this.communicationCategory.TabIndex = 4;
            // 
            // worldCategory
            // 
            this.worldCategory.CategoryColor = System.Drawing.Color.DarkBlue;
            this.worldCategory.CategoryName = "World";
            this.worldCategory.Location = new System.Drawing.Point(12, 157);
            this.worldCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.worldCategory.Name = "worldCategory";
            this.worldCategory.Size = new System.Drawing.Size(156, 23);
            this.worldCategory.TabIndex = 3;
            // 
            // runtimeCategory
            // 
            this.runtimeCategory.CategoryColor = System.Drawing.Color.Crimson;
            this.runtimeCategory.CategoryName = "Runtime";
            this.runtimeCategory.Location = new System.Drawing.Point(12, 128);
            this.runtimeCategory.MinimumSize = new System.Drawing.Size(100, 23);
            this.runtimeCategory.Name = "runtimeCategory";
            this.runtimeCategory.Size = new System.Drawing.Size(156, 23);
            this.runtimeCategory.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(-1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "TODO: Saving filter to config";
            // 
            // Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(180, 345);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uiManagerCateory);
            this.Controls.Add(this.categoryControl1);
            this.Controls.Add(this.warningCategory);
            this.Controls.Add(this.informationCategory);
            this.Controls.Add(this.phoenixCategory);
            this.Controls.Add(this.otherCategory);
            this.Controls.Add(this.journalCategory);
            this.Controls.Add(this.communicationCategory);
            this.Controls.Add(this.worldCategory);
            this.Controls.Add(this.runtimeCategory);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Filter";
            this.Text = "Log Window Filter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private LogCategoriesControl uiManagerCateory;
        private LogCategoriesControl categoryControl1;
        private LogCategoriesControl warningCategory;
        private LogCategoriesControl informationCategory;
        private LogCategoriesControl phoenixCategory;
        private LogCategoriesControl otherCategory;
        private LogCategoriesControl journalCategory;
        private LogCategoriesControl communicationCategory;
        private LogCategoriesControl worldCategory;
        private LogCategoriesControl runtimeCategory;
        private System.Windows.Forms.Label label1;
    }
}