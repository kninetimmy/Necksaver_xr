using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public class BooleanActionButton : Button, IActionPropertyGroups
    {
        private System.Drawing.Color _inactiveForeColour;
        private System.Drawing.Color _inactiveBackColour;
        private System.Drawing.Color _activeForeColour = System.Drawing.Color.LimeGreen;
        private System.Drawing.Color _activeBackColour = System.Drawing.Color.Black;
        private BooleanActionProperty _actionProperty;
        private bool _firstTimeRendered;
        private bool _isActive;
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

        private void InitialiseActionProperty()
        {
            if (!_firstTimeRendered)
            {
                _firstTimeRendered = true;
                _inactiveBackColour = BackColor;
                _inactiveForeColour = ForeColor;
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
            if (this.InDesignerMode())
            {
                return;
            }
            if (string.IsNullOrEmpty(ActionPropertyName))
            {
                throw new ArgumentException($"{nameof(ActionPropertyName)} can not be empty.");
            }
            _actionProperty = Config.Instance.ActionProperties?.FirstOrDefault(p => p.Name == ActionPropertyName) as BooleanActionProperty;
            if (_actionProperty == null)
            {
                _actionProperty = BooleanActionProperty.CreateProperty(ActionPropertyName);
                Config.Instance.ActionProperties.Add(_actionProperty);
            }
            _actionProperty.Triggered += ActionPropertyTriggered;
            _actionProperty.NameText = _actionPropertyNameText;
            _actionProperty.Description = _actionPropertyDescription;
            _actionProperty.Group = SelectedGroup;
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
                ForeColor = ActiveForeColour;
                BackColor = ActiveBackColour;
                return;
            }
            ForeColor = _inactiveForeColour;
            BackColor = _inactiveBackColour;
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
