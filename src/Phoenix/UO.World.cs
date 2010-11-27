using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Communication;

namespace Phoenix
{
    partial class UO
    {
        private const int DefaultPause = 500;

        /// <summary>
        /// Gets the backpack.
        /// </summary>
        /// <value>The backpack.</value>
        public static UOItem Backpack
        {
            get { return World.Player.Backpack; }
        }

        /// <summary>
        /// Counts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static int Count(ushort type)
        {
            return World.Player.Backpack.AllItems.Count(type);
        }

        /// <summary>
        /// Counts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static int Count(ushort type, ushort color)
        {
            return World.Player.Backpack.AllItems.Count(type, color);
        }

        /// <summary>
        /// Spolehlive zjisteni, zdali jsme nazivu.
        /// </summary>
        /// <remarks>
        /// Zkratka pro <c>World.Player.Dead or World.Player.Hits &lt;= 0</c>
        /// </remarks>
        public static bool Dead
        {
            get { return World.Player.Dead || World.Player.Hits <= 0; }
        }

        /// <summary>
        /// Opens the door.
        /// </summary>
        [Command]
        public static void OpenDoor()
        {
            Core.SendToServer(PacketBuilder.OpenDoor());
        }

        /// <summary>
        /// Uses the object.
        /// </summary>
        /// <param name="serial">The serial.</param>
        [Command]
        public static void UseObject(Serial serial)
        {
            if (serial.IsValid)
                Core.SendToServer(PacketBuilder.ObjectDoubleClick(serial));
            else
                ScriptErrorException.Throw("Invalid serial specified to UseObject.");

            Aliases.LastObject = serial;
        }

        /// <summary>
        /// Clicks the object.
        /// </summary>
        /// <param name="serial">The serial.</param>
        [Command]
        public static void ClickObject(Serial serial)
        {
            if (serial.IsValid)
                Core.SendToServer(PacketBuilder.ObjectClick(serial));
            else
                ScriptErrorException.Throw("Invalid serial specified to ClickObject.");
        }

        /// <summary>
        /// Uses the type.
        /// </summary>
        /// <param name="graphic">The graphic.</param>
        [Command]
        public static void UseType(Graphic graphic)
        {
            UOItem item = World.Player.Layers.FindType(graphic);

            if (item.Serial.IsValid)
                UO.UseObject(item);
            else
                ScriptErrorException.Throw("Type not found.");
        }

        /// <summary>
        /// Uses the type.
        /// </summary>
        /// <param name="graphic">The graphic.</param>
        /// <param name="color">The color.</param>
        [Command]
        public static void UseType(Graphic graphic, UOColor color)
        {
            UOItem item = World.Player.Layers.FindType(graphic, color);

            if (item.Serial.IsValid)
                UO.UseObject(item);
            else
                ScriptErrorException.Throw("Type not found.");
        }

        /// <summary>
        /// Sets the receiving container.
        /// </summary>
        [Command]
        public static void SetReceivingContainer()
        {
            Aliases.RecevingContainer = UIManager.TargetObject();
        }

        /// <summary>
        /// Uns the set receiving container.
        /// </summary>
        [Command]
        public static void UnSetReceivingContainer()
        {
            Aliases.RecevingContainer = Serial.Invalid;
        }

        /// <summary>
        /// Grabs this instance.
        /// </summary>
        /// <returns></returns>
        [Command]
        public static bool Grab()
        {
            return Grab(0);
        }

        /// <summary>
        /// Grabs the specified amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        [Command]
        public static bool Grab(ushort amount)
        {
            UO.Print("What item do you want to move?");
            return Grab(amount, UIManager.TargetObject());
        }

        [Command]
        public static bool Grab(ushort amount, Serial item)
        {
            return Grab(amount, item, Aliases.RecevingContainer);
        }

