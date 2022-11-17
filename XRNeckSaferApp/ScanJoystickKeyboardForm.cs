using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ScanJoystickKeyboardForm : Form
    {
        private readonly JoystickKeyboardScanner _scanner;
        private JoystickKeyboardInput _result;

        public static JoystickKeyboardInput ShowForm(
            FormStartPosition startPosition, 
            int top, 
            int left, 
            int maxPressedButtonsCount = 1, 
            DeviceType deviceType = DeviceType.Keyboard | DeviceType.Joystick)
        {
            using (var form = new ScanJoystickKeyboardForm(maxPressedButtonsCount, deviceType)
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

        private ScanJoystickKeyboardForm(int maxPressedButtonsCount, DeviceType deviceType)
        {
            InitializeComponent();
            MinimumSize = Size;
            MaximumSize = Size;
            SetFormHeaderText(maxPressedButtonsCount, deviceType);
            _scanner = new JoystickKeyboardScanner(maxPressedButtonsCount, deviceType);
            _scanner.BeforeReleased += OnScanningComplete;
            _scanner.OnCurrentlyPressedChanged += OnCurrentlyPressedChanged;
        }

        private void SetFormHeaderText(int maxPressedButtonsCount, DeviceType deviceType)
        {
            if (deviceType.HasFlag(DeviceType.Keyboard | DeviceType.Joystick))
            {
                Text = $"Scanning joysticks and keys. Press key/button{(maxPressedButtonsCount > 1 ? "(s)" : "")} now...";
                _scanText.Text = $"Pressed key/button{(maxPressedButtonsCount > 1 ? "(s)" : "")}:";
                return;
            }
            if (deviceType.HasFlag(DeviceType.Keyboard))
            {
                Text = $"Scanning keyboard. Press key{(maxPressedButtonsCount > 1 ? "(s)" : "")} now...";
                _scanText.Text = $"Pressed key{(maxPressedButtonsCount > 1 ? "(s)" : "")}:";
                return;
            }
            if (deviceType.HasFlag(DeviceType.Joystick))
            {
                Text = $"Scanning joysticks. Press button{(maxPressedButtonsCount > 1 ? "(s)" : "")} now...";
                _scanText.Text = $"Pressed button{(maxPressedButtonsCount > 1 ? "(s)" : "")}:";
            }
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
            _scanner.BeforeReleased -= OnScanningComplete;
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

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
