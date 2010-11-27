#if DEBUG
// #define WORLDDEBUG
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Phoenix.Communication;
using Phoenix.Communication.Packets;

namespace Phoenix.WorldData
{
    internal static class WorldPacketHandler
    {
        internal static readonly ObjectCallbacksCollection objectCallbacks = new ObjectCallbacksCollection();
        internal static readonly CharacterAppearedPublicEvent characterAppeared = new CharacterAppearedPublicEvent();
        internal static readonly ObjectChangedPublicEvent itemUpdated = new ObjectChangedPublicEvent();
        internal static readonly ObjectChangedPublicEvent itemAdded = new ObjectChangedPublicEvent();

        internal static void Init()
        {
            // Register handlers
            Core.RegisterServerMessageCallback(0x1A, new MessageCallback(OnItemDetails));
            Core.RegisterServerMessageCallback(0x1D, new MessageCallback(OnObjectRemove));
            Core.RegisterServerMessageCallback(0x24, new MessageCallback(OnOpenContainer));
            Core.RegisterServerMessageCallback(0x25, new MessageCallback(OnAddToContainer));
            Core.RegisterServerMessageCallback(0x3C, new MessageCallback(OnContainerContents));
            Core.RegisterServerMessageCallback(0x2E, new MessageCallback(OnCharacterAddItem));
            Core.RegisterServerMessageCallback(0x11, new MessageCallback(OnCharacterStatus));
            Core.RegisterServerMessageCallback(0x2D, new MessageCallback(OnUpdateStats));
            Core.RegisterServerMessageCallback(0xA1, new MessageCallback(OnUpdateHits));
            Core.RegisterServerMessageCallback(0xA2, new MessageCallback(OnUpdateMana));
            Core.RegisterServerMessageCallback(0xA3, new MessageCallback(OnUpdateStamina));
            Core.RegisterServerMessageCallback(0x1B, new MessageCallback(OnLoginConfirm), CallbackPriority.High);
            Core.RegisterServerMessageCallback(0x20, new MessageCallback(OnPlayerSync));
            Core.RegisterServerMessageCallback(0x77, new MessageCallback(OnCharacterUpdate));
            Core.RegisterServerMessageCallback(0x78, new MessageCallback(OnCharacterInformation));
            Core.RegisterServerMessageCallback(0x98, new MessageCallback(OnThingName));
            Core.RegisterServerMessageCallback(0x1C, new MessageCallback(OnAsciiSpeech));
            Core.RegisterServerMessageCallback(0xAE, new MessageCallback(OnUniSpeech));
            Core.RegisterServerMessageCallback(0x4F, new MessageCallback(OnSunLight));
            Core.RegisterServerMessageCallback(0x72, new MessageCallback(OnWarMode));

            Core.Disconnected += new EventHandler(Core_Disconnected);

            WalkHandling.Init();
        }

        private static void ObjectChanged(uint serial, ObjectChangeType changeType)
        {
            ObjectChanged(serial, changeType, false);
        }

        private static void ObjectChanged(uint serial, ObjectChangeType changeType, bool isStatusUpdate)
        {
            objectCallbacks.InvokeAsync(new ObjectChangedEventArgs(serial, changeType, isStatusUpdate));

            if (changeType == ObjectChangeType.ItemUpdated || changeType == ObjectChangeType.Removed) {
                ObjectChangeType subChange = (changeType == ObjectChangeType.Removed) ? ObjectChangeType.SubItemRemoved : ObjectChangeType.SubItemUpdated;

                RealItem item = World.FindRealItem(serial);
                if (item != null) {
                    do {
                        RealObject obj = World.FindRealObject(item.Container);

                        if (obj != null)
                            objectCallbacks.InvokeAsync(new ObjectChangedEventArgs(obj.Serial, serial, subChange));

                        item = obj as RealItem;
                    }
                    while (item != null);
                }
            }
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            World.Clear();
        }

