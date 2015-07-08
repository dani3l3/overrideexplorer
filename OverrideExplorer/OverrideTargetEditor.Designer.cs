namespace OverrideExplorer
{
    partial class OverrideTargetEditor
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
            this.allInstancesRadioButton = new System.Windows.Forms.RadioButton();
            this.groupRadioButton = new System.Windows.Forms.RadioButton();
            this.instanceRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.groupsComboBox = new System.Windows.Forms.ComboBox();
            this.instancesComboBox = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // allInstancesRadioButton
            // 
            this.allInstancesRadioButton.AutoSize = true;
            this.allInstancesRadioButton.Location = new System.Drawing.Point(13, 32);
            this.allInstancesRadioButton.Name = "allInstancesRadioButton";
            this.allInstancesRadioButton.Size = new System.Drawing.Size(85, 17);
            this.allInstancesRadioButton.TabIndex = 0;
            this.allInstancesRadioButton.TabStop = true;
            this.allInstancesRadioButton.Text = "All Instances";
            this.allInstancesRadioButton.UseVisualStyleBackColor = true;
            this.allInstancesRadioButton.CheckedChanged += new System.EventHandler(this.radiobutton_CheckChanged);
            // 
            // groupRadioButton
            // 
            this.groupRadioButton.AutoSize = true;
            this.groupRadioButton.Location = new System.Drawing.Point(13, 61);
            this.groupRadioButton.Name = "groupRadioButton";
            this.groupRadioButton.Size = new System.Drawing.Size(108, 17);
            this.groupRadioButton.TabIndex = 0;
            this.groupRadioButton.TabStop = true;
            this.groupRadioButton.Text = "A particular group";
            this.groupRadioButton.UseVisualStyleBackColor = true;
            this.groupRadioButton.CheckedChanged += new System.EventHandler(this.radiobutton_CheckChanged);
            // 
            // instanceRadioButton
            // 
            this.instanceRadioButton.AutoSize = true;
            this.instanceRadioButton.Location = new System.Drawing.Point(13, 90);
            this.instanceRadioButton.Name = "instanceRadioButton";
            this.instanceRadioButton.Size = new System.Drawing.Size(121, 17);
            this.instanceRadioButton.TabIndex = 0;
            this.instanceRadioButton.TabStop = true;
            this.instanceRadioButton.Text = "A particular instance";
            this.instanceRadioButton.UseVisualStyleBackColor = true;
            this.instanceRadioButton.CheckedChanged += new System.EventHandler(this.radiobutton_CheckChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "What would you like the override to apply to?";
            // 
            // groupsComboBox
            // 
            this.groupsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupsComboBox.FormattingEnabled = true;
            this.groupsComboBox.Location = new System.Drawing.Point(140, 62);
            this.groupsComboBox.Name = "groupsComboBox";
            this.groupsComboBox.Size = new System.Drawing.Size(340, 21);
            this.groupsComboBox.Sorted = true;
            this.groupsComboBox.TabIndex = 2;
            this.groupsComboBox.DropDown += new System.EventHandler(this.groupsComboBox_DropDown);
            // 
            // instancesComboBox
            // 
            this.instancesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.instancesComboBox.FormattingEnabled = true;
            this.instancesComboBox.Location = new System.Drawing.Point(140, 89);
            this.instancesComboBox.Name = "instancesComboBox";
            this.instancesComboBox.Size = new System.Drawing.Size(340, 21);
            this.instancesComboBox.Sorted = true;
            this.instancesComboBox.TabIndex = 2;
            this.instancesComboBox.DropDown += new System.EventHandler(this.instancesComboBox_DropDown);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(404, 359);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(323, 359);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // OverrideTargetEditor
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(494, 394);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.instancesComboBox);
            this.Controls.Add(this.groupsComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.instanceRadioButton);
            this.Controls.Add(this.groupRadioButton);
            this.Controls.Add(this.allInstancesRadioButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OverrideTargetEditor";
            this.Text = "Override Target Editor";
            this.Load += new System.EventHandler(this.overrideTargetEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton allInstancesRadioButton;
        private System.Windows.Forms.RadioButton groupRadioButton;
        private System.Windows.Forms.RadioButton instanceRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox groupsComboBox;
        private System.Windows.Forms.ComboBox instancesComboBox;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}