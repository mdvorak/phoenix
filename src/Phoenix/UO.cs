using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using Phoenix.Communication;
using Phoenix.Configuration;
using Phoenix.WorldData;
using Phoenix.Runtime;
using System.Windows.Forms;

namespace Phoenix
{
    public static partial class UO
    {
        [Command]
        public static void Exec(string script, params object[] args)
        {
            RuntimeCore.Executions.Execute(RuntimeCore.ExecutableList[script], args);
        }

        [Command("run")]
        public static object Run(string script, params object[] args)
        {
            return RuntimeCore.Executions.Run(RuntimeCore.ExecutableList[script], args);
        }

        public static object RunCmd(string script, params object[] args)
        {
            return RuntimeCore.Executions.Run(RuntimeCore.CommandList[script], args);
        }

        [Command]
        public static void Loop(string script, params object[] args)
        {
            Loop(-1, script, args);
        }

        [Command]
        public static void Loop(int executions, string script, params object[] args)
        {
            while (executions != 0) {
                RuntimeCore.Executions.Run(RuntimeCore.ExecutableList[script], args);

                if (executions > 0)
                    executions--;

                UO.Wait(1000);
            }
        }

        [Command]
        public static void LoopCmd(string command, params object[] args)
        {
            LoopCmd(-1, command, args);
        }

        [Command]
        public static void LoopCmd(int executions, string command, params object[] args)
        {
            while (executions != 0) {
                RuntimeCore.Executions.Run(RuntimeCore.CommandList[command], args);

                if (executions > 0)
                    executions--;

                UO.Wait(1000);
            }
        }

        [Command]
        public static void Macro(string name)
        {
            Phoenix.Macros.Macro macro;

            if (!RuntimeCore.Macros.TryGetValue(name, out macro)) {
                ScriptErrorException.Throw("Macro {0} not found.", name);
                return;
            }

            try {
                macro.Run();
            }
            catch (Exception e) {
                Trace.WriteLine("Unhandled exception during macro execution. Details:\n" + e.ToString(), "Runtime");
                UO.Print(Env.ErrorColor, "Macro Error: {0}", e.Message);
            }
        }

        /// <summary>
        /// Ukonci vsechny bezici scripty.
        /// </summary>
        [Command]
        public static void TerminateAll()
        {
            RuntimeCore.Executions.TerminateAll();
        }

