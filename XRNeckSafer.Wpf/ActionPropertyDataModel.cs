namespace XRNeckSafer.Wpf
{
    public class ActionPropertyDataModel
    {
        public string InputCombination { get; set; }
        public string ActionPropertyName { get; set; }
        public bool ToggleValue { get; set; }
        public bool IsToggleEnabled { get; set; }
        public bool InvertValue { get; set; }
        public bool IsInvertEnabled { get; set; }
        public string EventName { get; set; }

        public object Event { get; set; }
        public object NewInputCombination { get; set; }
    }
}
