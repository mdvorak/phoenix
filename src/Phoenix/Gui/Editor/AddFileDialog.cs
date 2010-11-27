using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Gui.Editor
{
    public partial class AddFileDialog : Form
    {
        public AddFileDialog()
        {
            InitializeComponent();

            templateList.Groups.Add(null, "Default Templates");
            templateList.Groups.Add("phoenix", "Phoenix Templates");

            AddTemplate(new BooScriptTemplate());

            AddTemplate(new CSClassTemplate());
            AddTemplate(new EmptyFileTemplate("C# Code File", "A blank C# code file", ".cs", null, TemplateIcons.CSFile));

            AddTemplate(new VBClassTemplate());
            AddTemplate(new EmptyFileTemplate("VB Code File", "A blank Visual Basic code file", ".vb", null, TemplateIcons.VBFile));
        }

        public string ItemName
        {
            get { return nameTextBox.Text; }
            set { nameTextBox.Text = value; }
        }

        public IItemTemplate SelectedTemplate
        {
            get { return (templateList.SelectedItems.Count > 0) ? (templateList.SelectedItems[0].Tag as IItemTemplate) : null; }
        }

        public void AddTemplate(IItemTemplate template)
        {
            if (template != null)
            {
                if (templateList.Items.ContainsKey(template.Name))
                    throw new InvalidOperationException("Template of specified name already exists.");

                imageList.Images.Add(template.Name, template.Icon);

                ListViewItem item = new ListViewItem(template.Name);
                item.Name = template.Name;
                item.ImageKey = template.Name;
                item.Tag = template;
                item.Group = templateList.Groups[template.Group];

                templateList.Items.Add(item);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (templateList.Items.Count > 0)
            {
                templateList.SelectedIndices.Clear();
                templateList.SelectedIndices.Add(0);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void templateList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (templateList.SelectedItems.Count > 0)
            {
                descriptionBox.Text = SelectedTemplate.Description;
                addButton.Enabled = nameTextBox.TextLength > 0;
            }
            else
            {
                descriptionBox.ResetText();
                addButton.Enabled = false;
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            addButton.Enabled = nameTextBox.TextLength > 0 && SelectedTemplate != null;
        }
    }
}