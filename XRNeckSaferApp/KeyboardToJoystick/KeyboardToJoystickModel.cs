using System;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace XRNeckSafer
{
    [DataContract]
    public class KeyboardToJoystickModel: JoystickKeyboardInput
    {
        [DataMember(EmitDefaultValue = false)]
        [Obsolete("Needs to be removed in a new version")]
        public JoystickButton JoystickButton { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [Obsolete("Needs to be removed in a new version")]
        public Keys KeyboardKey { get; set; }
    }
}
