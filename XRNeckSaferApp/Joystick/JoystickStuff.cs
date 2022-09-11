using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace XRNeckSafer
{
    public class JoystickStuff
    {
        private readonly DirectInput _directInput;
        private IList<DeviceInstance> _devices;
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

        public DeviceInstance GetDeviceByIndex(int index)
        {
            return _devices.Count < index + 1 ? null : _devices[index];
        }

        public StickItem GetStickItemByIndex(int index)
        {
            return _stickItems.Count < index + 1 ? null : _stickItems[index];
        }

        public int GetDevicesCount()
        {
            return _devices.Count;
        }

        public string GetDeviceNameByGuid(string guid)
        {
            if (guid == "none") return guid;
            string name = "invalid";
            for (int i = 0; i < _devices.Count; i++)
            {
                if (_devices[i].InstanceGuid.ToString() == guid)
                {
                    name = _devices[i].InstanceName;
                    break;
                }
            }
            return name;
        }
        public int IndexFromGuid(string guid)
        {
            int index = -1;
            for (int i = 0; i < GetDevicesCount(); i++)
            {
                if (GetDeviceByIndex(i).InstanceGuid.ToString().Equals(guid, StringComparison.OrdinalIgnoreCase))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public void ReloadJoysticks()
        {
            _devices = _directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            _stickItems = new List<StickItem>();
            _lastButtons = new List<bool[]>();
            _lastPOVs = new List<int[]>();

            for (int i = 0; i < _devices.Count; i++)
            {
                _stickItems.Add(new StickItem());
                if (_directInput.IsDeviceAttached(_devices[i].InstanceGuid))
                {
                    _stickItems[i].Attached = true;
                }
                _lastButtons.Add(new bool[128]);
                _lastPOVs.Add(new int[4]);
                _stickItems[i].Stick = new Joystick(_directInput, _devices[i].InstanceGuid);
                _stickItems[i].Stick.Acquire();
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

        public bool IsPressed(bool use8wayhat, string JoystickGUID, string Button)
        {
            int j, b = -1, p = -1;

            j = IndexFromGuid(JoystickGUID);
            if (j == -1) return false;

            if (Button.StartsWith("But:"))
            {
                int.TryParse(Button.Substring(5), out b);
            }
            else if ((Button.StartsWith("Pov ")))
            {
                int.TryParse(Button.Substring(4, 1), out p);
                switch (Button.Substring(7))
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
            else if (Button.StartsWith("P"))
            {
                int.TryParse(Button.Substring(1, 1), out p);
                int.TryParse(Button.Substring(4), out b);
                b *= 100;
            }
            try
            {
                JoystickState State = _stickItems[j].Stick.GetCurrentState();
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
                if (!DisableJoystickReconnect)
                    if (_directInput.IsDeviceAttached(_devices[j].InstanceGuid))
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
                JoystickState State = _stickItems[i].Stick.GetCurrentState();
                for (int k = 0; k < _stickItems[i].Stick.Capabilities.ButtonCount; k++)
                {
                    if (State.Buttons[k] != _lastButtons[i][k])
                    {
                        var joyBut = new JoyBut
                        {
                            JoyIndex = i,
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
                for (int k = 0; k < _stickItems[i].Stick.Capabilities.PovCount; k++)
                {
                    if (State.PointOfViewControllers[k] != _lastPOVs[i][k])
                    {
                        var joyBut = new JoyBut
                        {
                            JoyIndex = i,
                            Button = State.PointOfViewControllers[k],
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
