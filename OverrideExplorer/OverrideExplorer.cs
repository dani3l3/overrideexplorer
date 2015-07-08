using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.EnterpriseManagement.Common;


namespace OverrideExplorer
{
    public partial class OverrideExplorer : Form
    {
        ManagementGroup                         m_managementGroup;
        BackgroundWorker                        m_backgroundWorker;
        ProgressDialog                          m_progressDialog;
        Cursor                                  m_oldCursor;
        MPOverridesInfo                         m_overridesInfo;
       
        //---------------------------------------------------------------------
        public OverrideExplorer()
        {
            InitializeComponent();

            InitializeBackgroundWorkerThread();

            m_progressDialog = new ProgressDialog();
        }

        //---------------------------------------------------------------------
        private void LoadOverrides()
        {
            m_overridesInfo = new MPOverridesInfo(m_managementGroup);

            m_overridesInfo.OverrideLoadingProgress += new MPOverridesInfo.OverrideLoadingProgressDelegate(overridesInfo_OverrideLoadingProgress);

            m_overridesInfo.LoadOverrides();
        }

        //---------------------------------------------------------------------
        void overridesInfo_OverrideLoadingProgress(int percentage, string status)
        {
            m_backgroundWorker.ReportProgress(percentage, status);
        }

        //---------------------------------------------------------------------
        private bool ConnectToManagementGroup()
        {
            ConnectionDialog    connectionDialog = new ConnectionDialog();
            DialogResult        dialogResult;

            dialogResult = connectionDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                m_managementGroup = connectionDialog.ManagementGroup;

                Text = string.Format("Overrides Explorer 2012 - MS: {0} - MG: {1}", m_managementGroup.ConnectionSettings.ServerName, m_managementGroup.Name);
            }

            return (dialogResult == DialogResult.OK);
        }

        //---------------------------------------------------------------------
        private void PopulateTypeBasedOverrideTree()
        {         
            mpElementTree.Nodes.Clear();

            foreach (KeyValuePair<Guid,MP> mpPair in m_overridesInfo.ManagementPacks)
            {
                TreeNode mpTreeNode;

                mpTreeNode      = new TreeNode();
                mpTreeNode.Text = string.Format("{0} ({1})", mpPair.Value.Name, mpPair.Value.NumberOfOverrides);
                mpTreeNode.Tag  = mpPair.Value;

                mpElementTree.Nodes.Add(mpTreeNode);

                AddClassNodesToObjectTree(mpPair, mpTreeNode);
            }

            mpElementTree.Sort();
        }

        //---------------------------------------------------------------------
        private void AddClassNodesToObjectTree(
            KeyValuePair<Guid, MP>  mpPair,
            TreeNode                mpTreeNode
            )
        {
            foreach (KeyValuePair<Guid, MPClass> classPair in mpPair.Value.Classes)
            {
                TreeNode classNode;

                classNode       = new TreeNode();
                classNode.Text  = string.Format("{0} ({1})", classPair.Value.Name, classPair.Value.NumberOfOverrides);
                classNode.Tag   = classPair.Value;

                mpTreeNode.Nodes.Add(classNode);

                AddWorkflowNodesToObjectTree(classPair.Value, classNode);
            }
        }

        //---------------------------------------------------------------------
        private void AddWorkflowNodesToObjectTree(
            MPClass                 mpClass,
            TreeNode                classNode
            )
        {
            TreeNode monitorsNode       = new TreeNode("Monitors");;
            TreeNode rulesNode          = new TreeNode("Rules");
            TreeNode discoveriesNode    = new TreeNode("Discoveries");

            foreach (KeyValuePair<Guid, MPWorkflow> workflowPair in mpClass.MPWorkflows)
            {
                string      workflowName    = Common.GetBestElementName(workflowPair.Value.Workflow);
                TreeNode    treeNode        = new TreeNode(workflowName);

                treeNode.Tag = workflowPair.Value;

                if (workflowPair.Value.Workflow is ManagementPackMonitor)
                {
                    monitorsNode.Nodes.Add(treeNode);
                }
                else if (workflowPair.Value.Workflow is ManagementPackRule)
                {
                    rulesNode.Nodes.Add(treeNode);
                }
                else if (workflowPair.Value.Workflow is ManagementPackDiscovery)
                {
                    discoveriesNode.Nodes.Add(treeNode);
                }
            }

            if (monitorsNode.Nodes.Count > 0)
            {
                classNode.Nodes.Add(monitorsNode);
            }

            if (rulesNode.Nodes.Count > 0)
            {
                classNode.Nodes.Add(rulesNode);
            }

            if (discoveriesNode.Nodes.Count > 0)
            {
                classNode.Nodes.Add(discoveriesNode);
            }
        }