        static CallbackResult OnSunLight(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                Trace.WriteLine("Light changed to " + data[1].ToString(), "Phoenix");
                World.SunLight = data[1];
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnItemDetails(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                PacketReader reader = new PacketReader(data);

                byte id = reader.ReadByte();
                if (id != 0x1A) throw new Exception("Invalid packet passed to OnItemDetails.");

                ushort blockSize = reader.ReadUInt16();

                if (blockSize != reader.Length)
                    return CallbackResult.Normal;

                uint serial = reader.ReadUInt32();

                bool isNew = false;
                RealItem item = World.FindRealItem(serial);
                if (item == null) {
                    item = new RealItem(serial);
                    World.Add(item);
                    isNew = true;
                }

                ushort dispId = reader.ReadUInt16();

                if ((serial & 0x80000000) != 0) {
                    item.Amount = reader.ReadUInt16();
                }

                if ((dispId & 0x8000) != 0) {
                    dispId += reader.ReadByte();
                }

                item.Graphic = (ushort)(dispId & 0x7FFF);

                ushort x = reader.ReadUInt16();
                item.X = (ushort)(x & 0x7FFF);

                ushort y = reader.ReadUInt16();
                item.Y = (ushort)(y & 0x3FFF);

                if ((x & 0x8000) != 0) {
                    byte direction = reader.ReadByte();
                }

                item.Z = reader.ReadSByte();

                if ((y & 0x8000) != 0) {
                    item.Color = reader.ReadUInt16();
                }

                if ((y & 0x4000) != 0) {
                    item.Flags = reader.ReadByte();
                }

                item.Detach();

#if WORLDDEBUG
                Trace.WriteLine(String.Format("Item updated ({0}).", item.Description), "World");
#endif
                if (isNew)
                    itemAdded.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.NewItem));

