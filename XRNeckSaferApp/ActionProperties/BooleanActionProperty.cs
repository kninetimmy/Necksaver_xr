using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class BooleanActionProperty : ActionProperty<bool>
    {
        const string SWITCH_EVENT_NAME = "On/Off";

        [DataMember]
        [JsonProperty(Order = 1)]
        public bool Invert { get; set; }

        public override bool GetValue()
        {
            return Invert ? !base.GetValue() : base.GetValue();
        }

        public override void DispatchEvent(ActionPropertyEvent actionEvent, JoystickKeyboardInput input, bool sameKeys, bool matched)
        {
            if (sameKeys)
            {
                return;
            }
            var toggle = ((ActionPropertyToggleEvent)actionEvent).Toggle;
            switch (actionEvent.Name)
            {
                case SWITCH_EVENT_NAME:
                    Value = GetNewValue(matched, toggle);
                    ProcessEvent(actionEvent, input);
                    break;
            }
        }

        private bool GetNewValue(bool matched, bool toggle)
        {
            if (toggle && matched)
            {
                return !Value;
            }
            if (toggle && !matched)
            {
                return Value;
            }
            return matched;
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
                        InputCombinations = new List<JoystickKeyboardInput> { 
                            new JoystickKeyboardInput()
                        },
                        Toggle = false
                    }
                }
            };
        }
    }
}
