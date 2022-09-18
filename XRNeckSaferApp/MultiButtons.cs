using System;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class MultiButtons : Form
    {
        public ButtonConfig _bconf1, _bconf2, _bconf3;
        public ButtonConfig bconf1, bconf2, bconf3;
        private readonly int _mainFormTop;
        private readonly int _mainFormRight;
        private readonly string _sideText;

        public MultiButtons(int mainFormTop, int mainFormRight, string sideText, ButtonConfig bc1, ButtonConfig bc2, ButtonConfig bc3)
        {
            _mainFormTop = mainFormTop;
            _mainFormRight = mainFormRight;
            _sideText = sideText;

            bconf1 = bc1;
            bconf2 = bc2;
            bconf3 = bc3;
            _bconf1 = new ButtonConfig();
            _bconf1 = bc1.CopyConfig(_bconf1);
            _bconf2 = new ButtonConfig();
            _bconf2 = bc2.CopyConfig(_bconf2);
            _bconf3 = new ButtonConfig();
            _bconf3 = bc3.CopyConfig(_bconf3);

            InitializeComponent();

            StartPosition = FormStartPosition.Manual;
            Top = mainFormTop;
            Left = mainFormRight - 10;

            TopLabel.Text = "Manual " + _sideText + " Rotation:";
            setButtonToolTip(button1, bconf1);
            setButtonToolTip(button2, bconf2);
            setButtonToolTip(button3, bconf3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var frm = new ButtonForm(_mainFormTop, _mainFormRight, "Button for" + _sideText + " Rotation:", _bconf1))
            {
                frm.ShowDialog();
            }
            setButtonToolTip(button1, _bconf1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            using (var frm = new ButtonForm(_mainFormTop, _mainFormRight, "Button for" + _sideText + " Rotation:", _bconf2))
            {
                frm.ShowDialog();
            }
            setButtonToolTip(button2, _bconf2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var frm = new ButtonForm(_mainFormTop, _mainFormRight, "Button for" + _sideText + " Rotation:", _bconf3))
            {
                frm.ShowDialog();
            }
            setButtonToolTip(button3, _bconf3);
        }

        public void setButtonToolTip(Button b, ButtonConfig bc)
        {
            string Text = JoystickService.GetJoystickName(bc.JoystickGUID) + ": " + bc.Button;
            if (bc.UseModifier)
            {
                Text += "   +   " + JoystickService.GetJoystickName(bc.ModJoystickGUID) + ": " + bc.ModButton;
            }
            toolTip1.SetToolTip(b, Text);
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            bconf1 = _bconf1.CopyConfig(bconf1);
            bconf2 = _bconf2.CopyConfig(bconf2);
            bconf3 = _bconf3.CopyConfig(bconf3);
            
            Config.Instance.WriteConfig();

            Close();
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            Config.ReloadConfig();
            Close();
        }
    }
}
