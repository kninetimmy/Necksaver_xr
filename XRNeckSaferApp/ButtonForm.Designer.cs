
namespace XRNeckSafer
{
    partial class ButtonForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonForm));
            this.MainScanButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.MainButtonComboBox = new System.Windows.Forms.ComboBox();
            this.MainDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ModifierButtonComboBox = new System.Windows.Forms.ComboBox();
            this.ModifierDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.BFCancelButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.UseModifierCheckBox = new System.Windows.Forms.CheckBox();
            this.InvertcheckBox = new System.Windows.Forms.CheckBox();
            this.toggleCheckBox = new System.Windows.Forms.CheckBox();
            this.Use8WayHatCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // MainScanButton
            // 
            this.MainScanButton.Location = new System.Drawing.Point(247, 26);
            this.MainScanButton.Name = "MainScanButton";
            this.MainScanButton.Size = new System.Drawing.Size(51, 50);
            this.MainScanButton.TabIndex = 0;
            this.MainScanButton.Text = "Scan";
            this.MainScanButton.UseVisualStyleBackColor = true;
            this.MainScanButton.Click += new System.EventHandler(this.OnMainScanButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Button";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Joystick";
            // 
            // MainButtonComboBox
            // 
            this.MainButtonComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MainButtonComboBox.FormattingEnabled = true;
            this.MainButtonComboBox.Items.AddRange(new object[] {
            "none"});
            this.MainButtonComboBox.Location = new System.Drawing.Point(178, 27);
            this.MainButtonComboBox.Name = "MainButtonComboBox";
            this.MainButtonComboBox.Size = new System.Drawing.Size(64, 21);
            this.MainButtonComboBox.TabIndex = 1;
            // 
            // MainDeviceComboBox
            // 
            this.MainDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MainDeviceComboBox.FormattingEnabled = true;
            this.MainDeviceComboBox.Items.AddRange(new object[] {
            "none"});
            this.MainDeviceComboBox.Location = new System.Drawing.Point(51, 27);
            this.MainDeviceComboBox.Name = "MainDeviceComboBox";
            this.MainDeviceComboBox.Size = new System.Drawing.Size(121, 21);
            this.MainDeviceComboBox.TabIndex = 4;
            this.MainDeviceComboBox.SelectedValueChanged += new System.EventHandler(this.MainDeviceComboBox_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(2, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Modifier";
            // 
            // ModifierButtonComboBox
            // 
            this.ModifierButtonComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModifierButtonComboBox.Enabled = false;
            this.ModifierButtonComboBox.FormattingEnabled = true;
            this.ModifierButtonComboBox.Items.AddRange(new object[] {
            "none"});
            this.ModifierButtonComboBox.Location = new System.Drawing.Point(178, 55);
            this.ModifierButtonComboBox.Name = "ModifierButtonComboBox";
            this.ModifierButtonComboBox.Size = new System.Drawing.Size(64, 21);
            this.ModifierButtonComboBox.TabIndex = 8;
            // 
            // ModifierDeviceComboBox
            // 
            this.ModifierDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModifierDeviceComboBox.Enabled = false;
            this.ModifierDeviceComboBox.FormattingEnabled = true;
            this.ModifierDeviceComboBox.Items.AddRange(new object[] {
            "none"});
            this.ModifierDeviceComboBox.Location = new System.Drawing.Point(51, 55);
            this.ModifierDeviceComboBox.Name = "ModifierDeviceComboBox";
            this.ModifierDeviceComboBox.Size = new System.Drawing.Size(121, 21);
            this.ModifierDeviceComboBox.TabIndex = 7;
            this.ModifierDeviceComboBox.SelectedIndexChanged += new System.EventHandler(this.ModifierDeviceComboBox_SelectedIndexChanged);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(102, 112);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(71, 23);
            this.OKButton.TabIndex = 13;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // BFCancelButton
            // 
            this.BFCancelButton.Location = new System.Drawing.Point(177, 112);
            this.BFCancelButton.Name = "BFCancelButton";
            this.BFCancelButton.Size = new System.Drawing.Size(65, 23);
            this.BFCancelButton.TabIndex = 14;
            this.BFCancelButton.Text = "Cancel";
            this.BFCancelButton.UseVisualStyleBackColor = true;
            this.BFCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(247, 112);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(51, 23);
            this.ClearButton.TabIndex = 15;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // UseModifierCheckBox
            // 
            this.UseModifierCheckBox.AutoSize = true;
            this.UseModifierCheckBox.Location = new System.Drawing.Point(12, 116);
            this.UseModifierCheckBox.Name = "UseModifierCheckBox";
            this.UseModifierCheckBox.Size = new System.Drawing.Size(82, 17);
            this.UseModifierCheckBox.TabIndex = 17;
            this.UseModifierCheckBox.Text = "use modifier";
            this.UseModifierCheckBox.UseVisualStyleBackColor = true;
            this.UseModifierCheckBox.CheckedChanged += new System.EventHandler(this.UseModifierCheckBox_CheckedChanged);
            // 
            // InvertcheckBox
            // 
            this.InvertcheckBox.AutoSize = true;
            this.InvertcheckBox.Location = new System.Drawing.Point(12, 87);
            this.InvertcheckBox.Name = "InvertcheckBox";
            this.InvertcheckBox.Size = new System.Drawing.Size(52, 17);
            this.InvertcheckBox.TabIndex = 18;
            this.InvertcheckBox.Text = "invert";
            this.InvertcheckBox.UseVisualStyleBackColor = true;
            // 
            // toggleCheckBox
            // 
            this.toggleCheckBox.AutoSize = true;
            this.toggleCheckBox.Location = new System.Drawing.Point(102, 87);
            this.toggleCheckBox.Name = "toggleCheckBox";
            this.toggleCheckBox.Size = new System.Drawing.Size(55, 17);
            this.toggleCheckBox.TabIndex = 19;
            this.toggleCheckBox.Text = "toggle";
            this.toggleCheckBox.UseVisualStyleBackColor = true;
            // 
            // Use8WayHatCheckBox
            // 
            this.Use8WayHatCheckBox.AutoSize = true;
            this.Use8WayHatCheckBox.Location = new System.Drawing.Point(178, 87);
            this.Use8WayHatCheckBox.Name = "Use8WayHatCheckBox";
            this.Use8WayHatCheckBox.Size = new System.Drawing.Size(79, 17);
            this.Use8WayHatCheckBox.TabIndex = 20;
            this.Use8WayHatCheckBox.Text = "8-way HAT";
            this.Use8WayHatCheckBox.UseVisualStyleBackColor = true;
            this.Use8WayHatCheckBox.CheckedChanged += new System.EventHandler(this.Use8WayHatCheckBox_CheckedChanged);
            // 
            // ButtonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 140);
            this.Controls.Add(this.Use8WayHatCheckBox);
            this.Controls.Add(this.toggleCheckBox);
            this.Controls.Add(this.InvertcheckBox);
            this.Controls.Add(this.UseModifierCheckBox);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.BFCancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ModifierButtonComboBox);
            this.Controls.Add(this.ModifierDeviceComboBox);
            this.Controls.Add(this.MainScanButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MainButtonComboBox);
            this.Controls.Add(this.MainDeviceComboBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(320, 179);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(320, 179);
            this.Name = "ButtonForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ButtonForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button MainScanButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        // private System.Windows.Forms.Button ModifierScanButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button BFCancelButton;
        private System.Windows.Forms.Button ClearButton;
        public System.Windows.Forms.ComboBox MainButtonComboBox;
        public System.Windows.Forms.ComboBox MainDeviceComboBox;
        public System.Windows.Forms.ComboBox ModifierButtonComboBox;
        public System.Windows.Forms.ComboBox ModifierDeviceComboBox;
        public System.Windows.Forms.CheckBox UseModifierCheckBox;
        public System.Windows.Forms.CheckBox InvertcheckBox;
        public System.Windows.Forms.CheckBox toggleCheckBox;
        public System.Windows.Forms.CheckBox Use8WayHatCheckBox;
    }
}