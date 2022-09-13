using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class JoystickKeyboardInput
    {
        public List<Keys> KeyboardKeys { get; set; }
        public List<JoyBut> JoystickButtons { get; set; }

        public bool IsEqual(JoystickKeyboardInput input)
        {
            var notEqualKeys = (KeyboardKeys?.Count != input?.KeyboardKeys?.Count) ||
                (KeyboardKeys != null && input.KeyboardKeys != null && KeyboardKeys.Any(k => !input.KeyboardKeys.Contains(k)));
            var notEqualJoystickButtons = (JoystickButtons?.Count != input?.JoystickButtons?.Count) ||
                    (JoystickButtons != null && input.JoystickButtons != null && JoystickButtons.Any(k => !input.JoystickButtons.Any(i => i.GetId().Equals(k.GetId()))));
            return !notEqualKeys && !notEqualJoystickButtons;
        }
    }
}
