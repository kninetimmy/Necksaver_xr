using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public static class KeyInterceptor
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private static readonly IntPtr WM_KEYDOWN_POINER = (IntPtr)WM_KEYDOWN;
        private static readonly IntPtr WM_KEYUP_POINER = (IntPtr)WM_KEYUP;
        private static readonly List<Keys> _pressedKeys = new List<Keys>();

        private delegate IntPtr LowLevelKeyboardHandler(int nCode, IntPtr wParam, IntPtr lParam);

        public static event Action<Keys[]> KeyPressed;

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
                    KeyPressed -= (invokerDelegate as Action<Keys[]>);
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
            var keyDown = wParam == WM_KEYDOWN_POINER;
            var keyUp = wParam == WM_KEYUP_POINER;
            if (nCode >= 0 && (keyDown || keyUp))
            {
                var key = (Keys)Marshal.ReadInt32(lParam);
                lock (_pressedKeys)
                {
                    if (keyDown && !_pressedKeys.Contains(key))
                    {
                        _pressedKeys.Add(key);
                    }
                    if (keyUp && _pressedKeys.Contains(key))
                    {
                        _pressedKeys.RemoveAll(k => k == key);
                    }
                }
                // LogPressedKeys(_pressedKeys);
                KeyPressed?.Invoke(_pressedKeys.ToArray());
                
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        //private static void LogPressedKeys(List<Keys> keys)
        //{
        //    if (keys.Count == 0)
        //    {
        //        return;
        //    }
        //    var builder = new StringBuilder();
        //    for (var i = 0; i < keys.Count; i++)
        //    {
        //        if (i > 0)
        //        {
        //            builder.Append("+");
        //        }
        //        builder.Append(keys[i]);
        //    }
        //    Console.WriteLine(builder.ToString());
        //}

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
