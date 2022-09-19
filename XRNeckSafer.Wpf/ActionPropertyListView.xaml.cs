using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace XRNeckSafer.Wpf
{
    /// <summary>
    /// Interaction logic for ActioonPropertyListView.xaml
    /// </summary>
    public partial class ActioonPropertyListView : UserControl
    {
        public ObservableCollection<ActionPropertyDataModel> Properties { get; set; } = new ObservableCollection<ActionPropertyDataModel>();

        public event Action<ActionPropertyDataModelChangeEventArgs> Changed;
        public event Action<ActionPropertyDataModelEventArgs> ScanClick;
        public event Action<ActionPropertyDataModelEventArgs> ClearClick;

        public ActioonPropertyListView()
        {
            InitializeComponent();
        }

        public void PopulateProperties(IEnumerable<ActionPropertyDataModel> models)
        {
            Properties.Clear();
            foreach (var model in models)
            {
                Properties.Add(model);
            }
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_listView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ActionPropertyName");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void OnScanButtonClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)e.OriginalSource).DataContext as Input;
            ScanClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model });
        }

        private void OnInvertCheckBoxClick(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)e.Source;
            var model = checkBox.DataContext as ActionPropertyDataModel;
            model.InvertValue = checkBox.IsChecked ?? false;
            Changed?.Invoke(new ActionPropertyDataModelChangeEventArgs
            {
                Model = model,
                ChangedProperty = nameof(model.InvertValue)
            });
        }

        private void OnToggleCheckboxClick(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)e.Source;
            var model = checkBox.DataContext as ActionPropertyDataModel;
            model.ToggleValue = checkBox.IsChecked ?? false;

            Changed?.Invoke(new ActionPropertyDataModelChangeEventArgs
            {
                Model = model,
                ChangedProperty = nameof(model.ToggleValue)
            });
        }

        private void ClearEventClick(object sender, RoutedEventArgs e)
        {
            var model = ((Button)sender).DataContext as Input;
            ClearClick?.Invoke(new ActionPropertyDataModelEventArgs { Model = model });
        }
    }
}
