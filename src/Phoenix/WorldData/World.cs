using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Phoenix.Configuration;

namespace Phoenix.WorldData
{
    public static partial class World
    {
        /// <summary>
        /// Used only in "Real World".
        /// </summary>
        internal const uint InvalidSerial = 0x7FFFFFFF;

        internal static object SyncRoot = new object();

        private static RealObject invalidObject = new RealObject(InvalidSerial);
        private static RealItem invalidItem = new RealItem(InvalidSerial);
        private static RealCharacter invalidCharacter = new RealCharacter(InvalidSerial);

        private static Dictionary<uint, RealItem> itemList;
        private static Dictionary<uint, RealCharacter> charList;

        private static ItemsCollection ground;
        private static uint playerSerial;
        private static UOPlayer uoplayer;
        private static byte sunLight;
        private static DefaultPublicEvent sunLightChanged;

        private static int cleanUpInterval;
        private static int cleanUpDistance;
        private static Timer cleanUpTimer;
        private static DefaultPublicEvent worldCleaned;

        [ThreadStatic]
        private static int findDistance;

        static World()
        {
            itemList = new Dictionary<uint, RealItem>(128);
            charList = new Dictionary<uint, RealCharacter>(16);

            ground = new ItemsCollection(0x00000000, false);
            playerSerial = World.InvalidSerial;
            uoplayer = new UOPlayer(World.InvalidSerial);
            sunLight = 0xFF;
            sunLightChanged = new DefaultPublicEvent();

            cleanUpInterval = Config.InternalSettings.GetAttribute(5, "CleanUpInterval", "Config", "World");
            cleanUpDistance = Config.InternalSettings.GetAttribute(30, "CleanUpDistance", "Config", "World");

            cleanUpTimer = new Timer(new TimerCallback(CleanUpCallback), null, 10000, cleanUpInterval * 1000);
            worldCleaned = new DefaultPublicEvent();

            findDistance = Config.GroundFindDistance;
        }

        internal static void ThreadInit()
        {
            findDistance = Config.GroundFindDistance;
        }

        #region World CleanUp

        private static void CleanUpCallback(object state)
        {
            CleanUp(200);
        }

        internal static void GetContainerContents(uint serial, List<uint> list)
        {
            serial &= 0x7FFFFFFF;

            lock (SyncRoot) {
                foreach (KeyValuePair<uint, RealItem> pair in World.ItemList) {
                    if (pair.Value.Container == serial) {
                        list.Add(pair.Value.Serial);
                        GetContainerContents(pair.Value.Serial, list);
                    }
                }
            }
        }

        [Command("cleanworld")]
        public static void CleanUp()
        {
            CleanUp(int.MaxValue);
        }

        /// <param name="limit">Maximum count of items that can be deleted.</param>
        public static void CleanUp(int limit)
        {
            if (Core.LoggedIn) {
                System.Diagnostics.Stopwatch watch = new Stopwatch();
                watch.Start();

                List<uint> itemRemoveList = new List<uint>();
                List<uint> charRemoveList = new List<uint>();

                lock (SyncRoot) {
                    if (playerSerial != 0 && playerSerial != uint.MaxValue) {
                        foreach (KeyValuePair<uint, RealItem> pair in itemList) {
                            RealItem item = pair.Value;
                            int dist = item.GetDistance(World.RealPlayer);
                            if ((item.Container == 0 && dist > cleanUpDistance) ||
                                (item.Container != 0 && !itemList.ContainsKey(item.Container) && !charList.ContainsKey(item.Container))) {
                                itemRemoveList.Add(item.Serial);
                                GetContainerContents(item.Serial, itemRemoveList);
                            }

                            if (itemRemoveList.Count > limit)
                                break;
                        }

                        foreach (KeyValuePair<uint, RealCharacter> pair in charList) {
                            RealCharacter chr = pair.Value;
                            if (chr.GetDistance(World.RealPlayer) > cleanUpDistance) {
                                charRemoveList.Add(chr.Serial);

                                for (int l = 0; l < chr.Layers.Length; l++) {
                                    if (chr.Layers[l] != 0) {
                                        itemRemoveList.Add(chr.Layers[l]);
                                        GetContainerContents(chr.Layers[l], itemRemoveList);
                                    }
                                }

                                if (charRemoveList.Count > limit)
                                    break;
                            }
                        }

                        for (int i = 0; i < charRemoveList.Count; i++) {
                            charList.Remove(charRemoveList[i]);
                        }

                        for (int i = 0; i < itemRemoveList.Count; i++) {
                            itemList.Remove(itemRemoveList[i]);
                        }
                    }
                }

                watch.Stop();

                if (itemRemoveList.Count > 0 || charRemoveList.Count > 0) {
                    Trace.WriteLine(String.Format("World cleaned ({0} seconds). Wiped items: {1} characters: {2}", watch.ElapsedMilliseconds / 1000.0f, itemRemoveList.Count, charRemoveList.Count), "World");
                    Trace.WriteLine(String.Format("{0} items {1} characters.", itemList.Count, charList.Count), "World");
                }
            }

            worldCleaned.InvokeAsync(null, EventArgs.Empty);
        }

