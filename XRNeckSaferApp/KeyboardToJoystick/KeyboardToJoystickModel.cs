using System.Runtime.Serialization;
using System.Windows.Forms;

namespace XRNeckSafer
{
    [DataContract]
    public class KeyboardToJoystickModel
    {
        [DataMember]
        public JoystickButton JoystickButton { get; set; }
        [DataMember]
        public Keys KeyboardKey { get; set; }
    }
}
