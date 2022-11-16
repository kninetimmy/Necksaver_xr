using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XRNeckSafer
{
    public class JoystickKeyboardMapper : IDisposable
    {
        private JoystickButtonScanner _joystickScanner;
        private readonly List<JoystickToKeyboardMapping> _mappings;
        private static readonly ILogger _logger = LogManager.GetLogger(nameof(JoystickKeyboardMapper));
        private readonly List<JoystickButton> _pressedJoystickButtons = new List<JoystickButton>();

        public JoystickKeyboardMapper(List<JoystickToKeyboardMapping> mappings, int maxPressedButtonsCount = int.MaxValue)
        {
            _joystickScanner = new JoystickButtonScanner(maxPressedButtonsCount);
            _joystickScanner.CurrentlyPressedChanged += OnJoystickPressedChanged;
            _joystickScanner.StartScan();
            _mappings = mappings;
        }

        private void OnJoystickPressedChanged(List<JoystickButton> buttons)
        {
            lock (_pressedJoystickButtons)
            {
                var newPressedButtons = buttons.FindAll(b => !_pressedJoystickButtons.Any(p => p.GetId() == b.GetId()));
                var releasedButtons = _pressedJoystickButtons.Where(p => !buttons.Any(b => b.GetId() == p.GetId())).ToList();
                newPressedButtons.ForEach(b => PressButton(b));
                releasedButtons.ForEach(b => ReleaseButton(b));
                _pressedJoystickButtons.Clear();
                _pressedJoystickButtons.AddRange(buttons);
            }
        }

        private void PressButton(JoystickButton button)
        {
            var mapping = _mappings.FirstOrDefault(m => m.JoystickButtonId.Equals(button.GetId()));
            if (mapping != null)
            {
                KeyPressSimulator.PressKey(mapping.KeyboardButton);
                _logger.Debug($"'{mapping.KeyboardButton}' pressed.");
            }
        }

        private void ReleaseButton(JoystickButton button)
        {
            var mapping = _mappings.FirstOrDefault(m => m.JoystickButtonId.Equals(button.GetId()));
            if (mapping != null)
            {
                KeyPressSimulator.ReleaseKey(mapping.KeyboardButton);
                _logger.Debug($"'{mapping.KeyboardButton}' released.");
            }
        }

        public void Dispose()
        {
            if (_joystickScanner != null)
            {
                _joystickScanner.Stop();

                _joystickScanner.Dispose();
                _joystickScanner = null;
            }
        }
    }
}
