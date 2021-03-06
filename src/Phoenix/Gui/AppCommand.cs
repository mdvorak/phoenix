﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Gui
{
    public enum AppCommand : ushort
    {
        None = 0,
        BassBoost = 20,
        BassDown = 19,
        BassUp = 21,
        BrowserBackward = 1,
        BrowserFavorites = 6,
        BrowserForward = 2,
        BrowserHome = 7,
        BrowserRefresh = 3,
        BrowserSearch = 5,
        BrowserStop = 4,
        LaunchApp1 = 17,
        LaunchApp2 = 18,
        LaunchMail = 15,
        LaunchMediaSelect = 16,
        MediaNexttrack = 11,
        MediaPlayPause = 14,
        MediaPrevioustrack = 12,
        MediaStop = 13,
        TrebleDown = 22,
        TrebleUp = 23,
        VolumeDown = 9,
        VolumeMute = 8,
        VolumeUp = 10,
        MicrophoneVolumeMute = 24,
        MicrophoneVolumeDown = 25,
        MicrophoneVolumeUp = 26,
        Close = 31,
        Copy = 36,
        CorrectionList = 45,
        Cut = 37,
        DictateOrCommandControlToggle = 43,
        Find = 28,
        ForwardMail = 40,
        Help = 27,
        MediaChannelDown = 52,
        MediaChannelUp = 51,
        MediaFastforward = 49,
        MediaPause = 47,
        MediaPlay = 46,
        MediaRecord = 48,
        MediaRewind = 50,
        MicOnOffToggle = 44,
        New = 29,
        Open = 30,
        Paste = 38,
        Print = 33,
        Redo = 35,
        ReplyToMail = 39,
        Save = 32,
        SendMail = 41,
        SpellCheck = 42,
        Undo = 34,
        Delete = 53,
        DwmFlip3D = 54
    }
}
