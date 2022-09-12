using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XRNeckSafer
{
    public class JoystickStuff
    {
        private readonly DirectInput _directInput;
        private List<DeviceInstance> _devices;
        private List<StickItem> _stickItems;
        private List<bool[]> _lastButtons;
        private List<int[]> _lastPOVs;

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
            return _stickItems.Find(s => s.Guid.Equals(guid, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyCollection<StickItem> GetSticks()
        {
            return _stickItems.AsReadOnly();
        }

        public string GetDeviceNameByGuid(string guid)
        {
            var stickItem = GetStickItemByGuid(guid);
            return stickItem?.GetInstanceName() ?? guid;
        }

        public void ReloadJoysticks()
        {
            _devices = _directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).ToList();
            _stickItems = new List<StickItem>();
            _lastButtons = new List<bool[]>();
            _lastPOVs = new List<int[]>();

            for (int i = 0; i < _devices.Count; i++)
            {
                var device = _devices[i];
                var stickItem = new StickItem 
                {
                    Attached = _directInput.IsDeviceAttached(device.InstanceGuid),
                    Guid = device.InstanceGuid.ToString()
                };
                _lastButtons.Add(new bool[128]);
                _lastPOVs.Add(new int[4]);
                stickItem.Stick = new Joystick(_directInput, device.InstanceGuid);
                stickItem.Stick.Acquire();
                _stickItems.Add(stickItem);
            }
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
                JoystickState State = stickItem.Stick.GetCurrentState();
                if ((p == -1) && (b == -1)) return false;

                if (p == -1)
                {
                    return State.Buttons[b - 1];
                }
                else
                {
                    if (State.PointOfViewControllers[p] == -1) return false;
                    if (use8wayhat)
                    {
                        return State.PointOfViewControllers[p] == b;
                    }
                    return (Math.Abs(State.PointOfViewControllers[p] - b) < 5000) || (State.PointOfViewControllers[p] == 31500 && b == 0);
                }
            }
            catch (Exception)
            {
                if (!DisableJoystickReconnect && _directInput.IsDeviceAttached(new Guid(stickItem.Guid)))
                {
                    ReloadJoysticks();
                }
                return false;
            }
        }

        public List<JoyBut> GetPressedButtons()
        {
            List<JoyBut> result = null;

            for (int i = 0; i < _stickItems.Count; i++)
            {
                StickItem stickItem = _stickItems[i];
                Joystick joystick = stickItem.Stick;
                JoystickState joystickState = joystick.GetCurrentState();
                for (int k = 0; k < joystick.Capabilities.ButtonCount; k++)
                {
                    if (joystickState.Buttons[k] != _lastButtons[i][k])
                    {
                        var joyBut = new JoyBut
                        {
                            JoystickGuid = stickItem.Guid,
                            Button = k,
                            POV = -1
                        };
                        if (result == null)
                        {
                            result = new List<JoyBut>();
                        }
                        result.Add(joyBut);
                    }
                }
                for (int k = 0; k < joystick.Capabilities.PovCount; k++)
                {
                    if (joystickState.PointOfViewControllers[k] != _lastPOVs[i][k])
                    {
                        var joyBut = new JoyBut
                        {
                            JoystickGuid = stickItem.Guid,
                            Button = joystickState.PointOfViewControllers[k],
                            POV = k
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

        public void InitScan()
        {
            for (int i = 0; i < _stickItems.Count; i++)
            {
                JoystickState currentState = _stickItems[i].Stick.GetCurrentState();
                Array.Copy(currentState.Buttons, _lastButtons[i], 128);
                Array.Copy(currentState.PointOfViewControllers, _lastPOVs[i], 4);
            }
        }
    }
}
