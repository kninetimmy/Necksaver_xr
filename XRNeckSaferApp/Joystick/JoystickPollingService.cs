using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace XRNeckSafer
{
    public class JoystickPollingService: IDisposable
    {
        private readonly Joystick _joystick;
        private JoystickState _currentState;
        private readonly int _pollingIntervalMsec;
        private AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private Exception _error;

        public JoystickPollingUpdate[] Updates { get; private set; }

        public JoystickPollingService(Joystick joystick, int pollingIntervalMsec)
        {
            _pollingIntervalMsec = pollingIntervalMsec;
            _joystick = joystick;
            _currentState = _joystick.GetCurrentState();
        }

        public void Poll()
        {
            _error = null;
            using (var worker = new BackgroundWorker())
            {
                worker.DoWork += DoWork;
                worker.RunWorkerAsync();
                _resetEvent.WaitOne();
                worker.DoWork -= DoWork;
            }
            if (_error != null)
            {
                throw _error;
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                List<JoystickPollingUpdate> updates;
                while (!GetUpdates(_joystick.GetCurrentState(), out updates))
                {
                    Thread.Sleep(_pollingIntervalMsec);
                }
                Updates = updates.ToArray();
            }
            catch (Exception ex)
            {
                _error = ex;
            }
            _resetEvent.Set();
        }

        private bool GetUpdates(JoystickState updatedState, out List<JoystickPollingUpdate> updates)
        {
            updates = null;
            for (var buttonIndex = 0; buttonIndex < 128; buttonIndex++)
            {
                if (updatedState.Buttons[buttonIndex] != _currentState.Buttons[buttonIndex])
                {
                    var update = new JoystickPollingUpdate
                    {
                        RawOffset = buttonIndex + (int)JoystickOffset.Buttons0,
                        Value = updatedState.Buttons[buttonIndex] ? 128 : 0,
                    };
                    if (updates == null)
                    {
                        updates = new List<JoystickPollingUpdate>();
                    }
                    updates.Add(update);
                }
            }
            for (var povIndex = 0; povIndex < 4; povIndex++)
            {
                if (updatedState.PointOfViewControllers[povIndex] != _currentState.PointOfViewControllers[povIndex])
                {
                    var update = new JoystickPollingUpdate
                    {
                        RawOffset = 4 * povIndex + (int)JoystickOffset.PointOfViewControllers0,
                        Value = updatedState.PointOfViewControllers[povIndex]
                    };
                    if (updates == null)
                    {
                        updates = new List<JoystickPollingUpdate>();
                    }
                    updates.Add(update);
                }
            }
            if (updates != null)
            {
                _currentState = updatedState;
            }
            return updates != null;
        }

        public void Dispose()
        {
            if (_resetEvent != null)
            {
                _resetEvent.Dispose();
                _resetEvent = null;
            }
        }
    }
}
