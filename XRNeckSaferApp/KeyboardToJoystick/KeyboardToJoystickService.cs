using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XRNeckSafer
{
    public class KeyboardToJoystickService : IDisposable
    {
        private JoystickButtonScanner _joystickScanner;
        private readonly List<KeyboardToJoystickModel> _mappings;
        private static readonly ILogger _logger = LogManager.GetLogger(nameof(KeyboardToJoystickService));
        private readonly List<JoystickButton> _pressedJoystickButtons = new List<JoystickButton>();

        public bool Enabled { get; set; } = true;

        private static KeyboardToJoystickService _instanse;

        public static KeyboardToJoystickService Instanse
        {
            get
            {
                if (_instanse == null)
                {
                    _instanse = new KeyboardToJoystickService(Config.Instance.KeyboardToJoystickAssignments);
                }
                return _instanse;
            }
        }

        private KeyboardToJoystickService(List<KeyboardToJoystickModel> mappings, int maxPressedButtonsCount = int.MaxValue)
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
            if (!Enabled)
            {
                return;
            }
            var mapping = _mappings.FirstOrDefault(m => m.JoystickButton.GetId().Equals(button.GetId()));
            if (mapping != null)
            {
                KeyPressSimulator.PressKey(mapping.KeyboardKey);
                _logger.Debug($"'{mapping.KeyboardKey}' pressed.");
            }
        }

        private void ReleaseButton(JoystickButton button)
        {
            if (!Enabled)
            {
                return;
            }
            var mapping = _mappings.FirstOrDefault(m => m.JoystickButton.GetId().Equals(button.GetId()));
            if (mapping != null)
            {
                KeyPressSimulator.ReleaseKey(mapping.KeyboardKey);
                _logger.Debug($"'{mapping.KeyboardKey}' released.");
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
