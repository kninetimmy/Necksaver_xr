using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class ActionPropertyEvent
    {
        [DataMember]
        public JoystickKeyboardInput InputCombination { get; set; }
        [DataMember]
        public bool Toggle { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
