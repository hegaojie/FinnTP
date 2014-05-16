using System.Runtime.InteropServices;
using MyNotifyIcon;

namespace FinnTorget
{
    public class Win32Api
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetCursorPos(ref Point lpPoint);
    }
}