        /// <summary>
        /// Grabs the specified amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="item">The item.</param>
        /// <param name="recevingContainer">The receving container.</param>
        /// <returns></returns>
        [Command]
        public static bool Grab(ushort amount, Serial item, Serial recevingContainer)
        {
            if (!item.IsValid) {
                return ScriptErrorException.Throw("Invalid item serial.");
            }

            if (recevingContainer.IsValid) {
                return UIManager.MoveItem(item, amount, 0xFFFF, 0xFFFF, 0, recevingContainer);
            }
            else {
                return ScriptErrorException.Throw("Invalid RecevingContainer.");
            }
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        [Command]
        public static bool MoveItem(Serial item, ushort amount, Serial destination)
        {
            return MoveItem(item, amount, destination, 0xFFFF, 0xFFFF);
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        [Command]
        public static bool MoveItem(Serial item, ushort amount, Serial destination, ushort x, ushort y)
        {
            if (!item.IsValid)
                return ScriptErrorException.Throw("Invalid item serial.");

            if (!destination.IsValid)
                return ScriptErrorException.Throw("Destination container serial is invalid.");

            return UIManager.MoveItem(item, amount, x, y, 0, destination);
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        [Command]
        public static bool MoveItem(Serial item, ushort amount, ushort x, ushort y, sbyte z)
        {
            if (!item.IsValid)
                return ScriptErrorException.Throw("Invalid item serial.");

            return UIManager.MoveItem(item, amount, x, y, z, 0);
        }

        /// <summary>
        /// Equips the item.
        /// </summary>
        /// <returns></returns>
        [Command("equip")]
        public static bool EquipItem()
        {
            UO.Print("What item do you want to equip?");
            return EquipItem(UIManager.TargetObject());
        }

        /// <summary>
        /// Equips the item.
        /// </summary>
        /// <param name="serial">The serial.</param>
        /// <returns></returns>
        [Command("equip")]
        public static bool EquipItem(Serial serial)
        {
            if (!serial.IsValid)
                return ScriptErrorException.Throw("Item serial is invalid.");

            byte layer = 0;

            try {
                UOItem item = World.GetItem(serial);
                if (item.Exist)
                    layer = DataFiles.Tiledata.GetArt(item.Graphic).Layer;
            }
            catch (Exception e) {
                System.Diagnostics.Trace.WriteLine("Unexpected error in UO.EquipItem(). Exception:\n" + e.ToString(), "Runtime");
            }

            return UIManager.EquipItem(serial, World.Player.Serial, layer);
        }

        /// <summary>
        /// Empties the container.
        /// </summary>
        /// <returns></returns>
        [Command]
        public static int EmptyContainer()
        {
            return EmptyContainer(DefaultPause, UIManager.TargetObject(), Aliases.RecevingContainer);
        }

        /// <summary>
        /// Empties the container.
        /// </summary>
        /// <param name="pause">The pause.</param>
        /// <returns></returns>
        [Command]
        public static int EmptyContainer(int pause)
        {
            return EmptyContainer(pause, UIManager.TargetObject(), Aliases.RecevingContainer);
        }

        /// <summary>
        /// Empties the container.
        /// </summary>
        /// <param name="pause">The pause.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        [Command]
        public static int EmptyContainer(int pause, Serial source)
        {
            return EmptyContainer(pause, source, Aliases.RecevingContainer);
        }

        /// <summary>
        /// Empties the container.
        /// </summary>
        /// <param name="pause">The pause.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        [Command]
        public static int EmptyContainer(int pause, Serial source, Serial target)
        {
            pause = Math.Max(pause, 100);

            if (!source.IsValid) {
                ScriptErrorException.Throw("Source container serial is invalid.");
                return -1;
            }

            if (!target.IsValid) {
                ScriptErrorException.Throw("Target container serial is invalid.");
                return -1;
            }

            int movedCount = 0;
            foreach (UOItem item in World.GetItem(source).Items) {
                if (item.Move(0, target))
                    movedCount++;

                UO.Wait(pause);
            }

            return movedCount;
        }

        /// <summary>
        /// Drops the here.
        /// </summary>
        /// <returns></returns>
        [Command]
        public static bool DropHere()
        {
            return Drop(0, 0, 0, 0, UIManager.TargetObject());
        }

        /// <summary>
        /// Drops the here.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        [Command]
        public static bool DropHere(ushort amount)
        {
            return Drop(amount, 0, 0, 0, UIManager.TargetObject());
        }

        /// <summary>
        /// Drops the here.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        [Command]
        public static bool DropHere(ushort amount, Serial item)
        {
            return Drop(amount, 0, 0, 0, item);
        }

        /// <summary>
        /// Drops the specified amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="relativeX">The relative X.</param>
        /// <param name="relativeY">The relative Y.</param>
        /// <param name="relativeZ">The relative Z.</param>
        /// <returns></returns>
        [Command]
        public static bool Drop(ushort amount, short relativeX, short relativeY, sbyte relativeZ)
        {
            return Drop(amount, relativeX, relativeY, relativeZ, UIManager.TargetObject());
        }

        /// <summary>
        /// Drops the specified amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="relativeX">The relative X.</param>
        /// <param name="relativeY">The relative Y.</param>
        /// <param name="relativeZ">The relative Z.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        [Command]
        public static bool Drop(ushort amount, short relativeX, short relativeY, sbyte relativeZ, Serial item)
        {
            return MoveItem(item, amount, (ushort)(World.Player.X + relativeX), (ushort)(World.Player.Y + relativeY), (sbyte)(World.Player.Z + relativeZ));
        }

        /// <summary>
        /// Requests status of character from server.
        /// </summary>
        /// <param name="serial">Character serial.</param>
        public static void RequestCharacterStatus(Serial serial)
        {
            if (!serial.IsValid)
                ScriptErrorException.Throw("Character serial is invalid.");

            Core.SendToServer(PacketBuilder.CharacterStatsRequest(serial, PacketBuilder.StatsRequestType.Stats));
        }

        /// <summary>
        /// Requests status of character from server.
        /// </summary>
        /// <param name="serial">Character serial.</param>
        /// <param name="timeout">Timeout period.</param>
        /// <returns>True when status update received; otherwise false.</returns>
        public static bool RequestCharacterStatus(Serial serial, int timeout)
        {
            if (!serial.IsValid)
                return ScriptErrorException.Throw("Character serial is invalid.");

            using (SpecializedObjectChangedEventWaiter ew = new SpecializedObjectChangedEventWaiter(serial, ObjectChangeType.CharUpdated)) {
                Core.SendToServer(PacketBuilder.CharacterStatsRequest(serial, PacketBuilder.StatsRequestType.Stats));
                return ew.Wait(timeout);
            }
        }
    }
}
