using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class NumericUpDownActionProperty : ActionProperty<int>
    {
        const string UP_EVENT_NAME = "Up";
        const string DOWN_EVENT_NAME = "Down";

        public override void DispatchEvent(ActionPropertyEvent actionEvent, bool sameKeys, bool matched)
        {
            if (!matched)
            {
                return;
            }
            switch (actionEvent.Name)
            {
                case UP_EVENT_NAME:
                    Value++;
                    ProcessEvent(actionEvent);
                    break;
                case DOWN_EVENT_NAME:
                    Value--;
                    ProcessEvent(actionEvent);
                    break;
            }
        }

        public static NumericUpDownActionProperty CreateProperty(string name)
        {
            return new NumericUpDownActionProperty
            {
                Name = name,
                Value = 0,
                Events = new[]
                {
                    new ActionPropertyEvent
                    {
                        Name = UP_EVENT_NAME,
                        InputCombination = new JoystickKeyboardInput(),
                    },
                    new ActionPropertyEvent
                    {
                        Name = DOWN_EVENT_NAME,
                        InputCombination = new JoystickKeyboardInput(),
                    }
                }
            };
        }
    }
}
