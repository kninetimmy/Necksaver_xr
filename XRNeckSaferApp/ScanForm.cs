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
            _scanner.BeforeButtonReleased += OnBeforeButtonReleased;
            _scanner.CurrentlyPressedChanged += ChangePressedButtonsLabel;
        }

        private void ChangePressedButtonsLabel(List<JoyBut> buttons)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<List<JoyBut>>(ChangePressedButtonsLabel), buttons);
                return;
            }
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
                var name = JoystickService.GetJoystickName(button.JoystickGuid);
                builder.Append($"[{name} But:{button.Button + 1}]");
            }
            return builder.ToString();
        }

        private void OnBeforeButtonReleased(List<JoyBut> buttons)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<List<JoyBut>>(OnBeforeButtonReleased), buttons);
                return;
            }
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
            _scanner.BeforeButtonReleased -= OnBeforeButtonReleased;
            _scanner.CurrentlyPressedChanged -= ChangePressedButtonsLabel;
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
                _scanner.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
