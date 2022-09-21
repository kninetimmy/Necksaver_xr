using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace XRNeckSafer
{
    public static class JoystickService
    {
        static readonly Dictionary<Guid, Joystick> _joysticGuids = new Dictionary<Guid, Joystick>();
        static readonly Dictionary<Guid, BackgroundWorker> _joystickWorkers = new Dictionary<Guid, BackgroundWorker>();

        static readonly Dictionary<Guid, List<JoyBut>> _pressedJoystickButtons = new Dictionary<Guid, List<JoyBut>>();
        static readonly List<JoystickOffset> _povOffsets = new List<JoystickOffset> 
        { 
            JoystickOffset.PointOfViewControllers0, 
            JoystickOffset.PointOfViewControllers1, 
            JoystickOffset.PointOfViewControllers2, 
            JoystickOffset.PointOfViewControllers3 
        };

        public static event Action<Guid, JoyBut, bool> PressedButtonsUpdate;
        public static event Action<Guid, string> DeviceDisconnected;
        public static event Action<Guid, string> DeviceConnected;

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

        public static void Start()
        {
            StartJoysticksWorker();
        }

        public static Guid[] GetJoystickGuids()
        {
            return _joysticGuids.Keys.ToArray();
        }

        public static List<JoyBut> GetPressedButtons()
        {
            var result = new List<JoyBut>();
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

        public static JoyBut CreateJoyBut(Guid guid, JoystickOffset offset, int value)
        {
            var isPov = offset >= JoystickOffset.PointOfViewControllers0 && offset <= JoystickOffset.PointOfViewControllers3;
            var button = new JoyBut
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
            Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: Started scanning joysticks");
            while (!worker.CancellationPending)
            {
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly))
                {
                    lock (_joysticGuids)
                    {
                        if (!_joysticGuids.ContainsKey(deviceInstance.InstanceGuid))
                        {
                            var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
                            Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: Found Joystick with GUID: {deviceInstance.InstanceGuid}." +
                                $" {joystick.Capabilities.ButtonCount} buttons, {joystick.Capabilities.PovCount} POVs");
                            _joysticGuids.Add(deviceInstance.InstanceGuid, joystick);
                            _joystickWorkers.Add(deviceInstance.InstanceGuid, RunJoystickStatePoll(deviceInstance.InstanceGuid, joystick));
                            DeviceConnected?.Invoke(deviceInstance.InstanceGuid, joystick.Properties.InstanceName);
                        }
                    }
                }
                System.Threading.Thread.Sleep(10000);
            }
            e.Cancel = true;
        }

        private static BackgroundWorker RunJoystickStatePoll(Guid guid, Joystick joystick)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += PollJoystickUpdate;
            worker.RunWorkerCompleted += JoystickPollComplete;
            worker.RunWorkerAsync(new Tuple<Guid, Joystick>(guid, joystick));
            return worker;
        }

        private static void JoystickPollComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Guid guid = (Guid)e.Result;
            Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: Completed polling Joystick with GUID: {guid}");
            var joystickName = GetJoystickName(guid.ToString());
            lock (_joysticGuids)
            {
                if (_joysticGuids.TryGetValue(guid, out Joystick joystick))
                {
                    joystick.Dispose();
                }
                _pressedJoystickButtons.Remove(guid);
                _joysticGuids.Remove(guid);
                _joystickWorkers[guid]?.Dispose();
                _joystickWorkers.Remove(guid);
            }
            Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: Removed Joystick with GUID: {guid}");
            DeviceDisconnected?.Invoke(guid, joystickName);
        }

        private static void PollJoystickUpdate(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender; 
            var tuple = (Tuple<Guid, Joystick>)e.Argument;
            Joystick joystick = tuple.Item2;
            Guid joystickGuid = tuple.Item1;
            e.Result = joystickGuid;
            Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: Started polling Joystick with GUID: {joystickGuid}");
            try
            {
                joystick.Properties.BufferSize = 128;
                joystick.Acquire();
                while (!worker.CancellationPending)
                {
                    joystick.Poll();
                    var datas = joystick.GetBufferedData();
                    foreach (JoystickUpdate state in datas.Where(d => d.Offset >= JoystickOffset.PointOfViewControllers0 && d.Offset <= JoystickOffset.Buttons127))
                    {
                        var isButton = state.Offset >= JoystickOffset.Buttons0 && state.Offset <= JoystickOffset.Buttons127;
                        var pressed = isButton ? state.Value > 0 : state.Value >= 0;
                        var joyBut = CreateJoyBut(joystickGuid, state.Offset, state.Value);
                        lock (_pressedJoystickButtons)
                        {
                            if (pressed)
                            {
                                if (!_pressedJoystickButtons.ContainsKey(joystickGuid))
                                {
                                    _pressedJoystickButtons.Add(joystickGuid, new List<JoyBut>());
                                }
                                _pressedJoystickButtons[joystickGuid].Add(joyBut);
                            }
                            else
                            {
                                if (_pressedJoystickButtons.ContainsKey(joystickGuid))
                                {
                                    if (isButton)
                                    {
                                        _pressedJoystickButtons[joystickGuid].RemoveAll(b => b.GetId() == joyBut.GetId());
                                    } 
                                    else
                                    {
                                        _pressedJoystickButtons[joystickGuid].RemoveAll(b => b.JoystickGuid == joyBut.JoystickGuid && b.POV == joyBut.POV);
                                    }
                                    
                                }
                            }
                        }
                        // Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: {state}");
                        PressedButtonsUpdate?.Invoke(joystickGuid, joyBut, pressed);
                        // worker.ReportProgress(0, state);
                    }
                }
                e.Cancel = true;
            }
            catch (SharpDX.SharpDXException err)
            {
                Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: ERROR {err.Message}");
            }
        }
    }
}
