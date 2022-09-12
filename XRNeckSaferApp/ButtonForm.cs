using System;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ButtonForm : Form
    {
        private readonly ButtonConfig _buttonConfig;

        public ButtonForm(int mainFormTop, int mainFormRight, string title, ButtonConfig bc)
        {
            _buttonConfig = bc;
            InitializeComponent();
            MinimumSize = Size;
            MaximumSize = Size;
            StartPosition = FormStartPosition.Manual;
            Top = mainFormTop;
            Left = mainFormRight - 10;
            Text = title;
            JoystickStuff.Instance.ReloadJoysticks();

            UseModifierCheckBox.Checked = _buttonConfig.UseModifier;
            InvertcheckBox.Checked = _buttonConfig.Invert;
            toggleCheckBox.Checked = _buttonConfig.Toggle;
            Use8WayHatCheckBox.Checked = _buttonConfig.Use8WayHat;

            FillComboBoxes();
        }

        private void FillComboBoxes()
        {
            MainDeviceComboBox.Items.Clear();
            MainDeviceComboBox.Items.Add("none");
            ModifierDeviceComboBox.Items.Clear();
            ModifierDeviceComboBox.Items.Add("none");
            MainButtonComboBox.Items.Clear();
            MainButtonComboBox.Items.Add("none");
            ModifierButtonComboBox.Items.Clear();
            ModifierButtonComboBox.Items.Add("none");


            for (int i = 0; i < JoystickStuff.Instance.GetDevicesCount(); i++)
            {
                var device = JoystickStuff.Instance.GetDeviceByIndex(i);
                MainDeviceComboBox.Items.Add(device.InstanceName);
                ModifierDeviceComboBox.Items.Add(device.InstanceName);
            }

            int Index = JoystickStuff.Instance.IndexFromGuid(_buttonConfig.JoystickGUID);
            MainDeviceComboBox.SelectedIndex = Index + 1;
            FillButtonComboBox(Index, MainButtonComboBox);

            Index = JoystickStuff.Instance.IndexFromGuid(_buttonConfig.ModJoystickGUID);
            ModifierDeviceComboBox.SelectedIndex = Index + 1;
            FillButtonComboBox(Index, ModifierButtonComboBox);
            ModifierButtonComboBox.Text = _buttonConfig.ModButton;

            MainButtonComboBox.Text = _buttonConfig.Button;
        }

        private void FillButtonComboBox(int joyIndex, ComboBox cb)
        {
            cb.Items.Clear();
            cb.Items.Add("none");
            if (joyIndex == -1) return;
            var stickItem = JoystickStuff.Instance.GetStickItemByIndex(joyIndex);
            for (int i = 0; i < stickItem.Stick.Capabilities.ButtonCount; i++)
            {
                cb.Items.Add("But: " + (i + 1));
            }
            for (int i = 0; i < stickItem.Stick.Capabilities.PovCount; i++)
            {
                if (_buttonConfig.Use8WayHat)
                {
                    for (int j = 0; j < 360; j += 45)
                    {
                        cb.Items.Add("P" + i + ": " + j);
                    }
                }
                else
                {
                    cb.Items.Add("Pov " + i + ": U");
                    cb.Items.Add("Pov " + i + ": R");
                    cb.Items.Add("Pov " + i + ": D");
                    cb.Items.Add("Pov " + i + ": L");
                }
            }

        }

        private void OnMainScanButtonClick(object sender, EventArgs e)
        {
            var buttons = ScanForm.ShowForm(FormStartPosition.Manual, Top + 10, Left, 2);
            if (buttons == null || !buttons.Any())
            {
                return;
            }
            var modifierButton = buttons.Count > 1 ? buttons[0] : null;
            var mainButton = buttons.Count > 1 ? buttons[1] : buttons[0];
            if (modifierButton != null)
            {
                ProcessModifierButton(modifierButton);
                UseModifierCheckBox.Checked = true;
            }
            else
            {
                UseModifierCheckBox.Checked = false;
            }
            ProcessMainButton(mainButton);
        }

        private void ProcessMainButton(JoyBut joyBut)
        {
            MainDeviceComboBox.Text = MainDeviceComboBox.Items[joyBut.JoyIndex + 1].ToString();
            FillButtonComboBox(joyBut.JoyIndex, MainButtonComboBox);
            if (joyBut.POV == -1)
            {
                MainButtonComboBox.Text = MainButtonComboBox.Items[joyBut.Button + 1].ToString();
            }
            else
            {
                var stickItem = JoystickStuff.Instance.GetStickItemByIndex(joyBut.JoyIndex);
                if (_buttonConfig.Use8WayHat)
                {
                    int butindex =
                        stickItem.Stick.Capabilities.ButtonCount
                        + joyBut.POV * 8
                        + joyBut.Button / 4500
                        + 1;
                    MainButtonComboBox.Text = MainButtonComboBox.Items[butindex].ToString();
                }
                else
                {
                    int butindex =
                        stickItem.Stick.Capabilities.ButtonCount
                        + joyBut.POV * 4
                        + joyBut.Button / 9000
                        + 1;
                    MainButtonComboBox.Text = MainButtonComboBox.Items[butindex].ToString();
                }
            }
        }

        private void ProcessModifierButton(JoyBut joyBut)
        {
            ModifierDeviceComboBox.Text = ModifierDeviceComboBox.Items[joyBut.JoyIndex + 1].ToString();
            FillButtonComboBox(joyBut.JoyIndex, ModifierButtonComboBox);
            if (joyBut.POV == -1)
            {
                ModifierButtonComboBox.Text = ModifierButtonComboBox.Items[joyBut.Button + 1].ToString();
            }
            else
            {
                int butindex =
                    JoystickStuff.Instance.GetStickItemByIndex(joyBut.JoyIndex).Stick.Capabilities.ButtonCount
                    + joyBut.POV * 4
                    + joyBut.Button / 9000
                    + 1;
                ModifierButtonComboBox.Text = ModifierButtonComboBox.Items[butindex].ToString();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            MainDeviceComboBox.Text = "none";
            MainButtonComboBox.Text = "none";
            ModifierDeviceComboBox.Text = "none";
            ModifierButtonComboBox.Text = "none";
        }

        private void UseModifierCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = UseModifierCheckBox.Checked;
            ModifierDeviceComboBox.Enabled = UseModifierCheckBox.Checked;
            ModifierButtonComboBox.Enabled = UseModifierCheckBox.Checked;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (MainDeviceComboBox.SelectedIndex == 0)
                _buttonConfig.JoystickGUID = "none";
            else
                _buttonConfig.JoystickGUID = JoystickStuff.Instance.GetDeviceByIndex(MainDeviceComboBox.SelectedIndex - 1).InstanceGuid.ToString();
            _buttonConfig.Button = MainButtonComboBox.Text;

            if (ModifierDeviceComboBox.SelectedIndex == 0)
                _buttonConfig.ModJoystickGUID = "none";
            else
                _buttonConfig.ModJoystickGUID = JoystickStuff.Instance.GetDeviceByIndex(ModifierDeviceComboBox.SelectedIndex - 1).InstanceGuid.ToString();

            _buttonConfig.ModButton = ModifierButtonComboBox.Text;
            _buttonConfig.UseModifier = UseModifierCheckBox.Checked;
            _buttonConfig.Invert = InvertcheckBox.Checked;
            _buttonConfig.Toggle = toggleCheckBox.Checked;
            _buttonConfig.Use8WayHat = Use8WayHatCheckBox.Checked;

            Config.Instance.WriteConfig();
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Config.ReloadConfig();
            Close();
        }

        private void MainDeviceComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            FillButtonComboBox(MainDeviceComboBox.SelectedIndex - 1, MainButtonComboBox);
            MainButtonComboBox.Text = "none";
        }

        private void ModifierDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillButtonComboBox(MainDeviceComboBox.SelectedIndex - 1, MainButtonComboBox);
            MainButtonComboBox.Text = "none";
        }

        private void Use8WayHatCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _buttonConfig.Use8WayHat = Use8WayHatCheckBox.Checked;
            FillComboBoxes();
        }
    }
}
