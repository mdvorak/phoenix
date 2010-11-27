using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Phoenix.Communication;

namespace Phoenix.WorldData
{
    public static class WalkHandling
    {
        private struct Step
        {
            public byte Direction;
            public byte Sequence;
            public byte OriginalSequence;
            public DateTime TimeStamp;
        }

        private static Queue<Step> stepStack;
        private static WorldLocation desiredPos;
        private static byte desiredDir;
        private static byte sequence;

        public static byte NextSequence
        {
            get
            {
                lock (World.SyncRoot) {
                    return sequence;
                }
            }
        }


        public static WorldLocation DesiredPosition
        {
            get
            {
                lock (World.SyncRoot) {
                    return desiredPos;
                }
            }
        }


        /// <summary>
        /// Called by WorldPacketHandler.Init()
        /// </summary>
        internal static void Init()
        {
            stepStack = new Queue<Step>();

            Core.RegisterClientMessageCallback(0x02, new MessageCallback(OnWalkRequest), CallbackPriority.High);
            Core.RegisterServerMessageCallback(0x21, new MessageCallback(OnWalkRequestFailed));
            Core.RegisterServerMessageCallback(0x22, new MessageCallback(OnWalkRequestSucceed));

            Core.Disconnected += new EventHandler(Core_Disconnected);
        }

        static void Core_Disconnected(object sender, EventArgs e)
        {
            ClearStack();
        }

        internal static void ClearStack()
        {
            lock (World.SyncRoot) {
                stepStack.Clear();

                desiredPos = new WorldLocation(World.RealPlayer.X, World.RealPlayer.Y, World.RealPlayer.Z);
                desiredDir = World.RealPlayer.Direction;
                sequence = 0;

                Debug.WriteLine("Step stack cleared.", "World");
            }
        }

        private static DateTime lastRequest = DateTime.Now;

        static CallbackResult OnWalkRequest(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x02) throw new Exception("Invalid packet passed to OnWalkRequest.");

#if DEBUG
                TimeSpan elapsed = DateTime.Now - lastRequest;
                lastRequest = DateTime.Now;
                //Debug.WriteLine("Since last walk request: " + elapsed.TotalMilliseconds, "Debug");
#endif

                // Update to proper sequence
                byte origSequence = data[2];

                //if (sequence != origSequence)
                //    Debug.WriteLine("Fixing walk sequence from " + data[2] + " to " + sequence + ".", "World");

                data[2] = sequence++;

                // Push to stack
                Step step = new Step {
                    Direction = data[1],
                    Sequence = data[2],
                    OriginalSequence = origSequence,
                    TimeStamp = DateTime.Now
                };

                // Don't allow more unconfirmed steps
                /*
                if (stepStack.Count > 4)
                {
                    Trace.WriteLine("Denying next step request.", "World");
                    step.Denied = true;
                }
                 * */

                stepStack.Enqueue(step);

                if ((desiredDir & 0x0F) == (data[1] & 0x0F)) {
                    switch (data[1] & 0x0F) {
                        case 0:
                            desiredPos.Y--;
                            break;
                        case 1:
                            desiredPos.Y--;
                            desiredPos.X++;
                            break;
                        case 2:
                            desiredPos.X++;
                            break;
                        case 3:
                            desiredPos.Y++;
                            desiredPos.X++;
                            break;
                        case 4:
                            desiredPos.Y++;
                            break;
                        case 5:
                            desiredPos.Y++;
                            desiredPos.X--;
                            break;
                        case 6:
                            desiredPos.X--;
                            break;
                        case 7:
                            desiredPos.Y--;
                            desiredPos.X--;
                            break;
                        default:
                            throw new ApplicationException("Invalid direction requested.");
                    }
                }

                desiredDir = data[1];

                /*
                if (!step.Denied)
                    return CallbackResult.Normal;
                else
                    return CallbackResult.Eat;
                 * */

                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnWalkRequestFailed(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x21) throw new Exception("Invalid packet passed to OnWalkRequestFailed.");

                World.RealPlayer.X = ByteConverter.BigEndian.ToUInt16(data, 2);
                World.RealPlayer.Y = ByteConverter.BigEndian.ToUInt16(data, 4);
                World.RealPlayer.Direction = data[6];
                World.RealPlayer.Z = (sbyte)data[7];

