using System.Runtime.Serialization;
using System.Windows.Forms;

namespace XRNeckSafer
{
    [DataContract]
    public class JoystickToKeyboardMapping
    {
        [DataMember]
        public string JoystickButtonId { get; set; }
        [DataMember]
        public Keys KeyboardButton { get; set; }
    }
}
