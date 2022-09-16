using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class BooleanActionProperty : ActionProperty<bool>
    {
        const string SWITCH_EVENT_NAME = "Switch";

        [DataMember]
        [JsonProperty(Order = 1)]
        public bool Invert { get; set; }

        public override bool GetValue()
        {
            return Invert ? !base.GetValue() : base.GetValue();
        }

        public override void DispatchEvent(ActionPropertyEvent actionEvent, bool sameKeys, bool keyReleased)
        {
            if (sameKeys)
            {
                return;
            }
            switch (actionEvent.Name)
            {
                case SWITCH_EVENT_NAME:
                    Value = !Value;
                    ProcessEvent(actionEvent);
                    break;
            }
        }

        public static BooleanActionProperty CreateProperty(string name)
        {
            return new BooleanActionProperty
            {
                Name = name,
                Value = false,
                Events = new[] 
                { 
                    new ActionPropertyToggleEvent 
                    { 
                        Name = SWITCH_EVENT_NAME,
                        InputCombination = new JoystickKeyboardInput(),
                        Toggle = false
                    } 
                }
            };
        }
    }
}