                ClearStack();

                WorldPacketHandler.objectCallbacks.InvokeAsync(new ObjectChangedEventArgs(World.PlayerSerial, ObjectChangeType.CharUpdated));
                return CallbackResult.Normal;
            }
        }

        static CallbackResult OnWalkRequestSucceed(byte[] data, CallbackResult prevResult)
        {
            lock (World.SyncRoot) {
                if (data[0] != 0x22) throw new Exception("Invalid packet passed to OnWalkRequestSucceed.");

                if (stepStack.Count == 0) {
                    Trace.WriteLine(String.Format("Walk stack is empty."), "World");
                    // Snad to tu nebude chybet :(
                    // UO.Resync();
                    return CallbackResult.Eat;
                }

                Step step = stepStack.Dequeue();

                // Validate sequence
                if (step.Sequence != data[1]) {
                    Trace.WriteLine(String.Format("Invalid walk sequence."), "World");
                    //UO.Resync();
                    // return CallbackResult.Eat;
                }

                // Restore sequence
                data[1] = step.OriginalSequence;

                if ((World.RealPlayer.Direction & 0x0F) == (step.Direction & 0x0F)) {
                    switch (step.Direction & 0x0F) {
                        case 0:
                            World.RealPlayer.Y--;
                            break;
                        case 1:
                            World.RealPlayer.Y--;
                            World.RealPlayer.X++;
                            break;
                        case 2:
                            World.RealPlayer.X++;
                            break;
                        case 3:
                            World.RealPlayer.Y++;
                            World.RealPlayer.X++;
                            break;
                        case 4:
                            World.RealPlayer.Y++;
                            break;
                        case 5:
                            World.RealPlayer.Y++;
                            World.RealPlayer.X--;
                            break;
                        case 6:
                            World.RealPlayer.X--;
                            break;
                        case 7:
                            World.RealPlayer.Y--;
                            World.RealPlayer.X--;
                            break;
                        default:
                            throw new ApplicationException("Invalid direction in step stack.");
                    }
                }

                World.RealPlayer.Direction = step.Direction;
                World.RealPlayer.Status = data[2];

                WorldPacketHandler.objectCallbacks.InvokeAsync(new ObjectChangedEventArgs(World.PlayerSerial, ObjectChangeType.CharUpdated));

                CallbackResult result = CallbackResult.Normal;
                /*
                if (step.Custom)
                {
                    // Send resync instead
                    PacketWriter w = new PacketWriter(0x20);
                    w.Write(World.PlayerSerial);
                    w.Write(World.RealPlayer.Graphic);
                    w.WriteBytes(0, 1);
                    w.Write(World.RealPlayer.Color);
                    w.Write(World.RealPlayer.Status);
                    w.Write(World.RealPlayer.X);
                    w.Write(World.RealPlayer.Y);
                    w.WriteBytes(0, 4);
                    w.Write(World.RealPlayer.Direction);
                    w.Write(World.RealPlayer.Z);

                    byte[] resync = w.GetBytes();
                    Core.SendToClient(resync);

                    result = CallbackResult.Sent;
                }

                // Handle forced denial
                if (stepStack.Count > 0 && stepStack.Peek().Denied)
                {
                    if (result == CallbackResult.Normal)
                    {
                        // Send to client now
                        Core.SendToClient(data);
                    }

                    // Send deny message
                    byte[] deny = Phoenix.Communication.PacketBuilder.WalkRequestFailed(stepStack.Dequeue().Sequence, (ushort)World.RealPlayer.X, (ushort)World.RealPlayer.Y, (byte)World.RealPlayer.Z, World.RealPlayer.Direction);
                    Core.SendToClient(deny, true);

                    return CallbackResult.Sent;
                }*/

                return result;
            }
        }

        /*
        public static byte SendWalkRequest(byte direction)
        {
            lock (World.SyncRoot)
            {
                byte[] data = new byte[7];
                data[0] = 0x2;
                data[1] = direction;s
                data[2] = sequence;

                Core.SendToServer(data);

                // FIXME
            }
        }
         */
    }
}
