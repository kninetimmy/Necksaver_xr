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
                var joystickName = JoystickService.GetJoystickName(button.JoystickGuid) ?? "UNPLUGGED";
                if (button.POV != -1)
                {
                    builder.Append($"[{joystickName} POV:{button.POV + 1} {button.Button / 100}°]");
                    continue;
                }
                builder.Append($"[{joystickName} But:{button.Button + 1}]");
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

        private bool IsEmpty()
        {
            return KeyboardKeys.Count == 0 && JoystickButtons.Count == 0;
        }

        public bool Match(JoystickKeyboardInput input)
        {
            var keyboardMatched = !KeyboardKeys.Any()
                || (KeyboardKeys.Any() && input.KeyboardKeys.Any() && KeyboardKeys.All(k => input.KeyboardKeys.Any(nk => nk == k)));
            var joystickMatched = !JoystickButtons.Any()
                || (JoystickButtons.Any() && input.JoystickButtons.Any() && JoystickButtons.All(b => input.JoystickButtons.Any(nb => nb.GetId() == b.GetId())));
            return keyboardMatched && joystickMatched && !IsEmpty();
        }
    }
}
