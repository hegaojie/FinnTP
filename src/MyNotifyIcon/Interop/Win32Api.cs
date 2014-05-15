using System;
using System.Runtime.InteropServices;
using MyNotifyIcon.Interop;

namespace MyNotifyIcon
{
    public class Win32Api
    {
        [DllImport("shell32.dll", EntryPoint = "Shell_NotifyIcon")]
        public static extern bool Shell_NotifyIcon(IconMessageType dwMessage, [In] ref NotifyIconData lpdata);

        [DllImport("User32.dll", EntryPoint = "RegisterClassW", SetLastError = true)]
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
    /// Callback delegate which is used by the Windows API to
    /// submit window messages.
    /// </summary>
    public delegate long WindowProcedureHandler(IntPtr hwnd, uint uMsg, uint wparam, uint lparam);
}