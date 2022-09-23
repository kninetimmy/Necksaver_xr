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
        private static readonly Stopwatch _devicesWatch = new Stopwatch();
        private static readonly ILogger _logger = LogManager.GetLogger("JoystickService", typeof(JoystickService));
        private static readonly Dictionary<Guid, Joystick> _joysticGuids = new Dictionary<Guid, Joystick>();
        private static readonly Dictionary<Guid, JoystickState> _joystickStates = new Dictionary<Guid, JoystickState>();

        private static readonly Dictionary<Guid, List<JoystickButton>> _pressedJoystickButtons = new Dictionary<Guid, List<JoystickButton>>();
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
            foreach (var joystick in _joysticGuids.Values)
            {
                joystick.Unacquire();
            }
        }

        private static void StartJoysticksWorker()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += ScanConnectedDevicesWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
        }

        private static void ScanConnectedDevicesWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var directInput = new DirectInput();
            _logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}]: Started scanning joysticks");
            while (!worker.CancellationPending)
            {
                PopulateJoysticks(directInput);
                Guid[] guids = _joysticGuids.Keys.ToArray();
                foreach (Guid guid in guids)
                {
                    CheckJoystickUpdates(guid);
                }
                Thread.Sleep(10);
            }
            e.Cancel = true;
        }

        private static void PopulateJoysticks(DirectInput directInput)
        {
            if (_devicesWatch.IsRunning && _devicesWatch.Elapsed < TimeSpan.FromSeconds(10))
            {
                return;
            }
            _devicesWatch.Restart();
            var devices = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            _logger.Trace($"[{Thread.CurrentThread.ManagedThreadId}]: {devices.Count} joysticks found");
            foreach (DeviceInstance device in devices)
            {
                lock (_joysticGuids)
                {
                    if (!_joysticGuids.ContainsKey(device.InstanceGuid))
                    {
                        var joystick = new Joystick(directInput, device.InstanceGuid);
                        _logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}]: Found Joystick with GUID: {device.InstanceGuid}." +
                            $" {joystick.Capabilities.ButtonCount} buttons, {joystick.Capabilities.PovCount} POVs");
                        _joysticGuids.Add(device.InstanceGuid, joystick);
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
                Button = isPov ? value : (int)offset - 49
            };
            if (isPov)
            {
                button.POV = _povOffsets.IndexOf(offset);
            }
            return button;
        }

        private static void CheckJoystickUpdates(Guid guid)
        {
            _logger.Trace($"[{Thread.CurrentThread.ManagedThreadId}]: Started polling Joystick with GUID: {guid}");
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
                    _logger.Trace($"[{Thread.CurrentThread.ManagedThreadId}]: {state}");
                    PressedButtonsUpdate?.Invoke(guid, joyBut, pressed);
                }
            }
            catch (SharpDX.SharpDXException err)
            {
                _logger.Error($"[{Thread.CurrentThread.ManagedThreadId}]: {err.Message}");
                StopJoystickPolling(guid);
            }
        }

        private static void StopJoystickPolling(Guid guid)
        {
            _logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}]: Completed polling Joystick with GUID: {guid}");
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
            }
            _logger.Debug($"[{Thread.CurrentThread.ManagedThreadId}]: Removed Joystick with GUID: {guid}");
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
            for (var buttonIndex = 0; buttonIndex < joystick.Capabilities.ButtonCount; buttonIndex++)
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
            for (var povIndex = 0; povIndex < joystick.Capabilities.PovCount; povIndex++)
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
