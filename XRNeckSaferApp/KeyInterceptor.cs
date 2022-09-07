using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public static class KeyInterceptor
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        
        private delegate IntPtr LowLevelKeyboardHandler(int nCode, IntPtr wParam, IntPtr lParam);

        public static event Action<Keys> KeyPressed;

        private static LowLevelKeyboardHandler _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static void SetHook()
        {
            _hookID = SetHook(_proc);
        }

        public static void RemoveHook()
        {
            UnsubscribeAllKeyPressedHandlers();
            UnhookWindowsHookEx(_hookID);
        }

        private static void UnsubscribeAllKeyPressedHandlers()
        {
            if (KeyPressed != null)
            {
                foreach (var invokerDelegate in KeyPressed.GetInvocationList())
                {
                    KeyPressed -= (invokerDelegate as Action<Keys>);
                }
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardHandler proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            {
                using (var curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                var vkCode = Marshal.ReadInt32(lParam);
                KeyPressed?.Invoke((Keys)vkCode);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardHandler lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
