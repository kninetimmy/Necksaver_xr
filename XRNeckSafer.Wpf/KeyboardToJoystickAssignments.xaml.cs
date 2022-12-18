using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<KeyboardToJoysticAssignmentModel> Assignments { get; set; }

        public event Action<ActionPropertyDataModelEventArgs> ScanKeyboardClick;
        public event Action<ActionPropertyDataModelEventArgs> ScanJoystickClick;

        public KeyboardToJoystickAssignments()
        {
            Assignments = new ObservableCollection<KeyboardToJoysticAssignmentModel>();
            InitializeComponent();
        }

        public void PopulateAssignments(IEnumerable<KeyboardToJoysticAssignmentModel> models)
        {
            Assignments.Clear();
            foreach (var model in models)
            {
                Assignments.Add(model);
            }
            if (!Assignments.Any())
            {
                Assignments.Add(new KeyboardToJoysticAssignmentModel
                {
                    JoystickInput = new Input(),
                    KeyboardInput = new Input()
                });
            }
            PopulateCanAddFlags();
        }

        private void PopulateCanAddFlags()
        {
            foreach (var model in Assignments)
            {
                model.CanAdd = false;
            }
            if (Assignments.Any())
            {
                Assignments.First().CanAdd = true;
            }
        }

        private void OnAddAssignmentClick(object sender, RoutedEventArgs e)
        {
            Assignments.Add(new KeyboardToJoysticAssignmentModel
            {
                JoystickInput = new Input(),
                KeyboardInput = new Input()
            });
            PopulateCanAddFlags();
        }

        private void OnRemoveAssignmentClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as KeyboardToJoysticAssignmentModel;
            Assignments.Remove(model);
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
}
