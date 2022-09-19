namespace XRNeckSafer
{
    public class ActionPropertyEventArgs<T>
    {
        public ActionPropertyEventArgs(T value, ActionPropertyEvent actionEvent, JoystickKeyboardInput input)
        {
            Value = value;
            ActionEvent = actionEvent;
            Input = input;
        }

        public T Value { get; set; }
        public ActionPropertyEvent ActionEvent { get; set; }
        public JoystickKeyboardInput Input { get; set; }
    }
}
