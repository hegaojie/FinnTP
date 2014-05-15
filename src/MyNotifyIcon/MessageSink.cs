using System;
using MyNotifyIcon.Interop;

namespace MyNotifyIcon
{
    public class MessageSink : IDisposable
    {
        private WindowProcedureHandler _msgHandler;

        public IntPtr MsgWinHandle { get; set; }

        public string WindowId { get; set; }

        public MessageSink()
        {
            WindowId = "WPFTaskbarIcon_" + DateTime.Now.Ticks;

            _msgHandler = OnWindowMessageReceived;

            WindowClass wc;

            wc.style = 0;
            wc.lpfnWndProc = _msgHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = "";
            wc.lpszClassName = WindowId;

            Win32Api.RegisterClass(ref wc);

            MsgWinHandle = Win32Api.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, 0, 0, 0, 0);
        }

        public event Action<MouseEvent> MouseEventReceived;

        private long OnWindowMessageReceived(IntPtr hwnd, uint umsg, uint wparam, uint lparam)
        {
            // TODO: handle events that raised in NotifyIcon area

            var msgId = (MessageType)lparam;
            switch (msgId)
            {
                case MessageType.MOUSE_RBUTTON_PRESS:
                    // TODO: show context menu
                    MouseEventReceived(MouseEvent.RightMouseDown);
                    break;

                case MessageType.MOUSE_RBUTTON_RELEASE:
                    break;

                case MessageType.MOUSE_LBUTTON_DOUBLE_CLICK:
                    MouseEventReceived(MouseEvent.LeftMouseDoubleClick);
                    break;

                case MessageType.MOUSE_LBUTTON_PRESS:
                    //TODO: show balloon tip
                    MouseEventReceived(MouseEvent.LeftMouseDown);
                    break;

                default:
                    break;
            }

            return Win32Api.DefWindowProc(hwnd, umsg, wparam, lparam);
        }

        public enum MessageType : uint
        {
            MOUSE_LBUTTON_PRESS = 0x0201,

            MOUSE_LBUTTON_RELEASE = 0x0202,

            MOUSE_LBUTTON_DOUBLE_CLICK = 0x0203,

            MOUSE_RBUTTON_PRESS = 0x0204,

            MOUSE_RBUTTON_RELEASE = 0x0205,

            MOUSE_RBUTTON_DOUBLE_CLICK = 0x0206,
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Win32Api.DestroyWindow(MsgWinHandle);
                _msgHandler = null;
            }
        }
    }
}