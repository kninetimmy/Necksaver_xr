using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class ActionPropertyEvent
    {
        [DataMember]
        public List<JoystickKeyboardInput> InputCombinations { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
