using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;

namespace Phoenix
{
    public static class UIManager
    {
        public const int PhoenixTargetTimeout = 120;
        public const int WaitTargetTimeout = 30;
        public const int WaitMenuTimeout = 30;
        public const int PhoenixMoveItemTimeout = 30;

        #region OperationResult and WaitQuery classes

        private class OperationResult
        {
            public bool Success = false;
            public readonly AutoResetEvent Event = new AutoResetEvent(false);
        }

        private class WaitQuery : IRequestResult
        {
            public WaitQuery(IClientTarget target)
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId;
                Target = target;
                Menu = new MenuSelection();
            }

            public WaitQuery(MenuSelection menu)
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId;
                Target = null;
                Menu = menu;
            }

            public readonly int ThreadId;
            public MenuSelection Menu;
            public IClientTarget Target;
            public RequestState State;
            private ManualResetEvent e = new ManualResetEvent(false);

            public bool IsMenu
            {
                get { return Target == null; }
            }

            public void Finish(RequestState state)
            {
                this.State = state;
                Trace.Assert(e.Set(), "Unable to set IRequestResult finished event.");
            }

            RequestState IRequestResult.State
            {
                get { return State; }
            }

            void IRequestResult.Wait()
            {
                e.WaitOne();
            }

            bool IRequestResult.Wait(int timeout)
            {
                return e.WaitOne(timeout, false);
            }

            bool IRequestResult.Finished
            {
                get { return State == RequestState.Cancelled || State == RequestState.Completed || State == RequestState.Timeout || State == RequestState.Failed; }
            }

            bool IRequestResult.Failed
            {
                get { return State == RequestState.Cancelled || State == RequestState.Timeout || State == RequestState.Failed; ; }
            }

