using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class NumericActionUpDown : NumericUpDown, IActionPropertyGroups
    {
        private NumericUpDownActionProperty _actionProperty;
        private bool _firstTimeRendered;
        private string _actionPropertyName;
        private string _actionPropertyNameText;
        private string _actionPropertyDescription;
        private ActionPropertyGroup _selectedGroup;

        [Category("ActionProperty"), ImmutableObject(true)]
        public ActionPropertyGroups GroupsComponent { get; set; }

        [Category("ActionProperty"), ImmutableObject(true)]
        [Editor(typeof(ActionPropertyGroupTypeEditor), typeof(UITypeEditor))]
        public ActionPropertyGroup SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                if (_actionProperty != null)
                {
                    _actionProperty.Group = value;
                }
            }
        }

        [Category("ActionProperty"), Description("ActionProperty ID")]
        public string ActionPropertyName
        {
            get => _actionPropertyName;
            set
            {
                _actionPropertyName = value;
                if (this.InDesignerMode())
                {
                    return;
                }
                InitialiseActionProperty();
            }
        }

        [Category("ActionProperty"), Description("ActionProperty user firendly name")]
        public string ActionPropertyNameText
        {
            get => _actionPropertyNameText;
            set
            {
                _actionPropertyNameText = value;
                if (_actionProperty == null)
                {
                    return;
                }
                _actionProperty.NameText = _actionPropertyNameText;
            }
        }

        [Category("ActionProperty"), Description("ActionProperty description")]
        public string ActionPropertyDescription
        {
            get => _actionPropertyDescription;
            set
            {
                _actionPropertyDescription = value;
                if (_actionProperty == null)
                {
                    return;
                }
                _actionProperty.Description = _actionPropertyDescription;
            }
        }

        public NumericActionUpDown() : base()
        {
            Config.ConfigReloaded += OnConfigReloaded;
            ValueChanged += OnValueChanged;
        }

        private void InitialiseActionProperty()
        {
            if (!_firstTimeRendered)
            {
                _firstTimeRendered = true;
                SubscribeActionProperty();
            }
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
            if (string.IsNullOrEmpty(ActionPropertyName))
            {
                throw new ArgumentException($"{nameof(ActionPropertyName)} can not be empty.");
            }
            _actionProperty = Config.Instance.ActionProperties?.FirstOrDefault(p => p.Name == ActionPropertyName) as NumericUpDownActionProperty;
            if (_actionProperty == null)
            {
                _actionProperty = NumericUpDownActionProperty.CreateProperty(ActionPropertyName);
                Config.Instance.ActionProperties.Add(_actionProperty);
            }

            Value = _actionProperty.GetValue();
            _actionProperty.NameText = _actionPropertyNameText;
            _actionProperty.Description = _actionPropertyDescription;
            _actionProperty.Group = SelectedGroup; // _actionPropertyGroup;
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
