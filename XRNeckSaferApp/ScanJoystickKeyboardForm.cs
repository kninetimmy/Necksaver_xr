using System;
using System.ComponentModel;
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
            _scanner.BeforeRelesed += OnScanningComplete;
            _scanner.OnCurrentlyPressedChanged += OnCurrentlyPressedChanged;
        }

        private void OnCurrentlyPressedChanged(JoystickKeyboardInput input, bool sameKeys)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<JoystickKeyboardInput, bool>(OnCurrentlyPressedChanged), input, sameKeys);
                return;
            }
            _pressedButtonsLabel.Text = input.ToString();
        }

        private void OnScanningComplete(JoystickKeyboardInput input)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<JoystickKeyboardInput>(OnScanningComplete), input);
                return;
            }
            _result = input.Clone();
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _scanner.BeforeRelesed -= OnScanningComplete;
            _scanner.OnCurrentlyPressedChanged -= OnCurrentlyPressedChanged;
            base.OnClosing(e);
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
