using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace XRNeckSafer
{
    public partial class ActionPropertiesForm : Form
    {
        private readonly BindingList<ActionPropertyDataModel> _bindingSource = new BindingList<ActionPropertyDataModel>();
        private readonly string _actionPropertyName;
        private List<int> _selectedIndexes;

        public static void ShowForm(string actionPropertyName, int mainFormTop, int mainFormRight)
        {
            using (var form = new ActionPropertiesForm(actionPropertyName, mainFormTop, mainFormRight))
            {
                form.ShowDialog();
            }
        }

        private ActionPropertiesForm(string actionPropertyName, int mainFormTop, int mainFormRight)
        {
            _actionPropertyName = actionPropertyName;
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            Top = mainFormTop;
            Left = mainFormRight - 10;
            PopulateBindingList();
            MinimumSize = Size;
        }

        private void PopulateBindingList()
        {
            var modelList = new List<ActionPropertyDataModel>();
            _selectedIndexes = new List<int>();
            Config.Instance.ActionProperties.ForEach(p => 
            {
                var currentProperty = _actionPropertyName.Equals(p.Name);
                foreach(var actionEvent in p.Events)
                {
                    modelList.Add(new ActionPropertyDataModel 
                    { 
                        Name = $"{p.Name}->[{actionEvent.Name}]", 
                        InputCombination = actionEvent.InputCombination?.ToString() ?? "-",
                        ActionPropertyName = p.Name,
                        Event = actionEvent
                    });
                    if (currentProperty)
                    {
                        _selectedIndexes.Add(modelList.Count - 1);
                    }
                }
            });
            modelList.ForEach(m => _bindingSource.Add(m));
            _actionPropertiesGridView.AutoGenerateColumns = false;
            _actionPropertiesGridView.DataSource = _bindingSource;
        }

        private void ActionPropertiesGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var buttonColumn = _actionPropertiesGridView.Columns[e.ColumnIndex] as DataGridViewButtonColumn;
            if (buttonColumn == null)
            {
                return;
            }
            var result = ScanJoystickKeyboardForm.ShowForm(FormStartPosition.CenterParent, Top, Left, 2);
            if (result == null)
            {
                return;
            }
            var model = _bindingSource[e.RowIndex];
            model.InputCombination = result.ToString();
            model.NewInputCombination = result;
            _actionPropertiesGridView.DataSource = null;
            _actionPropertiesGridView.DataSource = _bindingSource;
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            foreach (var model in _bindingSource.Where(m => m.NewInputCombination != null))
            {
                model.Event.InputCombination = model.NewInputCombination;
            }
            Close();
        }

        private void GridViewDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (_selectedIndexes.Any())
            {
                _selectedIndexes.ForEach(i => _actionPropertiesGridView.Rows[i].Selected = true);
                var firstIndex = _selectedIndexes.First();
                _actionPropertiesGridView.CurrentCell = _actionPropertiesGridView.Rows[firstIndex].Cells[0];
            }
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }

    public class ActionPropertyDataModel
    {
        public string Name { get; set; }
        public string InputCombination { get; set; }
        public string ButtonText { get; set; } = "Scan";
        public string ActionPropertyName { get; set; }
        public ActionPropertyEvent Event { get; set; }
        public JoystickKeyboardInput NewInputCombination { get; set; }
    }
}
