using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ScanForm : Form
    {
        private readonly JoystickButtonScanner _scanner;
        private readonly List<JoyBut> _result = new List<JoyBut>();

        public static List<JoyBut> ShowForm(FormStartPosition startPosition, int top, int left, int maxPressedButtonsCount = 1)
        {
            using (var form = new ScanForm(maxPressedButtonsCount)
            {
                StartPosition = startPosition,
                Top = top,
                Left = left
            })
            {
                form.StartScan();
                form.ShowDialog();
                return form._result;
            }
        }

        private ScanForm(int maxPressedButtonsCount = 1)
        {
            InitializeComponent();
            _scanner = new JoystickButtonScanner(maxPressedButtonsCount);
            _scanner.OnScanningComplete += OnButtonScanned;
            _scanner.OnCurrentlyPressedChanged += OnCurrentlyPressedChanged;
        }

        private void OnCurrentlyPressedChanged(List<JoyBut> buttons)
        {
            var builder = new System.Text.StringBuilder();
            foreach(var button in buttons)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                var device = JoystickStuff.Instance.GetDeviceByIndex(button.JoyIndex);
                var text = $"[{device.InstanceName} But:{button.Button + 1}]";
                builder.Append(text);
            }
            _pressedButtonsLabel.Text = builder.ToString();
        }

        private void OnButtonScanned(List<JoyBut> buttons)
        {
            _result.AddRange(buttons);
            Close();
        }

        private void StartScan()
        {
            _scanner.StartScan();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            _scanner.Cancel();
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
