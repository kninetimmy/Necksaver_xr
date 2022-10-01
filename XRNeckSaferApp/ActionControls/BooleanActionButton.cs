using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class BooleanActionButton : Button, IActionPropertyGroups, IActionPropertyName
    {
        private Color _inactiveForeColour;
        private Color _inactiveBackColour;
        private Color _activeForeColour = Color.LightGreen;
        private Color _activeBackColour = Color.Black;
        private BooleanActionProperty _actionProperty;
        private bool _firstTimeRendered;
        private bool _isActive;
        private string _actionPropertyId;
        private string _actionPropertyName;
        private string _actionPropertyDescription;
        private int _actionPropertyOrder;
        private ActionPropertyGroupItem _selectedGroup;
        private ActionPropertyGroups _groupsComponent;

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
            set
            {
                _actionPropertyId = value;
                if (DesignMode)
                {
                    return;
                }
                InitialiseActionProperty();
            }
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
        public int ActionPropertyOrder
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

        [Category("ActionProperty"), Description("Fore coulor of the button in active state")]
        public Color ActiveForeColour 
        { 
            get => _activeForeColour; 
            set 
            { 
                _activeForeColour = value; 
                Invalidate();
                SetButtonColor();
            } 
        }

        [Category("ActionProperty"), Description("Back coulor of the button in active state")]
        public Color ActiveBackColour 
        { 
            get => _activeBackColour; 
            set 
            { 
                _activeBackColour = value; 
                Invalidate();
                SetButtonColor();
            } 
        }

        [Category("ActionProperty"), Description("Fore coulor of the button in active state")]
        public Color InActiveForeColour
        {
            get => _inactiveForeColour;
            set
            {
                _inactiveForeColour = value;
                Invalidate();
                SetButtonColor();
            }
        }

        [Category("ActionProperty"), Description("Back coulor of the button in active state")]
        public Color InActiveBackColour
        {
            get => _inactiveBackColour;
            set
            {
                _inactiveBackColour = value;
                Invalidate();
                SetButtonColor();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color ForeColor
        {
            get => base.ForeColor;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackColor
        {
            get => base.BackColor;
        }

        [Category("ActionProperty"), Description("Fired when Active Property value changed")]
        public event Action<bool> ActionPropertyValueChanged;

        public bool ActionPropertyValue { get => _actionProperty?.GetValue() ?? false; }

        public BooleanActionButton() : base()
        {
            Config.ConfigReloaded += OnConfigReloaded;
        }

        private void InitialiseActionProperty()
        {
            if (!_firstTimeRendered)
            {
                _firstTimeRendered = true;
                SubscribeActionProperty();
            }
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
            _actionProperty = Config.Instance.ActionProperties?.FirstOrDefault(p => p.Id == ActionPropertyId) as BooleanActionProperty;
            if (_actionProperty == null)
            {
                _actionProperty = BooleanActionProperty.CreateProperty(ActionPropertyId);
                Config.Instance.ActionProperties.Add(_actionProperty);
            }
            _actionProperty.Triggered += ActionPropertyTriggered;
            _actionProperty.Name = _actionPropertyName;
            _actionProperty.Description = _actionPropertyDescription;
            _actionProperty.Order = _actionPropertyOrder;
            _actionProperty.Group = SelectedGroup?.Tag;
            _isActive = _actionProperty.GetValue();
            SetButtonColor();
        }

        private void ActionPropertyTriggered(ActionPropertyEventArgs<bool> args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ActionPropertyEventArgs<bool>>(ActionPropertyTriggered), args);
                return;
            }
            if (_isActive == args.Value)
            {
                return;
            }
            _isActive = args.Value;
            SetButtonColor();
            ActionPropertyValueChanged?.Invoke(_isActive);
        }

        private void SetButtonColor()
        {
            if (_isActive)
            {
                base.ForeColor = ActiveForeColour;
                base.BackColor = ActiveBackColour;
                return;
            }
            base.ForeColor = _inactiveForeColour;
            base.BackColor = _inactiveBackColour;
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
