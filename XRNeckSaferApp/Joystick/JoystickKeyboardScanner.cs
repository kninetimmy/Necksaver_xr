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

        public event Action<JoystickKeyboardInput> OnScanningComplete;
        public event Action<JoystickKeyboardInput> OnCurrentlyPressedChanged;

        public JoystickKeyboardScanner(int maxPressedButtonsCount = 1)
        {
            _maxPressedButtonsCount = maxPressedButtonsCount;
            _result = new JoystickKeyboardInput { KeyboardKeys = new List<Keys>(), JoystickButtons = new List<JoyBut>() };
            _joystickScanner = new JoystickButtonScanner(maxPressedButtonsCount);
            _joystickScanner.OnCurrentlyPressedChanged += OnJoystickPressedChanged;
            _joystickScanner.OnScanningComplete += OnJoystickScanningComplete;
            KeyInterceptor.KeyPressed += OnKeyPressed;
            _joystickScanner.StartScan();
        }

        private void OnKeyPressed(Keys[] pressedKeys)
        {
            var sameKeysPressed = _result.KeyboardKeys.Count == pressedKeys.Length && _result.KeyboardKeys.All(k => pressedKeys.Any(p => p.Equals(k)));
            if (sameKeysPressed)
            {
                return;
            }
            var someKeyReleased = _result.KeyboardKeys.Count > pressedKeys.Length && !pressedKeys.Any(p => !_result.KeyboardKeys.Any(k => p != k));
            if (someKeyReleased)
            {
                _keyboardScanComplete = true;
                TryFireCompleteEvent();
                return;
            }
            var inconsistentSequence = _result.KeyboardKeys.Count < pressedKeys.Length && _result.KeyboardKeys.Any(p => !pressedKeys.Any(k => p != k));
            if (inconsistentSequence)
            {
                _keyboardScanComplete = true;
                TryFireCompleteEvent();
                return;
            }
            if (AddKeys(pressedKeys))
            {
                // Console.WriteLine(CreatePressedButtonsText(_result));
                OnCurrentlyPressedChanged?.Invoke(_result);
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
            AddJoystickButtons(buttons);
            _joystickScanComplete = true;
            TryFireCompleteEvent();
        }

        private void OnJoystickPressedChanged(List<JoyBut> buttons)
        {
            if (AddJoystickButtons(buttons))
            {
                OnCurrentlyPressedChanged?.Invoke(_result);
            }
        }

        private void TryFireCompleteEvent()
        {
            var keyboardComplete = _keyboardScanComplete || !_result.KeyboardKeys.Any();
            var joystickComplete = _joystickScanComplete || !_result.JoystickButtons.Any();
            var somethingScanned = _result.JoystickButtons.Any() || _result.KeyboardKeys.Any();
            if (somethingScanned && (keyboardComplete || joystickComplete))
            {
                OnScanningComplete?.Invoke(_result);
            }
        }

        public void Dispose()
        {
            if (_joystickScanner != null)
            {
                KeyInterceptor.KeyPressed -= OnKeyPressed;
                _joystickScanner.Stop();
                _joystickScanner.Dispose();
                _joystickScanner = null;
            }
        }

        //private string CreatePressedButtonsText(JoystickKeyboardInput input)
        //{
        //    var builder = new System.Text.StringBuilder();
        //    foreach (var button in input.JoystickButtons)
        //    {
        //        if (builder.Length > 0)
        //        {
        //            builder.Append("+");
        //        }
        //        var stickItem = JoystickStuff.Instance.GetStickItemByGuid(button.JoystickGuid);
        //        builder.Append($"[{stickItem.InstanceName} But:{button.Button + 1}]");
        //    }
        //    foreach (var key in input.KeyboardKeys)
        //    {
        //        if (builder.Length > 0)
        //        {
        //            builder.Append("+");
        //        }
        //        builder.Append($"[Key:{key}]");
        //    }
        //    return builder.ToString();
        //}
    }
}
