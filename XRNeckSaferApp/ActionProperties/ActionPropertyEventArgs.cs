namespace XRNeckSafer
{
    public class ActionPropertyEventArgs<T>
    {
        public ActionPropertyEventArgs(T value, ActionPropertyEvent actionEvent)
        {
            Value = value;
            ActionEvent = actionEvent;
        }

        public T Value { get; set; }
        public ActionPropertyEvent ActionEvent { get; set; }
    }
}