        /// <summary>
        /// Blocks the current execution for a specified time.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds for which the thread is blocked.</param>
        [Command("wait")]
        public static void Wait(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Zobrazi target a vybranou vec v klientovy skryje (pouze a jen v klientovy).
        /// </summary>
        /// <remarks>Muze byt povazovano za bug, proto pouzivat pouze k vyndavani zapadlych veci atp.</remarks>
        [Command]
        public static void Hide()
        {
            Serial serial = UIManager.TargetObject();
            Wait(500); // Client crashes if object that was targetted is instantly removed.
            Hide(serial);
        }

        /// <summary>
        /// Zadanou vec v klientovy skryje (pouze a jen v klientovy).
        /// </summary>
        /// <param name="serial">ID skryvane veci. Pokud neexistuje, nestane se nic.</param>
        /// <remarks>Muze byt povazovano za bug, proto pouzivat pouze k vyndavani zapadlych veci atp.</remarks>
        [Command]
        public static void Hide(Serial serial)
        {
            if (serial.IsValid) {
                Core.SendToClient(PacketBuilder.ObjectRemove(serial));
            }
            else {
                ScriptErrorException.Throw("Invalid serial.");
            }
        }

        /// <summary>
        /// Hodi na sebe bandy.
        /// </summary>
        /// <returns>true pokud prikaz uspel (neznamena, ze se lecime), false pokud nenalezl bandy.</returns>
        [Command]
        [BlockMultipleExecutions]
        public static bool BandageSelf()
        {
            UOItem bandages = World.Player.Backpack.AllItems.FindType(0x0E21);

            if (bandages.Exist) {
                UO.WaitTargetSelf();
                bandages.Use();
                Wait(50);
                return true;
            }
            else {
                ScriptErrorException.Throw("No bandages found.");
                return false;
            }
        }

        [Command("info")]
        public static void Info()
        {
            IClientTarget target = UIManager.Target();

            if (target.Serial != 0) {
                Info(target.Serial);
            }
            else {
                string name = null;
                if (target.Graphic != 0 && target.Graphic < DataFiles.Tiledata.Count)
                    name = DataFiles.Tiledata.GetArt(target.Graphic).Name;

                string format = "Tile X={0} Y={1} Z={2} Graphic=0x{3:X4}";
                if (name != null && name.Length > 0)
                    format += " Name={4}";

                Notepad.WriteLine(format, target.X, target.Y, target.Z, target.Graphic, name);
                Notepad.WriteLine();
            }
        }

        [Command("info")]
        public static void Info(Serial serial)
        {
            UOObject obj = World.GetObject(serial);
            if (obj.Exist) {
                Notepad.WriteLine(obj.Description);
                Notepad.WriteLine();
            }
        }

        [Command("version")]
        public static void Version()
        {
            Version(false);
        }

        [Command("version")]
        public static void Version(bool details)
        {
            UO.Say(0x35, "{0} is currently using {1} v{2}", World.RealPlayer.Name, Core.VersionString, Core.Version);

            if (details) {
                UO.Say(0x35, "Latency {0} ms", Core.Latency);
            }
        }

        [Command]
        public static void Latency()
        {
            UO.Say(0x35, "Latency {0} ms", Core.Latency);
        }

        [Command]
        public static void RemoveJewelry()
        {
            World.Player.Layers[Layer.Ring].Move(0, World.Player.Backpack);
            World.Player.Layers[Layer.Bracelet].Move(0, World.Player.Backpack);
            World.Player.Layers[Layer.Earrings].Move(0, World.Player.Backpack);

            UOItem neck = World.Player.Layers[Layer.Neck];

            Graphic[] neclases = new Graphic[] { 0x1085, 0x1088, 0x1089, 0x1F05, 0x1F08, 0x1F0A };
            if (Array.IndexOf<Graphic>(neclases, neck.Graphic) >= 0) {
                neck.Move(0, World.Player.Backpack);
            }
        }

        /// <summary>
        /// This function ins provided only for compatilibity with Injection.
        /// Objects are deleted on logout.
        /// </summary>
        /// <param name="name">Can contain only letters. It is not case sensitive.</param>
        [Command]
        public static void AddObject(string name)
        {
            if (Helper.CheckName(ref name, false)) {
                UO.Print("Select {0}:", name);

                Serial serial = UIManager.TargetObject();

                if (serial.IsValid)
                    Aliases.SetObject(name, serial);
                else
                    ScriptErrorException.Throw("Invalid object serial.");
            }
            else {
                ScriptErrorException.Throw("Invalid object name.");
            }
        }

        /// <summary>
        /// This function ins provided only for compatilibity with Injection.
        /// Objects are deleted on logout.
        /// </summary>
        /// <param name="name">Can contain only letters. It is not case sensitive.</param>
        [Command]
        public static void AddObject(string name, Serial value)
        {
            if (Helper.CheckName(ref name, false)) {
                Aliases.SetObject(name, value);
            }
            else {
                ScriptErrorException.Throw("Invalid object name.");
            }
        }

        /// <summary>
        /// Finds item type and adds it to Aliases as <paramref name="objectName"/>.
        /// </summary>
        /// <param name="objectName">Object alias.</param>
        /// <param name="graphic">Searched item graphic.</param>
        /// <param name="color">Searched item color.</param>
        /// <param name="container">Container serial, character serial, or 0 to search ground.</param>
        /// <returns>Found item.</returns>
        [Command]
        public static UOItem FindType(string objectName, Graphic graphic, UOColor color, Serial container)
        {
            UOItem foundItem;

            if (container != 0) {
                UOObject obj = World.GetObject(container);
                if (obj.Serial.IsValid) {
                    if (obj is UOItem)
                        foundItem = ((UOItem)obj).AllItems.FindType(graphic, color);
                    else
                        foundItem = ((UOCharacter)obj).Layers.FindType(graphic, color);
                }
                else {
                    ScriptErrorException.Throw("Invalid container serial.");
                    foundItem = new UOItem(Serial.Invalid);
                }
            }
            else {
                foundItem = World.Ground.FindType(graphic, color);
            }

            Aliases.SetObject(objectName, foundItem);
            return foundItem;
        }

        /// <summary>
        /// Prehraje .wav soubor.
        /// </summary>
        /// <param name="soundLocation">Cesta k souboru.</param>
        /// <remarks>
        /// Volani funkce je ukonceno az po zkonceni prehravani souboru.
        /// <para/>
        /// Pokud chcete prehravat neco jineho nez wav, zkuzte <see cref="Phoenix.SoundPlayer.Play"/> z pluginy
        /// <c>Phoenix.Mp3</c>. Vyzaduje ale DirectX, nemusi fungovat vsem.
        /// </remarks>
        /// <seealso cref="System.Media.SoundPlayer.PlaySync()"/>
        public static void PlayWav(string soundLocation)
        {
            new System.Media.SoundPlayer(soundLocation).PlaySync();
        }


        /// <summary>
        /// Simuluje stisk klavesy v klientovy. 
        /// </summary>
        /// <param name="key">Klavesa pro zmacknuti. Nesmi obsahovat ctrl/shift/alt.</param>
        /// <remarks>Pro kompatibilitu s yokem.</remarks>
        public static void Press(Keys key)
        {
            if ((key & Keys.Modifiers) != Keys.None)
                throw new ScriptErrorException("Modifiers are not supported.");
            if (key == System.Windows.Forms.Keys.None)
                return;

            Client.PostMessage(Client.WM_KEYDOWN, (int)key, 0);
            Client.PostMessage(Client.WM_KEYUP, (int)key, 3);
            UO.Wait(100);
        }
    }
}
