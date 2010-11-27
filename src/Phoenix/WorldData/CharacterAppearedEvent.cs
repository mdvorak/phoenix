using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.WorldData
{
    public class CharacterAppearedEventArgs : EventArgs
    {
        private Serial serial;

        public CharacterAppearedEventArgs(Serial serial)
        {
            this.serial = serial;
        }

        /// <summary>
        /// Character serial.
        /// </summary>
        public Serial Serial
        {
            get { return serial; }
        }
    }

    public delegate void CharacterAppearedEventHandler(object sender, CharacterAppearedEventArgs e);

    internal class CharacterAppearedPublicEvent : PublicEvent<CharacterAppearedEventHandler, CharacterAppearedEventArgs>
    {
    }
}
