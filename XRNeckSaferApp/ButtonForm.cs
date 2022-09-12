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
            MainDeviceComboBox.Items.Add(new ComboBoxStickItem());
            ModifierDeviceComboBox.Items.Clear();
            ModifierDeviceComboBox.Items.Add(new ComboBoxStickItem());
            MainButtonComboBox.Items.Clear();
            MainButtonComboBox.Items.Add("none");
            ModifierButtonComboBox.Items.Clear();
            ModifierButtonComboBox.Items.Add("none");

            foreach (StickItem stick in JoystickStuff.Instance.GetSticks())
            {
                var comboBoxItem = new ComboBoxStickItem
                {
                    StickItem = stick
                };
                MainDeviceComboBox.Items.Add(comboBoxItem);
                ModifierDeviceComboBox.Items.Add(comboBoxItem);
                if (_buttonConfig.JoystickGUID.Equals(stick.JoystickGuid))
                {
                    MainDeviceComboBox.SelectedItem = comboBoxItem;
                }
                if (_buttonConfig.ModJoystickGUID.Equals(stick.JoystickGuid))
                {
                    ModifierDeviceComboBox.SelectedItem = comboBoxItem;
                }
            }

            if (MainDeviceComboBox.SelectedItem == null)
            {
                MainDeviceComboBox.SelectedIndex = 0;
            }
            if (ModifierDeviceComboBox.SelectedItem == null)
            {
                ModifierDeviceComboBox.SelectedIndex = 0;
            }

            var mainStickItem = JoystickStuff.Instance.GetStickItemByGuid(_buttonConfig.JoystickGUID);
            FillButtonComboBox(mainStickItem, MainButtonComboBox);

            var modifierStickItem = JoystickStuff.Instance.GetStickItemByGuid(_buttonConfig.ModJoystickGUID);
            FillButtonComboBox(modifierStickItem, ModifierButtonComboBox);
            ModifierButtonComboBox.Text = _buttonConfig.ModButton;

            MainButtonComboBox.Text = _buttonConfig.Button;
        }

        private void FillButtonComboBox(StickItem stickItem, ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("none");
            if (stickItem == null)
            {
                comboBox.SelectedIndex = 0;
                return;
            }
            for (int i = 0; i < stickItem.ButtonCount; i++)
            {
                comboBox.Items.Add("But: " + (i + 1));
            }
            for (int i = 0; i < stickItem.PovCount; i++)
            {
                if (_buttonConfig.Use8WayHat)
                {
                    for (int j = 0; j < 360; j += 45)
                    {
                        comboBox.Items.Add("P" + i + ": " + j);
                    }
                }
                else
                {
                    comboBox.Items.Add("Pov " + i + ": U");
                    comboBox.Items.Add("Pov " + i + ": R");
                    comboBox.Items.Add("Pov " + i + ": D");
                    comboBox.Items.Add("Pov " + i + ": L");
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
            var stickItem = JoystickStuff.Instance.GetStickItemByGuid(joyBut.JoystickGuid);
            MainDeviceComboBox.Text = stickItem?.InstanceName ?? "none";
            FillButtonComboBox(stickItem, MainButtonComboBox);
            if (stickItem == null)
            {
                MainButtonComboBox.Text = "none";
                return;
            }
            if (joyBut.POV == -1)
            {
                MainButtonComboBox.Text = MainButtonComboBox.Items[joyBut.Button + 1].ToString();
            }
            else
            {
                if (_buttonConfig.Use8WayHat)
                {
                    int butindex =
                        stickItem.ButtonCount
                        + joyBut.POV * 8
                        + joyBut.Button / 4500
                        + 1;
                    MainButtonComboBox.Text = MainButtonComboBox.Items[butindex].ToString();
                }
                else
                {
                    int butindex =
                        stickItem.ButtonCount
                        + joyBut.POV * 4
                        + joyBut.Button / 9000
                        + 1;
                    MainButtonComboBox.Text = MainButtonComboBox.Items[butindex].ToString();
                }
            }
        }

        private void ProcessModifierButton(JoyBut joyBut)
        {
            var stickItem = JoystickStuff.Instance.GetStickItemByGuid(joyBut.JoystickGuid);
            ModifierDeviceComboBox.Text = stickItem?.InstanceName ?? "none";
            FillButtonComboBox(stickItem, ModifierButtonComboBox);
            if (stickItem == null)
            {
                ModifierButtonComboBox.Text = "none";
                return;
            }
            if (joyBut.POV == -1)
            {
                ModifierButtonComboBox.Text = ModifierButtonComboBox.Items[joyBut.Button + 1].ToString();
            }
            else
            {
                var butindex =
                    stickItem.ButtonCount
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
            {
                _buttonConfig.JoystickGUID = "none";
            }
            else
            {
                _buttonConfig.JoystickGUID = (MainDeviceComboBox.SelectedItem as ComboBoxStickItem).StickItem.JoystickGuid;
            }
            _buttonConfig.Button = MainButtonComboBox.Text;

            if (ModifierDeviceComboBox.SelectedIndex == 0)
                _buttonConfig.ModJoystickGUID = "none";
            else
                _buttonConfig.ModJoystickGUID = (ModifierDeviceComboBox.SelectedItem as ComboBoxStickItem).StickItem.JoystickGuid;

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
            var selectedStickItem = (MainDeviceComboBox.SelectedItem as ComboBoxStickItem).StickItem;
            FillButtonComboBox(selectedStickItem, MainButtonComboBox);
            MainButtonComboBox.Text = "none";
            if (selectedStickItem == null)
            {
                ModifierDeviceComboBox.SelectedIndex = 0;
                UseModifierCheckBox.Checked = false;
            }
        }

        private void ModifierDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedStickItem = (ModifierDeviceComboBox.SelectedItem as ComboBoxStickItem).StickItem;
            FillButtonComboBox(selectedStickItem, ModifierButtonComboBox);
            ModifierButtonComboBox.Text = "none";
        }

        private void Use8WayHatCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _buttonConfig.Use8WayHat = Use8WayHatCheckBox.Checked;
            FillComboBoxes();
        }
    }

    public class ComboBoxStickItem
    {
        public StickItem StickItem { get; set; }

        public override string ToString()
        {
            return StickItem?.InstanceName ?? "none";
        }
    }
}
