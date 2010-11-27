using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Collections
{
    public class StringList : ListEx<String>
    {
        public StringList()
        {
        }

        public virtual void AddRange(StringCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (string item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Returns string consisting from all elements each on one line.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(Items.Count);
            for (int i = 0; i < Items.Count; i++)
            {
                builder.AppendLine(Items[i]);
            }
            return builder.ToString();
        }
    }
}
