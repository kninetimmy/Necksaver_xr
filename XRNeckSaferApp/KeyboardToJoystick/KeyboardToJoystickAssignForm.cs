using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    JoystickInput = new Input { InputCombination = mapping.JoystickButton?.ToString() ?? string.Empty },
                    KeyboardInput = new Input { InputCombination = mapping.KeyboardKey != Keys.None ? mapping.KeyboardKey.ToDisplayString() : string.Empty },
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
            var result = ScanJoystickKeyboardForm.ShowForm(FormStartPosition.CenterParent, Top, Left, 1, deviceType);
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
                        m.KeyboardKey.ToDisplayString() == keyboardInput.InputCombination);
                    if (existing != null)
                    {
                        newMapping.KeyboardKey = existing.KeyboardKey;
                    }
                } 
                else
                {
                    var newKeyboardInput = keyboardInput.NewInputCombination as JoystickKeyboardInput;
                    newMapping.KeyboardKey = newKeyboardInput.KeyboardKeys.First();
                }
                if (joystickInput.NewInputCombination == null)
                {
                    var existing = Config.Instance.KeyboardToJoystickAssignments.FirstOrDefault(m =>
                        m.JoystickButton != null &&
                        m.JoystickButton.ToString() == joystickInput.InputCombination);
                    if (existing != null)
                    {
                        newMapping.JoystickButton = existing.JoystickButton;
                    }
                } 
                else
                {
                    var newJoystickInput = joystickInput.NewInputCombination as JoystickKeyboardInput;
                    newMapping.JoystickButton = newJoystickInput.JoystickButtons.First();
                }
                result.Add(newMapping);
            }
            Config.Instance.KeyboardToJoystickAssignments.Clear();
            Config.Instance.KeyboardToJoystickAssignments.AddRange(result);
            Close();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
