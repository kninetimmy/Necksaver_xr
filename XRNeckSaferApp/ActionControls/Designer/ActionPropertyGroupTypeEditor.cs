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

            var listBox = new ListBox();
            listBox.SelectionMode = SelectionMode.One;
            listBox.SelectedValueChanged += OnListBoxSelectedValueChanged;
            // listBox.DisplayMember = "Name";
            IActionPropertyGroups control = (IActionPropertyGroups)context.Instance;
            if (control.GroupsComponent?.Groups == null)
            {
                return value;
            }
            foreach (ActionPropertyGroup group in control.GroupsComponent.Groups)
            {
                // we store benchmarks objects directly in the listbox
                int index = listBox.Items.Add(group);
                if (group.Equals(value))
                {
                    listBox.SelectedIndex = index;
                }
                if (string.IsNullOrEmpty(listBox.DisplayMember))
                {
                    listBox.DisplayMember = nameof(group.Name);
                }
            }

            // show this model stuff
            _editorService.DropDownControl(listBox);
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
