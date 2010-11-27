using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix
{
    partial class UO
    {
        /// <summary>
        /// Zjisti, zdali se dany text nachazi v journalu.
        /// </summary>
        /// <param name="text">
        /// Hledany text. Nerozlisuje se velka/mala pismena. Casti oddelene | (po vzoru Yoka) jsou hledany samostatne.
        /// </param>
        /// <returns>true pokud byl text nalezen, jinak false.</returns>
        /// <seealso cref="Journal.Contains"/>
        public static bool InJournal(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            return Journal.Contains(true, text.Split('|'));
        }

        /// <summary>
        /// Smaze obsah journalu (tyka se pouze aktualniho scriptu).
        /// </summary>
        /// <seealso cref="Journal.Clear"/>
        [Command]
        public static void DeleteJournal()
        {
            Journal.Clear();
        }
    }
}
