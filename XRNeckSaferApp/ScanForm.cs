using System;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ScanForm : Form
    {
        private ButtonForm bf;
        public ScanForm(ButtonForm f)
        {
            bf = f;
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            bf.scanTimer.Stop();
            bf.jb.joyIndex = -1;
            Close();
        }
    }
}
