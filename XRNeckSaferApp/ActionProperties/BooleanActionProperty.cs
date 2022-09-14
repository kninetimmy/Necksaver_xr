using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class BooleanActionProperty : ActionProperty<bool>
    {
        const string TOGGLE_EVENT_NAME = "Toggle";

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
                case TOGGLE_EVENT_NAME:
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
                    new ActionPropertyEvent 
                    { 
                        Name = TOGGLE_EVENT_NAME,
                        InputCombination = new JoystickKeyboardInput(),
                        Toggle = false
                    } 
                }
            };
        }
    }
}
