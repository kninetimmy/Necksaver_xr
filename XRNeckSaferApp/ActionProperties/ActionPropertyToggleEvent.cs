using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class ActionPropertyToggleEvent : ActionPropertyEvent
    {
        [DataMember]
        public bool Toggle { get; set; }
    }
}
