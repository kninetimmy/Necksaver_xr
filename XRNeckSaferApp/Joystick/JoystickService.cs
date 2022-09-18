using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace XRNeckSafer
{
    public class JoystickService
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

        public static string GetJoystickName(string stringGuid)
        {
            if (!Guid.TryParse(stringGuid, out Guid guid))
            {
                return null;
            }
            return _joysticGuids.ContainsKey(guid) ? _joysticGuids[guid].Properties.InstanceName : null;
        }

        public static void Start()
        {
            StartJoysticksWorker();
        }

        [Obsolete("Keep this method for compartibility with old way of pressed buttons check")]
        public static StickItem[] GetJoysticks()
        {
            lock (_joysticGuids)
            {
                return _joysticGuids.Keys.Select(guid =>
                {
                    var joystick = _joysticGuids[guid];
                    if (joystick.IsDisposed)
                    {
                        return null;
                    }
                    return new StickItem
                    {
                        Attached = true,
                        InstanceName = joystick.Properties.InstanceName,
                        ButtonCount = joystick.Capabilities.ButtonCount,
                        PovCount = joystick.Capabilities.PovCount,
                        JoystickGuid = guid.ToString()
                    };
                }).Where(s => s != null).ToArray();
            }
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
            lock (_joysticGuids)
            {
                _pressedJoystickButtons.Remove(guid);
                _joysticGuids.Remove(guid);
                _joystickWorkers[guid]?.Dispose();
                _joystickWorkers.Remove(guid);
            }
            Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: Removed Joystick with GUID: {guid}");
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
                        Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: {state}");
                        PressedButtonsUpdate?.Invoke(joystickGuid, joyBut, pressed);
                        // worker.ReportProgress(0, state);
                    }
                }
                e.Cancel = true;
            }
            catch (SharpDX.SharpDXException err)
            {
                Console.WriteLine($"{System.Threading.Thread.CurrentThread.ManagedThreadId}: ERROR {err.Message}");
                // disconnected
                joystick?.Dispose();
            }
        }

        [Obsolete("Keep this method for compartibility with old way of pressed buttons check")]
        public static bool IsButtonPressed(ButtonConfig butconf)
        {
            if (butconf == null)
            {
                return false;
            }
            bool pressed = IsPressed(butconf.Use8WayHat, butconf.JoystickGUID, butconf.Button);
            if (butconf.UseModifier)
                pressed = pressed && IsPressed(butconf.Use8WayHat, butconf.ModJoystickGUID, butconf.ModButton);
            if (butconf.Toggle)
            {
                if (pressed && !butconf.laststate)
                    butconf.togglestate = !butconf.togglestate;
                butconf.laststate = pressed;
                return butconf.togglestate;
            }
            pressed = butconf.Invert ? !pressed : pressed;
            return pressed;
        }



        private static bool IsPressed(bool use8wayhat, string joystickGuid, string button)
        {
            int b = -1, p = -1;
            var stickItem = GetJoysticks().FirstOrDefault(j => j.JoystickGuid.Equals(joystickGuid, StringComparison.Ordinal));
            if (stickItem == null)
            {
                return false;
            }

            if (button.StartsWith("But:"))
            {
                int.TryParse(button.Substring(5), out b);
            }
            else if (button.StartsWith("Pov "))
            {
                int.TryParse(button.Substring(4, 1), out p);
                switch (button.Substring(7))
                {
                    case "U":
                        b = 0;
                        break;
                    case "R":
                        b = 9000;
                        break;
                    case "D":
                        b = 18000;
                        break;
                    case "L":
                        b = 27000;
                        break;
                }
            }
            else if (button.StartsWith("P"))
            {
                int.TryParse(button.Substring(1, 1), out p);
                int.TryParse(button.Substring(4), out b);
                b *= 100;
            }
            try
            {
                if (!_joysticGuids.TryGetValue(new Guid(stickItem.JoystickGuid), out Joystick joystick))
                {
                    return false;
                }
                JoystickState state = joystick.GetCurrentState();
                if ((p == -1) && (b == -1)) return false;

                if (p == -1)
                {
                    return state.Buttons[b]; // state.Buttons[b - 1];
                }
                else
                {
                    if (state.PointOfViewControllers[p] == -1) return false;
                    if (use8wayhat)
                    {
                        return state.PointOfViewControllers[p] == b;
                    }
                    return (Math.Abs(state.PointOfViewControllers[p] - b) < 5000) || (state.PointOfViewControllers[p] == 31500 && b == 0);
                }
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
