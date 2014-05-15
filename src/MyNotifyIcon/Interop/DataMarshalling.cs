using System;
using System.Runtime.InteropServices;

namespace MyNotifyIcon.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// Marshal type of WNDCLASSEX
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClassEx
    {
        public uint cbSize;
        public uint style;
        public WindowProcedureHandler lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    /// <summary>
    /// Win API WNDCLASS struct - represents a single window.
    /// Used to receive window messages.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowClass
    {
        public uint style;
        public WindowProcedureHandler lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NotifyIconData
    {
        public uint cbSize;

        public IntPtr hWnd;

        public uint uID;

        public NotifyIconDisplayMode uFlags;

        public uint uCallbackMessage;

        public IntPtr hIcon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szTip;

        public StateMode dwState;

        public StateMode dwStateMask;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;

        public uint uTimeoutOruVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;

        public InfoFlag dwInfoFlags;

        public Guid guidItem;

        public IntPtr hBalloonIcon;
    }

    public enum MouseEvent
    {
        RightMouseUp,

        RightMouseDown,

        LeftMouseDoubleClick,

        LeftMouseDown,

        LeftMouseUp,
    }

    [Flags]
    public enum NotifyIconDisplayMode : uint
    {
        NIF_MESSAGE = 0x01,
        NIF_ICON = 0x02,
        NIF_TIP = 0x04,
        NIF_STATE = 0x08,
        NIF_INFO = 0x10,
        NIF_GUID = 0x20,
        NIF_REALTIME = 0x40,
        NIF_SHOWTIP = 0x80
    }

    public enum StateMode : uint
    {
        NIS_HIDDEN = 0x01,
        NIS_SHAREDICON = 0x02
    }

    public enum InfoFlag : uint
    {
        NIIF_NONE = 0x00000000,
        NIIF_INFO = 0x00000001,
        NIIF_WARNING = 0x00000002,
        NIIF_ERROR = 0x00000003,
        NIIF_USER = 0x00000004,
        NIIF_NOSOUND = 0x00000010,
        NIIF_LARGE_ICON = 0x00000020,
        NIIF_RESPECT_QUIET_TIME = 0x00000080,
        NIIF_ICON_MASK = 0x0000000F
    }

    public enum IconMessageType : uint
    {
        NIM_ADD = 0x00000000,
        NIM_MODIFY = 0x00000001,
        NIM_DELETE = 0x00000002,
        NIM_SETFOCUS = 0x00000003,
        NIM_SETVERSION = 0x00000004
    }
}
