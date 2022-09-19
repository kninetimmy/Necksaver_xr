using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class NumericActionUpDown : NumericUpDown
    {
        private NumericUpDownActionProperty _actionProperty;
        private bool _firstTimeRendered;

        [Category("ActionProperty"), Description("ActionProperty name")]
        public string ActionPropertyName { get; set; }

        public NumericActionUpDown() : base()
        {
            Config.ConfigReloaded += OnConfigReloaded;
            ValueChanged += OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            _actionProperty?.SetValue((int)Value);
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
            _actionProperty = Config.Instance.ActionProperties?.FirstOrDefault(p => p.Name == ActionPropertyName) as NumericUpDownActionProperty;
            if (_actionProperty == null)
            {
                _actionProperty = NumericUpDownActionProperty.CreateProperty(ActionPropertyName);
                Config.Instance.ActionProperties.Add(_actionProperty);
            }

            Value = _actionProperty.GetValue();
            _actionProperty.Triggered += ActionPropertyTriggered;
        }

        private void ActionPropertyTriggered(ActionPropertyEventArgs<int> args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ActionPropertyEventArgs<int>>(ActionPropertyTriggered), args);
                return;
            }
            Value = args.Value;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!_firstTimeRendered)
            {
                _firstTimeRendered = true;
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
