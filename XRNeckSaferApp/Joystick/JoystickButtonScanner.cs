using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class JoystickButtonScanner : IDisposable
    {
        private Timer _scanTimer;
        private readonly Dictionary<string, JoyBut> _pressedButtons;
        private readonly int _maxPressedButtonsCount;

        public event Action<List<JoyBut>> OnScanningComplete;
        public event Action<List<JoyBut>> OnCurrentlyPressedChanged;

        public JoystickButtonScanner(int maxPressedButtonsCount = 1)
        {
            _pressedButtons = new Dictionary<string, JoyBut>();
            _scanTimer = new Timer()
            {
                Interval = 100
            };
            _scanTimer.Tick += new EventHandler(ScanTimerLoop);
            _maxPressedButtonsCount = maxPressedButtonsCount;
        }

        public void StartScan()
        {
            JoystickStuff.Instance.ReloadJoysticks();
            _scanTimer.Start();
        }

        private void ScanTimerLoop(object sender, EventArgs e)
        {
            var pressedButtons = JoystickStuff.Instance.GetPressedButtons();
            if (pressedButtons != null)
            {
                if (AddButtons(pressedButtons))
                {
                    // Debug();
                    OnCurrentlyPressedChanged?.Invoke(_pressedButtons.Values.ToList());
                }
                return;
            }
            if (_pressedButtons.Any())
            {
                // Stop();
                OnScanningComplete?.Invoke(_pressedButtons.Values.ToList());
                _pressedButtons.Clear();
            }
        }

        public void Stop()
        {
            _scanTimer.Stop();
        }

        public void Dispose()
        {
            _scanTimer?.Dispose();
            UnsubscribeAllHandlers();
            _scanTimer = null;
        }

        private bool AddButtons(List<JoyBut> buttons)
        {
            if (_maxPressedButtonsCount <= _pressedButtons.Count)
            {
                return false;
            }
            lock (_pressedButtons)
            {
                var added = false;
                foreach (var button in buttons)
                {
                    var id = button.GetId();
                    if (_maxPressedButtonsCount > _pressedButtons.Count && !_pressedButtons.ContainsKey(id))
                    {
                        _pressedButtons.Add(id, button);
                        added = true;
                    }
                }
                return added;
            }
        }

        private void UnsubscribeAllHandlers()
        {
            OnScanningComplete?.GetInvocationList().ToList().ForEach(d => OnScanningComplete -= d as Action<List<JoyBut>>);
            OnCurrentlyPressedChanged?.GetInvocationList().ToList().ForEach(d => OnCurrentlyPressedChanged -= d as Action<List<JoyBut>>);
        }

        //private void Debug()
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
        //    Console.WriteLine(builder.ToString());
        //}
    }
}
