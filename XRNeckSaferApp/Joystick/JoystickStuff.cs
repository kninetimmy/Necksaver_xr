using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace XRNeckSafer
{
    public class JoystickStuff
    {
        private readonly DirectInput _directInput;
        private readonly List<DeviceInstance> _devices = new List<DeviceInstance>();
        private readonly List<StickItem> _stickItems = new List<StickItem>();
        private readonly Dictionary<string, Joystick> _joysticks = new Dictionary<string, Joystick>();
        private readonly List<bool[]> _lastButtons = new List<bool[]>();
        private readonly List<int[]> _lastPOVs = new List<int[]>();

        public bool DisableJoystickReconnect { get; set; }

        private static JoystickStuff _instance;

        public static JoystickStuff Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new JoystickStuff();
                }
                return _instance;
            }
        }

        private JoystickStuff()
        {
            DisableJoystickReconnect = false;
            _directInput = new DirectInput();
            ReloadJoysticks();
        }

        public StickItem GetStickItemByGuid(string guid)
        {
            return _stickItems.Find(s => s.JoystickGuid.Equals(guid, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyCollection<StickItem> GetSticks()
        {
            return _stickItems.AsReadOnly();
        }

        public string GetDeviceNameByGuid(string guid)
        {
            var stickItem = GetStickItemByGuid(guid);
            return stickItem?.InstanceName ?? guid;
        }

        public void ReloadJoysticks()
        {
            _devices.Clear();
            _stickItems.Clear();
            _lastButtons.Clear();
            _lastPOVs.Clear();
            _joysticks.Clear();
            _devices.AddRange(_directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly));

            foreach (var device in _devices)
            {
                var joystick = new Joystick(_directInput, device.InstanceGuid);
                joystick.Acquire();
                _joysticks.Add(device.InstanceGuid.ToString(), joystick);
                var stickItem = new StickItem 
                {
                    Attached = _directInput.IsDeviceAttached(device.InstanceGuid),
                    JoystickGuid = device.InstanceGuid.ToString(),
                    ButtonCount = joystick.Capabilities.ButtonCount,
                    PovCount = joystick.Capabilities.PovCount,
                    InstanceName = joystick.Properties.InstanceName,
                };
                _lastButtons.Add(new bool[128]);
                _lastPOVs.Add(new int[4]);
                
                _stickItems.Add(stickItem);
            }
            InitScan();
        }

        public bool IsButtonPressed(ButtonConfig butconf)
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

        public bool IsPressed(bool use8wayhat, string joystickGuid, string button)
        {
            int b = -1, p = -1;
            var stickItem = GetStickItemByGuid(joystickGuid);
            if (stickItem == null)
            {
                return false;
            }

            if (button.StartsWith("But:"))
            {
                int.TryParse(button.Substring(5), out b);
            }
            else if ((button.StartsWith("Pov ")))
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
                if (!_joysticks.ContainsKey(stickItem.JoystickGuid))
                {
                    return false;
                }
                JoystickState state = _joysticks[stickItem.JoystickGuid].GetCurrentState();
                if ((p == -1) && (b == -1)) return false;

                if (p == -1)
                {
                    return state.Buttons[b - 1];
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
                if (!DisableJoystickReconnect && _directInput.IsDeviceAttached(new Guid(stickItem.JoystickGuid)))
                {
                    ReloadJoysticks();
                }
                return false;
            }
        }

        public List<JoyBut> GetPressedButtons()
        {
            List<JoyBut> result = null;

            for (var stickIndex = 0; stickIndex < _stickItems.Count; stickIndex++)
            {
                StickItem stickItem = _stickItems[stickIndex];
                if (!_joysticks.ContainsKey(stickItem.JoystickGuid))
                {
                    continue;
                }
                JoystickState joystickState = _joysticks[stickItem.JoystickGuid].GetCurrentState();
                for (var buttonIndex = 0; buttonIndex < stickItem.ButtonCount; buttonIndex++)
                {
                    if (joystickState.Buttons[buttonIndex] != _lastButtons[stickIndex][buttonIndex])
                    {
                        var joyBut = new JoyBut
                        {
                            JoystickGuid = stickItem.JoystickGuid,
                            Button = buttonIndex,
                            POV = -1
                        };
                        if (result == null)
                        {
                            result = new List<JoyBut>();
                        }
                        result.Add(joyBut);
                    }
                }
                for (int povIndex = 0; povIndex < stickItem.PovCount; povIndex++)
                {
                    if (joystickState.PointOfViewControllers[povIndex] != _lastPOVs[stickIndex][povIndex])
                    {
                        var joyBut = new JoyBut
                        {
                            JoystickGuid = stickItem.JoystickGuid,
                            Button = joystickState.PointOfViewControllers[povIndex],
                            POV = povIndex
                        };
                        if (result == null)
                        {
                            result = new List<JoyBut>();
                        }
                        result.Add(joyBut);
                    }
                }
            }
            return result;
        }

        private void InitScan()
        {
            for (int i = 0; i < _stickItems.Count; i++)
            {
                var joystickGuid = _stickItems[i].JoystickGuid;
                JoystickState currentState = _joysticks[joystickGuid].GetCurrentState();
                Array.Copy(currentState.Buttons, _lastButtons[i], 128);
                Array.Copy(currentState.PointOfViewControllers, _lastPOVs[i], 4);
            }
        }
    }
}
