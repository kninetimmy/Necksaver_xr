using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace XRNeckSafer
{
    [DataContract]
    public abstract class ActionProperty
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ActionPropertyEvent[] Events { get; set; }

        [JsonIgnore]
        public bool Valid { get; private set; } = true;

        [JsonIgnore]
        public string ValidationError { get; private set; }

        public abstract void DispatchEvent(ActionPropertyEvent actionEvent, bool sameKeys, bool keyReleased);

        public abstract void UnsubscribeTriggerHandlers();

        public bool Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Valid = false;
                ValidationError = "Name is missing";
                return Valid;
            }
            if (Events != null)
            {
                var nameGrouped = Events.GroupBy(t => t.Name).ToDictionary(g => g.Key, g => g.ToArray());
                var invalidName = nameGrouped.Keys.FirstOrDefault(key => nameGrouped[key].Length > 1);
                if (!string.IsNullOrEmpty(invalidName))
                {
                    Valid = false;
                    ValidationError = $"Duplicate event name \"{invalidName}\" in \"{Name}\" action property";
                    return Valid;
                }

                var tuples = Events.Select(t => new Tuple<string, JoystickKeyboardInput>(t.Name, t.InputCombination)).ToList();
                foreach(var tuple in tuples)
                {
                    if (tuples.Count(c => c.Item2.IsEqual(tuple.Item2)) > 1)
                    {
                        Valid = false;
                        ValidationError = $"Duplicate combination \"{tuple.Item2}\" in \"{tuple.Item1}\" event in \"{Name}\" action property";
                        return Valid;
                    }
                }
            }
            return Valid;
        }
    }

    [DataContract]
    public abstract class ActionProperty<T> : ActionProperty
    {
        public event Action<ActionPropertyEventArgs<T>> Triggered;

        [DataMember]
        protected virtual T Value { get; set; }

        public virtual T GetValue()
        {
            return Value;
        }

        public virtual T SetValue(T value)
        {
            return Value = value;
        }

        protected void ProcessEvent(ActionPropertyEvent actionEvent)
        {
            Triggered?.Invoke(new ActionPropertyEventArgs<T>(GetValue(), actionEvent));
        }

        public override void UnsubscribeTriggerHandlers()
        {
            if (Triggered != null)
            {
                foreach (var invokerDelegate in Triggered.GetInvocationList())
                {
                    Triggered -= invokerDelegate as Action<ActionPropertyEventArgs<T>>;
                }
            }
        }
    }
}
