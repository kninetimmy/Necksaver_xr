using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XRNeckSafer
{

    public class JoystickButtonScanner : IDisposable
    {
        private static readonly ILogger _logger = LogManager.GetLogger(nameof(JoystickButtonScanner));
        private readonly Dictionary<string, JoystickButton> _pressedButtons;
        private readonly Dictionary<string, JoystickButton> _pressedResultButtons;
        private readonly int _maxPressedButtonsCount;
        private List<JoystickButton> _excludeButtons = new List<JoystickButton>();

        public event Action<List<JoystickButton>> CurrentlyPressedChanged;
        public event Action<List<JoystickButton>> BeforeButtonReleased;

        public JoystickButtonScanner(int maxPressedButtonsCount = int.MaxValue)
        {
            _pressedButtons = new Dictionary<string, JoystickButton>();
            _pressedResultButtons = new Dictionary<string, JoystickButton>();
            _maxPressedButtonsCount = maxPressedButtonsCount;
            JoystickService.PressedButtonsUpdate += OnJoystickPressedButtonsUpdate;
        }

        private void OnJoystickPressedButtonsUpdate(Guid guid, JoystickButton joyBut, bool pressed)
        {
            var isExcludedButton = _excludeButtons.Any(e => e.GetId() == joyBut.GetId());
            if (isExcludedButton)
            {
                return;
            }
            if (pressed)
            {
                if (AddResultButton(joyBut))
                {
                    _logger.Trace(DebugPressedButtons());
                    CurrentlyPressedChanged?.Invoke(_pressedResultButtons.Values.ToList());
                }
                return;
            }

            if (isExcludedButton)
            {
                _excludeButtons.RemoveAll(e => e.GetId() == joyBut.GetId());
            }
            if (RemoveResultButton(joyBut))
            {
                _logger.Trace(DebugPressedButtons());
                CurrentlyPressedChanged?.Invoke(_pressedResultButtons.Values.ToList());
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
            return _maxPressedButtonsCount <= _pressedResultButtons.Count;
        }

        private bool AddResultButton(JoystickButton button)
        {
            lock (_pressedButtons)
            { 
                var id = button.GetId();
                if (!_pressedButtons.ContainsKey(id))
                {
                    _pressedButtons.Add(id, button);
                }
                if (IsLimitReached())
                {
                    return false;
                }
                if (_maxPressedButtonsCount > _pressedResultButtons.Count && !_pressedResultButtons.ContainsKey(id))
                {
                    _pressedResultButtons.Add(id, button);
                    return true;
                }
                return false;
            }
        }

        private bool RemoveResultButton(JoystickButton button)
        {
            lock (_pressedButtons)
            {
                var isPov = button.POV > -1;
                if (isPov)
                {
                    var removed = false;
                    var povButtonKeys = _pressedButtons.Keys.Where(key => _pressedButtons[key].POV == button.POV && _pressedButtons[key].JoystickGuid == button.JoystickGuid).ToArray();
                    foreach (var povKey in povButtonKeys)
                    {
                        removed |= RemoveButton(povKey);
                    }
                    return removed;
                }
                return RemoveButton(button.GetId());
            }
        }

        private bool RemoveButton(string id)
        {
            if (_pressedButtons.ContainsKey(id))
            {
                _pressedButtons.Remove(id);
            }
            if (_pressedResultButtons.ContainsKey(id))
            {
                BeforeButtonReleased?.Invoke(_pressedResultButtons.Values.ToList());
                _pressedResultButtons.Remove(id);
                if (!IsLimitReached() && _pressedResultButtons.Count < _pressedButtons.Count)
                {
                    var shiftToResultKey = _pressedButtons.Keys.FirstOrDefault(key => !_pressedResultButtons.ContainsKey(key));
                    if (shiftToResultKey != null)
                    {
                        _pressedResultButtons.Add(shiftToResultKey, _pressedButtons[shiftToResultKey]);
                    }
                }
                return true;
            }
            return false;
        }

        private void UnsubscribeAllHandlers()
        {

            BeforeButtonReleased?.GetInvocationList().ToList().ForEach(d => BeforeButtonReleased -= d as Action<List<JoystickButton>>);
            CurrentlyPressedChanged?.GetInvocationList().ToList().ForEach(d => CurrentlyPressedChanged -= d as Action<List<JoystickButton>>);
            JoystickService.PressedButtonsUpdate -= OnJoystickPressedButtonsUpdate;
        }

        private string DebugPressedButtons()
        {
            var builder = new System.Text.StringBuilder();
            foreach (var key in _pressedButtons.Keys)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                builder.Append($"[{key}]");
            }
            return builder.ToString();
        }

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
