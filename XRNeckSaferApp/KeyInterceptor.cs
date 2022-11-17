using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public enum KeyEvents
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        SKeyDown = 0x0104,
        SKeyUp = 0x0105
    }

    public static class KeyInterceptor
    {
        private static readonly ILogger _logger = LogManager.GetLogger(nameof(KeyInterceptor));
        private const int WH_KEYBOARD_LL = 13;
        private static readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();
        private delegate IntPtr LowLevelKeyboardHandler(int nCode, IntPtr wParam, IntPtr lParam);

        public static event Action<Keys[]> KeyPressed;
        public static event Action<Keys[]> BeforeKeyReleased;

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

        public static bool CheckPressed(params Keys[] keysToCheck)
        {
            return _pressedKeys.CheckMatch(keysToCheck);
        }

        private static void UnsubscribeAllKeyPressedHandlers()
        {
            if (KeyPressed != null)
            {
                foreach (var invokerDelegate in KeyPressed?.GetInvocationList())
                {
                    KeyPressed -= invokerDelegate as Action<Keys[]>;
                }
            }
            if (BeforeKeyReleased != null)
            {
                foreach (var invokerDelegate in BeforeKeyReleased?.GetInvocationList())
                {
                    BeforeKeyReleased -= invokerDelegate as Action<Keys[]>;
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
            KeyEvents kEvent = (KeyEvents)wParam;
            var keyDown = kEvent == KeyEvents.KeyDown || kEvent == KeyEvents.SKeyDown;
            var keyUp = kEvent == KeyEvents.KeyUp || kEvent == KeyEvents.SKeyUp;
            if (nCode >= 0 && (keyDown || keyUp))
            {
                var key = (Keys)Marshal.ReadInt32(lParam);
                lock (_pressedKeys)
                {
                    if (keyDown)
                    {
                        _pressedKeys.Add(key);
                    }
                    if (keyUp)
                    {
                        BeforeKeyReleased?.Invoke(_pressedKeys.ToArray());
                        _pressedKeys.Remove(key);
                    }
                }
                LogPressedKeys(_pressedKeys);
                KeyPressed?.Invoke(_pressedKeys.ToArray());
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void LogPressedKeys(HashSet<Keys> keys)
        {
            if (keys.Count == 0)
            {
                return;
            }
            var builder = new StringBuilder();
            foreach (var key in keys)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                builder.Append(key);
            }
            _logger.Trace(builder.ToString());
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
