using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ActionPropertiesForm : Form
    {
        // private readonly BindingList<ActionPropertyDataModel> _bindingSource = new BindingList<ActionPropertyDataModel>();
        private readonly string _actionPropertyName;
        private List<int> _selectedIndexes;

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
            _wpfList.Changed += ActionPropertyListChanged;
            _wpfList.ScanClick += OnScanClick;
        }

        private void OnScanClick(Wpf.ActionPropertyDataModelScanEventArgs args)
        {
            var result = ScanJoystickKeyboardForm.ShowForm(FormStartPosition.CenterParent, Top, Left, 2);
            if (result == null)
            {
                return;
            }
            var property = Config.Instance.ActionProperties.FirstOrDefault(p => p.Name == args.Model.ActionPropertyName);
            if (property == null)
            {
                return;
            }
            var actionEvent = property.Events.FirstOrDefault(e => e.Name == args.Model.EventName);
            args.Model.InputCombination = result.ToString();
            args.Model.NewInputCombination = result;
        }

        private void ActionPropertyListChanged(Wpf.ActionPropertyDataModelChangeEventArgs args)
        {
            var property = Config.Instance.ActionProperties.FirstOrDefault(p => p.Name == args.Model.ActionPropertyName);
            if (property == null)
            {
                return;
            }
            var actionEvent = property.Events.FirstOrDefault(e => e.Name == args.Model.EventName);
            //switch(args.ChangedProperty)
            //{
            //    case nameof(args.Model.NewToggleValue):
            //        actionEvent.Toggle = args.Model.NewToggleValue;
            //        break;
            //    case nameof(args.Model.NewInvertValue):
            //        if (property is BooleanActionProperty booleanProp)
            //        {
            //            booleanProp.Invert = args.Model.NewInvertValue;
            //        }
            //        break;
            //}
        }

        private void PopulateBindingList()
        {
            _selectedIndexes = new List<int>();
            var props = new List<Wpf.ActionPropertyDataModel>();
            Config.Instance.ActionProperties.ForEach(prop =>
            {
                var currentProperty = _actionPropertyName.Equals(prop.Name);
                var boolProp = prop as BooleanActionProperty;
                foreach (var actionEvent in prop.Events)
                {
                    var toggleAction = actionEvent as ActionPropertyToggleEvent;
                    var dataModel = new Wpf.ActionPropertyDataModel
                    {
                        InputCombination = actionEvent.InputCombination?.ToString(),
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
            foreach (var model in _wpfList.Properties)
            {
                var configActionProperty = Config.Instance.ActionProperties.FirstOrDefault(p => p.Name.Equals(model.ActionPropertyName, StringComparison.Ordinal));
                if (configActionProperty == null)
                {
                    continue;
                }
                var actionEvent = (ActionPropertyEvent)model.Event;
                var newInput = model.NewInputCombination as JoystickKeyboardInput;
                if (newInput != null)
                {
                    actionEvent.InputCombination = newInput;
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