                ObjectChanged(serial, ObjectChangeType.ItemUpdated);
                itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(item.Serial, ObjectChangeType.ItemUpdated));

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnObjectRemove(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x1D) throw new Exception("Invalid packet passed to OnObjectRemove.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                if (serial != World.PlayerSerial) {
                    // Get item before we remove it, so we know its parent
                    // It will be null for chars, thats ok
                    RealItem item = World.FindRealItem(serial);

                    if (World.Remove(serial))
                        Trace.WriteLine(String.Format("Object 0x{0:X8} removed from world.", serial), "World");
                    else
                        Trace.WriteLine(String.Format("Cannot remove 0x{0:X8}. Object doesn't exist.", serial), "World");

                    // Invoke all events manually
                    objectCallbacks.InvokeAsync(new ObjectChangedEventArgs(serial, ObjectChangeType.Removed));

                    while (item != null) {
                        RealObject obj = World.FindRealObject(item.Container);

                        if (obj != null)
                            objectCallbacks.InvokeAsync(new ObjectChangedEventArgs(obj.Serial, serial, ObjectChangeType.SubItemRemoved));

                        item = obj as RealItem;
                    }

                    return CallbackResult.Normal;
                }
                else {
                    Trace.WriteLine("Cannot remove player. Packet dropped.", "World");
                    return CallbackResult.Eat;
                }
            }
        }

        static CallbackResult OnOpenContainer(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x24) throw new Exception("Invalid packet passed to OnOpenContainer.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);
                ushort gump = ByteConverter.BigEndian.ToUInt16(data, 5);

                RealItem container = World.FindRealItem(serial);
                if (container == null) {
                    Trace.WriteLine("Cannot open non-existing container.", "World");
                    return CallbackResult.Normal;
                }

                // Items will be populated later.
                List<uint> removeList = new List<uint>();

                World.GetContainerContents(container.Serial, removeList);

                for (int i = 0; i < removeList.Count; i++) {
                    World.ItemList.Remove(removeList[i]);
                }

                container.Opened = true;

                Trace.WriteLine(String.Format("Opening container 0x{0}..", serial.ToString("X8")), "World");

                ObjectChanged(serial, ObjectChangeType.ItemOpened);

                return CallbackResult.Normal;
            }
        }

        static uint AddToContainerData(PacketReader reader, out bool isNew)
        {
            uint serial = reader.ReadUInt32();

            RealItem item = World.FindRealItem(serial);
            if (item == null) {
                item = new RealItem(serial);
                World.Add(item);
                isNew = true;
            }
            else {
                isNew = false;
            }

            item.Detach();

            item.Graphic = reader.ReadUInt16();

            byte unknown = reader.ReadByte();

            item.Amount = reader.ReadUInt16();

            item.X = reader.ReadUInt16();
            item.Y = reader.ReadUInt16();
            item.Z = 0;
            item.Container = reader.ReadUInt32();
            item.Color = reader.ReadUInt16();

#if WORLDDEBUG
            Trace.WriteLine(String.Format("Item updated ({0}).", item.Description), "World");
#endif

            return serial;
        }

        static CallbackResult OnAddToContainer(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                PacketReader reader = new PacketReader(data);

                if (reader.ReadByte() != 0x25)
                    throw new Exception("Invalid packet passed to OnAddToContainer.");

                bool isNew;
                uint serial = AddToContainerData(reader, out isNew);

                if (isNew)
                    itemAdded.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.NewItem));
                itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.ItemUpdated));
                ObjectChanged(serial, ObjectChangeType.ItemUpdated);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnContainerContents(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                Trace.WriteLine(String.Format("Populating container.."), "World");

                PacketReader reader = new PacketReader(data);

                byte id = reader.ReadByte();
                if (id != 0x3C) throw new Exception("Invalid packet passed to OnContainerContents.");

                ushort blockSize = reader.ReadUInt16();

                if (blockSize != reader.Length)
                    Trace.WriteLine(String.Format("BlockSize ({0}) for dynamic packet 0x3C doesn't meet data lenght ({1}).", blockSize, data.Length), "World");

                ushort items = reader.ReadUInt16();

                while (items > 0) {
                    bool isNew;
                    uint serial = AddToContainerData(reader, out isNew);

                    if (isNew)
                        itemAdded.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.NewItem));

                    itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.ItemUpdated));
                    ObjectChanged(serial, ObjectChangeType.ItemUpdated);
                    items--;
                }

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnCharacterAddItem(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x2E) throw new Exception("Invalid packet passed to OnCharacterAddItem.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                bool isNew = false;
                RealItem item = World.FindRealItem(serial);
                if (item == null) {
                    item = new RealItem(serial);
                    World.Add(item);
                    isNew = true;
                }

                item.Detach();

                item.Graphic = ByteConverter.BigEndian.ToUInt16(data, 5);
                item.Layer = ByteConverter.BigEndian.ToByte(data, 8);
                item.Color = ByteConverter.BigEndian.ToUInt16(data, 13);

                item.Container = ByteConverter.BigEndian.ToUInt32(data, 9);

                RealCharacter chr = World.FindRealCharacter(item.Container);
                if (chr != null) {
                    chr.Layers[item.Layer] = item.Serial;
                }

#if WORLDDEBUG
                Trace.WriteLine(String.Format("Item updated ({0}).", item.Description), "World");
#endif
                if (isNew)
                    itemAdded.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.NewItem));

                itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.ItemUpdated));
                ObjectChanged(serial, ObjectChangeType.ItemUpdated);

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnCharacterStatus(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                PacketReader reader = new PacketReader(data);

                if (reader.ReadByte() != 0x11) throw new Exception("Invalid packet passed to OnCharacterStatus.");

                ushort blockSize = reader.ReadUInt16();
                if (data.Length != blockSize)
                    Trace.WriteLine(String.Format("BlockSize ({0}) for dynamic packet 0x11 doesn't meet data lenght ({1}).", blockSize, data.Length), "World");

                uint serial = reader.ReadUInt32();

                RealCharacter chr = World.FindRealCharacter(serial);
                if (chr == null) {
                    Trace.WriteLine(String.Format("Cannot update status for unknown character (serial=0x{0:X8}).", serial), "World");
                    return CallbackResult.Normal;
                }

                chr.Name = reader.ReadAnsiString(30);
                chr.Hits = reader.ReadInt16();
                chr.MaxHits = reader.ReadInt16();
                chr.Renamable = reader.ReadByte() > 0;

                byte more = reader.ReadByte();

                if (more > 0) {
                    byte gender = reader.ReadByte();
                    chr.Strenght = reader.ReadInt16();
                    chr.Dexterity = reader.ReadInt16();
                    chr.Intelligence = reader.ReadInt16();
                    chr.Stamina = reader.ReadInt16();
                    chr.MaxStamina = reader.ReadInt16();
                    chr.Mana = reader.ReadInt16();
                    chr.MaxMana = reader.ReadInt16();
                    chr.Gold = reader.ReadInt32();
                    chr.Armor = reader.ReadUInt16();
                    chr.Weight = reader.ReadUInt16();
                }

#if WORLDDEBUG
                Trace.WriteLine(String.Format("Character status updated ({0}).", chr.Description), "World");
#endif

                ObjectChanged(serial, ObjectChangeType.CharUpdated, true);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnUpdateStats(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x2D) throw new Exception("Invalid packet passed to OnUpdateStats.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                RealCharacter chr = World.FindRealCharacter(serial);
                if (chr == null) {
                    Trace.WriteLine(String.Format("Cannot update stats for unknown character (serial=0x{0}).", serial.ToString("X8")), "World");
                    return CallbackResult.Normal;
                }

                chr.MaxHits = ByteConverter.BigEndian.ToInt16(data, 5);
                chr.Hits = ByteConverter.BigEndian.ToInt16(data, 7);
                chr.MaxMana = ByteConverter.BigEndian.ToInt16(data, 9);
                chr.Mana = ByteConverter.BigEndian.ToInt16(data, 11);
                chr.MaxStamina = ByteConverter.BigEndian.ToInt16(data, 13);
                chr.Stamina = ByteConverter.BigEndian.ToInt16(data, 15);

                ObjectChanged(serial, ObjectChangeType.CharUpdated);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnUpdateHits(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0xA1) throw new Exception("Invalid packet passed to OnUpdateHits.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                RealCharacter chr = World.FindRealCharacter(serial);
                if (chr == null) {
                    Trace.WriteLine(String.Format("Cannot update hitpoints for unknown character (serial=0x{0}).", serial.ToString("X8")), "World");
                    return CallbackResult.Normal;
                }

                chr.MaxHits = ByteConverter.BigEndian.ToInt16(data, 5);
                chr.Hits = ByteConverter.BigEndian.ToInt16(data, 7);

                ObjectChanged(serial, ObjectChangeType.CharUpdated);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnUpdateMana(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0xA2) throw new Exception("Invalid packet passed to OnUpdateMana.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                RealCharacter chr = World.FindRealCharacter(serial);
                if (chr == null) {
                    Trace.WriteLine(String.Format("Cannot update mana for unknown character (serial=0x{0}).", serial.ToString("X8")), "World");
                    return CallbackResult.Normal;
                }

                chr.MaxMana = ByteConverter.BigEndian.ToInt16(data, 5);
                chr.Mana = ByteConverter.BigEndian.ToInt16(data, 7);

                ObjectChanged(serial, ObjectChangeType.CharUpdated);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnUpdateStamina(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0xA3) throw new Exception("Invalid packet passed to OnUpdateStamina.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                RealCharacter chr = World.FindRealCharacter(serial);
                if (chr == null) {
                    Trace.WriteLine(String.Format("Cannot update stamina for unknown character (serial=0x{0}).", serial.ToString("X8")), "World");
                    return CallbackResult.Normal;
                }

                chr.MaxStamina = ByteConverter.BigEndian.ToInt16(data, 5);
                chr.Stamina = ByteConverter.BigEndian.ToInt16(data, 7);

                ObjectChanged(serial, ObjectChangeType.CharUpdated);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnLoginConfirm(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x1B) throw new Exception("Invalid packet passed to OnLoginConfirm.");

                bool expected = (World.PlayerSerial == World.InvalidSerial);
                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                if (!expected && World.PlayerSerial != serial) {
                    Trace.WriteLine("Invalid serial in LoginConfirm! Packet ignored.", "World");
                    return CallbackResult.Normal;
                }

                World.PlayerSerial = serial;

                RealCharacter chr = World.FindRealCharacter(World.PlayerSerial);
                if (chr == null) {
                    chr = new RealCharacter(World.PlayerSerial);
                    World.Add(chr);
                }

                chr.Graphic = ByteConverter.BigEndian.ToUInt16(data, 9);

                chr.X = ByteConverter.BigEndian.ToUInt16(data, 11);
                chr.Y = ByteConverter.BigEndian.ToUInt16(data, 13);
                chr.Z = ByteConverter.BigEndian.ToSByte(data, 16);

                chr.Direction = ByteConverter.BigEndian.ToByte(data, 17);

                WalkHandling.ClearStack();

                if (expected)
                    Trace.WriteLine(String.Format("Player logged in. ({0}).", chr), "World");
                else
                    Trace.WriteLine("Unexpected LoginConfirm packet.", "World");

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnPlayerSync(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x20) throw new Exception("Invalid packet passed to OnPlayerSync.");

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                if (World.RealPlayer == null) {
                    Trace.WriteLine(String.Format("LoginConfirm packet not received yet."), "World");
                    return CallbackResult.Normal;
                }

                if (World.PlayerSerial != serial)
                    throw new InvalidOperationException("Invalid serial in 0x20 packet.");

                World.RealPlayer.Graphic = ByteConverter.BigEndian.ToUInt16(data, 5);
                World.RealPlayer.Status = data[7];
                World.RealPlayer.Color = ByteConverter.BigEndian.ToUInt16(data, 8);
                World.RealPlayer.Flags = ByteConverter.BigEndian.ToByte(data, 10);

                World.RealPlayer.X = ByteConverter.BigEndian.ToUInt16(data, 11);
                World.RealPlayer.Y = ByteConverter.BigEndian.ToUInt16(data, 13);
                World.RealPlayer.Z = ByteConverter.BigEndian.ToSByte(data, 18);

                World.RealPlayer.Direction = ByteConverter.BigEndian.ToByte(data, 17);

                WalkHandling.ClearStack();

                Trace.WriteLine(String.Format("Player updated ({0}).", World.RealPlayer), "World");

                ObjectChanged(serial, ObjectChangeType.CharUpdated);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnCharacterUpdate(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                PacketReader reader = new PacketReader(data);

                if (reader.ReadByte() != 0x77) throw new Exception("Invalid packet passed to OnCharacterUpdate.");

                bool newCharacter = false;
                uint serial = reader.ReadUInt32();

                RealCharacter chr = World.FindRealCharacter(serial);
                if (chr == null) {
                    chr = new RealCharacter(serial);
                    World.Add(chr);
                    newCharacter = true;
                }

                chr.Graphic = reader.ReadUInt16();
                chr.X = reader.ReadUInt16();
                chr.Y = reader.ReadUInt16();
                chr.Z = reader.ReadSByte();

                chr.Direction = reader.ReadByte();
                chr.Color = reader.ReadUInt16();
                chr.Flags = reader.ReadByte();
                chr.Notoriety = reader.ReadByte();

#if WORLDDEBUG
                Debug.WriteLine(String.Format("Character updated ({0}).", chr.Description), "World");
#endif

                if (newCharacter)
                    characterAppeared.InvokeAsync(null, new CharacterAppearedEventArgs(serial));

                ObjectChanged(serial, ObjectChangeType.CharUpdated);
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnCharacterInformation(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                PacketReader reader = new PacketReader(data);

                if (reader.ReadByte() != 0x78) throw new Exception("Invalid packet passed to OnCharacterInformation.");

                ushort blockSize = reader.ReadUInt16();
                if (data.Length != blockSize)
                    Trace.WriteLine(String.Format("BlockSize ({0}) for dynamic packet 0x78 doesn't meet data lenght ({1}).", data.Length), "World");

                bool newCharacter = false;
                uint serial = reader.ReadUInt32();

                RealCharacter chr = World.FindRealCharacter(serial);
                if (chr == null) {
                    chr = new RealCharacter(serial);
                    World.Add(chr);
                    newCharacter = true;
                }

                chr.Graphic = reader.ReadUInt16();
                chr.X = reader.ReadUInt16();
                chr.Y = reader.ReadUInt16();
                chr.Z = reader.ReadSByte();

                chr.Direction = reader.ReadByte();
                chr.Color = reader.ReadUInt16();
                chr.Flags = reader.ReadByte();
                chr.Notoriety = reader.ReadByte();

                if (newCharacter)
                    characterAppeared.InvokeAsync(null, new CharacterAppearedEventArgs(serial));

                ObjectChanged(serial, ObjectChangeType.CharUpdated);

                // Items
                while (reader.Offset < blockSize) {
                    uint itemSerial = reader.ReadUInt32();

                    if (itemSerial == 0)
                        return CallbackResult.Normal;

                    bool isNew = false;
                    RealItem item = World.FindRealItem(itemSerial);
                    if (item == null) {
                        item = new RealItem(itemSerial);
                        World.Add(item);
                        isNew = true;
                    }

                    item.Detach();

                    ushort graphic = reader.ReadUInt16();
                    item.Graphic = (ushort)(graphic & 0x7FFF);

                    item.Layer = reader.ReadByte();

                    if ((graphic & 0x8000) != 0) {
                        item.Color = reader.ReadUInt16();
                    }

                    item.Container = chr.Serial;
                    chr.Layers[item.Layer] = item.Serial;

#if WORLDDEBUG
                    Trace.WriteLine(String.Format("Item updated ({0}).", item.Description), "World");
#endif
                    if (isNew)
                        itemAdded.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.NewItem));

                    itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.ItemUpdated));
                    ObjectChanged(itemSerial, ObjectChangeType.ItemUpdated);
                }

#if WORLDDEBUG
                Trace.WriteLine(String.Format("Character updated ({0}).", chr), "World");
#endif

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnThingName(byte[] data, CallbackResult prevResult)
        {
            // This packet is probably sent for Characters only
            lock (World.SyncRoot) {
                if (data[0] != 0x98) throw new Exception("Invalid packet passed to OnThingName.");

                ushort blockSize = ByteConverter.BigEndian.ToUInt16(data, 1);
                if (data.Length != blockSize)
                    Trace.WriteLine(String.Format("BlockSize ({0}) for dynamic packet 0x98 doesn't meet data lenght ({1}).", data.Length), "World");

                if (blockSize != 37) {
                    Trace.WriteLine(String.Format("Invalid 0x98 ThingName packet lenght. Maybe a client version of packet."), "World");
                    return CallbackResult.Normal;
                }

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 3);

                RealObject obj = World.FindRealObject(serial);
                if (obj == null) {
                    Trace.WriteLine(String.Format("Cannot set name for non-existing object (serial=0x{0}).", serial.ToString("X8")), "World");
                    return CallbackResult.Normal;
                }

                string name = ByteConverter.BigEndian.ToAsciiString(data, 7, 30);
                if (name != obj.Name) {
                    obj.Name = name;
                    Trace.WriteLine(String.Format("Object name upadted: {0}", obj), "World");

                    if (obj is RealItem) {
                        itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(serial, ObjectChangeType.ItemUpdated));
                        ObjectChanged(serial, ObjectChangeType.ItemUpdated);
                    }
                    else {
                        ObjectChanged(serial, ObjectChangeType.CharUpdated);
                    }
                }

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnAsciiSpeech(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                AsciiSpeech packet = new AsciiSpeech(data);

                RealObject obj = World.FindRealObject(packet.Serial);
                if (obj == null) {
                    // System speech
                    return CallbackResult.Normal;
                }

                if (obj.Name != packet.Name) {
                    obj.Name = packet.Name;
#if WORLDDEBUG
                    Trace.WriteLine(String.Format("Object name upadted: {0}", obj.Description), "World");
#endif

                    if (obj is RealItem) {
                        itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(obj.Serial, ObjectChangeType.ItemUpdated));
                        ObjectChanged(obj.Serial, ObjectChangeType.ItemUpdated);
                    }
                    else {
                        ObjectChanged(obj.Serial, ObjectChangeType.CharUpdated);
                    }
                }

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnUniSpeech(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                UnicodeSpeech packet = new UnicodeSpeech(data);

                RealObject obj = World.FindRealObject(packet.Serial);
                if (obj == null) {
                    // System speech
                    return CallbackResult.Normal;
                }

                if (obj.Name != packet.Name) {
                    obj.Name = packet.Name;
#if WORLDDEBUG
                    Trace.WriteLine(String.Format("Object name updated: {0}", obj.Description), "World");
#endif

                    if (obj is RealItem) {
                        itemUpdated.InvokeAsync(null, new ObjectChangedEventArgs(obj.Serial, ObjectChangeType.ItemUpdated));
                        ObjectChanged(obj.Serial, ObjectChangeType.ItemUpdated);
                    }
                    else {
                        ObjectChanged(obj.Serial, ObjectChangeType.CharUpdated);
                    }
                }

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnWarMode(byte[] data, CallbackResult prevResult)
        {
            World.RealPlayer.WarMode = data[1] > 0;
            ObjectChanged(World.RealPlayer.Serial, ObjectChangeType.CharUpdated);
            return CallbackResult.Normal;
        }
    }
}
