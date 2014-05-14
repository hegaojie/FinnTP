using System;
using System.Runtime.InteropServices;
using FinnTorget.Interop;

namespace FinnTorget
{
    public class Win32Api
    {
        [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIcon")]
        public static extern bool Shell_NotifyIcon(IconMessageType dwMessage, [In] ref NotifyIconData lpdata);

        [DllImport("User32.dll" , EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass(ref WindowClass lpWndClass);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern ushort RegisterClassEx(ref WindowClassEx lpwcx);

        [DllImport("User32.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle, [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
                               [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName, int dwStyle, int x, int y,
                               int nWidth, int nHeight, uint hWndParent, int hMenu, int hInstance,
                               int lpParam);

        [DllImport("User32.dll", EntryPoint = "DefWindowProc")]
        public static extern long DefWindowProc(IntPtr hwnd, uint uMsg, uint wParam, uint lParam);

        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();

        /// <summary>
        /// Used to destroy the hidden helper window that receives messages from the
        /// taskbar icon.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);


        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }


    /// <summary>
    /// Marshal type of WNDPROC
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="uMsg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate IntPtr WindowProcedure(IntPtr hwnd, uint uMsg, UIntPtr wParam, IntPtr lParam);


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
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;
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

}