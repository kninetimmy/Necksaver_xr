
namespace XRNeckSafer
{
    partial class ScanForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.CancelScanButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this._pressedButtonsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(56, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Scanning joysticks. Press button(s) now...";
            // 
            // CancelScanButton
            // 
            this.CancelScanButton.Location = new System.Drawing.Point(173, 96);
            this.CancelScanButton.Name = "CancelScanButton";
            this.CancelScanButton.Size = new System.Drawing.Size(75, 23);
            this.CancelScanButton.TabIndex = 1;
            this.CancelScanButton.Text = "Cancel";
            this.CancelScanButton.UseVisualStyleBackColor = true;
            this.CancelScanButton.Click += new System.EventHandler(this.OnCancelButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Pressed:";
            // 
            // _pressedButtonsLabel
            // 
            this._pressedButtonsLabel.AutoSize = true;
            this._pressedButtonsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._pressedButtonsLabel.Location = new System.Drawing.Point(79, 61);
            this._pressedButtonsLabel.Name = "_pressedButtonsLabel";
            this._pressedButtonsLabel.Size = new System.Drawing.Size(38, 16);
            this._pressedButtonsLabel.TabIndex = 3;
            this._pressedButtonsLabel.Text = "none";
            // 
            // ScanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 131);
            this.Controls.Add(this._pressedButtonsLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CancelScanButton);
            this.Controls.Add(this.label1);
            this.Name = "ScanForm";
            this.Text = "ScanForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CancelScanButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label _pressedButtonsLabel;
    }
}