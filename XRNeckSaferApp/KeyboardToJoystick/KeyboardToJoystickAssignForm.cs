using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XRNeckSafer;
using XRNeckSafer.Wpf;

namespace XRNeckSaferApp
{
    public partial class KeyboardToJoystickAssignForm : Form
    {
        public static void ShowForm(int mainFormTop, int mainFormRight)
        {
            using (var form = new KeyboardToJoystickAssignForm(mainFormTop, mainFormRight))
            {
                form.ShowDialog();
            }
        }

        private KeyboardToJoystickAssignForm(int mainFormTop, int mainFormRight)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            Top = mainFormTop;
            Left = mainFormRight - 10;
            PopulateBindingList();
            MinimumSize = Size;
            _wpfList.ScanKeyboardClick += OnScanKeyboardClick;
            _wpfList.ScanJoystickClick += OnScanJoystickClick;
            //_wpfMappingList.ClearClick += OnClearClick;
            //_wpfMappingList.AddInputClick += OnAddInputClick;
            //_wpfMappingList.RemoveInputClick += OnRemoveInputClick;
        }

        private void PopulateBindingList()
        {
            var props = new List<KeyboardToJoysticAssignmentModel>();
            Config.Instance.KeyboardToJoystickAssignments.ForEach(mapping =>
            {
                var dataModel = new KeyboardToJoysticAssignmentModel
                {
                    JoystickInput = new Input { InputCombination = mapping.JoystickButtons?.ToString() ?? string.Empty },
                    KeyboardInput = new Input { InputCombination = mapping.KeyboardKeys?.ToString() ?? string.Empty },
                };
                props.Add(dataModel);
            });
            _wpfList.PopulateAssignments(props);
        }

        private void OnScanKeyboardClick(ActionPropertyDataModelEventArgs args)
        {
            ScanInput(args, DeviceType.Keyboard);
        }

        private void OnScanJoystickClick(ActionPropertyDataModelEventArgs args)
        {
            ScanInput(args, DeviceType.Joystick);
        }

        private void ScanInput(ActionPropertyDataModelEventArgs args, DeviceType deviceType)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ActionPropertyDataModelEventArgs, DeviceType>(ScanInput), args, deviceType);
                return;
            }
            KeyboardToJoystickService.Instanse.Enabled = false;
            var result = ScanJoystickKeyboardForm.ShowForm(FormStartPosition.CenterParent, Top, Left, 2, deviceType);
            KeyboardToJoystickService.Instanse.Enabled = true;
            if (result == null)
            {
                return;
            }
            args.Model.InputCombination = result.ToString();
            args.Model.NewInputCombination = result;
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            var result = new List<KeyboardToJoystickModel>();
            foreach (var model in _wpfList.Assignments)
            {
                var keyboardInput = model.KeyboardInput;
                var joystickInput = model.JoystickInput;
                if (string.IsNullOrEmpty(keyboardInput.InputCombination) && keyboardInput.NewInputCombination == null 
                    && string.IsNullOrEmpty(joystickInput.InputCombination) && joystickInput.NewInputCombination == null)
                {
                    continue;
                }
                var newMapping = new KeyboardToJoystickModel();
                if (keyboardInput.NewInputCombination == null)
                {
                    var existing = Config.Instance.KeyboardToJoystickAssignments.FirstOrDefault(m =>
                        m.KeyboardKeys.ToString() == keyboardInput.InputCombination);
                    if (existing != null)
                    {
                        newMapping.KeyboardKeys = existing.KeyboardKeys.Clone();
                    }
                } 
                else
                {
                    var newKeyboardInput = keyboardInput.NewInputCombination as JoystickKeyboardInput;
                    newMapping.KeyboardKeys = newKeyboardInput.KeyboardKeys.Clone();
                }
                if (joystickInput.NewInputCombination == null)
                {
                    var existing = Config.Instance.KeyboardToJoystickAssignments.FirstOrDefault(m =>
                        m.JoystickButtons != null &&
                        m.JoystickButtons.ToString() == joystickInput.InputCombination);
                    if (existing != null)
                    {
                        newMapping.JoystickButtons = existing.JoystickButtons.Clone();
                    }
                } 
                else
                {
                    var newJoystickInput = joystickInput.NewInputCombination as JoystickKeyboardInput;
                    newMapping.JoystickButtons = newJoystickInput.JoystickButtons.Clone();
                }
                result.Add(newMapping);
            }
            Config.Instance.KeyboardToJoystickAssignments.Clear();
            Config.Instance.KeyboardToJoystickAssignments.AddRange(result);
            Config.Instance.WriteConfig();
            Close();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