        public static event EventHandler WorldCleaned
        {
            add { worldCleaned.AddHandler(value); }
            remove { worldCleaned.RemoveHandler(value); }
        }

        #endregion

        #region Internal fields and functions

        internal static Dictionary<uint, RealItem> ItemList
        {
            get { return World.itemList; }
        }

        internal static Dictionary<uint, RealCharacter> CharList
        {
            get { return World.charList; }
            set { World.charList = value; }
        }

        internal static void Clear()
        {
            lock (World.SyncRoot) {
                itemList.Clear();
                charList.Clear();
                WorldPacketHandler.objectCallbacks.Clear();
                playerSerial = World.InvalidSerial;
                uoplayer = new UOPlayer(World.InvalidSerial);
                sunLight = 0xFF;

                Trace.WriteLine("World cleared.", "World");
            }
        }

        internal static uint PlayerSerial
        {
            get { return World.playerSerial; }
            set
            {
                World.playerSerial = value;
                uoplayer = new UOPlayer(value);
            }
        }

        internal static void Add(RealItem item)
        {
            lock (World.SyncRoot) {
                if (item == null) throw new ArgumentNullException("item");
                if (itemList.ContainsKey(item.Serial))
                    throw new InternalErrorException("Serial already exists.");

                itemList.Add(item.Serial, item);
            }
        }

        internal static void Add(RealCharacter character)
        {
            lock (World.SyncRoot) {
                if (character == null) throw new ArgumentNullException("character");
                if (itemList.ContainsKey(character.Serial))
                    throw new InternalErrorException("Serial already exists.");

                charList.Add(character.Serial, character);
            }
        }

        internal static bool Remove(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                RealItem item;
                if (itemList.TryGetValue(serial, out item)) {
                    item.Detach();
                    return itemList.Remove(serial);
                }

                return charList.Remove(serial);
            }
        }

