using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
            MinimumSize = Size;
            MaximumSize = Size;
            _scanner = new JoystickButtonScanner(maxPressedButtonsCount);
            _scanner.OnScanningComplete += OnButtonScanned;
            _scanner.OnCurrentlyPressedChanged += ChangePressedButtonsLabel;
        }

        private void ChangePressedButtonsLabel(List<JoyBut> buttons)
        {
            _pressedButtonsLabel.Text = CreatePressedButtonsLabel(buttons);
        }

        private string CreatePressedButtonsLabel(List<JoyBut> buttons)
        {
            var builder = new StringBuilder();
            foreach (var button in buttons)
            {
                if (builder.Length > 0)
                {
                    builder.Append("+");
                }
                var stickItem = JoystickStuff.Instance.GetStickItemByGuid(button.JoystickGuid);
                builder.Append($"[{stickItem.InstanceName} But:{button.Button + 1}]");
            }
            return builder.ToString();
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
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _scanner.Stop();
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
