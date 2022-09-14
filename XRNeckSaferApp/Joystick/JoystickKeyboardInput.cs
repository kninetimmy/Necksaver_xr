using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

namespace XRNeckSafer
{
    [DataContract]
    public class JoystickKeyboardInput
    {
        [DataMember]
        public List<Keys> KeyboardKeys { get; set; } = new List<Keys>();

        [DataMember]
        public List<JoyBut> JoystickButtons { get; set; } = new List<JoyBut>();

        public bool IsEqual(JoystickKeyboardInput input)
        {
            var notEqualKeys = ((KeyboardKeys?.Count ?? 0) != (input?.KeyboardKeys?.Count ?? 0)) ||
                (KeyboardKeys != null && input.KeyboardKeys != null && KeyboardKeys.Any(k => !input.KeyboardKeys.Contains(k)));
            var notEqualJoystickButtons = ((JoystickButtons?.Count ?? 0) != (input?.JoystickButtons?.Count ?? 0)) ||
                    (JoystickButtons != null && input.JoystickButtons != null && JoystickButtons.Any(k => !input.JoystickButtons.Any(i => i.GetId().Equals(k.GetId()))));
            return !notEqualKeys && !notEqualJoystickButtons;
        }

        public JoystickKeyboardInput Clone()
        {
            var clone = new JoystickKeyboardInput();
            clone.KeyboardKeys.AddRange(KeyboardKeys);
            clone.JoystickButtons.AddRange(JoystickButtons);
            return clone;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var button in JoystickButtons)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                var stickItem = JoystickStuff.Instance.GetStickItemByGuid(button.JoystickGuid);
                builder.Append($"[{stickItem.InstanceName} But:{button.Button + 1}]");
            }
            foreach (var key in KeyboardKeys)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                builder.Append($"[Key:{key}]");
            }
            return builder.ToString();
        }
    }
}