        internal static RealObject FindRealObject(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                RealCharacter character;
                if (charList.TryGetValue(serial, out character))
                    return character;

                RealItem item;
                if (itemList.TryGetValue(serial, out item))
                    return item;

                return null;
            }
        }

        /// <summary>
        /// Never returns null.
        /// </summary>
        internal static RealObject GetRealObject(uint serial)
        {
            lock (World.SyncRoot) {
                RealObject obj = FindRealObject(serial);
                if (obj != null) return obj;
                else return invalidObject;
            }
        }

        internal static RealItem FindRealItem(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                RealItem item;
                if (itemList.TryGetValue(serial & 0x7FFFFFFF, out item))
                    return item;

                return null;
            }
        }

        /// <summary>
        /// Never returns null.
        /// </summary>
        internal static RealItem GetRealItem(uint serial)
        {
            lock (World.SyncRoot) {
                RealItem obj = FindRealItem(serial);
                if (obj != null) return obj;
                else return invalidItem;
            }
        }

        internal static RealCharacter FindRealCharacter(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                RealCharacter character;
                if (charList.TryGetValue(serial, out character))
                    return character;

                return null;
            }
        }

        /// <summary>
        /// Never returns null.
        /// </summary>
        internal static RealCharacter GetRealCharacter(uint serial)
        {
            lock (World.SyncRoot) {
                RealCharacter obj = FindRealCharacter(serial);
                if (obj != null) return obj;
                else return invalidCharacter;
            }
        }

        internal static RealCharacter RealPlayer
        {
            get { return World.GetRealCharacter(playerSerial); }
        }

        #endregion

        #region Public fields and functions

        /// <summary>
        /// Gets distance that filters items during search in ItemsCollection.
        /// This value is thread-dependent. When thread starts it is initalized to Config.GroundFindDistance.
        /// </summary>
        public static int FindDistance
        {
            get { return World.findDistance; }
            set { World.findDistance = value; }
        }

        /// <summary>
        /// Gets collection of all known items (doesn't have to correspond to client).
        /// </summary>
        public static ItemsCollection Ground
        {
            get { return ground; }
        }

        /// <summary>
        /// Gets collection of all known characters (doesn't have to correspond to client).
        /// </summary>
        public static CharacterCollection Characters
        {
            get { return new CharacterCollection(); }
        }

        /// <summary>
        /// Gets object representing Player.
        /// </summary>
        public static UOPlayer Player
        {
            get { return uoplayer; }
        }

        /// <summary>
        /// Gets current sun light level.
        /// </summary>
        public static byte SunLight
        {
            get { return sunLight; }
            internal set
            {
                if (value != sunLight) {
                    sunLight = value;
                    sunLightChanged.BeginInvoke(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Returns whether object exist in world.
        /// </summary>
        /// <param name="serial">Object serial.</param>
        /// <returns>True if object has been found; otherwise false.</returns>
        public static bool Exists(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                if (charList.ContainsKey(serial))
                    return true;

                if (itemList.ContainsKey(serial))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Gets object of specified serial.
        /// </summary>
        /// <param name="serial">Object serial</param>
        /// <returns>
        /// Returns instance of UOCharacter or UOItem.
        /// When object not found, returns UOObject with invalid serial.
        /// </returns>
        public static UOObject GetObject(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                if (charList.ContainsKey(serial))
                    return new UOCharacter(serial);

                if (itemList.ContainsKey(serial))
                    return new UOItem(serial);

                return new UOObject(Serial.Invalid);
            }
        }

        /// <summary>
        /// Gets item of specified serial.
        /// </summary>
        /// <param name="serial">Item serial.</param>
        /// <returns>When object not found, returns UOItem with invalid serial.</returns>
        public static UOItem GetItem(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                if (!itemList.ContainsKey(serial)) serial = Serial.Invalid;
                return new UOItem(serial);
            }
        }

        /// <summary>
        /// Gets character of specified serial.
        /// </summary>
        /// <param name="serial">Character serial.</param>
        /// <returns>When character not found, returns UOCharacter with invalid serial.</returns>
        /// <remarks>If you request character of player serial, it returns UOCharacter, not UOPlayer.</remarks>
        public static UOCharacter GetCharacter(uint serial)
        {
            serial &= 0x7FFFFFFF;

            lock (World.SyncRoot) {
                if (!charList.ContainsKey(serial)) serial = Serial.Invalid;
                return new UOCharacter(serial);
            }
        }

        #endregion

        #region Events

        public static void AddObjectChangedCallback(Serial serial, ObjectChangedEventHandler handler)
        {
            WorldPacketHandler.objectCallbacks.Add(serial, handler);
        }

        public static void RemoveObjectChangedCallback(Serial serial, ObjectChangedEventHandler handler)
        {
            WorldPacketHandler.objectCallbacks.Remove(serial, handler);
        }


        /// <summary>
        /// Raised when player's light level has changed.
        /// </summary>
        public static event EventHandler SunLightChanged
        {
            add { sunLightChanged.AddHandler(value); }
            remove { sunLightChanged.RemoveHandler(value); }
        }

        /// <summary>
        /// Raised when appeared character that wasn't in World. This event is asynchronous.
        /// </summary>
        public static event CharacterAppearedEventHandler CharacterAppeared
        {
            add { WorldPacketHandler.characterAppeared.AddHandler(value); }
            remove { WorldPacketHandler.characterAppeared.RemoveHandler(value); }
        }

        /// <summary>
        /// Raised when item is updated.
        /// </summary>
        public static event ObjectChangedEventHandler ItemUpdated
        {
            add { WorldPacketHandler.itemUpdated.AddHandler(value); }
            remove { WorldPacketHandler.itemUpdated.RemoveHandler(value); }
        }

        public static event ObjectChangedEventHandler ItemAdded
        {
            add { WorldPacketHandler.itemAdded.AddHandler(value); }
            remove { WorldPacketHandler.itemAdded.RemoveHandler(value); }
        }

        #endregion
    }
}
