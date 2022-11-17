using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace XRNeckSafer.Wpf
{
    /// <summary>
    /// Interaction logic for KeyboardToJoystickAssignments.xaml
    /// </summary>
    public partial class KeyboardToJoystickAssignments : UserControl
    {
        public ObservableCollection<KeyboardToJoysticAssignmentModel> Mappings { get; set; }

        public event Action<ActionPropertyDataModelEventArgs> ScanKeyboardClick;
        public event Action<ActionPropertyDataModelEventArgs> ScanJoystickClick;

        public KeyboardToJoystickAssignments()
        {
            Mappings = new ObservableCollection<KeyboardToJoysticAssignmentModel>();
            InitializeComponent();
        }

        public void PopulateMappings(IEnumerable<KeyboardToJoysticAssignmentModel> models)
        {
            Mappings.Clear();
            foreach (var model in models)
            {
                Mappings.Add(model);
            }
            if (!Mappings.Any())
            {
                Mappings.Add(new KeyboardToJoysticAssignmentModel
                {
                    JoystickInput = new Input(),
                    KeyboardInput = new Input()
                });
            }
            PopulateCanAddFlags();
        }

        private void PopulateCanAddFlags()
        {
            foreach (var model in Mappings)
            {
                model.CanAdd = false;
            }
            var someAdded = Mappings.Any();
            if (someAdded)
            {
                Mappings.First().CanAdd = true;
            }
        }

        private void OnAddAssignmentClick(object sender, RoutedEventArgs e)
        {
            Mappings.Add(new KeyboardToJoysticAssignmentModel
            {
                JoystickInput = new Input(),
                KeyboardInput = new Input()
            });
            PopulateCanAddFlags();
        }

        private void OnRemoveAssignmentClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as KeyboardToJoysticAssignmentModel;
            Mappings.Remove(model);
        }

        private void OnScanJoystickButtonClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as KeyboardToJoysticAssignmentModel;
            ScanJoystickClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model.JoystickInput });
        }

        private void OnScanKeyboardButtonClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as KeyboardToJoysticAssignmentModel;
            ScanKeyboardClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model.KeyboardInput });
        }

        private void OnClearAssignmentClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as KeyboardToJoysticAssignmentModel;
            model.JoystickInput.NewInputCombination = null;
            model.JoystickInput.InputCombination = null;
            model.KeyboardInput.NewInputCombination = null;
            model.KeyboardInput.InputCombination = null;
        }
    }

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
