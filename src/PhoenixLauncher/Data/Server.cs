using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Configuration;

namespace PhoenixLauncher.Data
{
    public struct Server
    {
        public string Name;
        public string Address;
        public string ClientExe;
        public string UltimaDir;
        public string Encryption;

        public Server(string name)
        {
            Name = name;
            Address = "";
            ClientExe = "";
            UltimaDir = "";
            Encryption = "";
        }

        public Server(string name, ISettings settings)
        {
            Name = name;
            Address = "";
            ClientExe = "";
            UltimaDir = "";
            Encryption = "";
            Load(settings);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool IsEmpty
        {
            get { return Name == null || Name.Length == 0; }
        }

        public ElementInfo Element
        {
            get { return new ElementInfo("Server", new AttributeInfo("Name", Name)); }
        }

        public void Load(ISettings settings)
        {
            ElementInfo nameElement = Element;

            Address = settings.GetAttribute("", "Address", "Servers", nameElement);
            ClientExe = settings.GetAttribute("", "ClientExe", "Servers", nameElement);
            UltimaDir = settings.GetAttribute("", "UltimaDir", "Servers", nameElement);
            Encryption = settings.GetAttribute("", "Encryption", "Servers", nameElement);
        }

        public void Save(ISettings settings)
        {
            ElementInfo nameElement = Element;

            settings.SetAttribute(Address, "Address", "Servers", nameElement);
            settings.SetAttribute(ClientExe, "ClientExe", "Servers", nameElement);
            settings.SetAttribute(UltimaDir, "UltimaDir", "Servers", nameElement);
            settings.SetAttribute(Encryption, "Encryption", "Servers", nameElement);
        }
    }
}
