using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class MultiButtons : Form
    {
        public ButtonConfig _bconf1, _bconf2, _bconf3;
        public ButtonConfig bconf1, bconf2, bconf3;
        public MainForm mf;
        public string side;
        public MultiButtons(MainForm f, string s, ButtonConfig bc1, ButtonConfig bc2, ButtonConfig bc3)
        {
            side = s;
            mf = f;

            bconf1 = bc1;
            bconf2 = bc2;
            bconf3 = bc3;
            _bconf1 = new ButtonConfig();
            _bconf1 = bc1.copyButtonConfig(_bconf1);
            _bconf2 = new ButtonConfig();
            _bconf2 = bc2.copyButtonConfig(_bconf2);
            _bconf3 = new ButtonConfig();
            _bconf3 = bc3.copyButtonConfig(_bconf3);

            InitializeComponent();

            StartPosition = FormStartPosition.Manual;
            Top = mf.Top;
            Left = mf.Right - 10;

            TopLabel.Text = "Manual " + side + " Rotation:";
            setButtonToolTip(button1, bconf1);
            setButtonToolTip(button2, bconf2);
            setButtonToolTip(button3, bconf3);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonForm frm = new ButtonForm(mf, "Button for" + side + " Rotation:", _bconf1);
            frm.ShowDialog();
            setButtonToolTip(button1, _bconf1); 
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ButtonForm frm = new ButtonForm(mf, "Button for" + side + " Rotation:", _bconf2);
            frm.ShowDialog();
            setButtonToolTip(button2, _bconf2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ButtonForm frm = new ButtonForm(mf, "Button for" + side + " Rotation:", _bconf3);
            frm.ShowDialog();
            setButtonToolTip(button3, _bconf3);
        }

        public void setButtonToolTip(Button b, ButtonConfig bc)
        {
            string Text = mf.js.NameFromGuid(bc.JoystickGUID) + ": " + bc.Button;
            if (bc.UseModifier)
            {
                Text += "   +   " + mf.js.NameFromGuid(bc.ModJoystickGUID) + ": " + bc.ModButton;
            }
            toolTip1.SetToolTip(b, Text);
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            bconf1 = _bconf1.copyButtonConfig(bconf1);
            bconf2 = _bconf2.copyButtonConfig(bconf2);
            bconf3 = _bconf3.copyButtonConfig(bconf3);
            
            mf.conf.WriteConfig();

            Close();
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            mf.conf = Config.ReadConfig();
            Close();
        }
    }
}
