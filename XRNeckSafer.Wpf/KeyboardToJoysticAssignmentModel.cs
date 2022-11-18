using System.ComponentModel;

namespace XRNeckSafer.Wpf
{
    public class KeyboardToJoysticAssignmentModel : INotifyPropertyChanged
    {
        public Input JoystickInput { get; set; }
        public Input KeyboardInput { get; set; }

        private bool _canAdd;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanAdd
        {
            get => _canAdd;
            set
            {
                if (_canAdd != value)
                {
                    _canAdd = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanAdd)));
                }
            }
        }
    }
}
