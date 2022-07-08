using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace XRNeckSafer
{
    public class JoystickButton
    {
        public String JoystickGUID;
        public String Button;
        public String ModJoystickGUID;
        public String ModButton;
    }
    public class StickItem
    {
        public Joystick Stick;
        public bool Attached;
    }
    public class JoystickStuff
    {
        DirectInput DInput;
        public IList<DeviceInstance> ll;
        public List<StickItem> Sticks;
        public JoyBut jb;

        public List<bool[]> LastButtons;
        public List<int[]> LastPOVs;

        public JoystickStuff()
        {
            DInput = new DirectInput();
            GetJoysticks();
        }

        public string NameFromGuid(string guid)
        {
            if (guid == "none") return guid;
            string name = "invalid";
            for (int i = 0; i < ll.Count; i++)
            {
                if (ll[i].InstanceGuid.ToString() == guid)
                {
                    name = ll[i].InstanceName;
                    break;
                }
            }
            return name;
        }
        public int IndexFromGuid(string guid)
        {
            int index = -1;
            for (int i = 0; i < ll.Count; i++)
            {
                if (ll[i].InstanceGuid.ToString() == guid)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        public void GetJoysticks()
        {
            ll = DInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            Sticks = new List<StickItem>();
            LastButtons = new List<bool[]>();
            LastPOVs = new List<int[]>();

            for (int i = 0; i < ll.Count; i++)
            {
                Sticks.Add(new StickItem());
                if (DInput.IsDeviceAttached(ll[i].InstanceGuid))
                {
                    Sticks[i].Attached = true;
                }
                LastButtons.Add(new bool[128]);
                LastPOVs.Add(new int[4]);
                Sticks[i].Stick= new Joystick(DInput, ll[i].InstanceGuid);
                Sticks[i].Stick.Acquire();
            }
        }

        public bool IsButtonPressed(ButtonConfig butconf)
        {
            if (butconf==null)
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
            else if ((Button.StartsWith("P")))
            {
                int.TryParse(Button.Substring(1, 1), out p);
                int.TryParse(Button.Substring(4), out b);
                b *= 100;
            }
            try
            {
                //                bool isCurrentlyAttached = DInput.IsDeviceAttached(ll[j].InstanceGuid);
//                if (DInput.IsDeviceAttached(ll[j].InstanceGuid))
                {
//                    if (!Sticks[j].Attached) // Joystick reaquired?
//                    {
//                        GetJoysticks();
//                        j = IndexFromGuid(JoystickGUID);
//                        if (j == -1) return false;
//                    }

                    JoystickState State = Sticks[j].Stick.GetCurrentState();
                    if ((p == -1) && (b == -1)) return false;

                    if (p == -1)
                    {
                        return State.Buttons[b - 1];
                    }
                    else
                    {
                        if (State.PointOfViewControllers[p] == -1) return false;
                        if (use8wayhat)
                            return State.PointOfViewControllers[p] == b;

                        return (Math.Abs(State.PointOfViewControllers[p] - b) < 5000) || (State.PointOfViewControllers[p] == 31500 && b == 0);
                    }
                }
//                else
//                {
//                    Sticks[j].Attached = false;
//                    return false;
//                }
            }
            catch (Exception)
            {
                if (DInput.IsDeviceAttached(ll[j].InstanceGuid))
                {
 //                   if (!Sticks[j].Attached) // Joystick reaquired?
                    {
                        GetJoysticks();
                    }
                }
                return false;
            }
        }
        public JoyBut ScanJoysticks()
        {
            jb = new JoyBut() { joyIndex = -1, btn = -1, pov = -1 };

            bool found = false;
            for (int i = 0; i < Sticks.Count; i++)
            {
                JoystickState State = Sticks[i].Stick.GetCurrentState();
                for (int k = 0; k < Sticks[i].Stick.Capabilities.ButtonCount; k++)
                {
                    if (State.Buttons[k] != LastButtons[i][k])
                    {
                        jb.joyIndex = i;
                        jb.btn = k;
                        jb.pov = -1;
                        found = true;
                        break;
                    }
                }
                for (int k = 0; k < Sticks[i].Stick.Capabilities.PovCount; k++)
                {
                    if (State.PointOfViewControllers[k] != LastPOVs[i][k])
                    {
                        jb.joyIndex = i;
                        jb.btn = State.PointOfViewControllers[k];
                        jb.pov = k;
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            return jb;
        }
        public void InitScan()
        {
            for (int i = 0; i < Sticks.Count; i++)
            {
                JoystickState State = Sticks[i].Stick.GetCurrentState();
                Array.Copy(State.Buttons, LastButtons[i], 128);
                Array.Copy(State.PointOfViewControllers, LastPOVs[i], 4);
            }
        }
    }

    public class JoyBut
    {
        public int joyIndex;
        public int btn;
        public int pov;

    }
}
