
namespace XRNeckSafer
{
    partial class ActionPropertiesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionPropertiesForm));
            this._actionPropertiesGridView = new System.Windows.Forms.DataGridView();
            this._saveButton = new System.Windows.Forms.Button();
            this._actionPropertyNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._actionPropertyInputColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._actionPropertyScanColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this._actionPropertiesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _actionPropertiesGridView
            // 
            this._actionPropertiesGridView.AllowUserToAddRows = false;
            this._actionPropertiesGridView.AllowUserToDeleteRows = false;
            this._actionPropertiesGridView.AllowUserToResizeRows = false;
            this._actionPropertiesGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._actionPropertiesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._actionPropertiesGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._actionPropertyNameColumn,
            this._actionPropertyInputColumn,
            this._actionPropertyScanColumn});
            this._actionPropertiesGridView.Location = new System.Drawing.Point(4, 3);
            this._actionPropertiesGridView.Name = "_actionPropertiesGridView";
            this._actionPropertiesGridView.ReadOnly = true;
            this._actionPropertiesGridView.RowHeadersVisible = false;
            this._actionPropertiesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._actionPropertiesGridView.Size = new System.Drawing.Size(534, 309);
            this._actionPropertiesGridView.TabIndex = 1;
            this._actionPropertiesGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ActionPropertiesGridViewCellContentClick);
            this._actionPropertiesGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.GridViewDataBindingComplete);
            // 
            // _saveButton
            // 
            this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._saveButton.Location = new System.Drawing.Point(454, 325);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(75, 23);
            this._saveButton.TabIndex = 2;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this.OnSaveButtonClick);
            // 
            // _actionPropertyNameColumn
            // 
            this._actionPropertyNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this._actionPropertyNameColumn.DataPropertyName = "Name";
            this._actionPropertyNameColumn.HeaderText = "Action Name->[Event]";
            this._actionPropertyNameColumn.Name = "_actionPropertyNameColumn";
            this._actionPropertyNameColumn.ReadOnly = true;
            this._actionPropertyNameColumn.Width = 124;
            // 
            // _actionPropertyInputColumn
            // 
            this._actionPropertyInputColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._actionPropertyInputColumn.DataPropertyName = "InputCombination";
            this._actionPropertyInputColumn.HeaderText = "Action Property Key/Joystick mapping";
            this._actionPropertyInputColumn.Name = "_actionPropertyInputColumn";
            this._actionPropertyInputColumn.ReadOnly = true;
            // 
            // _actionPropertyScanColumn
            // 
            this._actionPropertyScanColumn.DataPropertyName = "ButtonText";
            this._actionPropertyScanColumn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._actionPropertyScanColumn.HeaderText = "Edit";
            this._actionPropertyScanColumn.Name = "_actionPropertyScanColumn";
            this._actionPropertyScanColumn.ReadOnly = true;
            this._actionPropertyScanColumn.Text = "Scan";
            this._actionPropertyScanColumn.Width = 50;
            // 
            // ActionPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 359);
            this.Controls.Add(this._saveButton);
            this.Controls.Add(this._actionPropertiesGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActionPropertiesForm";
            this.Text = "Action Properties Configuration";
            ((System.ComponentModel.ISupportInitialize)(this._actionPropertiesGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView _actionPropertiesGridView;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn _actionPropertyNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _actionPropertyInputColumn;
        private System.Windows.Forms.DataGridViewButtonColumn _actionPropertyScanColumn;
    }
}