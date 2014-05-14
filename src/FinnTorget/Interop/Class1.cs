using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FinnTorget.Interop
{
    /// <summary>
    /// Callback delegate which is used by the Windows API to
    /// submit window messages.
    /// </summary>
    public delegate long WindowProcedureHandler(IntPtr hwnd, uint uMsg, uint wparam, uint lparam);


}
