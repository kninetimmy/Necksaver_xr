using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace XRNeckSafer.Wpf
{
    /// <summary>
    /// Interaction logic for InputCombinations.xaml
    /// </summary>
    public partial class InputCombinations : UserControl
    {
        private static readonly DependencyProperty InputsProperty = 
            DependencyProperty.Register(nameof(Inputs), typeof(ObservableCollection<Input>), typeof(InputCombinations));

        public ObservableCollection<Input> Inputs
        {
            get => (ObservableCollection<Input>)GetValue(InputsProperty);
            set => SetValue(InputsProperty, value);
        }

        public event Action<ActionPropertyDataModelEventArgs> ScanClick;
        public event Action<ActionPropertyDataModelEventArgs> ClearClick;
        public event Action<ActionPropertyDataModelEventArgs> AddInputClick;
        public event Action<ActionPropertyDataModelEventArgs> RemoveInputClick;

        public InputCombinations()
        {
            InitializeComponent();
        }

        private void OnScanButtonClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as Input;
            ScanClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model });
        }

        private void OnAddRemoveShortcutClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as Input;
            if (model.CanAdd)
            {
                AddInputClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model });
                return;
            }
            RemoveInputClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model });
        }

        private void ClearEventClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)sender).DataContext as Input;
            ClearClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model });
        }
    }
}
