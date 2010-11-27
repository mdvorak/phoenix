using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Phoenix.Configuration
{
    public struct ClientInfo
    {
        public string Hash;
        public string Key1;
        public string Key2;
    }

    public class ClientList : Settings
    {
        public const string Filename = "Clients.xml";

        public ClientList()
            : base("ClientKeys")
        {
        }

        public ClientInfo FindClient(string Hash)
        {
            ClientInfo info = new ClientInfo();
            if (ElementExists(new ElementInfo("Client", new AttributeInfo("Hash", Hash))))
            {
                info.Hash = Hash;
                info.Key1 = GetAttribute("", "Key1", new ElementInfo("Client", new AttributeInfo("Hash", Hash)));
                info.Key2 = GetAttribute("", "Key2", new ElementInfo("Client", new AttributeInfo("Hash", Hash)));
                return info;
            }
            else
            {
                return new ClientInfo();
            }
        }

        public void AddClient(ClientInfo info)
        {
            if (info.Hash == null) throw new ArgumentNullException("info.Hash");
            if (info.Key1 == null) throw new ArgumentNullException("info.Key1");
            if (info.Key2 == null) throw new ArgumentNullException("info.Key2");

            SetAttribute(info.Key1, "Key1", new ElementInfo("Client", new AttributeInfo("Hash", info.Hash)));
            SetAttribute(info.Key2, "Key2", new ElementInfo("Client", new AttributeInfo("Hash", info.Hash)));
        }

        public ClientInfo[] EnumClients()
        {
            ElementInfo[] eiList = EnumarateElements("Client");
            ClientInfo[] clientList = new ClientInfo[eiList.Length];

            for (int i = 0; i < eiList.Length; i++)
            {
                clientList[i].Hash = eiList[i].Attributes["Hash"].ToString();
                clientList[i].Key1 = eiList[i].Attributes["Key1"].ToString();
                clientList[i].Key2 = eiList[i].Attributes["Key2"].ToString();
            }

            return clientList;
        }
    }
}
