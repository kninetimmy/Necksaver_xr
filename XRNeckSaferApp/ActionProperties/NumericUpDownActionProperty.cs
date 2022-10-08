using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class NumericUpDownActionProperty : ActionProperty<decimal>
    {
        const string UP_EVENT_NAME = "Up";
        const string DOWN_EVENT_NAME = "Down";

        [IgnoreDataMember]
        public decimal Minimum { get; set; } = decimal.MinValue;

        [IgnoreDataMember]
        public decimal Maximum { get; set; } = decimal.MaxValue;

        [IgnoreDataMember]
        public decimal Increment { get; set; } = 1;

        public override void DispatchEvent(ActionPropertyEvent actionEvent, JoystickKeyboardInput input, bool sameKeys, bool matched)
        {
            if (!matched)
            {
                return;
            }
            switch (actionEvent.Name)
            {
                case UP_EVENT_NAME:
                    var newValue = Value + Increment;
                    if (newValue > Maximum)
                    {
                        break;
                    }
                    Value = newValue;
                    ProcessEvent(actionEvent, input);
                    break;
                case DOWN_EVENT_NAME:
                    var newVal = Value - Increment;
                    if (newVal < Minimum)
                    {
                        break;
                    }
                    Value = newVal;
                    ProcessEvent(actionEvent, input);
                    break;
            }
        }

        public static NumericUpDownActionProperty CreateProperty(string name)
        {
            return new NumericUpDownActionProperty
            {
                Id = name,
                Value = 0,
                Events = new[]
                {
                    new ActionPropertyEvent
                    {
                        Name = UP_EVENT_NAME,
                        InputCombinations = new List<JoystickKeyboardInput> { new JoystickKeyboardInput() },
                    },
                    new ActionPropertyEvent
                    {
                        Name = DOWN_EVENT_NAME,
                        InputCombinations = new List<JoystickKeyboardInput> { new JoystickKeyboardInput() },
                    }
                }
            };
        }
    }
}
