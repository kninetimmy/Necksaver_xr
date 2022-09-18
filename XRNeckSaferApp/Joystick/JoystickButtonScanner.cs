using System;
using System.Collections.Generic;
using System.Linq;

namespace XRNeckSafer
{
    public class JoystickButtonScanner : IDisposable
    {
        private readonly Dictionary<string, JoyBut> _pressedButtons;
        private readonly int _maxPressedButtonsCount;
        private List<JoyBut> _excludeButtons = new List<JoyBut>();

        public event Action<List<JoyBut>> OnScanningComplete;
        public event Action<List<JoyBut>> OnCurrentlyPressedChanged;

        public JoystickButtonScanner(int maxPressedButtonsCount = 1)
        {
            _pressedButtons = new Dictionary<string, JoyBut>();
            _maxPressedButtonsCount = maxPressedButtonsCount;
            JoystickService.PressedButtonsUpdate += JoysticPollingPressedButtonsUpdate;
        }

        private void JoysticPollingPressedButtonsUpdate(Guid guid, JoyBut joyBut, bool pressed)
        {
            var isExcludedButton = _excludeButtons.Any(e => e.GetId() == joyBut.GetId());
            if (pressed)
            {
                if (IsLimitReached())
                {
                    return;
                }
                if (isExcludedButton)
                {
                    return;
                }
                if (AddButton(joyBut))
                {
                    // Console.WriteLine($"COLLECTED - {DebugPressedButtons()}");
                    // Debug();
                    OnCurrentlyPressedChanged?.Invoke(_pressedButtons.Values.ToList());
                    
                }
                return;
            }

            if (isExcludedButton)
            {
                _excludeButtons.RemoveAll(e => e.GetId() == joyBut.GetId());
                return;
            }

            if (_pressedButtons.Values.Any())
            {
                // completed
                // Console.WriteLine($"Scanning complete - {DebugPressedButtons()}");
                OnScanningComplete?.Invoke(_pressedButtons.Values.ToList());
                _pressedButtons.Clear();
            }
        }

        public void StartScan()
        {
            _excludeButtons = JoystickService.GetPressedButtons();
        }

        public void Stop()
        {
            _excludeButtons.Clear();
        }

        public void Dispose()
        {
            UnsubscribeAllHandlers();
        }

        private bool IsLimitReached()
        {
            return _maxPressedButtonsCount <= _pressedButtons.Count;
        }

        private bool AddButton(JoyBut button)
        {
            if (_maxPressedButtonsCount <= _pressedButtons.Count)
            {
                return false;
            }
            lock (_pressedButtons)
            {
                var id = button.GetId();
                if (_maxPressedButtonsCount > _pressedButtons.Count && !_pressedButtons.ContainsKey(id))
                {
                    _pressedButtons.Add(id, button);
                    // Console.WriteLine($"Added button - {button.GetId()}");
                    return true;
                }
                return false;
            }
        }

        private void UnsubscribeAllHandlers()
        {

            OnScanningComplete?.GetInvocationList().ToList().ForEach(d => OnScanningComplete -= d as Action<List<JoyBut>>);
            OnCurrentlyPressedChanged?.GetInvocationList().ToList().ForEach(d => OnCurrentlyPressedChanged -= d as Action<List<JoyBut>>);
            JoystickService.PressedButtonsUpdate -= JoysticPollingPressedButtonsUpdate;
        }

        //private string DebugPressedButtons()
        //{
        //    var builder = new System.Text.StringBuilder();
        //    foreach (var key in _pressedButtons.Keys)
        //    {
        //        if (builder.Length > 0)
        //        {
        //            builder.Append("+");
        //        }
        //        builder.Append(key);
        //    }
        //    return builder.ToString();
        //}

        //private string DebugButtons(List<JoyBut> buttons)
        //{
        //    if (buttons == null || !buttons.Any())
        //    {
        //        return "NONE";
        //    }
        //    var builder = new System.Text.StringBuilder();
        //    foreach (var button in buttons)
        //    {
        //        if (builder.Length > 0)
        //        {
        //            builder.Append("+");
        //        }
        //        builder.Append(button.GetId());
        //    }
        //    return builder.ToString();
        //}
    }
}
