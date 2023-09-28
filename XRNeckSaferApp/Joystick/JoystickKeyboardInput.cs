using System.Linq;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class JoystickKeyboardInput
    {
        [DataMember]
        public KeyboardKeys KeyboardKeys { get; set; } = new KeyboardKeys();

        [DataMember]
        public JoystickButtons JoystickButtons { get; set; } = new JoystickButtons();

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
            return JoystickButtons.ToString()  + KeyboardKeys.ToString();
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
