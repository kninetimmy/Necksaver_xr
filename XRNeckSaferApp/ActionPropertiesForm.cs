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
            _wpfList.ScanClick += OnScanClick;
            _wpfList.ClearClick += OnClearClick;
            _wpfList.AddInputClick += OnAddInputClick;
            _wpfList.RemoveInputClick += OnRemoveInputClick;
        }

        private void OnRemoveInputClick(ActionPropertyDataModelEventArgs args)
        {
            var actionProperty = _wpfList.Properties.FirstOrDefault(p => p.InputCombinations.Any(i => i == args.Model));
            if (actionProperty == null)
            {
                return;
            }
            actionProperty.InputCombinations.Remove(args.Model);
        }

        private void OnAddInputClick(ActionPropertyDataModelEventArgs args)
        {
            var actionProperty = _wpfList.Properties.FirstOrDefault(p => p.InputCombinations.Any(i => i == args.Model));
            if (actionProperty == null)
            {
                return;
            }
            actionProperty.InputCombinations.Add(new Input { NewInputCombination = new JoystickKeyboardInput() });
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
            args.Model.InputCombination = result.ToString();
            args.Model.NewInputCombination = result;
        }

        private void PopulateBindingList()
        {
            var props = new ObservableCollection<ActionPropertyDataModel>();
            Config.Instance.ActionProperties.ForEach(prop =>
            {
                var currentProperty = _actionPropertyName?.Equals(prop.Name);
                var boolProp = prop as BooleanActionProperty;
                foreach (var actionEvent in prop.Events)
                {
                    var toggleAction = actionEvent as ActionPropertyToggleEvent;
                    var dataModel = new ActionPropertyDataModel
                    {
                        InputCombinations = new ObservableCollection<Input>(),
                        ActionPropertyName = prop.Name,
                        ActionPropertyNameText = prop.NameText,
                        Description = prop.Description,
                        GroupName = prop.GroupName,
                        EventName = actionEvent.Name,
                        IsToggleEnabled = toggleAction != null,
                        ToggleValue = toggleAction != null && toggleAction.Toggle,
                        IsInvertEnabled = boolProp != null,
                        InvertValue = boolProp?.Invert ?? false,
                        Selected = currentProperty ?? false,
                        Event = actionEvent,
                    };
                    for (var index = 0; index < actionEvent.InputCombinations.Count; index++)
                    {
                        var input = new Input
                        {
                            InputCombination = actionEvent.InputCombinations[index].ToString(),
                            CanAdd = index == 0
                        };
                        dataModel.InputCombinations.Add(input);
                    }
                    props.Add(dataModel);
                }
            });
            _wpfList.PopulateProperties(props.OrderBy(p => p.ActionPropertyName));
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
                var newInputsCount = model.InputCombinations.Count;
                var actionEvent = (ActionPropertyEvent)model.Event;
                var resultInputs = new List<JoystickKeyboardInput>();
                for (int index = 0; index < newInputsCount; index++)
                {
                    Input input = model.InputCombinations[index];
                    var newInput = input.NewInputCombination as JoystickKeyboardInput;
                    if (newInput != null)
                    {
                        resultInputs.Add(newInput);
                        continue;
                    }
                    var existingInput = actionEvent.InputCombinations.FirstOrDefault(i => i.ToString() == input.InputCombination);
                    if (existingInput != null)
                    {
                        resultInputs.Add(existingInput);
                    }
                    else if (string.IsNullOrEmpty(input.InputCombination))
                    {
                        resultInputs.Add(new JoystickKeyboardInput());
                    }
                }
                actionEvent.InputCombinations = resultInputs;
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
