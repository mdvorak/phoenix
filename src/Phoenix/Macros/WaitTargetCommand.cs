using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Phoenix.Runtime;

namespace Phoenix.Macros
{
    class WaitTargetCommand : ObjectCommandBase
    {
        private bool tile;
        private ushort x;
        private ushort y;
        private sbyte z;
        private ushort graphic;

        private int relX;
        private int relY;
        private int relZ;
        private bool relEnabled;

        public WaitTargetCommand(Serial serial)
            : base(serial)
        {
        }

        public WaitTargetCommand(Phoenix.Communication.Packets.TargetData targetData)
            : base(targetData.Serial != 0 ? targetData.Serial : Serial.Invalid)
        {
            if (targetData != null && (targetData.Serial == 0 || targetData.Serial == Serial.Invalid)) {
                if (targetData.X == 0xFFFF && targetData.Y == 0xFFFF) {
                    // CancelTarget
                    CommandTarget = "Cancel";
                }
                else {
                    x = targetData.X;
                    y = targetData.Y;
                    z = targetData.Z;
                    graphic = targetData.Graphic;
                    tile = true;
                    CommandTarget = "Tile";

                    lock (WorldData.World.SyncRoot) {
                        if (WorldData.World.RealPlayer.Serial != WorldData.World.InvalidSerial) {
                            relX = x - WorldData.World.RealPlayer.X;
                            relY = y - WorldData.World.RealPlayer.Y;
                            relZ = z - WorldData.World.RealPlayer.Z;
                            relEnabled = true;
                        }
                    }
                }
            }
        }

        public override MacroCommandType CommandType
        {
            get { return MacroCommandType.PrecedingActive; }
        }

        protected override string CommandPrefix
        {
            get { return "WaitTarget"; }
        }

        public override object[] Arguments
        {
            get
            {
                if (CommandTarget == "Tile")
                    return new object[] { x, y, z, graphic };
                else if (CommandTarget == "TileRel")
                    return new object[] { relX, relY, relZ, graphic };
                else if (CommandTarget == "Cancel")
                    return new object[0];
                else
                    return base.Arguments;
            }
        }

        public override ToolStripDropDownItem[] CreateCustomMenu()
        {
            List<ToolStripDropDownItem> menuList = new List<ToolStripDropDownItem>();
            menuList.AddRange(base.CreateCustomMenu());

            if (tile) {
                ToolStripMenuItem menuItem = new ToolStripMenuItem("Absolute");
                menuItem.ToolTipText = "Targets static or map tile at specific coordinates.";
                menuItem.Checked = CommandTarget == "Tile";
                menuItem.Click += new EventHandler(tileMenuItem_Click);

                menuList.Add(menuItem);
            }

            if (relEnabled) {
                ToolStripMenuItem menuItem = new ToolStripMenuItem("Relative");
                menuItem.ToolTipText = "Targets static or map tile at player-relative coordinates.";
                menuItem.Checked = CommandTarget == "TileRel";
                menuItem.Click += new EventHandler(tileRelMenuItem_Click);

                menuList.Add(menuItem);
            }

            ToolStripMenuItem cancelMenuItem = new ToolStripMenuItem("Cancel");
            cancelMenuItem.ToolTipText = "Cancels target.";
            cancelMenuItem.Checked = CommandTarget == "Cancel";
            cancelMenuItem.Click += new EventHandler(cancelMenuItem_Click);
            menuList.Add(cancelMenuItem);

            return menuList.ToArray();
        }

        void cancelMenuItem_Click(object sender, EventArgs e)
        {
            CommandTarget = "Cancel";
        }

        void tileMenuItem_Click(object sender, EventArgs e)
        {
            CommandTarget = "Tile";
        }

        void tileRelMenuItem_Click(object sender, EventArgs e)
        {
            CommandTarget = "TileRel";
        }
    }
}
