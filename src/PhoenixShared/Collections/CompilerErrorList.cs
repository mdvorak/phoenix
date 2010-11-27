using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;

namespace Phoenix.Collections
{
    public class CompilerErrorList : ListEx<CompilerError>
    {
        public CompilerErrorList()
        {
        }

        public virtual void AddRange(CompilerErrorCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (CompilerError item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection contains errors.
        /// </summary>
        public virtual bool HasErrors
        {
            get
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (!Items[i].IsWarning)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection contains warnings.
        /// </summary>
        public virtual bool HasWarnings
        {
            get
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].IsWarning)
                        return true;
                }
                return false;
            }
        }
    }
}
