using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Phoenix.WorldData;

namespace Phoenix.Gui.Pages.InfoGroups
{
    [InfoGroup("Player")]
    public partial class PlayerInfo : UserControl
    {
        public PlayerInfo()
        {
            InitializeComponent();

            ResetText();
            accountBox.Text = "";

            Core.LoginComplete += new EventHandler(Core_LoginComplete);
            Core.Disconnected += new EventHandler(Core_Disconnected);
            LoginInfo.Changed += new EventHandler(LoginInfo_Changed);
        }

        void Core_Disconnected(object sender, EventArgs e)
        {
            ResetText();
        }

        void Core_LoginComplete(object sender, EventArgs e)
        {
            World.Player.Changed += new ObjectChangedEventHandler(Player_Changed);
        }

        void LoginInfo_Changed(object sender, EventArgs e)
        {
            accountBox.Text = LoginInfo.Account;
        }

        void Player_Changed(object sender, ObjectChangedEventArgs e)
        {
            if (e.Type == ObjectChangeType.CharUpdated)
            {
                hitsBox.Text = String.Format("{0}/{1}", World.RealPlayer.Hits, World.RealPlayer.MaxHits);
                manabox.Text = String.Format("{0}/{1}", World.RealPlayer.Mana, World.RealPlayer.MaxMana);
                staminaBox.Text = String.Format("{0}/{1}", World.RealPlayer.Stamina, World.RealPlayer.MaxStamina);
                positionBox.Text = String.Format("{0}.{1}.{2}", World.RealPlayer.X, World.RealPlayer.Y, World.RealPlayer.Z);
                characterBox.Text = World.RealPlayer.Name;
            }
        }

        public override void ResetText()
        {
            base.ResetText();

            hitsBox.ResetText();
            manabox.ResetText();
            staminaBox.ResetText();
            positionBox.ResetText();
            characterBox.ResetText();
        }
    }
}
