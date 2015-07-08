namespace OverrideExplorer
{
    partial class OverrideExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverrideExplorer));
            this.mpElementTree = new System.Windows.Forms.TreeView();
            this.typesSplitContainer = new System.Windows.Forms.SplitContainer();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.overrideViewsTabControl = new System.Windows.Forms.TabControl();
            this.typesTabPage = new System.Windows.Forms.TabPage();
            this.computersTabPage = new System.Windows.Forms.TabPage();
            this.agentsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.monitoringObjectTreeView = new MonitoringObjectTreeView();
            overrideListView = new OverrideListView();
            overrideListView2 = new OverrideListView();
            ((System.ComponentModel.ISupportInitialize)(this.typesSplitContainer)).BeginInit();
            this.typesSplitContainer.Panel1.SuspendLayout();
            this.typesSplitContainer.Panel2.SuspendLayout();
            this.typesSplitContainer.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.overrideViewsTabControl.SuspendLayout();
            this.typesTabPage.SuspendLayout();
            this.computersTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.agentsSplitContainer)).BeginInit();
            this.agentsSplitContainer.Panel1.SuspendLayout();
            this.agentsSplitContainer.Panel2.SuspendLayout();
            this.agentsSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mpElementTree
            // 
            this.mpElementTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mpElementTree.HideSelection = false;
            this.mpElementTree.Location = new System.Drawing.Point(0, 0);
            this.mpElementTree.Name = "mpElementTree";
            this.mpElementTree.Size = new System.Drawing.Size(333, 599);
            this.mpElementTree.TabIndex = 0;
            this.mpElementTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mpElementTree_AfterSelect);
            // 
            // typesSplitContainer
            // 
            this.typesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typesSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.typesSplitContainer.Name = "typesSplitContainer";
            // 
            // typesSplitContainer.Panel1
            // 
            this.typesSplitContainer.Panel1.Controls.Add(this.mpElementTree);
            // 
            // typesSplitContainer.Panel2
            // 
            this.typesSplitContainer.Panel2.Controls.Add(this.overrideListView);
            this.typesSplitContainer.Size = new System.Drawing.Size(1006, 599);
            this.typesSplitContainer.SplitterDistance = 333;
            this.typesSplitContainer.TabIndex = 2;
            // 
            // overrideListView
            // 
            this.overrideListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overrideListView.FullRowSelect = true;
            this.overrideListView.GridLines = true;
            this.overrideListView.HideSelection = false;
            this.overrideListView.Location = new System.Drawing.Point(0, 0);
            this.overrideListView.MultiSelect = false;
            this.overrideListView.Name = "overrideListView";
            this.overrideListView.Size = new System.Drawing.Size(669, 599);
            this.overrideListView.TabIndex = 0;
            this.overrideListView.UseCompatibleStateImageBehavior = false;
            this.overrideListView.View = System.Windows.Forms.View.Details;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1020, 24);
            this.mainMenuStrip.TabIndex = 3;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.exportToXMLToolStripMenuItem,
            this.exportToExcelToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // exportToXMLToolStripMenuItem
            // 
            this.exportToXMLToolStripMenuItem.Enabled = false;
            this.exportToXMLToolStripMenuItem.Name = "exportToXMLToolStripMenuItem";
            this.exportToXMLToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.exportToXMLToolStripMenuItem.Text = "Export to XML";
            this.exportToXMLToolStripMenuItem.Click += new System.EventHandler(this.exportToXMLToolStripMenuItem_Click);
            // 
            // exportToExcelToolStripMenuItem
            // 
            this.exportToExcelToolStripMenuItem.Enabled = false;
            this.exportToExcelToolStripMenuItem.Name = "exportToExcelToolStripMenuItem";
            this.exportToExcelToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.exportToExcelToolStripMenuItem.Text = "Export to Excel";
            this.exportToExcelToolStripMenuItem.Click += new System.EventHandler(this.exportToExcelToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // overrideViewsTabControl
            // 
            this.overrideViewsTabControl.Controls.Add(this.typesTabPage);
            this.overrideViewsTabControl.Controls.Add(this.computersTabPage);
            this.overrideViewsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overrideViewsTabControl.Location = new System.Drawing.Point(0, 24);
            this.overrideViewsTabControl.Name = "overrideViewsTabControl";
            this.overrideViewsTabControl.SelectedIndex = 0;
            this.overrideViewsTabControl.Size = new System.Drawing.Size(1020, 631);
            this.overrideViewsTabControl.TabIndex = 4;
            // 
            // typesTabPage
            // 
            this.typesTabPage.Controls.Add(this.typesSplitContainer);
            this.typesTabPage.Location = new System.Drawing.Point(4, 22);
            this.typesTabPage.Name = "typesTabPage";
            this.typesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.typesTabPage.Size = new System.Drawing.Size(1012, 605);
            this.typesTabPage.TabIndex = 0;
            this.typesTabPage.Text = "Type based view";
            this.typesTabPage.UseVisualStyleBackColor = true;
            // 
            // computersTabPage
            // 
            this.computersTabPage.Controls.Add(this.agentsSplitContainer);
            this.computersTabPage.Location = new System.Drawing.Point(4, 22);
            this.computersTabPage.Name = "computersTabPage";
            this.computersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.computersTabPage.Size = new System.Drawing.Size(1012, 605);
            this.computersTabPage.TabIndex = 1;
            this.computersTabPage.Text = "Computers based view";
            this.computersTabPage.UseVisualStyleBackColor = true;
            // 
            // agentsSplitContainer
            // 
            this.agentsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.agentsSplitContainer.Location = new System.Drawing.Point(3, 3);
            this.agentsSplitContainer.Name = "agentsSplitContainer";
            // 
            // agentsSplitContainer.Panel1
            // 
            this.agentsSplitContainer.Panel1.Controls.Add(this.monitoringObjectTreeView);
            // 
            // agentsSplitContainer.Panel2
            // 
            this.agentsSplitContainer.Panel2.Controls.Add(this.overrideListView2);
            this.agentsSplitContainer.Size = new System.Drawing.Size(1006, 599);
            this.agentsSplitContainer.SplitterDistance = 334;
            this.agentsSplitContainer.TabIndex = 0;
            // 
            // monitoringObjectTreeView
            // 
            this.monitoringObjectTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitoringObjectTreeView.HideSelection = false;
            this.monitoringObjectTreeView.Location = new System.Drawing.Point(0, 0);
            this.monitoringObjectTreeView.Name = "monitoringObjectTreeView";
            this.monitoringObjectTreeView.Size = new System.Drawing.Size(334, 599);
            this.monitoringObjectTreeView.TabIndex = 0;
            this.monitoringObjectTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.monitoringObjectTreeView_AfterSelect);
            // 
            // overrideListView2
            // 
            this.overrideListView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overrideListView2.FullRowSelect = true;
            this.overrideListView2.GridLines = true;
            this.overrideListView2.HideSelection = false;
            this.overrideListView2.Location = new System.Drawing.Point(0, 0);
            this.overrideListView2.MultiSelect = false;
            this.overrideListView2.Name = "overrideListView2";
            this.overrideListView2.Size = new System.Drawing.Size(668, 599);
            this.overrideListView2.TabIndex = 1;
            this.overrideListView2.UseCompatibleStateImageBehavior = false;
            this.overrideListView2.View = System.Windows.Forms.View.Details;
            // 
            // OverrideExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 655);
            this.Controls.Add(this.overrideViewsTabControl);
            this.Controls.Add(this.mainMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "OverrideExplorer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Override Explorer 2012";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.typesSplitContainer.Panel1.ResumeLayout(false);
            this.typesSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.typesSplitContainer)).EndInit();
            this.typesSplitContainer.ResumeLayout(false);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.overrideViewsTabControl.ResumeLayout(false);
            this.typesTabPage.ResumeLayout(false);
            this.computersTabPage.ResumeLayout(false);
            this.agentsSplitContainer.Panel1.ResumeLayout(false);
            this.agentsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.agentsSplitContainer)).EndInit();
            this.agentsSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView mpElementTree;
        private OverrideListView overrideListView;
        private System.Windows.Forms.SplitContainer typesSplitContainer;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToExcelToolStripMenuItem;
        private System.Windows.Forms.TabControl overrideViewsTabControl;
        private System.Windows.Forms.TabPage typesTabPage;
        private System.Windows.Forms.TabPage computersTabPage;
        private System.Windows.Forms.SplitContainer agentsSplitContainer;
        private MonitoringObjectTreeView monitoringObjectTreeView;
        private OverrideListView overrideListView2;
    }
}

