using System;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class SplashScreen : Form
    {
        public SplashScreen(int durationMsec = 3000)
        {
            InitializeComponent();
            _timer.Interval = durationMsec;
            _timer.Start();
        }

        private void OnTimeTick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
