using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class NumericActionUpDown : NumericUpDown, IActionPropertyGroups, IActionPropertyName
    {
        private Form _form;
        private NumericUpDownActionProperty _actionProperty;
        private bool _firstTimeRendered;
        private string _actionPropertyId;
        private string _actionPropertyName;
        private string _actionPropertyDescription;
        private short _actionPropertyOrder;
        private ActionPropertyGroupItem _selectedGroup;
        private ActionPropertyGroups _groupsComponent;
        private decimal _defaultValue;
        private bool _newProperty;

        [Category("ActionProperty"), ImmutableObject(true)]
        public ActionPropertyGroups GroupsComponent
        {
            get => _groupsComponent;
            set
            {
                _groupsComponent = value;
                if (_groupsComponent?.Groups?.Length < 1)
                {
                    SelectedGroup = null;
                }
            }
        }

        [Category("ActionProperty"), ImmutableObject(true)]
        [Editor(typeof(ActionPropertyGroupTypeEditor), typeof(UITypeEditor))]
        public ActionPropertyGroupItem SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                if (_actionProperty != null)
                {
                    _actionProperty.Group = value?.Tag;
                }
            }
        }

        [Category("ActionProperty"), Description("ActionProperty ID"), DisplayName("ActionProperty ID")]
        public string ActionPropertyId
        {
            get => _actionPropertyId;
            set => _actionPropertyId = value;
        }

        [Category("ActionProperty"), DisplayName("Action Property Name"), Description("ActionProperty user firendly name")]
        public string ActionPropertyName
        {
            get => _actionPropertyName;
            set
            {
                _actionPropertyName = value;
                if (_actionProperty == null)
                {
                    return;
                }
                _actionProperty.Name = _actionPropertyName;
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

        [Category("ActionProperty"), Description("ActionProperty description")]
        public short ActionPropertyOrder
        {
            get => _actionPropertyOrder;
            set
            {
                _actionPropertyOrder = value;
                if (_actionProperty == null)
                {
                    return;
                }
                _actionProperty.Order = _actionPropertyOrder;
            }
        }

        [Category("ActionProperty"), Description("ActionProperty default value")]
        public decimal DefaultValue 
        { 
            get => _defaultValue; 
            set 
            { 
                _defaultValue = value;
                if (_newProperty || DesignMode)
                {
                    Value = _defaultValue;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new decimal Value 
        { 
            get => base.Value;
            set => base.Value = value;
        }

        public new decimal Minimum
        {
            get => base.Minimum;
            set
            {
                base.Minimum = value;
                if (_actionProperty != null)
                {
                    _actionProperty.Minimum = value;
                }
            }
        }

        public new decimal Maximum
        {
            get => base.Maximum;
            set
            {
                base.Maximum = value;
                if (_actionProperty != null)
                {
                    _actionProperty.Maximum = value;
                }
            }
        }

        public new decimal Increment
        {
            get => base.Increment;
            set
            {
                base.Increment = value;
                if (_actionProperty != null)
                {
                    _actionProperty.Increment = value;
                }
            }
        }

        public NumericActionUpDown() : base()
        {
            Config.ConfigReloaded += OnConfigReloaded;
            ValueChanged += OnValueChanged;
            
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            var form = FindForm();
            if (form != null && _form == null)
            {
                _form = form;
                form.Activated += OnParentFormActivated;
            }
        }

        private void OnParentFormActivated(object sender, EventArgs e)
        {
            InitialiseActionProperty();
        }

        private void InitialiseActionProperty()
        {
            if (!_firstTimeRendered && ActionPropertyId != null)
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
            if (DesignMode)
            {
                return;
            }
            if (string.IsNullOrEmpty(ActionPropertyId))
            {
                throw new ArgumentException($"{nameof(ActionPropertyId)} can not be empty.");
            }
            _actionProperty = Config.Instance.ActionProperties?.FirstOrDefault(p => p.Id == ActionPropertyId) as NumericUpDownActionProperty;
            if (_actionProperty == null)
            {
                _actionProperty = NumericUpDownActionProperty.CreateProperty(ActionPropertyId);
                Config.Instance.ActionProperties.Add(_actionProperty);
                _newProperty = true;
                Value = DefaultValue;
            }

            Value = _actionProperty.GetValue();
            _actionProperty.Name = _actionPropertyName;
            _actionProperty.Description = _actionPropertyDescription;
            _actionProperty.Order = _actionPropertyOrder;
            _actionProperty.Group = SelectedGroup?.Tag;
            _actionProperty.Minimum = Minimum;
            _actionProperty.Maximum = Maximum;
            _actionProperty.Increment = Increment;
            _actionProperty.Triggered += ActionPropertyTriggered;
        }

        private void ActionPropertyTriggered(ActionPropertyEventArgs<decimal> args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ActionPropertyEventArgs<decimal>>(ActionPropertyTriggered), args);
                return;
            }
            Value = args.Value;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _actionProperty != null)
            {
                Config.ConfigReloaded -= OnConfigReloaded;
                _actionProperty.Triggered -= ActionPropertyTriggered;
                if (_form != null)
                {
                    _form.Activated -= OnParentFormActivated;
                    _form = null;
                }
            }
        }
    }
}
