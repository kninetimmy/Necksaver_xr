using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    /// <summary>
    /// Aggregated scanner for Joystick + Keyboard input
    /// </summary>
    public class JoystickKeyboardScanner : IDisposable
    {
        private JoystickButtonScanner _joystickScanner;
        private readonly JoystickKeyboardInput _result;
        private readonly int _maxPressedButtonsCount;
        private readonly HashSet<Keys> _excludedButtons = new HashSet<Keys>
        {
            Keys.Apps, Keys.Enter, Keys.Escape, Keys.LWin, Keys.RWin, Keys.Tab, Keys.CapsLock
        }; 

        public event Action<JoystickKeyboardInput, bool> OnCurrentlyPressedChanged;
        public event Action<JoystickKeyboardInput> BeforeRelesed;

        public JoystickKeyboardScanner(int maxPressedButtonsCount)
        {
            _maxPressedButtonsCount = maxPressedButtonsCount;
            _result = new JoystickKeyboardInput();
            _joystickScanner = new JoystickButtonScanner(_maxPressedButtonsCount);
            _joystickScanner.CurrentlyPressedChanged += OnJoystickPressedChanged;
            _joystickScanner.BeforeButtonReleased += OnBeforeJoystickButtonReleased;
            KeyInterceptor.KeyPressed += OnKeyPressed;
            _joystickScanner.StartScan();
        }

        private void OnKeyPressed(Keys[] pressedKeys)
        {
            var _filteredKeys = pressedKeys.Where(p => !_excludedButtons.Any(e => e == p)).ToArray();
            lock (_result)
            {
                var sameKeysPressed = _result.KeyboardKeys.Count == _filteredKeys.Length && _result.KeyboardKeys.All(k => _filteredKeys.Any(p => p.Equals(k)));
                var someKeyReleased = _result.KeyboardKeys.Count > _filteredKeys.Length && !_filteredKeys.Any(p => !_result.KeyboardKeys.Any(k => p != k));
                if (!sameKeysPressed && someKeyReleased)
                {
                    BeforeRelesed?.Invoke(_result);
                }
                if (AddKeys(_filteredKeys))
                {
                    // Console.WriteLine("OnCurrentlyPressedChanged: " + _result.ToString() + " Same key(s):" + sameKeysPressed);
                    OnCurrentlyPressedChanged?.Invoke(_result, sameKeysPressed);
                    return;
                }
                if (!sameKeysPressed)
                {
                    // Console.WriteLine("OnCurrentlyPressedChanged: " + _result.ToString());
                    OnCurrentlyPressedChanged?.Invoke(_result, sameKeysPressed);
                }
            }
            
        }

        private bool AddKeys(IEnumerable<Keys> pressedKeys)
        {
            var added = false;
            lock (_result)
            {
                _result.KeyboardKeys.Clear();
                foreach (var key in pressedKeys.Where(k => !_excludedButtons.Any(e => e == k)))
                {
                    if (IsLimitReached())
                    {
                        break;
                    }
                    added = true;
                    _result.KeyboardKeys.Add(key);
                }
            }
            return added;
        }

        private bool UpdateJoystickButtons(IEnumerable<JoyBut> buttons)
        {
            var updated = _result.JoystickButtons.Any() && !buttons.Any();
            lock (_result)
            {
                _result.JoystickButtons.Clear();
                foreach (var button in buttons)
                {
                    if (IsLimitReached())
                    {
                        break;
                    }
                    updated = true;
                    _result.JoystickButtons.Add(button);
                }
            }
            return updated;
        }

        private bool IsLimitReached()
        {
            return _result.KeyboardKeys.Count + _result.JoystickButtons.Count >= _maxPressedButtonsCount;
        }

        private void OnBeforeJoystickButtonReleased(List<JoyBut> buttons)
        {
            BeforeRelesed?.Invoke(_result);
        }

        private void OnJoystickPressedChanged(List<JoyBut> buttons)
        {
            if (UpdateJoystickButtons(buttons))
            {
                // Console.WriteLine("OnCurrentlyPressedChanged: " + _result.ToString());
                OnCurrentlyPressedChanged?.Invoke(_result, false);
            }
        }

        public void Dispose()
        {
            UnsubscribeAllHandlers();
            if (_joystickScanner != null)
            {
                _joystickScanner.Stop();

                _joystickScanner.Dispose();
                _joystickScanner = null;
            }
        }

        private void UnsubscribeAllHandlers()
        {
            KeyInterceptor.KeyPressed -= OnKeyPressed;
            BeforeRelesed?.GetInvocationList().ToList().ForEach(d => BeforeRelesed -= d as Action<JoystickKeyboardInput>);
            OnCurrentlyPressedChanged?.GetInvocationList().ToList().ForEach(d => OnCurrentlyPressedChanged -= d as Action<JoystickKeyboardInput, bool>);
        }
    }
}
