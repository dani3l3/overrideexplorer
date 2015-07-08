namespace OverrideExplorer
{
    partial class MPBrowserDialog
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
            this.lblManagementPacks = new System.Windows.Forms.Label();
            this.lstManagementPacks = new System.Windows.Forms.ListView();
            this.mpNameColumn = new System.Windows.Forms.ColumnHeader();
            this.mpVersionColumn = new System.Windows.Forms.ColumnHeader();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblManagementPacks
            // 
            this.lblManagementPacks.AutoSize = true;
            this.lblManagementPacks.Location = new System.Drawing.Point(13, 13);
            this.lblManagementPacks.Name = "lblManagementPacks";
            this.lblManagementPacks.Size = new System.Drawing.Size(176, 13);
            this.lblManagementPacks.TabIndex = 0;
            this.lblManagementPacks.Text = "Management Packs (unsealed only)";
            // 
            // lstManagementPacks
            // 
            this.lstManagementPacks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.mpNameColumn,
            this.mpVersionColumn});
            this.lstManagementPacks.FullRowSelect = true;
            this.lstManagementPacks.HideSelection = false;
            this.lstManagementPacks.Location = new System.Drawing.Point(16, 34);
            this.lstManagementPacks.MultiSelect = false;
            this.lstManagementPacks.Name = "lstManagementPacks";
            this.lstManagementPacks.Size = new System.Drawing.Size(492, 258);
            this.lstManagementPacks.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstManagementPacks.TabIndex = 1;
            this.lstManagementPacks.UseCompatibleStateImageBehavior = false;
            this.lstManagementPacks.View = System.Windows.Forms.View.Details;
            this.lstManagementPacks.SelectedIndexChanged += new System.EventHandler(this.lstManagementPacks_SelectedIndexChanged);
            // 
            // mpNameColumn
            // 
            this.mpNameColumn.Text = "Management Pack";
            this.mpNameColumn.Width = 280;
            // 
            // mpVersionColumn
            // 
            this.mpVersionColumn.Text = "Version";
            this.mpVersionColumn.Width = 180;
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(432, 327);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(351, 327);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MPBrowserDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(520, 370);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lstManagementPacks);
            this.Controls.Add(this.lblManagementPacks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MPBrowserDialog";
            this.Text = "Management Pack Browser";
            this.Load += new System.EventHandler(this.MPBrowserDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblManagementPacks;
        private System.Windows.Forms.ListView lstManagementPacks;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColumnHeader mpNameColumn;
        private System.Windows.Forms.ColumnHeader mpVersionColumn;
    }
}