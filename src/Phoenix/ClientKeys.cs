using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Configuration;

namespace Phoenix
{
    class ClientKeys
    {
        public ClientInfo ClientInfo = new ClientInfo();

        public bool Calculated
        {
            get { return ClientInfo.Hash != null && ClientInfo.Key1 != null && ClientInfo.Key2 != null; }
        }

        public bool Load()
        {
            ClientList list = new ClientList();
            list.Path = System.IO.Path.Combine(Core.LaunchData.PhoenixDir, ClientList.Filename);
            list.Load();
            ClientInfo = list.FindClient(Core.LaunchData.ClientHash);

            if (Calculated)
            {
                Trace.WriteLine("Clients keys found.", "Phoenix");
                return true;
            }
            else
            {
                Trace.WriteLine("Clients keys not found.", "Phoenix");
                return false;
            }
        }

        public void Save()
        {
            try
            {
                ClientList list = new ClientList();
                list.Path = System.IO.Path.Combine(Core.LaunchData.PhoenixDir, ClientList.Filename);
                list.Load();

                list.AddClient(ClientInfo);

                list.Save();

                Trace.WriteLine("Clients keys updated.", "Phoenix");
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("Error saving client keys. Exception: {0}", e.Message), "Phoenix");
            }
        }
    }
}
