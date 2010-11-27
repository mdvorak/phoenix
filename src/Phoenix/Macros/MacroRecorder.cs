using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using Phoenix.Collections;

namespace Phoenix.Macros
{
    public class MacroRecorder : Component, IDisposable
    {
        private readonly object syncRoot = new object();
        private Macro commands;
        private bool recording = false;

        private int pause = 500;
        private bool pendingPause = false;

        [Category("Recorder")]
        [Description("Raised when recording is started or stopped.")]
        public event EventHandler RecordingChanged;

        public MacroRecorder()
        {
            commands = (Macro)new Macro().CreateSynchronized();
            commands.ListCleared += new EventHandler(commands_ListCleared);
        }

        public MacroRecorder(Macro commands)
        {
            this.commands = commands;
            commands.ListCleared += new EventHandler(commands_ListCleared);
        }

        [Browsable(false)]
        public Macro Commands
        {
            get { return commands; }
        }

        /// <summary>
        /// Gets or sets pause that is automaticly inserted after active commands.
        /// </summary>
        [Category("Recorder")]
        [Description("Gets or sets pause that is automaticly inserted after active commands.")]
        public int Pause
        {
            get { return pause; }
            set { pause = value; }
        }

        [Browsable(false)]
        public bool Recording
        {
            get { return recording; }
            protected set
            {
                if (value != recording) {
                    recording = value;
                    OnRecordingChanged(EventArgs.Empty);
                }
            }
        }

        void commands_ListCleared(object sender, EventArgs e)
        {
            pendingPause = false;
        }

        protected virtual void OnRecordingChanged(EventArgs e)
        {
            SyncEvent.BeginInvoke(RecordingChanged, this, e);
        }

        public void Start()
        {
            lock (syncRoot) {
                if (Recording)
                    throw new InvalidOperationException("Recorder is already running.");

                Recording = true;

                foreach (byte id in PacketTranslator.GetKnownPackets()) {
                    Core.RegisterClientMessageCallback(id, new MessageCallback(PacketHandler));
                }
            }
        }

        public void Stop()
        {
            lock (syncRoot) {
                foreach (byte id in PacketTranslator.GetKnownPackets()) {
                    Core.UnregisterClientMessageCallback(id, new MessageCallback(PacketHandler));
                }

                Recording = false;
            }
        }

        private CallbackResult PacketHandler(byte[] data, CallbackResult prevResult)
        {
            lock (syncRoot) {
                IMacroCommand cmd = PacketTranslator.Translate(data);
                if (cmd != null) {
                    AddCommand(cmd);
                }
            }

            return CallbackResult.Normal;
        }

        private int FindLastActiveCommand()
        {
            lock (commands.SyncRoot) {
                for (int i = commands.Count - 1; i >= 0; i--) {
                    if (commands[i].CommandType == MacroCommandType.Active) {
                        return i;
                    }
                }

                return -1;
            }
        }

        public void AddCommand(IMacroCommand command)
        {
            lock (commands.SyncRoot) {
                int index = -1;

                if (command.CommandType == MacroCommandType.PrecedingActive) {
                    index = FindLastActiveCommand();
                }

                InsertCommand(index, command);
            }
        }

        public void InsertCommand(int index, IMacroCommand command)
        {
            lock (commands.SyncRoot) {
                if (index >= 0) {
                    // Insert item
                    commands.Insert(index++, command);

                    if (pause > 0 && command.CommandType == MacroCommandType.Active) {
                        IMacroCommand waitCmd = new WaitMacroCommand(pause);

                        if (index >= 0)
                            commands.Insert(index, waitCmd);
                        else
                            commands.Add(waitCmd);
                    }
                }
                else {
                    // Add item
                    if (pendingPause && pause > 0) {
                        IMacroCommand waitCmd = new WaitMacroCommand(pause);
                        commands.Add(waitCmd);
                        pendingPause = false;
                    }

                    commands.Add(command);

                    if (command.CommandType == MacroCommandType.Active)
                        pendingPause = true;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (syncRoot) {
                Stop();

                commands = null;
            }

            base.Dispose(disposing);
        }
    }
}
