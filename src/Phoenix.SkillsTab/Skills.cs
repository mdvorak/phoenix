using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MulLib;
using Phoenix.WorldData;
using XPTable.Models;

namespace Phoenix.SkillsTab
{
    [PhoenixWindowTabPage("Skills", Position = 10, Icon = "Phoenix.SkillsTab.Skills.Icon")]
    public partial class Skills : UserControl
    {
        public static System.Drawing.Icon Icon
        {
            get { return Phoenix.SkillsTab.Properties.Resources.light; }
        }

        public Skills()
        {
            InitializeComponent();

            table.Sort(1);

            PlayerSkills.SkillChanged += new SkillChangedEventHandler(PlayerSkills_SkillChanged);
            PlayerSkills.SkillsCleared += new EventHandler(PlayerSkills_SkillsCleared);
        }

        private Dictionary<int, ushort> oldSkills = new Dictionary<int, ushort>();

        void PlayerSkills_SkillChanged(object sender, SkillChangedEventArgs e)
        {
            int id = e.Value.ID;

            if (id < 0)
                throw new InternalErrorException("id < 0 in Skills.PlayerSkills_SkillChanged()");

            Row row = null;

            foreach (Row r in tableModel.Rows) {
                if (id.Equals(r.Tag)) {
                    row = r;
                    break;
                }
            }

            if (row == null) {
                row = new Row();
                row.Tag = id;
                row.Cells.Add(new Cell());
                row.Cells.Add(new Cell(id.ToString()));
                row.Cells.Add(new Cell());
                row.Cells.Add(new Cell());
                row.Cells.Add(new Cell());
                row.Cells.Add(new Cell());

                row.Cells[0].Enabled = false;

                if (id < DataFiles.Skills.Count) {
                    SkillData data = DataFiles.Skills[id];

                    row.Cells[1].Text = data.Name;

                    if (data.Action) {
                        row.Cells[0].Image = null;
                        row.Cells[0].Enabled = true;
                    }
                }

                tableModel.Rows.Add(row);
            }

            row.Cells[2].Data = (float)(e.Value.RealValue / 10.0f);
            row.Cells[3].Data = (float)(e.Value.Value / 10.0f);
            if (oldSkills.ContainsKey(id))
                row.Cells[4].Data = (float)((e.Value.RealValue - oldSkills[id]) / 10.0f);
            else {
                row.Cells[4].Data = 0;
                oldSkills.Add(id, e.Value.RealValue);
            }

            table.Sort();
        }

        void PlayerSkills_SkillsCleared(object sender, EventArgs e)
        {
            tableModel.Rows.Clear();
        }

        private void table_CellButtonClicked(object sender, XPTable.Events.CellButtonEventArgs e)
        {
            if (e.Cell.Row.Tag is int) {
                int id = (int)e.Cell.Row.Tag;
                UO.UseSkill((ushort)id);
            }
        }
    }
}
