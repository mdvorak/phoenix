using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.Configuration;
using UOEncryption;

namespace PhoenixLauncher.Data
{
    public struct Account
    {
        public string Name;
        public string Password;

        public Account(string name)
        {
            Name = name;
            Password = "";
        }

        public Account(string name, ISettings settings, Server server)
        {
            Name = name;
            Password = "";
            Load(settings, server);
        }

        public override string ToString()
        {
            return Name;
        }

        public ElementInfo Element
        {
            get { return new ElementInfo("Account", new AttributeInfo("Name", Name)); }
        }

        public void Load(ISettings settings, Server server)
        {
            ElementInfo nameElement = Element;

            Password = PasswordEncryption.Decrypt(settings.GetAttribute("", "Password", "Servers", server.Element, nameElement));
        }

        public void Save(ISettings settings, Server server)
        {
            ElementInfo nameElement = Element;

            if (Password != null)
                settings.SetAttribute(PasswordEncryption.Encrypt(Password), "Password", "Servers", server.Element, nameElement);
        }
    }
}
