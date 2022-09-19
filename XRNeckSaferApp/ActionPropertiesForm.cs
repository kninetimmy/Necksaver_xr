using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using XRNeckSafer.Wpf;

namespace XRNeckSafer
{
    public partial class ActionPropertiesForm : Form
    {
        private readonly string _actionPropertyName;
        // private List<int> _selectedIndexes;

        public static void ShowForm(string actionPropertyName, int mainFormTop, int mainFormRight)
        {
            using (var form = new ActionPropertiesForm(actionPropertyName, mainFormTop, mainFormRight))
            {
                form.ShowDialog();
            }
        }

        private ActionPropertiesForm(string actionPropertyName, int mainFormTop, int mainFormRight)
        {
            _actionPropertyName = actionPropertyName;
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            Top = mainFormTop;
            Left = mainFormRight - 10;
            PopulateBindingList();
            MinimumSize = Size;
            // _wpfList.Changed += ActionPropertyListChanged;
            _wpfList.ScanClick += OnScanClick;
            _wpfList.ClearClick += OnClearClick;
        }

        private void OnClearClick(ActionPropertyDataModelEventArgs args)
        {
            args.Model.InputCombination = string.Empty;
            args.Model.NewInputCombination = new JoystickKeyboardInput();
        }

        private void OnScanClick(ActionPropertyDataModelEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ActionPropertyDataModelEventArgs>(OnScanClick), args);
                return;
            }
            var result = ScanJoystickKeyboardForm.ShowForm(FormStartPosition.CenterParent, Top, Left, 2);
            if (result == null)
            {
                return;
            }
            //var property = Config.Instance.ActionProperties.FirstOrDefault(p => p.Name == args.Model.ActionPropertyName);
            //if (property == null)
            //{
            //    return;
            //}
            // var actionEvent = property.Events.FirstOrDefault(e => e.Name == args.Model.EventName);
            args.Model.InputCombination = result.ToString();
            args.Model.NewInputCombination = result;
        }

        private void PopulateBindingList()
        {
            // _selectedIndexes = new List<int>();
            var props = new ObservableCollection<ActionPropertyDataModel>();
            Config.Instance.ActionProperties.ForEach(prop =>
            {
                var currentProperty = _actionPropertyName.Equals(prop.Name);
                var boolProp = prop as BooleanActionProperty;
                foreach (var actionEvent in prop.Events)
                {
                    var toggleAction = actionEvent as ActionPropertyToggleEvent;
                    var dataModel = new ActionPropertyDataModel
                    {
                        InputCombinations = new ObservableCollection<Input>(actionEvent.InputCombinations
                            .Select(i => new Input
                            { 
                                InputCombination = i.ToString(),
                                EventName = actionEvent.Name,
                                ActionPropertyName = prop.Name,
                            }).ToArray()),
                        ActionPropertyName = prop.Name,
                        EventName = actionEvent.Name,
                        IsToggleEnabled = toggleAction != null,
                        ToggleValue = toggleAction != null && toggleAction.Toggle,
                        IsInvertEnabled = boolProp != null,
                        InvertValue = boolProp?.Invert ?? false,
                        Event = actionEvent,
                    };
                    props.Add(dataModel);
                }
            });
            _wpfList.PopulateProperties(props);
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            foreach (ActionPropertyDataModel model in _wpfList.Properties)
            {
                var configActionProperty = Config.Instance.ActionProperties.FirstOrDefault(p => p.Name.Equals(model.ActionPropertyName, StringComparison.Ordinal));
                if (configActionProperty == null)
                {
                    continue;
                }
                var actionEvent = (ActionPropertyEvent)model.Event;
                for (int index = 0; index < model.InputCombinations.Count; index++)
                {
                    Input input = model.InputCombinations[index];
                    var newInput = input.NewInputCombination as JoystickKeyboardInput;
                    if (newInput != null)
                    {
                        actionEvent.InputCombinations[index] = newInput;
                    }
                }
                if (actionEvent is ActionPropertyToggleEvent toggleEvent && toggleEvent != null)
                {
                    toggleEvent.Toggle = model.ToggleValue;
                }
                if (configActionProperty is BooleanActionProperty boolProp && boolProp != null)
                {
                    boolProp.Invert = model.InvertValue;
                }
            }
            Close();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
