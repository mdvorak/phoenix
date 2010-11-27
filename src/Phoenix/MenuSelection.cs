using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    [Serializable]
    public struct MenuSelection
    {
        public string Name;
        public string Option;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Menu title.</param>
        /// <param name="option">Option name. Specify null to cancel menu.</param>
        public MenuSelection(string name, string option)
        {
            Name = name;
            Option = option;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Name, Option);
        }
    }
}
