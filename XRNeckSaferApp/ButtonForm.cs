using System;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ButtonForm : Form
    {
        private MainForm mf;
        private ScanForm sf;
        public JoyBut jb;
        private ButtonConfig butconf;

        public ButtonForm(MainForm f, String titel, ButtonConfig bc)
        {
            butconf = bc;
            mf = f;
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            Top = mf.Top;
            Left = mf.Right - 10;
            Text = titel;
            mf.js.GetJoysticks();

            UseModifierCheckBox.Checked = butconf.UseModifier;
            InvertcheckBox.Checked = butconf.Invert;
            toggleCheckBox.Checked = butconf.Toggle;
            Use8WayHatCheckBox.Checked = butconf.Use8WayHat;

            FillComboBoxes();
        }
        void FillComboBoxes()
        {
            MainDeviceComboBox.Items.Clear();
            MainDeviceComboBox.Items.Add("none");
            ModifierDeviceComboBox.Items.Clear();
            ModifierDeviceComboBox.Items.Add("none");
            MainButtonComboBox.Items.Clear();
            MainButtonComboBox.Items.Add("none");
            ModifierButtonComboBox.Items.Clear();
            ModifierButtonComboBox.Items.Add("none");


            for (int i = 0; i < mf.js.ll.Count; i++)
            {
                MainDeviceComboBox.Items.Add(mf.js.ll[i].InstanceName);
                ModifierDeviceComboBox.Items.Add(mf.js.ll[i].InstanceName);
            }

            int Index = mf.js.IndexFromGuid(butconf.JoystickGUID);
            MainDeviceComboBox.SelectedIndex = Index + 1;
            FillButtonComboBox(Index, MainButtonComboBox);

            Index = mf.js.IndexFromGuid(butconf.ModJoystickGUID);
            ModifierDeviceComboBox.SelectedIndex = Index + 1;
            FillButtonComboBox(Index, ModifierButtonComboBox);
            ModifierButtonComboBox.Text = butconf.ModButton;

            MainButtonComboBox.Text = butconf.Button;
        }
        void FillButtonComboBox(int joyIndex, ComboBox cb)
        {
            cb.Items.Clear();
            cb.Items.Add("none");
            if (joyIndex == -1) return;
            for (int i = 0; i < mf.js.Sticks[joyIndex].Stick.Capabilities.ButtonCount; i++)
            {
                cb.Items.Add("But: " + (i + 1));
            }
            for (int i = 0; i < mf.js.Sticks[joyIndex].Stick.Capabilities.PovCount; i++)
            {
                if (butconf.Use8WayHat)
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

        private void MainScanButton_Click(object sender, EventArgs e)
        {
            sf = new ScanForm(this);
            sf.StartPosition = FormStartPosition.Manual;
            sf.Top = Top + 10;
            sf.Left = Left;

            mf.js.InitScan();
            scanTimer.Start();
            sf.ShowDialog();

            MainDeviceComboBox.Text = MainDeviceComboBox.Items[jb.joyIndex + 1].ToString();
            FillButtonComboBox(jb.joyIndex, MainButtonComboBox);
            if (jb.pov == -1)
            {
                MainButtonComboBox.Text = MainButtonComboBox.Items[jb.btn + 1].ToString();
            }
            else
            {
                if (butconf.Use8WayHat)
                {
                    int butindex =
                        mf.js.Sticks[jb.joyIndex].Stick.Capabilities.ButtonCount
                        + jb.pov * 8
                        + jb.btn / 4500
                        + 1;
                    MainButtonComboBox.Text = MainButtonComboBox.Items[butindex].ToString();
                }
                else
                {
                    int butindex =
                        mf.js.Sticks[jb.joyIndex].Stick.Capabilities.ButtonCount
                        + jb.pov * 4
                        + jb.btn / 9000
                        + 1;
                    MainButtonComboBox.Text = MainButtonComboBox.Items[butindex].ToString();
                }
            }
        }

        private void ModifierScanButton_Click(object sender, EventArgs e)
        {
            sf = new ScanForm(this);
            sf.StartPosition = FormStartPosition.Manual;
            sf.Top = Top + 10;
            sf.Left = Left;

            mf.js.InitScan();
            scanTimer.Start();
            sf.ShowDialog();

            ModifierDeviceComboBox.Text = ModifierDeviceComboBox.Items[jb.joyIndex + 1].ToString();
            FillButtonComboBox(jb.joyIndex, ModifierButtonComboBox);
            if (jb.pov == -1)
            {
                ModifierButtonComboBox.Text = ModifierButtonComboBox.Items[jb.btn + 1].ToString();
            }
            else
            {
                int butindex =
                    mf.js.Sticks[jb.joyIndex].Stick.Capabilities.ButtonCount
                    + jb.pov * 4
                    + jb.btn / 9000
                    + 1;
                ModifierButtonComboBox.Text = ModifierButtonComboBox.Items[butindex].ToString();
            }
        }

        private void ScanTimerLoop(object sender, EventArgs e)
        {
            jb = mf.js.ScanJoysticks();
            if (jb.joyIndex != -1)
            {
                scanTimer.Stop();
                sf.Close();
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
            ModifierScanButton.Enabled = UseModifierCheckBox.Checked;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (MainDeviceComboBox.SelectedIndex == 0)
                butconf.JoystickGUID = "none";
            else
                butconf.JoystickGUID = mf.js.ll[MainDeviceComboBox.SelectedIndex - 1].InstanceGuid.ToString();
            butconf.Button = MainButtonComboBox.Text;

            if (ModifierDeviceComboBox.SelectedIndex == 0)
                butconf.ModJoystickGUID = "none";
            else
                butconf.ModJoystickGUID = mf.js.ll[ModifierDeviceComboBox.SelectedIndex - 1].InstanceGuid.ToString();

            butconf.ModButton = ModifierButtonComboBox.Text;
            butconf.UseModifier = UseModifierCheckBox.Checked;
            butconf.Invert = InvertcheckBox.Checked;
            butconf.Toggle = toggleCheckBox.Checked;
            butconf.Use8WayHat = Use8WayHatCheckBox.Checked;

            mf.conf.WriteConfig();

            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            mf.conf = Config.ReadConfig();
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
            butconf.Use8WayHat = Use8WayHatCheckBox.Checked;
            FillComboBoxes();
        }
    }
}
