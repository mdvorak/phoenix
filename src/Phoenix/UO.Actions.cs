using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Communication;

namespace Phoenix
{
    partial class UO
    {
        /// <summary>
        /// Warmodes the specified enabled.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        [Command]
        public static void Warmode(bool enabled)
        {
            Core.SendToServer(PacketBuilder.Warmode(Convert.ToByte(enabled)), true);
        }

        /// <summary>
        /// Uses the skill.
        /// </summary>
        /// <param name="id">The id.</param>
        [Command]
        public static void UseSkill(ushort id)
        {
            Core.SendToServer(PacketBuilder.UseSkill(id), true);
        }

        /// <summary>
        /// Uses the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        [Command]
        public static void UseSkill(string skill)
        {
            skill = skill.ToLowerInvariant();

            for (int i = 0; i < DataFiles.Skills.Count; i++) {
                if (DataFiles.Skills[i].Name.ToLowerInvariant().Contains(skill)) {
                    UseSkill((ushort)i);
                    return;
                }
            }

            ScriptErrorException.Throw("Unknown skill '{0}'", skill);
        }

        /// <summary>
        /// Uses the skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        public static void UseSkill(StandardSkill skill)
        {
            Core.SendToServer(PacketBuilder.UseSkill((ushort)skill), true);
        }

        /// <summary>
        /// Casts the specified spellnum.
        /// </summary>
        /// <param name="spellnum">The spellnum.</param>
        [Command]
        public static void Cast(byte spellnum)
        {
            Core.SendToServer(PacketBuilder.CastSpell(spellnum), true);
        }

        /// <summary>
        /// Casts the specified spellnum.
        /// </summary>
        /// <param name="spellnum">The spellnum.</param>
        /// <param name="target">The target.</param>
        [Command]
        public static void Cast(byte spellnum, Serial target)
        {
            if (target.IsValid)
                UIManager.WaitForTarget(target, 0, 0, 0, 0);

            Core.SendToServer(PacketBuilder.CastSpell(spellnum), true);
        }

        /// <summary>
        /// Casts the specified spellname.
        /// </summary>
        /// <param name="spellname">The spellname.</param>
        [Command]
        public static void Cast(string spellname)
        {
            Cast(spellname, Serial.Invalid);
        }

        /// <summary>
        /// Casts the specified spellname.
        /// </summary>
        /// <param name="spellname">The spellname.</param>
        /// <param name="target">The target.</param>
        [Command]
        public static void Cast(string spellname, Serial target)
        {
            Byte spellNum;

            if (Core.SpellList.TryFind(spellname, out spellNum)) {
                if (target.IsValid)
                    UIManager.WaitForTarget(target, 0, 0, 0, 0);

                Core.SendToServer(PacketBuilder.CastSpell(spellNum), true);
            }
            else {
                ScriptErrorException.Throw("Unknown spell '{0}'", spellname);
            }
        }

        /// <summary>
        /// Casts the specified spell.
        /// </summary>
        /// <param name="spell">The spell.</param>
        public static void Cast(StandardSpell spell)
        {
            Cast(spell, Serial.Invalid);
        }

        /// <summary>
        /// Casts the specified spell.
        /// </summary>
        /// <param name="spell">The spell.</param>
        /// <param name="target">The target.</param>
        public static void Cast(StandardSpell spell, Serial target)
        {
            if (target.IsValid)
                UIManager.WaitForTarget(target, 0, 0, 0, 0);

            Core.SendToServer(PacketBuilder.CastSpell((byte)spell), true);
        }


        /// <summary>
        /// Attacks this instance.
        /// </summary>
        [Command]
        public static void Attack()
        {
            Serial s = UIManager.TargetObject();
            if (s.IsValid)
                Attack(s);
        }

        /// <summary>
        /// Attacks the specified serial.
        /// </summary>
        /// <param name="serial">The serial.</param>
        [Command]
        public static void Attack(Serial serial)
        {
            if (serial.IsValid)
                Core.SendToServer(PacketBuilder.AttackRequest(serial), true);
            else
                ScriptErrorException.Throw("Invalid serial.");
        }

        /// <summary>
        /// Summons the creature.
        /// </summary>
        /// <param name="summon">The summon.</param>
        [Command("summon")]
        public static void SummonCreature(string summon)
        {
            SummonCreature(summon, Serial.Invalid);
        }

        /// <summary>
        /// Summons the creature.
        /// </summary>
        /// <param name="summon">The summon.</param>
        /// <param name="target">The target.</param>
        [Command("summon")]
        public static void SummonCreature(string summon, Serial target)
        {
            UIManager.WaitForMenu(new MenuSelection("summon", summon));

            if (target.IsValid)
                UIManager.WaitForTarget(target, 0, 0, 0, 0);

            UO.Cast("Summ. Creature");
        }
    }
}
