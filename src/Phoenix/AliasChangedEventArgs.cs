using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    public class AliasChangedEventArgs : EventArgs
    {
        private readonly string name;
        private readonly Serial value;
        private readonly Serial oldValue;

        public AliasChangedEventArgs(string name, Serial value, Serial oldValue)
        {
            this.name = name;
            this.value = value;
            this.oldValue = oldValue;
        }

        public string Name
        {
            get { return name; }
        }

        public Serial Value
        {
            get { return value; }
        }

        public Serial OldValue
        {
            get { return oldValue; }
        }
    }
}
