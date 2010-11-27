using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Phoenix.Gui.Controls
{
    [ToolboxBitmap(typeof(RichTextBox))]
    public class RichTextBoxEx : RichTextBox
    {
        public struct State
        {
            public int HScroll;
            public int VScroll;
            public int SelectionStart;
            public int SelectionLenght;
        }

        #region Imported native structures and functions

        protected static class Native
        {
            public enum ScrollBarTypes
            {
                SB_HORZ = 0,
                SB_VERT = 1,
                SB_CTL = 2,
                SB_BOTH = 3
            }

            [Flags]
            public enum ScrollBarInfoFlags : uint
            {
                SIF_RANGE = 0x1,
                SIF_PAGE = 0x2,
                SIF_POS = 0x4,
                SIF_DISABLENOSCROLL = 0x8,
                SIF_TRACKPOS = 0x10,
                SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS)
            }

            [Flags]
            public enum ScrollBarFlags
            {
                SBS_HORZ = 0x0,
                SBS_VERT = 0x1,
                SBS_TOPALIGN = 0x2,
                SBS_LEFTALIGN = 0x2,
                SBS_BOTTOMALIGN = 0x4,
                SBS_RIGHTALIGN = 0x4,
                SBS_SIZEBOXTOPLEFTALIGN = 0x2,
                SBS_SIZEBOXBOTTOMRIGHTALIGN = 0x4,
                SBS_SIZEBOX = 0x8,
                SBS_SIZEGRIP = 0x10
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct CharFormat2
            {
                public Int32 cbSize;
                public Int32 dwMask;
                public Int32 dwEffects;
                public Int32 yHeight;
                public Int32 yOffset;
                public Int32 crTextColor;
                public Byte bCharSet;
                public Byte bPitchAndFamily;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
                public String szFaceName;
                public Int16 wWeight;
                public Int16 sSpacing;
                public Int32 crBackColor;
                public Int32 lcid;
                public Int32 dwReserved;
                public Int16 sStyle;
                public Int16 wKerning;
                public Byte bUnderlineType;
                public Byte bAnimation;
                public Byte bRevAuthor;
                public Byte bReserved1;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct SCROLLINFO
            {
                public uint cbSize;
                public ScrollBarInfoFlags fMask;
                public int nMin;
                public int nMax;
                public uint nPage;
                public int nPos;
                public int nTrackPos;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int x;
                public int y;

                public POINT(int x, int y)
                {
                    this.x = x;
                    this.y = y;
                }
            }

            public const int WM_PAINT = 0x000F;
            public const int LF_FACESIZE = 32;
            public const int CFM_COLOR = 0x40000000;
            public const int CFM_BACKCOLOR = 0x04000000;
            public const int CFE_AUTOCOLOR = CFM_COLOR;
            public const int CFE_AUTOBACKCOLOR = CFM_BACKCOLOR;
            public const int WM_USER = 0x0400;
            public const int EM_SETCHARFORMAT = (WM_USER + 68);
            public const int EM_SETBKGNDCOLOR = (WM_USER + 67);
            public const int EM_GETCHARFORMAT = (WM_USER + 58);
            public const int WM_SETTEXT = 0xC;
            public const int SCF_SELECTION = 0x1;
            public const int EM_SETSCROLLPOS = (WM_USER + 222);
            public const int WM_APP = 0x8000;
            public const int WM_UPDATECONTENTS = (WM_APP + 1);

            public const int SB_HORZ = 0;
            public const int SB_VERT = 1;
            public const int SB_CTL = 2;
            public const int SB_BOTH = 3;

            [DllImport("user32.dll", EntryPoint = "PostMessageA")]
            public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

            [DllImport("user32.dll", EntryPoint = "SendMessageA")]
            public extern static bool SendMessage(IntPtr hWnd, int Msg, int wParam, ref CharFormat2 lParam);

            [DllImport("user32.dll", EntryPoint = "SendMessageA")]
            public extern static bool SendMessage(IntPtr hWnd, int Msg, int wParam, ref POINT lParam);

            [DllImport("user32.dll")]
            public extern static bool GetScrollInfo(IntPtr hWnd, ScrollBarTypes fnBar, ref SCROLLINFO lpsi);

            [DllImport("user32.dll")]
            public extern static bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

            [DllImport("user32.dll")]
            public extern static bool LockWindowUpdate(IntPtr hWnd);

            public static int GetScrollBarPos(IntPtr hWnd, ScrollBarTypes BarType)
            {
                SCROLLINFO Info = new SCROLLINFO();
                Info.fMask = ScrollBarInfoFlags.SIF_POS;
                Info.cbSize = (uint)Marshal.SizeOf(Info);
                GetScrollInfo(hWnd, BarType, ref Info);
                return Info.nPos;
            }
        }

        #endregion

        private int windownLock = 0;

        protected override void WndProc(ref Message m)
        {
            if (windownLock > 0)
            {
                if (m.Msg == Native.EM_SETSCROLLPOS || m.Msg == Native.WM_PAINT)
                    return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Gets horizontal scrollbar position.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(0)]
        public int HorizontalScrollPos
        {
            get
            {
                return Native.GetScrollBarPos(Handle, Native.ScrollBarTypes.SB_HORZ);
            }
        }

        /// <summary>
        /// Gets vertical scrollbar position.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(0)]
        public int VerticalScrollPos
        {
            get
            {
                return Native.GetScrollBarPos(Handle, Native.ScrollBarTypes.SB_VERT);
            }
        }

        /// <summary>
        /// Scrolls control scrollbars to specified positions.
        /// </summary>
        /// <param name="x">Horizontal scrollbar position.</param>
        /// <param name="y">Vertical scrollbar position.</param>
        public void SetScrollPos(int x, int y)
        {
            Native.POINT pt = new Native.POINT(x, y);
            Native.SendMessage(Handle, Native.EM_SETSCROLLPOS, 0, ref pt);
        }

        /// <summary>
        /// Sets color of specified text part. Doesn't return cursor to original position!
        /// </summary>
        /// <param name="startIndex">First colored character.</param>
        /// <param name="length">Colored text length.</param>
        /// <param name="color">New text color.</param>
        public void SetColor(int startIndex, int length, System.Drawing.Color color)
        {
            SelectionStart = startIndex;
            SelectionLength = length;

            Native.CharFormat2 format = new Native.CharFormat2();
            format.cbSize = Marshal.SizeOf(format);
            format.dwMask = Native.CFM_COLOR;
            format.crTextColor = ColorTranslator.ToOle(color);

            Native.SendMessage(Handle, Native.EM_SETCHARFORMAT, Native.SCF_SELECTION, ref format);
        }

        /// <summary>
        /// Sets back color of specified text part. Doesn't return cursor to original position!
        /// </summary>
        /// <param name="startIndex">First colored character.</param>
        /// <param name="length">Colored text length.</param>
        /// <param name="color">New text color.</param>
        public void SetBackColor(int startIndex, int length, System.Drawing.Color color)
        {
            SelectionStart = startIndex;
            SelectionLength = length;

            Native.CharFormat2 format = new Native.CharFormat2();
            format.cbSize = Marshal.SizeOf(format);
            format.dwMask = Native.CFM_BACKCOLOR;
            format.crTextColor = ColorTranslator.ToOle(color);

            Native.SendMessage(Handle, Native.EM_SETCHARFORMAT, Native.SCF_SELECTION, ref format);
        }

        /// <summary>
        /// Resets color of specified text part to original. Doesn't return cursor to original position!
        /// </summary>
        /// <param name="startIndex">First character.</param>
        /// <param name="length">Text length.</param>
        public void ResetColor(int startIndex, int length)
        {
            SelectionStart = startIndex;
            SelectionLength = length;

            Native.CharFormat2 format = new Native.CharFormat2();
            format.cbSize = Marshal.SizeOf(format);
            format.dwMask = Native.CFM_COLOR;
            format.dwEffects = Native.CFE_AUTOCOLOR;

            Native.SendMessage(Handle, Native.EM_SETCHARFORMAT, Native.SCF_SELECTION, ref format);
        }

        /// <summary>
        /// Resets back color of specified text part to original. Doesn't return cursor to original position!
        /// </summary>
        /// <param name="startIndex">First character.</param>
        /// <param name="length">Text length.</param>
        public void ResetBackColor(int startIndex, int length)
        {
            SelectionStart = startIndex;
            SelectionLength = length;

            Native.CharFormat2 format = new Native.CharFormat2();
            format.cbSize = Marshal.SizeOf(format);
            format.dwMask = Native.CFM_BACKCOLOR;
            format.dwEffects = Native.CFE_AUTOBACKCOLOR;

            Native.SendMessage(Handle, Native.EM_SETCHARFORMAT, Native.SCF_SELECTION, ref format);
        }

        /// <summary>
        /// Resets both fore and back color of specified text part to original. Doesn't return cursor to original position!
        /// </summary>
        /// <param name="startIndex">First character.</param>
        /// <param name="length">Text length.</param>
        public void ResetColors(int startIndex, int length)
        {
            SelectionStart = startIndex;
            SelectionLength = length;

            Native.CharFormat2 format = new Native.CharFormat2();
            format.cbSize = Marshal.SizeOf(format);
            format.dwMask = Native.CFM_BACKCOLOR | Native.CFM_COLOR;
            format.dwEffects = Native.CFE_AUTOBACKCOLOR | Native.CFE_AUTOCOLOR;

            Native.SendMessage(Handle, Native.EM_SETCHARFORMAT, Native.SCF_SELECTION, ref format);
        }

        public void LockWindowUpdate()
        {
            windownLock++;
        }

        public void UnlockWindowUpdate()
        {
            if (windownLock > 0)
                windownLock--;
        }

        public State GetState()
        {
            State p = new State();

            p.HScroll = HorizontalScrollPos;
            p.VScroll = VerticalScrollPos;
            p.SelectionStart = SelectionStart;
            p.SelectionLenght = SelectionLength;

            return p;
        }

        public void SetState(State state)
        {
            SetScrollPos(state.HScroll, state.VScroll);
            Select(state.SelectionStart, state.SelectionLenght);
        }

        public void ScrollToBottom()
        {
            int min, max;
            if (Native.GetScrollRange(Handle, Native.SB_VERT, out min, out max))
            {
                int pos = max - Height + Font.Height * 2;
                SetScrollPos(0, Math.Max(min, Math.Min(pos, max)));
            }
            else
            {
                Debug.WriteLine("Unable to get scroll bar range.", "Gui");
            }
        }
    }
}
