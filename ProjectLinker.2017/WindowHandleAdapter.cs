using System;
using System.Windows.Forms;

namespace ProjectLinker
{
    public class WindowHandleAdapter : IWin32Window
    {
        public WindowHandleAdapter(IntPtr hwnd)
        {
            Handle = hwnd;
        }

        public IntPtr Handle { get; }
    }
}