            bool IRequestResult.Succeeded
            {
                get { return State == RequestState.Completed; }
            }
        }

        #endregion

        public enum State
        {
            Ready,
            Target,
            ServerTarget,
            WaitTarget,
            WaitMenu,
            ClientPickedUpItem,
            ClientHoldingItem,
            ClientDroppedItem,
            MoveItem,
        }

        private static readonly object syncRoot = new object();
        private static State currentState = State.Ready;
        private static AutoResetEvent readySignal = new AutoResetEvent(true);
        private static OperationResult operationResult = null;
        private static Thread timeoutThread = null;
        private static DefaultPublicEvent stateChanged = new DefaultPublicEvent();

        private static uint pickedUpItem = 0;
        private static string pickedUpItemName = null;
        private static TargetData clientTarget = null;
        private static Queue<WaitQuery> waitQueue = null;

        public static readonly IRequestResult FailedResult;

        public static State CurrentState
        {
            get { return currentState; }
            private set
            {
                if (value != currentState) {
                    currentState = value;
                    Debug.WriteLine("CurrentState changed to " + currentState.ToString(), "UIManager");

                    stateChanged.BeginInvoke(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler StateChanged
        {
            add { stateChanged.AddHandler(value); }
            remove { stateChanged.RemoveHandler(value); }
        }

        public static uint PickedUpItem
        {
            get { return UIManager.pickedUpItem; }
            set
            {
                if (pickedUpItem != 0)
                    World.RemoveObjectChangedCallback(pickedUpItem, new ObjectChangedEventHandler(World_ObjectChanged));

                UIManager.pickedUpItem = value;

                if (pickedUpItem != 0)
                    World.AddObjectChangedCallback(pickedUpItem, new ObjectChangedEventHandler(World_ObjectChanged));
            }
        }

        public static uint ClientTargetId
        {
            get { return clientTarget.TargetId; }
        }

        private static void FinishWork()
        {
            lock (syncRoot) {
                if (CurrentState != State.Ready) {
                    StopTimeout();

                    if (CurrentState == State.WaitTarget || CurrentState == State.WaitMenu) {
                        Debug.Assert(waitQueue != null, "waitQueue == null");

                        if (waitQueue.Count > 0) {
                            if (waitQueue.Peek().IsMenu) {
                                CurrentState = State.WaitMenu;
                            }
                            else {
                                CurrentState = State.WaitTarget;
                            }
                            return;
                        }
                        else {
                            DeleteWaitQueue(false);
                        }
                    }

                    CurrentState = State.Ready;
                    readySignal.Set();
                }
            }
        }

        private static void DeleteWaitQueue(bool timeout)
        {
            lock (syncRoot) {
                Debug.Assert(waitQueue != null, "waitQueue != null");

                while (waitQueue.Count > 0) {
                    waitQueue.Dequeue().Finish(timeout ? RequestState.Timeout : RequestState.Cancelled);
                }

                Debug.Assert(waitQueue.Count == 0, "waitQueue.Count == 0");

                waitQueue = null;
            }
        }

        static UIManager()
        {
            FailedResult = new WaitQuery(null);
            ((WaitQuery)FailedResult).Finish(RequestState.Failed);
        }

        /// <summary>
        /// Called by Phoenix.Initialization()
        /// </summary>
        internal static void Init()
        {
            // Item pickup handling
            Core.RegisterClientMessageCallback(0x07, new MessageCallback(OnPickupRequest), CallbackPriority.High);
            Core.RegisterClientMessageCallback(0x08, new MessageCallback(OnDropRequest), CallbackPriority.High);
            Core.RegisterClientMessageCallback(0x13, new MessageCallback(OnEquipRequest), CallbackPriority.High);
            Core.RegisterServerMessageCallback(0x27, new MessageCallback(OnPickupItemFailed), CallbackPriority.High);
            Core.RegisterServerMessageCallback(0x1D, new MessageCallback(OnRemoveObject), CallbackPriority.High);

            // Targets handling
            Core.RegisterServerMessageCallback(0x6C, new MessageCallback(OnServerTarget), CallbackPriority.High);
            Core.RegisterClientMessageCallback(0x6C, new MessageCallback(OnClientTarget), CallbackPriority.High);
            // Multi
            Core.RegisterServerMessageCallback(0x99, new MessageCallback(OnServerTargetMulti), CallbackPriority.High);

            // ObjectPicker handling
            Core.RegisterServerMessageCallback(0x7C, new MessageCallback(OnObjectPicker), CallbackPriority.High);

            // Actions
            Core.RegisterClientMessageCallback(0x06, new MessageCallback(OnObjectDoubleClick), CallbackPriority.High);
            Core.RegisterClientMessageCallback(0x12, new MessageCallback(OnUse), CallbackPriority.High);
            Core.RegisterClientMessageCallback(0xBF, new MessageCallback(OnGeneralCommand), CallbackPriority.High);

            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            Reset();
        }

        #region BeginTimeout function

        private static void StopTimeout()
        {
            if (timeoutThread != null) {
                timeoutThread.Abort();
                timeoutThread = null;
            }
        }

        private static void BeginTimeout(int seconds)
        {
            StopTimeout();

            timeoutThread = new Thread(new ParameterizedThreadStart(TimeoutThread));
            timeoutThread.Start(seconds * 1000);
        }

        private static void TimeoutThread(object parameter)
        {
            Thread.Sleep((int)parameter);

            lock (syncRoot) {
                if (CurrentState != State.Ready) {
                    UO.Print("{0} timeout.", CurrentState);
                    ResetInternal(true);
                }
            }
        }

        #endregion

        #region Item manipulation related routines

        private static CallbackResult OnPickupRequest(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                switch (CurrentState) {
                    case State.Ready:
                        break;

                    case State.ClientDroppedItem:
                    case State.MoveItem:
                        Core.SendToClient(PacketBuilder.PickupItemFailed(6));
                        UO.Print("Cannot pick up item now.");
                        return CallbackResult.Eat;

                    default:
                        Reset();
                        break;
                }

                Debug.Assert(CurrentState == State.Ready, "CurrentState is not Ready. Internal error.");

                PickedUpItem = ByteConverter.BigEndian.ToUInt32(data, 1);

                RealObject obj = World.FindRealObject(PickedUpItem);
                if (obj != null && obj.Name != null && obj.Name.Length > 0)
                    pickedUpItemName = String.Format("\"{0}\"", obj.Name);
                else
                    pickedUpItemName = String.Format("0x{0:X8}", PickedUpItem);

                CurrentState = State.ClientPickedUpItem;

                return CallbackResult.Normal;
            }
        }

        private static CallbackResult OnDropRequest(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                if (CurrentState != State.ClientPickedUpItem && CurrentState != State.ClientHoldingItem) {
                    Trace.WriteLine("Dropped unexpected client drop request.", "UIManager");
                    return CallbackResult.Eat;
                }

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                if (serial != PickedUpItem) {
                    Trace.WriteLine("Trying to drop incorrect item. Packet dropped.", "UIManager");
                    Reset();
                    return CallbackResult.Eat;
                }

                if (CurrentState == State.ClientHoldingItem) {
                    PickedUpItem = 0;
                    pickedUpItemName = null;
                    FinishWork();
                }
                else {
                    CurrentState = State.ClientDroppedItem;
                }

                return CallbackResult.Normal;
            }
        }

        private static CallbackResult OnEquipRequest(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                if (CurrentState != State.ClientPickedUpItem && CurrentState != State.ClientHoldingItem) {
                    Trace.WriteLine("Dropped unexpected client equip request.", "UIManager");
                    return CallbackResult.Eat;
                }

                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                if (serial != PickedUpItem) {
                    Trace.WriteLine("Trying to equip incorrect item. Packet dropped.", "UIManager");
                    Reset();
                    return CallbackResult.Eat;
                }

                if (CurrentState == State.ClientHoldingItem) {
                    PickedUpItem = 0;
                    pickedUpItemName = null;
                    FinishWork();
                }
                else {
                    CurrentState = State.ClientDroppedItem;
                }

                return CallbackResult.Normal;
            }
        }

        private static CallbackResult OnPickupItemFailed(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                switch (CurrentState) {
                    case State.ClientPickedUpItem:
                        PickedUpItem = 0;
                        pickedUpItemName = null;
                        FinishWork();
                        return CallbackResult.Normal;

                    case State.ClientDroppedItem:
                        PickedUpItem = 0;
                        pickedUpItemName = null;
                        FinishWork();
                        return CallbackResult.Eat;

                    case State.MoveItem:
                        operationResult.Success = false;
                        operationResult.Event.Set();
                        operationResult = null;
                        PickedUpItem = 0;
                        pickedUpItemName = null;
                        FinishWork();
                        return CallbackResult.Eat;

                    default:
                        Trace.WriteLine("Dropped unexpected server PickupItemFailed packet.", "UIManager");
                        return CallbackResult.Eat;
                }
            }
        }

        private static CallbackResult OnRemoveObject(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                uint serial = ByteConverter.BigEndian.ToUInt32(data, 1);

                if (serial == PickedUpItem) {
                    switch (CurrentState) {
                        case State.ClientDroppedItem:
                            PickedUpItem = 0;
                            pickedUpItemName = null;
                            FinishWork();
                            break;

                        case State.ClientPickedUpItem:
                            CurrentState = State.ClientHoldingItem;
                            break;

                        case State.MoveItem:
                            operationResult.Success = true;
                            operationResult.Event.Set();
                            operationResult = null;
                            PickedUpItem = 0;
                            pickedUpItemName = null;
                            FinishWork();
                            break;
                    }

                    return CallbackResult.Normal;
                }
                else {
                    return CallbackResult.Normal;
                }
            }
        }

        static void World_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.Type == ObjectChangeType.ItemUpdated) {
                lock (syncRoot) {
                    if (e.Serial == PickedUpItem) {
                        switch (CurrentState) {
                            case State.ClientPickedUpItem:
                            case State.ClientHoldingItem:
                            case State.MoveItem:
                                Debug.WriteLine("Warning: Unexpected item update.", "UIManager");
                                return;

                            case State.ClientDroppedItem:
                                PickedUpItem = 0;
                                pickedUpItemName = null;
                                FinishWork();
                                return;

                            default:
                                Trace.WriteLine("pickedUpItem not cleared. Internal error.", "UIManager");
                                PickedUpItem = 0;
                                return;
                        }
                    }
                }
            }
        }

        #endregion

        #region Targetting and all around

        private static CallbackResult OnServerTarget(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                if (CurrentState == State.WaitTarget) {
                    Debug.Assert(waitQueue != null && waitQueue.Count > 0, "Empty waitQueue.");
                    Debug.Assert(!waitQueue.Peek().IsMenu, "Menu is in the top of the queue instead of Target.");

                    TargetData target = TargetData.FromData(data);
                    WaitQuery query = waitQueue.Dequeue();
                    IClientTarget waitTarget = query.Target;

                    if ((byte)waitTarget.Type > target.Type) {
                        Trace.WriteLine("Unexpected Server target type received. Target passed to client.", "UIManager");
                        query.Finish(RequestState.Failed);
                    }
                    else {
                        Core.SendToServer(TargetData.ToData(waitTarget, target.TargetId));

                        FinishWork();
                        Trace.WriteLine("Processed expected Server target.", "UIManager");

                        query.Finish(RequestState.Completed);
                        return CallbackResult.Eat;
                    }
                }

                if (CurrentState == State.ServerTarget) {
                    TargetData target = TargetData.FromData(data);

                    if (target.Flags == 0x03) {
                        Trace.WriteLine("Cancel-target packet passed to client.", "UIManager");
                        return CallbackResult.Normal;
                    }
                    else {
                        Trace.WriteLine("Warning: Server target updated without previous cancellation.", "UIManager");
                        clientTarget = target;
                        return CallbackResult.Normal;
                    }
                }

                if (CurrentState != State.Ready) {
                    Reset();
                }

                Debug.Assert(CurrentState == State.Ready, "CurrentState is not Ready. Internal error.");

                clientTarget = TargetData.FromData(data);
                CurrentState = State.ServerTarget;
            }
            return CallbackResult.Normal;
        }

        private static CallbackResult OnClientTarget(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                switch (CurrentState) {
                    case State.ServerTarget:
                        clientTarget = null;
                        FinishWork();
                        return CallbackResult.Normal;

                    case State.Target:
                        TargetData target = TargetData.FromData(data);

                        if (target.TargetId == clientTarget.TargetId && target.Type <= clientTarget.Type) {
                            clientTarget.Serial = target.Serial;
                            clientTarget.X = target.X;
                            clientTarget.Y = target.Y;
                            clientTarget.Z = target.Z;
                            clientTarget.Graphic = target.Graphic;
                        }
                        else {
                            Trace.WriteLine("Incorrect target received from client.", "UIManager");
                        }

                        operationResult.Event.Set();
                        operationResult = null;
                        clientTarget = null;
                        FinishWork();
                        return CallbackResult.Eat;

                    default:
                        Trace.WriteLine("Dropped unexpected client target.", "UIManager");
                        return CallbackResult.Eat;
                }
            }
        }

        private static CallbackResult OnServerTargetMulti(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                TargetData td = new TargetData();
                td.Type = data[1];
                td.TargetId = ByteConverter.BigEndian.ToUInt32(data, 2);
                ushort multi = ByteConverter.BigEndian.ToUInt16(data, 18);

                Trace.WriteLine("Server target \"Place Multi 0x" + multi.ToString("X4") + "\" received.", "UIManager");

                // Simulate standart target packet so i don't have to handle it specially :)
                return OnServerTarget(td.ToData(), prevResult);
            }
        }

        #endregion

        #region ObjectPicker handling

        private static CallbackResult OnObjectPicker(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                if (CurrentState == State.WaitMenu) {
                    Debug.Assert(waitQueue != null && waitQueue.Count > 0, "Empty waitQueue.");
                    Debug.Assert(waitQueue.Peek().IsMenu, "Target in top of the queue instead of menu.");

                    Menu packet = new Menu(data);
                    WaitQuery query = waitQueue.Dequeue();
                    MenuSelection menu = query.Menu;

                    if (packet.Title.ToLowerInvariant().Contains(menu.Name.ToLowerInvariant())) {
                        // Cancel menu
                        if (menu.Option == null) {
                            byte[] selectedData = PacketBuilder.ObjectPicked(packet.DialogSerial, packet.MenuSerial, 0, 0, 0);
                            Core.SendToServer(selectedData);

                            FinishWork();
                            query.Finish(RequestState.Completed);
                            return CallbackResult.Eat;
                        }

                        // Select option
                        string option = menu.Option.ToLowerInvariant();
                        for (int i = 0; i < packet.Options.Count; i++) {
                            if (packet.Options[i].Text.ToLowerInvariant().Contains(option)) {
                                byte[] selectedData = PacketBuilder.ObjectPicked(packet.DialogSerial, packet.MenuSerial, (ushort)(i + 1), packet.Options[i].Artwork, packet.Options[i].Hue);
                                Core.SendToServer(selectedData);

                                FinishWork();
                                query.Finish(RequestState.Completed);
                                return CallbackResult.Eat;
                            }
                        }

                        Trace.WriteLine("Unable to find requested option. Menu passed to client.", "UIManager");
                    }
                    else {
                        Trace.WriteLine("Unexpected menu received. Menu passed to client.", "UIManager");
                    }

                    query.Finish(RequestState.Failed);
                    FinishWork();
                }

                if (CurrentState != State.Ready) {
                    Reset();
                }

                return CallbackResult.Normal;
            }
        }

        #endregion

        #region Client actions handling (useskill, useobject, ...)

        private static CallbackResult OnObjectDoubleClick(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                if (CurrentState != State.Ready && CurrentState != State.WaitTarget && CurrentState != State.WaitMenu) {
                    Reset();
                }

                return CallbackResult.Normal;
            }
        }

        private static CallbackResult OnUse(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                byte command = data[3];

                if (command == 0x24 || command == 0x56 || command == 0x58) {
                    if (CurrentState != State.Ready && CurrentState != State.WaitTarget && CurrentState != State.WaitMenu) {
                        Reset();
                    }
                }

                return CallbackResult.Normal;
            }
        }

        private static CallbackResult OnGeneralCommand(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                ushort subcommand = ByteConverter.BigEndian.ToUInt16(data, 3);

                if (subcommand == 0x0A || subcommand == 0x09 || subcommand == 0x1C) {
                    if (CurrentState != State.Ready && CurrentState != State.WaitTarget && CurrentState != State.WaitMenu) {
                        Reset();
                    }
                }

                return CallbackResult.Normal;
            }
        }

        #endregion

        #region Phoenix-controlled actions

        public static bool MoveItem(uint serial, ushort amount, ushort x, ushort y, sbyte z, uint container)
        {
            if (!Core.LoggedIn)
                return false;

            if (serial == 0 || serial == UInt32.MaxValue)
                return false;

            if (container == 0) container = UInt32.MaxValue;
            if (amount == 0) amount = UInt16.MaxValue;

            OperationResult result = new OperationResult();

            bool packetsSent = false;
            while (!packetsSent) {
                readySignal.WaitOne();

                lock (syncRoot) {
                    if (CurrentState == State.Ready) {
                        Core.SendToServer(PacketBuilder.ItemPickupRequest(serial, amount));
                        Core.SendToServer(PacketBuilder.ItemDropRequest(serial, x, y, z, container));

                        operationResult = result;
                        PickedUpItem = serial;

                        CurrentState = State.MoveItem;
                        BeginTimeout(PhoenixMoveItemTimeout);

                        packetsSent = true;
                    }
                }
            }

            result.Event.WaitOne();
            return result.Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial">Equipped item serial.</param>
        /// <param name="character">Serial of character that is item equipped to.</param>
        /// <param name="layer">Layer to what item should be equipped. Mostly ignored by servers.</param>
        /// <returns></returns>
        public static bool EquipItem(uint serial, uint character, byte layer)
        {
            if (!Core.LoggedIn)
                return false;

            if (serial == 0 || serial == UInt32.MaxValue)
                return false;

            OperationResult result = new OperationResult();

            bool packetsSent = false;
            while (!packetsSent) {
                readySignal.WaitOne();

                lock (syncRoot) {
                    if (CurrentState == State.Ready) {
                        Core.SendToServer(PacketBuilder.ItemPickupRequest(serial, 1));
                        Core.SendToServer(PacketBuilder.ItemEquipRequest(serial, character, layer));

                        operationResult = result;
                        PickedUpItem = serial;

                        CurrentState = State.MoveItem;
                        BeginTimeout(PhoenixMoveItemTimeout);

                        packetsSent = true;
                    }
                }
            }

            result.Event.WaitOne();
            return result.Success;
        }

        public static Serial TargetObject()
        {
            if (!Core.LoggedIn)
                return Serial.Invalid;

            TargetData info = new TargetData();
            info.TargetId = (uint)new Random().Next();

            OperationResult result = new OperationResult();

            lock (syncRoot) {
                if (CurrentState != State.Ready) {
                    Reset();
                }

                Debug.Assert(CurrentState == State.Ready, "CurrentState is not Ready. Internal error.");

                clientTarget = info;
                operationResult = result;
                CurrentState = State.Target;
                BeginTimeout(PhoenixTargetTimeout);

                Core.SendToClient(info.ToData());
            }

            result.Event.WaitOne();

            if (info.Serial == 0)
                info.Serial = Serial.Invalid;

            Debug.WriteLine("Client returned object target. Serial=0x" + info.Serial.ToString("X8"), "UIManager");
            return info.Serial;
        }

        public static StaticTarget Target()
        {
            if (!Core.LoggedIn)
                return null;

            TargetData info = new TargetData();
            info.Type = 1;
            info.TargetId = (uint)new Random().Next();
            info.X = 0xFFFF;
            info.Y = 0xFFFF;

            OperationResult result = new OperationResult();

            lock (syncRoot) {
                if (CurrentState != State.Ready) {
                    Reset();
                }

                Debug.Assert(CurrentState == State.Ready, "CurrentState is not Ready. Internal error.");

                Core.SendToClient(info.ToData());

                clientTarget = info;
                operationResult = result;
                CurrentState = State.Target;
                BeginTimeout(PhoenixTargetTimeout);
            }

            result.Event.WaitOne();

            Debug.WriteLine(String.Format("Client returned target: Serial=0x{0:X8} X={1} Y={2} Z={3} Graphic=0x{4:X4}",
                                          info.Serial, info.X, info.Y, info.Z, info.Graphic), "UIManager");

            return new StaticTarget(info.Serial, info.X, info.Y, info.Z, info.Graphic);
        }

        public static IRequestResult WaitForTarget(IClientTarget target)
        {
            if (!Core.LoggedIn)
                return UIManager.FailedResult;

            IClientTarget info = (IClientTarget)target.Clone();

            lock (syncRoot) {
                if ((CurrentState != State.WaitMenu && CurrentState != State.WaitTarget) ||
                    waitQueue == null || waitQueue.Count == 0 ||
                    waitQueue.Peek().ThreadId != Thread.CurrentThread.ManagedThreadId) {
                    Reset();

                    waitQueue = new Queue<WaitQuery>();
                    CurrentState = State.WaitTarget;
                }

                Debug.Assert(waitQueue != null, "waitQueue == null");

                WaitQuery query = new WaitQuery(info);
                waitQueue.Enqueue(query);

                Debug.WriteLine("Target enqueued.", "UIManager");

                BeginTimeout(WaitTargetTimeout);

                query.State = RequestState.Pending;
                return query;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial">Object serial or zero to specify ground.</param>
        /// <param name="x">X position on ground. Ignored when nonzero serial.</param>
        /// <param name="y">Y position on ground. Ignored when nonzero serial.</param>
        /// <param name="z">Z position on ground. Ignored when nonzero serial.</param>
        /// <param name="graphic">Graphic of static, or 0 for map tile. Ignored when nonzero serial.</param>
        public static IRequestResult WaitForTarget(uint serial, ushort x, ushort y, sbyte z, ushort graphic)
        {
            return WaitForTarget(new StaticTarget(serial, x, y, z, graphic));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menus"></param>
        /// <returns>Request result for last menu from this list that will be processed.</returns>
        public static IRequestResult WaitForMenu(params MenuSelection[] menus)
        {
            if (menus == null)
                throw new ArgumentNullException("menus");

            if (!Core.LoggedIn)
                return UIManager.FailedResult;

            if (menus.Length == 0) {
                WaitQuery query = new WaitQuery(null);
                query.Finish(RequestState.Completed);
                return query;
            }

            lock (syncRoot) {
                if ((CurrentState != State.WaitMenu && CurrentState != State.WaitTarget) ||
                    waitQueue == null || waitQueue.Count == 0 ||
                    waitQueue.Peek().ThreadId != Thread.CurrentThread.ManagedThreadId) {
                    Reset();

                    waitQueue = new Queue<WaitQuery>();
                    CurrentState = State.WaitMenu;
                }

                Debug.Assert(waitQueue != null, "waitQueue == null");

                WaitQuery query = null;
                for (int i = 0; i < menus.Length; i++) {
                    query = new WaitQuery(menus[i]);
                    waitQueue.Enqueue(query);
                }

                Debug.Assert(query != null);
                Debug.WriteLine(menus.Length.ToString() + " MenuSelection item(s) enqueued.", "UIManager");

                BeginTimeout(WaitMenuTimeout);

                query.Finish(RequestState.Pending);
                return query;
            }
        }

        #endregion

        public static void Reset()
        {
            ResetInternal(false);
        }

        internal static void ResetInternal(bool timeout)
        {
            lock (syncRoot) {
                switch (CurrentState) {
                    case State.Ready:
                        return;

                    case State.ClientPickedUpItem:
                    case State.ClientHoldingItem:
                        Core.SendToClient(PacketBuilder.PickupItemFailed(6));
                        Core.SendToServer(PacketBuilder.ItemDropRequest(PickedUpItem, 0xFFFF, 0xFFFF, 0, 0));

                        pickedUpItemName = null;
                        PickedUpItem = 0;
                        break;

                    case State.ClientDroppedItem:
                        pickedUpItemName = null;
                        PickedUpItem = 0;
                        break;

                    case State.ServerTarget:
                        Debug.Assert(clientTarget != null, "clientTarget != null");
                        clientTarget.X = 0xFFFF;
                        clientTarget.Y = 0xFFFF;
                        clientTarget.Z = 0;
                        clientTarget.Serial = 0;
                        clientTarget.Graphic = 0;
                        Core.SendToServer(clientTarget.ToData());
                        clientTarget.Flags = 0x03;
                        Core.SendToClient(clientTarget.ToData());
                        clientTarget = null;
                        break;

                    case State.MoveItem:
                        Debug.Assert(operationResult != null, "operationResult != null", CurrentState.ToString());
                        operationResult.Success = false;
                        operationResult.Event.Set();
                        operationResult = null;
                        PickedUpItem = 0;
                        break;

                    case State.Target:
                        Debug.Assert(operationResult != null, "operationResult != null", CurrentState.ToString());
                        operationResult.Event.Set();
                        operationResult = null;
                        clientTarget = null;
                        break;

                    case State.WaitTarget:
                        DeleteWaitQueue(timeout);
                        break;

                    case State.WaitMenu:
                        DeleteWaitQueue(timeout);
                        break;

                    default:
                        Trace.WriteLine("Unhandled state in UIManager.Cancel()!!! Please report this error.", "UIManager");
                        return;
                }

                if (timeout)
                    timeoutThread = null;
                else
                    StopTimeout();

                Debug.Assert(PickedUpItem == 0, "PickedUpItem == 0");
                Debug.Assert(pickedUpItemName == null, "pickedUpItemName == null");
                Debug.Assert(clientTarget == null, "clientTarget == null");
                Debug.Assert(operationResult == null, "operationResult == null");

                CurrentState = State.Ready;
                readySignal.Set();
            }
        }
    }
}
