using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Phoenix.Configuration
{
    public class SpellList
    {
        public const string FileName = "SpellNames.xml";

        class SpellAlias
        {
            public SpellAlias(string alias, byte spell)
            {
                Alias = alias;
                Spell = spell;
            }

            public string Alias;
            public byte Spell;
        }

        private SpellAlias[] spellList;

        public SpellList()
        {
            List<SpellAlias> list = new List<SpellAlias>(64);

            Settings settings = new SynchronizedSettings("SpellNames");
            settings.Path = Path.Combine(Core.Directory, FileName);

            if (File.Exists(settings.Path)) {
                settings.Load();
            }
            else {
                CreateDefault(settings);
                settings.Save();
            }

            ElementInfo[] elementList = settings.EnumarateElements("Spell");

            for (int i = 0; i < elementList.Length; i++) {
                object aliasObj;
                object spellObj;

                if (!elementList[i].Attributes.TryGetValue("alias", out aliasObj))
                    continue;

                if (!elementList[i].Attributes.TryGetValue("number", out spellObj))
                    continue;

                string alias = aliasObj.ToString().ToLowerInvariant();
                string spellStr = spellObj.ToString();

                byte spell;

                if (spellStr.StartsWith("0x") && Byte.TryParse(spellStr.Remove(0, 2), NumberStyles.HexNumber, null, out spell)) {
                    list.Add(new SpellAlias(alias, spell));
                }
                else if (Byte.TryParse(spellStr, out spell)) {
                    list.Add(new SpellAlias(alias, spell));
                }
            }

            spellList = list.ToArray();
        }

        public bool TryFind(string spellName, out byte spellNum)
        {
            spellName = spellName.ToLowerInvariant();

            spellNum = 0xFF;

            for (int i = 0; i < spellList.Length; i++) {
                if (spellList[i].Alias == spellName) {
                    spellNum = spellList[i].Spell;
                    return true;
                }

                if (spellList[i].Alias.Contains(spellName)) {
                    spellNum = spellList[i].Spell;
                }
            }

            return spellNum < 0xFF;
        }

        private void CreateDefault(ISettings settings)
        {
            AddAlias(settings, "Clumsy", 1);
            AddAlias(settings, "Create Food", 2);
            AddAlias(settings, "Feeblemind", 3);
            AddAlias(settings, "Heal", 4);
            AddAlias(settings, "Magic Arrow", 5);
            AddAlias(settings, "Night Sight", 6);
            AddAlias(settings, "Reactive Armor", 7);
            AddAlias(settings, "Weaken", 8);
            AddAlias(settings, "Agility", 9);
            AddAlias(settings, "Cunning", 10);
            AddAlias(settings, "Cure", 11);
            AddAlias(settings, "Harm", 12);
            AddAlias(settings, "Magic Trap", 13);
            AddAlias(settings, "Magic Untrap", 14);
            AddAlias(settings, "Protection", 15);
            AddAlias(settings, "Strength", 16);
            AddAlias(settings, "Bless", 17);
            AddAlias(settings, "Fireball", 18);
            AddAlias(settings, "Magic Lock", 19);
            AddAlias(settings, "Poison", 20);
            AddAlias(settings, "Telekenisis", 21);
            AddAlias(settings, "Teleport", 22);
            AddAlias(settings, "Unlock", 23);
            AddAlias(settings, "Wall of Stone", 24);
            AddAlias(settings, "Arch Cure", 25);
            AddAlias(settings, "Arch Protection", 26);
            AddAlias(settings, "Curse", 27);
            AddAlias(settings, "Fire Field", 28);
            AddAlias(settings, "Greater Heal", 29);
            AddAlias(settings, "Lightning", 30);
            AddAlias(settings, "Mana Drain", 31);
            AddAlias(settings, "Recall", 32);
            AddAlias(settings, "Blade Spirit", 33);
            AddAlias(settings, "Dispel Field", 34);
            AddAlias(settings, "Incognito", 35);
            AddAlias(settings, "Reflection", 36);
            AddAlias(settings, "Mind Blast", 37);
            AddAlias(settings, "Paralyze", 38);
            AddAlias(settings, "Poison Field", 39);
            AddAlias(settings, "Summ. Creature", 40);
            AddAlias(settings, "Summon Creature", 40);
            AddAlias(settings, "Dispel", 41);
            AddAlias(settings, "Energy Bolt", 42);
            AddAlias(settings, "Explosion", 43);
            AddAlias(settings, "Invisibility", 44);
            AddAlias(settings, "Mark", 45);
            AddAlias(settings, "Mass Curse", 46);
            AddAlias(settings, "Paralyze Field", 47);
            AddAlias(settings, "Reveal", 48);
            AddAlias(settings, "Chain Lightning", 49);
            AddAlias(settings, "Energy Field", 50);
            AddAlias(settings, "Flame Strike", 51);
            AddAlias(settings, "Gate Travel", 52);
            AddAlias(settings, "Mana Vampire", 53);
            AddAlias(settings, "Mass Dispel", 54);
            AddAlias(settings, "Meteor Shower", 55);
            AddAlias(settings, "Polymorph", 56);
            AddAlias(settings, "Earthquake", 57);
            AddAlias(settings, "Energy Vortex", 58);
            AddAlias(settings, "Ressurection", 59);
            AddAlias(settings, "Summon Air Elemental", 60);
            AddAlias(settings, "Summon Daemon", 61);
            AddAlias(settings, "Summon Earth Elemental", 62);
            AddAlias(settings, "Summon Fire Elemental", 63);
            AddAlias(settings, "Summon Water Elemental", 64);
        }

        private void AddAlias(ISettings settings, string alias, byte spell)
        {
            settings.SetAttribute(spell, "number", new ElementInfo("Spell", new AttributeInfo("alias", alias)));
        }
    }
}
