using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class BooleanActionButton : Button
    {
        private System.Drawing.Color _inactiveForeColour;
        private System.Drawing.Color _inactiveBackColour;
        private System.Drawing.Color _activeForeColour = System.Drawing.Color.LimeGreen;
        private System.Drawing.Color _activeBackColour = System.Drawing.Color.Black;
        private BooleanActionProperty _actionProperty;
        private bool _firstTimeRendered;

        [Category("ActionProperty"), Description("ActionProperty name")]
        public string ActionPropertyName { get; set; }

        [Category("ActionProperty"), Description("Fore coulor of the button in active state")]
        public System.Drawing.Color ActiveForeColour { get => _activeForeColour; set { _activeForeColour = value; Invalidate(); } }

        [Category("ActionProperty"), Description("Back coulor of the button in active state")]
        public System.Drawing.Color ActiveBackColour { get => _activeBackColour; set { _activeBackColour = value; Invalidate(); } }

        [Category("ActionProperty"), Description("Fired when Active Property value changed")]
        public event Action<bool> ActionPropertyValueChanged;

        public bool ActionPropertyValue { get => _actionProperty?.GetValue() ?? false; }

        public BooleanActionButton() : base()
        {
            Config.ConfigReloaded += OnConfigReloaded;
        }

        private void OnConfigReloaded()
        {
            if (_actionProperty != null)
            {
                _actionProperty.Triggered -= ActionPropertyTriggered;
            }
            SubscribeActionProperty();
        }

        protected void SubscribeActionProperty()
        {
            if (this.InDesignerMode())
            {
                return;
            }
            _actionProperty = Config.Instance.ActionProperties?.FirstOrDefault(p => p.Name == ActionPropertyName) as BooleanActionProperty;
            if (_actionProperty == null)
            {
                _actionProperty = BooleanActionProperty.CreateProperty(ActionPropertyName);
                Config.Instance.ActionProperties.Add(_actionProperty);
            }
            _actionProperty.Triggered += ActionPropertyTriggered;
            SetButtonColor(_actionProperty.GetValue());
        }

        private void ActionPropertyTriggered(ActionPropertyEventArgs<bool> args)
        {
            SetButtonColor(args.Value);
            ActionPropertyValueChanged?.Invoke(args.Value);
        }

        private void SetButtonColor(bool active)
        {
            if (active)
            {
                ForeColor = ActiveForeColour;
                BackColor = ActiveBackColour;
                return;
            }
            ForeColor = _inactiveForeColour;
            BackColor = _inactiveBackColour;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!_firstTimeRendered)
            {
                _firstTimeRendered = true;
                _inactiveBackColour = BackColor;
                _inactiveForeColour = ForeColor;
                SubscribeActionProperty();
            }
            base.OnVisibleChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _actionProperty != null)
            {
                Config.ConfigReloaded -= OnConfigReloaded;
                _actionProperty.Triggered -= ActionPropertyTriggered;
            }
        }
    }
}
