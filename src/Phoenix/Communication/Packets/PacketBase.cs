using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Communication.Packets
{
    public class PacketBase
    {
        private byte[] data;

        public PacketBase(byte[] data)
        {
            this.data = data;

            if (data.Length < Lenght)
                throw new ArgumentException("Packet data has incorrect lenght.");

            if (data.Length != Lenght)
                System.Diagnostics.Debug.WriteLine("Packet data has incorrect lenght.", "Communication.Packets");

            if (data[0] != Id)
                throw new ArgumentException("Invalid packet.");
        }

        public byte[] Data
        {
            get { return data; }
        }

        public virtual byte Id
        {
            get { return data[0]; }
        }

        public virtual int Lenght
        {
            get { return data.Length; }
        }
    }
}
