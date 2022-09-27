using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace XRNeckSafer.Wpf
{
    /// <summary>
    /// Interaction logic for ActioonPropertyListView.xaml
    /// </summary>
    public partial class ActionPropertyListView : UserControl
    {
        public ObservableCollection<ActionPropertyDataModel> Properties { get; set; } = new ObservableCollection<ActionPropertyDataModel>();
        public event Action<ActionPropertyDataModelChangeEventArgs> Changed;
        public event Action<ActionPropertyDataModelEventArgs> ScanClick;
        public event Action<ActionPropertyDataModelEventArgs> ClearClick;
        public event Action<ActionPropertyDataModelEventArgs> AddInputClick;
        public event Action<ActionPropertyDataModelEventArgs> RemoveInputClick;

        public ActionPropertyListView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var selected = Properties.FirstOrDefault(p => p.Selected);
            if (selected != null)
            {
                DataGridRow row = (DataGridRow)_dataGrid.ItemContainerGenerator.ContainerFromItem(selected);
                var index = _dataGrid.ItemContainerGenerator.IndexFromContainer(row);
                _dataGrid.SelectedIndex = index;
                row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        public void PopulateProperties(IEnumerable<ActionPropertyDataModel> models)
        {
            Properties.Clear();
            foreach (var model in models)
            {
                Properties.Add(model);
            }
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_dataGrid.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("GroupName");
            PropertyGroupDescription subGroupDescription = new PropertyGroupDescription("ActionPropertyNameText");
            view.GroupDescriptions.Add(groupDescription);
            view.GroupDescriptions.Add(subGroupDescription);
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

        private void OnAddClick(ActionPropertyDataModelEventArgs args)
        {
            AddInputClick?.Invoke(args);
        }

        private void OnClearClick(ActionPropertyDataModelEventArgs obj)
        {
            ClearClick?.Invoke(obj);
        }

        private void OnScanClick(ActionPropertyDataModelEventArgs obj)
        {
            ScanClick?.Invoke(obj);
        }

        private void OnRemoveClick(ActionPropertyDataModelEventArgs obj)
        {
            RemoveInputClick?.Invoke(obj);
        }
    }
}
