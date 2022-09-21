using System.Collections.ObjectModel;
using System.ComponentModel;

namespace XRNeckSafer.Wpf
{
    public class ActionPropertyDataModel
    {
        public ObservableCollection<Input> InputCombinations { get; set; }
        public string ActionPropertyName { get; set; }
        public bool ToggleValue { get; set; }
        public bool IsToggleEnabled { get; set; }
        public bool InvertValue { get; set; }
        public bool IsInvertEnabled { get; set; }
        public string EventName { get; set; }
        public bool Selected { get; set; }
        public object Event { get; set; }
    }

    public class Input : INotifyPropertyChanged
    {
        private string _inputCombination;

        public event PropertyChangedEventHandler PropertyChanged;

        public string InputCombination 
        { 
            get => _inputCombination;
            set 
            {
                if (_inputCombination != value)
                {
                    _inputCombination = value; 
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InputCombination)));
                }
            } 
        }
        public object NewInputCombination { get; set; }
    }
}
