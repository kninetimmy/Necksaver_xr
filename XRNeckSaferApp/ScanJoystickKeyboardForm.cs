using System;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ScanJoystickKeyboardForm : Form
    {
        private readonly JoystickKeyboardScanner _scanner;
        private JoystickKeyboardInput _result;

        public static JoystickKeyboardInput ShowForm(FormStartPosition startPosition, int top, int left, int maxPressedButtonsCount = 1)
        {
            using (var form = new ScanJoystickKeyboardForm(maxPressedButtonsCount)
            {
                StartPosition = startPosition,
                Top = top,
                Left = left
            })
            {
                form.ShowDialog();
                return form._result;
            }
        }

        private ScanJoystickKeyboardForm(int maxPressedButtonsCount = 1)
        {
            InitializeComponent();
            MinimumSize = Size;
            MaximumSize = Size;
            _scanner = new JoystickKeyboardScanner(maxPressedButtonsCount);
            _scanner.OnScanningComplete += OnScanningComplete; ;
            _scanner.OnCurrentlyPressedChanged += OnCurrentlyPressedChanged;
        }

        private void OnCurrentlyPressedChanged(JoystickKeyboardInput input, bool sameKeys)
        {
            _pressedButtonsLabel.Text = input.ToString();
        }

        private void OnScanningComplete(JoystickKeyboardInput input)
        {
            _result = input.Clone();
            Close();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _scanner.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
