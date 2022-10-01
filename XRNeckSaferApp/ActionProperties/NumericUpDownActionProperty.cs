using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class NumericUpDownActionProperty : ActionProperty<int>
    {
        const string UP_EVENT_NAME = "Up";
        const string DOWN_EVENT_NAME = "Down";

        public override void DispatchEvent(ActionPropertyEvent actionEvent, JoystickKeyboardInput input, bool sameKeys, bool matched)
        {
            if (!matched)
            {
                return;
            }
            switch (actionEvent.Name)
            {
                case UP_EVENT_NAME:
                    Value++;
                    ProcessEvent(actionEvent, input);
                    break;
                case DOWN_EVENT_NAME:
                    Value--;
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
