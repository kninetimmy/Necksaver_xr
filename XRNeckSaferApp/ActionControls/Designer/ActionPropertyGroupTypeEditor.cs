using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace XRNeckSafer
{
    public class ActionPropertyGroupTypeEditor : UITypeEditor
    {
        private IWindowsFormsEditorService _editorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.One
            };
            listBox.SelectedValueChanged += OnListBoxSelectedValueChanged;
            var emptyItem = new ActionPropertyGroupItem { Name = "(none)" };
            listBox.Items.Add(emptyItem);
            listBox.DisplayMember = nameof(emptyItem.Name);
            listBox.SelectedIndex = 0;
            var selectedGroup = (ActionPropertyGroupItem)value;
            IActionPropertyGroups control = (IActionPropertyGroups)context.Instance;
            if (control?.GroupsComponent?.Groups != null)
            {
                foreach (ActionPropertyGroup group in control.GroupsComponent.Groups)
                {
                    int index = listBox.Items.Add(new ActionPropertyGroupItem { Name = group.Name, Tag = group });
                    if (group.Name == selectedGroup.Name)
                    {
                        listBox.SelectedIndex = index;
                    }
                }
            }
            
            _editorService.DropDownControl(listBox);
            listBox.SelectedValueChanged -= OnListBoxSelectedValueChanged;
            if (listBox.SelectedItem == null)
                return value;

            return listBox.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
