using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public class NumericUpDownActionProperty : ActionProperty<int>
    {
        public const string UP_EVENT_NAME = "Up";
        public const string DOWN_EVENT_NAME = "Down";

        public override void DispatchEvent(ActionPropertyEvent actionEvent, bool sameKeys, bool keyReleased)
        {
            if (keyReleased)
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
    }
}