        //---------------------------------------------------------------------
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ConnectToManagementGroup())
            {
                m_oldCursor = Cursor;

                m_progressDialog = new ProgressDialog();

                m_progressDialog.Show(this);

                m_backgroundWorker.RunWorkerAsync();

                exportToXMLToolStripMenuItem.Enabled = true;
                exportToExcelToolStripMenuItem.Enabled = true;

                overrideListView.Initialize(m_managementGroup, OverrideListViewColumnConfig.TypeOverrideColumns);
                overrideListView2.Initialize(m_managementGroup, OverrideListViewColumnConfig.InstanceColumns);
            }
        }

        //---------------------------------------------------------------------
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //---------------------------------------------------------------------
        private void InitializeBackgroundWorkerThread()
        {
            m_backgroundWorker = new BackgroundWorker();

            m_backgroundWorker.DoWork               += new DoWorkEventHandler(m_backgroundWorker_DoWork);
            m_backgroundWorker.ProgressChanged      += new System.ComponentModel.ProgressChangedEventHandler(ProgressReported);
            m_backgroundWorker.RunWorkerCompleted   += new RunWorkerCompletedEventHandler(m_backgroundWorker_RunWorkerCompleted);

            m_backgroundWorker.WorkerReportsProgress = true;
        }

        //---------------------------------------------------------------------
        void ProgressReported(object sender, ProgressChangedEventArgs e)
        {
            m_progressDialog.UpdateProgress(new ProgressInfo((string)e.UserState,e.ProgressPercentage));
        }

        //---------------------------------------------------------------------
        void ProgressChangedHandler(object sender, ProgressChangedEventArgs e)
        {
            m_progressDialog.UpdateProgress(new ProgressInfo((string)e.UserState, e.ProgressPercentage));
        }

        //---------------------------------------------------------------------
        void m_backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = m_oldCursor;
            m_progressDialog.Close();

            PopulateTypeBasedOverrideTree();
            PopulateAgentTree();
        }

        //---------------------------------------------------------------------
        private void PopulateAgentTree()
        {
            monitoringObjectTreeView.Initialize(m_managementGroup, m_overridesInfo.WindowsComputers, m_overridesInfo.UnixComputers);
        }                

        //---------------------------------------------------------------------
        void m_backgroundWorker_ProgressChanged(
            object                      sender,
            ProgressChangedEventArgs    e
            )
        {
            m_progressDialog.UpdateProgress(new ProgressInfo((string)e.UserState, e.ProgressPercentage));
        }

        //---------------------------------------------------------------------
        void m_backgroundWorker_DoWork(
            object          sender,
            DoWorkEventArgs e
            )
        {
            LoadOverrides();
        }       

        //---------------------------------------------------------------------
        private void exportToXMLToolStripMenuItem_Click(
            object sender,
            EventArgs e
            )
        {
            OverridesToXmlExporter exporter = new OverridesToXmlExporter(m_overridesInfo.ManagementPacks,
                                                                         m_managementGroup);

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.AddExtension     = true;
            saveFileDialog.CheckPathExists  = true;
            saveFileDialog.DefaultExt       = "txt";
            saveFileDialog.Filter           = "XML files (*.xml)|*.xml";
            saveFileDialog.InitialDirectory = "C:\\";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            exporter.Export(saveFileDialog.FileName);
        }



        //---------------------------------------------------------------------
        // export to Excel funzionality is a merge of the Excel exporter code in MPViewer 
        // and of this script here http://www.systemcentercentral.com/BlogDetails/tabid/143/IndexID/78323/Default.aspx
        //---------------------------------------------------------------------
        private void exportToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "xml";
            dlg.Filter = "Excel XML files (*.xml)|*.xml";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                OverridesToExcelExporter writer = new OverridesToExcelExporter(m_overridesInfo.ManagementPacks, m_managementGroup);

                writer.WriteTableHeader();

                foreach (KeyValuePair<Guid,MP> mpPair in m_overridesInfo.ManagementPacks)
                {
                    writer.WriteOverridesToExcel(mpPair.Value);
                }

                writer.FinalizeTableAndWorksheet();

                writer.SaveToFile(dlg.FileName);
            }
        }


        //---------------------------------------------------------------------
        private void mpElementTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                DisplayOverridesForMPWorkflow(e.Node);
            }
            catch (Microsoft.EnterpriseManagement.Common.ServerDisconnectedException)
            {
                m_managementGroup.Reconnect();

                DisplayOverridesForMPWorkflow(e.Node);
            }
        }
                
        //---------------------------------------------------------------------
        private void monitoringObjectTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Cursor oldCursor = this.Cursor;

            this.Cursor = Cursors.WaitCursor;

            try
            {
                DisplayOverridesForMonitoringObject(e.Node);
            }
            catch (Microsoft.EnterpriseManagement.Common.ServerDisconnectedException)
            {
                m_managementGroup.Reconnect();

                DisplayOverridesForMonitoringObject(e.Node);
            }

            this.Cursor = oldCursor;
        }

        //---------------------------------------------------------------------
        private void DisplayOverridesForMonitoringObject(TreeNode treeNode)
        {
            Dictionary<PartialMonitoringObject, List<MPOverride>>   monitoringObjectToOverridesMap = new Dictionary<PartialMonitoringObject, List<MPOverride>>();
            ReadOnlyCollection<PartialMonitoringObject>             containedObjects;
            PartialMonitoringObject                                 selectedMonitoringObject = (PartialMonitoringObject)treeNode.Tag;

            containedObjects = GetContainedMonitoringObjects(selectedMonitoringObject);

            monitoringObjectToOverridesMap.Add(selectedMonitoringObject,
                                               RetrieveEffectiveOverridesForMonitoringObject(selectedMonitoringObject));

            foreach (PartialMonitoringObject monitoringObject in containedObjects)
            {
                monitoringObjectToOverridesMap.Add(monitoringObject,
                                                   RetrieveEffectiveOverridesForMonitoringObject(monitoringObject));
            }

            overrideListView2.ClearOverrides();

            overrideListView2.AddOverrides(monitoringObjectToOverridesMap);
        }

        private List<MPOverride> RetrieveEffectiveOverridesForMonitoringObject(
            PartialMonitoringObject monitoringObject
            )
        {
            List<MPOverride> overrides = new List<MPOverride>();

            overrides.AddRange(Common.RetrieveRuleOverrides(monitoringObject,
                               m_overridesInfo.Types,
                               m_overridesInfo.TypeToRuleTable));

            overrides.AddRange(Common.RetrieveMonitorOverrides(monitoringObject,
                               m_overridesInfo.Types));

            overrides.AddRange(Common.RetrieveDiscoveryOverrides(monitoringObject,
                               m_overridesInfo.Types));
            return (overrides);
        }

        //---------------------------------------------------------------------
        private ReadOnlyCollection<PartialMonitoringObject> GetContainedMonitoringObjects(
            PartialMonitoringObject                     partialMonitoringObject
            )
        {
            return (partialMonitoringObject.GetRelatedPartialMonitoringObjects(TraversalDepth.Recursive));            
        }

        //---------------------------------------------------------------------
        private void DisplayOverridesForMPWorkflow(TreeNode treeNode)
        {
            overrideListView.ClearOverrides();

            if (!(treeNode.Tag is MPWorkflow))
            {
                return;
            }

            MPWorkflow workflow = (MPWorkflow)treeNode.Tag;
            MPClass workflowTarget = (MPClass)treeNode.Parent.Parent.Tag;

            List<MPOverride> overrides = new List<MPOverride>();

            foreach (ManagementPackOverride mpOverride in workflow.Overrides)
            {
                overrides.Add(new MPOverride(mpOverride, workflow, workflowTarget));
            }

            overrideListView.AddOverrides(overrides);
        }
    }
}