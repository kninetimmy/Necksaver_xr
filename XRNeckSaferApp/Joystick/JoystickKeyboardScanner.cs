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
        private bool _joystickScanComplete = false;
        private bool _keyboardScanComplete = false;
        private readonly int _maxPressedButtonsCount;
        private readonly HashSet<Keys> _excludedButtons = new HashSet<Keys>
        {
            Keys.Apps, Keys.Space, Keys.Enter, Keys.Escape, Keys.LWin, Keys.RWin, Keys.Tab, Keys.CapsLock
        }; 

        public event Action<JoystickKeyboardInput> OnScanningComplete;
        public event Action<JoystickKeyboardInput, bool> OnCurrentlyPressedChanged;

        public JoystickKeyboardScanner(int maxPressedButtonsCount)
        {
            _maxPressedButtonsCount = maxPressedButtonsCount;
            _result = new JoystickKeyboardInput();
            _joystickScanner = new JoystickButtonScanner(_maxPressedButtonsCount);
            _joystickScanner.OnCurrentlyPressedChanged += OnJoystickPressedChanged;
            _joystickScanner.OnScanningComplete += OnJoystickScanningComplete;
            KeyInterceptor.KeyPressed += OnKeyPressed;
            _joystickScanner.StartScan();
        }

        private void OnKeyPressed(Keys[] pressedKeys)
        {
            if (_keyboardScanComplete || _joystickScanComplete || _excludedButtons.Any(b => pressedKeys.Any(p => p.Equals(b))))
            {
                return;
            }
            lock (_result)
            {
                var sameKeysPressed = _result.KeyboardKeys.Count == pressedKeys.Length && _result.KeyboardKeys.All(k => pressedKeys.Any(p => p.Equals(k)));
                //if (sameKeysPressed)
                //{
                //    return;
                //}
                var someKeyReleased = _result.KeyboardKeys.Count > pressedKeys.Length && !pressedKeys.Any(p => !_result.KeyboardKeys.Any(k => p != k));
                if (someKeyReleased)
                {

                    _keyboardScanComplete = !pressedKeys.Any() && _result.KeyboardKeys.Any();
                    if (_keyboardScanComplete)
                    {
                        TryFireCompleteEvent();
                    }
                    return;
                }

                var inconsistentSequence = _result.KeyboardKeys.Count < pressedKeys.Length && _result.KeyboardKeys.Any(p => !pressedKeys.Any(k => p != k));
                if (inconsistentSequence)
                {
                    lock (_result)
                    {
                        _keyboardScanComplete = true;
                        if (TryFireCompleteEvent())
                        {
                            return;
                        }
                        _result.KeyboardKeys.Clear();
                        _result.KeyboardKeys.AddRange(pressedKeys);
                    }
                    return;
                }
                if (AddKeys(pressedKeys))
                {
                    // Console.WriteLine(_result.ToString() + " Same key(s):" + sameKeysPressed);
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
                foreach (var key in pressedKeys)
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

        private bool AddJoystickButtons(IEnumerable<JoyBut> buttons)
        {
            var added = false;
            lock (_result)
            {
                _result.JoystickButtons.Clear();
                foreach (var button in buttons)
                {
                    if (IsLimitReached())
                    {
                        break;
                    }
                    added = true;
                    _result.JoystickButtons.Add(button);
                }
            }
            return added;
        }

        private bool IsLimitReached()
        {
            return _result.KeyboardKeys.Count + _result.JoystickButtons.Count >= _maxPressedButtonsCount;
        }

        private void OnJoystickScanningComplete(List<JoyBut> buttons)
        {
            if (_result.JoystickButtons.Count > buttons.Count || _keyboardScanComplete)
            {
                return;
            }
            _joystickScanComplete = !buttons.Any() && _result.JoystickButtons.Any();
            TryFireCompleteEvent();
        }

        private void OnJoystickPressedChanged(List<JoyBut> buttons)
        {
            if (AddJoystickButtons(buttons))
            {
                OnCurrentlyPressedChanged?.Invoke(_result, false);
            }
        }

        private bool TryFireCompleteEvent()
        {
            var keyboardComplete = _keyboardScanComplete || !_result.KeyboardKeys.Any();
            var joystickComplete = _joystickScanComplete || !_result.JoystickButtons.Any();
            var somethingScanned = _result.JoystickButtons.Any() || _result.KeyboardKeys.Any();
            var scanComplete = somethingScanned && (keyboardComplete || joystickComplete);
            if (scanComplete)
            {
                // Console.WriteLine("Complete:" + _result.ToString());
                OnScanningComplete?.Invoke(_result.Clone());
                _result.KeyboardKeys.Clear();
                _result.JoystickButtons.Clear();
                _keyboardScanComplete = false;
                _joystickScanComplete = false;
            }
            return scanComplete;
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
            OnScanningComplete?.GetInvocationList().ToList().ForEach(d => OnScanningComplete -= d as Action<JoystickKeyboardInput>);
            OnCurrentlyPressedChanged?.GetInvocationList().ToList().ForEach(d => OnCurrentlyPressedChanged -= d as Action<JoystickKeyboardInput, bool>);
        }
    }
}
