using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace XRNeckSafer
{
    public static class JoystickService
    {
        private const int SCAN_DELAY_INTERVAL_MSEC = 50;
        private static BackgroundWorker _worker;
        private static readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private static readonly ILogger _logger = LogManager.GetLogger(nameof(JoystickService));

        private static readonly Dictionary<Guid, Joystick> _joysticGuids = new Dictionary<Guid, Joystick>();
        private static readonly Dictionary<Guid, JoystickState> _joystickStates = new Dictionary<Guid, JoystickState>();
        private static readonly Dictionary<Guid, List<JoystickButton>> _pressedJoystickButtons = new Dictionary<Guid, List<JoystickButton>>();
        private static readonly Dictionary<Guid, Tuple<int, int>> _joystickButtonPovCounts = new Dictionary<Guid, Tuple<int, int>>();

        private static readonly List<JoystickOffset> _povOffsets = new List<JoystickOffset>
        {
            JoystickOffset.PointOfViewControllers0,
            JoystickOffset.PointOfViewControllers1,
            JoystickOffset.PointOfViewControllers2,
            JoystickOffset.PointOfViewControllers3
        };

        public static event Action<Guid, JoystickButton, bool> PressedButtonsUpdate;
        public static event Action<Guid, string> DeviceDisconnected;
        public static event Action<Guid, string> DeviceConnected;

        public static void Start()
        {
            StartJoysticksWorker();
        }

        public static string GetJoystickName(string stringGuid)
        {
            if (!Guid.TryParse(stringGuid, out Guid guid))
            {
                return null;
            }
            return GetJoystickName(guid);
        }

        public static string GetJoystickName(Guid guid)
        {
            if (!_joysticGuids.TryGetValue(guid, out Joystick joystick))
            {
                return null;
            }
            return joystick?.Properties?.InstanceName;
        }

        public static List<JoystickButton> GetPressedButtons()
        {
            var result = new List<JoystickButton>();
            lock (_pressedJoystickButtons)
            {
                foreach (Guid guid in _pressedJoystickButtons.Keys)
                {
                    var buttons = _pressedJoystickButtons[guid].ToArray();
                    result.AddRange(buttons);
                }
            }
            return result;
        }

        public static Guid[] GetJoystickGuids()
        {
            return _joysticGuids.Keys.ToArray();
        }

        public static void Stop()
        {
            if (_worker != null)
            {
                _worker.CancelAsync();
                _waitHandle.WaitOne();
                _worker.Dispose();
                _worker = null;
            }
        }

        private static void StartJoysticksWorker()
        {
            _worker = new BackgroundWorker();
            _worker.DoWork += ScanConnectedDevicesWork;
            _worker.WorkerSupportsCancellation = true;
            _worker.RunWorkerCompleted += JoystickWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private static void JoystickWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (_joysticGuids)
            {
                foreach (var joystick in _joysticGuids.Values.ToArray())
                {
                    joystick.Unacquire();
                    joystick.Dispose();
                }
                _joysticGuids.Clear();
                _joystickStates.Clear();
                _pressedJoystickButtons.Clear();
                _joystickButtonPovCounts.Clear();
            }
            _worker.DoWork -= ScanConnectedDevicesWork;
            _worker.RunWorkerCompleted -= JoystickWorkerCompleted;
            _waitHandle.Set();
        }

        private static void ScanConnectedDevicesWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var directInput = new DirectInput();
            var stopwatch = new Stopwatch();
            _logger.Debug($"Started scanning joysticks");
            while (!worker.CancellationPending)
            {
                PopulateJoysticks(directInput, stopwatch);
                Guid[] guids = _joysticGuids.Keys.ToArray();
                foreach (Guid guid in guids)
                {
                    CheckJoystickUpdates(guid);
                }
                Thread.Sleep(SCAN_DELAY_INTERVAL_MSEC);
            }
        }

        private static void PopulateJoysticks(DirectInput directInput, Stopwatch stopwatch)
        {
            if (stopwatch.IsRunning && stopwatch.Elapsed < TimeSpan.FromSeconds(10))
            {
                return;
            }
            stopwatch.Restart();
            var devices = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            _logger.Trace($"{devices.Count} joysticks found");
            foreach (DeviceInstance device in devices)
            {
                lock (_joysticGuids)
                {
                    if (!_joysticGuids.ContainsKey(device.InstanceGuid))
                    {
                        var joystick = new Joystick(directInput, device.InstanceGuid);
                        var buttonCount = joystick.Capabilities.ButtonCount;
                        var povCount = joystick.Capabilities.PovCount;
                        _logger.Debug($"Found Joystick with GUID: {device.InstanceGuid}." +
                            $" {buttonCount} buttons, {povCount} POVs");
                        _joysticGuids.Add(device.InstanceGuid, joystick);
                        _joystickButtonPovCounts.Add(device.InstanceGuid, new Tuple<int, int>(buttonCount, povCount));
                        joystick.Acquire();
                        _joystickStates.Add(device.InstanceGuid, joystick.GetCurrentState());
                        DeviceConnected?.Invoke(device.InstanceGuid, joystick.Properties.InstanceName);
                    }
                }
            }
        }

        public static JoystickButton CreateJoystickButton(Guid guid, JoystickOffset offset, int value)
        {
            var isPov = offset >= JoystickOffset.PointOfViewControllers0 && offset <= JoystickOffset.PointOfViewControllers3;
            var button = new JoystickButton
            {
                JoystickGuid = guid.ToString(),
                Button = isPov ? value : (int)offset - (int)JoystickOffset.Buttons0,
            };
            if (isPov)
            {
                button.POV = _povOffsets.IndexOf(offset);
            }
            return button;
        }

        private static void CheckJoystickUpdates(Guid guid)
        {
            try
            {
                List<JoystickPollingUpdate> updates = GetUpdates(guid);
                if (updates == null)
                {
                    return;
                }
                foreach (JoystickPollingUpdate state in updates)
                {
                    var isButton = state.Offset >= JoystickOffset.Buttons0 && state.Offset <= JoystickOffset.Buttons127;
                    var pressed = isButton ? state.Value > 0 : state.Value >= 0;
                    var joyBut = CreateJoystickButton(guid, state.Offset, state.Value);
                    lock (_pressedJoystickButtons)
                    {
                        if (pressed)
                        {
                            if (!_pressedJoystickButtons.ContainsKey(guid))
                            {
                                _pressedJoystickButtons.Add(guid, new List<JoystickButton>());
                            }
                            _pressedJoystickButtons[guid].Add(joyBut);
                        }
                        else
                        {
                            if (_pressedJoystickButtons.ContainsKey(guid))
                            {
                                if (isButton)
                                {
                                    _pressedJoystickButtons[guid].RemoveAll(b => b.GetId() == joyBut.GetId());
                                }
                                else
                                {
                                    _pressedJoystickButtons[guid].RemoveAll(b => b.JoystickGuid == joyBut.JoystickGuid && b.POV == joyBut.POV);
                                }

                            }
                        }
                    }
                    _logger.Trace(state.ToString());
                    PressedButtonsUpdate?.Invoke(guid, joyBut, pressed);
                }
            }
            catch (SharpDX.SharpDXException err)
            {
                _logger.Error(err.Message);
                StopJoystickPolling(guid);
            }
        }

        private static void StopJoystickPolling(Guid guid)
        {
            _logger.Debug($"Completed polling Joystick with GUID: {guid}");
            var joystickName = GetJoystickName(guid.ToString());
            lock (_joysticGuids)
            {
                if (_joysticGuids.TryGetValue(guid, out Joystick joystick))
                {
                    joystick.Dispose();
                }
                _pressedJoystickButtons.Remove(guid);
                _joysticGuids.Remove(guid);
                _joystickStates.Remove(guid);
                _joystickButtonPovCounts.Remove(guid);
            }
            _logger.Debug($"Removed Joystick with GUID: {guid}");
            DeviceDisconnected?.Invoke(guid, joystickName);
        }

        private static List<JoystickPollingUpdate> GetUpdates(Guid guid)
        {
            List<JoystickPollingUpdate> updates = null;
            Joystick joystick = GetJoystick(guid);
            if (joystick == null)
            {
                return updates;
            }
            JoystickState currentState = GetJoystickState(guid);
            JoystickState updatedState = joystick.GetCurrentState();
            var buttonCount = _joystickButtonPovCounts[guid].Item1;
            for (var buttonIndex = 0; buttonIndex < buttonCount; buttonIndex++)
            {
                if (updatedState.Buttons[buttonIndex] != currentState.Buttons[buttonIndex])
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
            var povCount = _joystickButtonPovCounts[guid].Item2;
            for (var povIndex = 0; povIndex < povCount; povIndex++)
            {
                if (updatedState.PointOfViewControllers[povIndex] != currentState.PointOfViewControllers[povIndex])
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
                lock (_joystickStates)
                {
                    _joystickStates[guid] = updatedState;
                }
            }
            return updates;
        }

        private static Joystick GetJoystick(Guid guid)
        {
            _joysticGuids.TryGetValue(guid, out Joystick joystick);
            return joystick;
        }

        private static JoystickState GetJoystickState(Guid guid)
        {
            _joystickStates.TryGetValue(guid, out JoystickState state);
            return state;
        }
    }
}
