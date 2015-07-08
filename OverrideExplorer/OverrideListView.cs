using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement;
using System.Diagnostics;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Collections;

namespace OverrideExplorer
{
    internal enum OverrideListViewColumnConfig
    {
        TypeOverrideColumns,
        WorkflowColumns,
        InstanceColumns
    }

    internal struct OverrideInfo
    {
        internal MPOverride              m_override;
        internal PartialMonitoringObject m_monitoringObject;
    }

    internal class OverrideListView : ListView
    {
        ManagementGroup                 m_managementGroup;
        OverrideListViewColumnConfig    m_columnConfig;
        ListViewColumnSorter            m_columnSorter;
        bool                            m_groupingEnabled;

        //---------------------------------------------------------------------
        internal OverrideListView()
        {
            View            = System.Windows.Forms.View.Details;
            MultiSelect     = false;
            GridLines       = true;
            FullRowSelect   = true;
            Dock            = DockStyle.Fill;
            SetAutoSizeMode(AutoSizeMode.GrowAndShrink);

            m_columnSorter = new ListViewColumnSorter();
            ListViewItemSorter = m_columnSorter;

            ColumnClick += new ColumnClickEventHandler(OverrideListView_ColumnClick);

            m_groupingEnabled = true;
        }

        //---------------------------------------------------------------------
        void OverrideListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == m_columnSorter.SortColumn)
            {
                if (m_columnSorter.Order == SortOrder.Ascending)
                {
                    m_columnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    m_columnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {                
                m_columnSorter.SortColumn = e.Column;
                m_columnSorter.Order = SortOrder.Ascending;
            }
                        
            Sort();            
        }

        //---------------------------------------------------------------------
        internal void Initialize(
            ManagementGroup                 managementGroup,
            OverrideListViewColumnConfig    columnConfig
            )
        {
            Debug.Assert(managementGroup != null);

            m_managementGroup   = managementGroup;
            m_columnConfig      = columnConfig;

            CreateColumns();
            CreateContextMenu();
        }       

        //---------------------------------------------------------------------
        internal void ClearOverrides()
        {
            Items.Clear();
        }

        //---------------------------------------------------------------------
        internal void AddOverrides(
            List<MPOverride> overrides
            )
        {
            foreach (MPOverride mpOverride in overrides)
            {
                if (mpOverride.ManagementPackOverride is ManagementPackCategoryOverride)
                {
                    continue;
                }
                
                AddOverride(mpOverride,null);
            }

            AdjustColumnSizes();
        }

        //---------------------------------------------------------------------
        internal void AddOverrides(
            Dictionary<PartialMonitoringObject, List<MPOverride>> monitoringObjectToOverridesMap
            )
        {
            BeginUpdate();

            InternalAddOverrides(monitoringObjectToOverridesMap);

            CreateGroups();

            AdjustColumnSizes();

            OverrideListView_ColumnClick(this, new ColumnClickEventArgs(0));
            OverrideListView_ColumnClick(this, new ColumnClickEventArgs(1));

            Columns[0].Width = 0;

            EndUpdate();
        }

        //---------------------------------------------------------------------
        private void InternalAddOverrides(
            Dictionary<PartialMonitoringObject, List<MPOverride>> monitoringObjectToOverridesMap
            )
        {
            foreach (KeyValuePair<PartialMonitoringObject, List<MPOverride>> pair in monitoringObjectToOverridesMap)
            {
                foreach (MPOverride mpOverride in pair.Value)
                {
                    if (mpOverride.ManagementPackOverride is ManagementPackCategoryOverride)
                    {
                        continue;
                    }

                    AddOverride(mpOverride, pair.Key);
                }
            }
        }

        //---------------------------------------------------------------------
        private void CreateGroups()
        {
            Dictionary<PartialMonitoringObject, List<ListViewItem>> itemDictionary = new Dictionary<PartialMonitoringObject, List<ListViewItem>>();

            ShowGroups = true;

            foreach (ListViewItem item in Items)
            {
                PartialMonitoringObject monitoringObject = ((OverrideInfo)item.Tag).m_monitoringObject;

                if (!itemDictionary.ContainsKey(monitoringObject))
                {
                    itemDictionary.Add(monitoringObject, new List<ListViewItem>());                    
                }

                itemDictionary[monitoringObject].Add(item);
            }

            foreach (KeyValuePair<PartialMonitoringObject, List<ListViewItem>> pair in itemDictionary)
            {
                PartialMonitoringObject monitoringObject        = pair.Key;
                MonitoringClass         monitoringObjectClass   = monitoringObject.GetLeastDerivedNonAbstractMonitoringClass();
                string                  groupName;
                string                  instanceName;
                string                  className;

                instanceName    = Common.GetBestMonitoringObjectName(monitoringObject);
                className       = Common.GetBestElementName(monitoringObjectClass);

                groupName = string.Format("{0} ({1})", instanceName, className);
                
                ListViewGroup group = new ListViewGroup(groupName);

                Groups.Add(group);

                foreach (ListViewItem item in pair.Value)
                {
                    item.Group = group;
                }                
            }
        }

        //---------------------------------------------------------------------
        private void AdjustColumnSizes()
        {
            foreach (ColumnHeader column in Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        //---------------------------------------------------------------------
        private void AddOverride(
            MPOverride              mpOverride,
            PartialMonitoringObject monitoringObject
            )
        {
            ManagementPackOverride actualOverride = mpOverride.ManagementPackOverride; 
            ManagementPack         overrideMp;
            ListViewItem           overrideItem;
            string                 parameter;
            string                 newValue;

            Common.RetrieveParameterInfo(actualOverride, out parameter, out newValue);

            overrideMp      = mpOverride.ManagementPackOverride.GetManagementPack();
            overrideItem    = new ListViewItem();

            if (m_columnConfig == OverrideListViewColumnConfig.InstanceColumns)
            {
                overrideItem.Text = Common.GetBestMonitoringObjectName(monitoringObject);
            }

            if (m_columnConfig == OverrideListViewColumnConfig.WorkflowColumns ||
                m_columnConfig == OverrideListViewColumnConfig.InstanceColumns)
            {
                if (overrideItem.Text == null || overrideItem.Text.Length == 0)
                {
                    overrideItem.Text = mpOverride.Workflow.Workflow.DisplayName;
                }
                else
                {
                    overrideItem.SubItems.Add(mpOverride.Workflow.Workflow.DisplayName);
                }

                overrideItem.SubItems.Add(GetOverridenWorkflowType(mpOverride.Workflow.Workflow));

                overrideItem.SubItems.Add(parameter);
            }
            else
            {
                overrideItem.Text = parameter;
            }

            OverrideInfo overrideInfo = new OverrideInfo();

            overrideInfo.m_monitoringObject = monitoringObject;
            overrideInfo.m_override         = mpOverride;

            overrideItem.Tag = overrideInfo;

            overrideItem.SubItems.Add(newValue);
            overrideItem.SubItems.Add(actualOverride.TimeAdded.ToLocalTime().ToString());
            overrideItem.SubItems.Add(Common.RetrieveContext(mpOverride.ManagementPackOverride, m_managementGroup));
            overrideItem.SubItems.Add(actualOverride.Enforced.ToString());

            overrideItem.SubItems.Add(Common.GetBestManagementPackName(overrideMp));

            overrideItem.SubItems.Add(Common.GetManagementPackSealedFlag(overrideMp));

            overrideItem.SubItems.Add(mpOverride.ManagementPackOverride.Description);

            Items.Add(overrideItem);
        }

        //---------------------------------------------------------------------
        private string GetOverridenWorkflowType(
            ManagementPackElement managementPackElement
            )
        {
            string workflowType;

            if (managementPackElement is ManagementPackMonitor)
            {
                workflowType = "Monitor";
            }
            else if (managementPackElement is ManagementPackRule)
            {
                workflowType = "Rule";
            }
            else if (managementPackElement is ManagementPackDiscovery)
            {
                workflowType = "Discovery";
            }
            else if (managementPackElement is ManagementPackDiagnostic)
            {
                workflowType = "Diagnostic";
            }
            else if (managementPackElement is ManagementPackRecovery)
            {
                workflowType = "Recovery";
            }
            else
            {
                workflowType = managementPackElement.GetType().ToString();
            }

            return (workflowType);
        }

        //---------------------------------------------------------------------
        private void moveToDifferentMPMenuItem_Click(
            object      sender, 
            EventArgs   e
            )
        {
            if (SelectedItems.Count == 0)
            {
                return;
            }

            MPBrowserDialog mpBrowserDialog = new MPBrowserDialog(m_managementGroup);

            if (mpBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                MPOverride sourceOverride = ((OverrideInfo)SelectedItems[0].Tag).m_override;

                ManagementPack targetManagementPack = mpBrowserDialog.ManagementPack;
                ManagementPack sourceManagementPack = sourceOverride.ManagementPackOverride.GetManagementPack();

                if (!sourceManagementPack.Sealed)
                {
                    OverrideMover overrideMover = new OverrideMover(sourceManagementPack,
                                                                    targetManagementPack,
                                                                    sourceOverride.ManagementPackOverride,
                                                                    sourceOverride.Workflow);
                    ManagementPackOverride newOverride;

                    newOverride = overrideMover.PerformMove();

                    sourceOverride.Workflow.RemoveOverride(sourceOverride.ManagementPackOverride);
                    sourceOverride.Workflow.AddOverride(newOverride);
                }
                else 
                {
                    MessageBox.Show("Cannot move an override out of a Sealed MP!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //---------------------------------------------------------------------
        private void groupWorkflows_Click(
            object      sender,
            EventArgs   e
            )
        {
            if (m_groupingEnabled)
            {                
                ((ToolStripItem)sender).Text = "Group workflows";
                m_groupingEnabled = false;

                ShowGroups = false;

                Columns[0].Width = 100;

                AdjustColumnSizes();
            }
            else
            {
                ((ToolStripItem)sender).Text = "Disable grouping of workflows";  
                
                BeginUpdate();
                
                CreateGroups();

                AdjustColumnSizes();

                Columns[0].Width = 0;

                EndUpdate();

                m_groupingEnabled = true;
            }
        }

        //---------------------------------------------------------------------
        private void deleteOverrideMenuItem_Click(
            object      sender,
            EventArgs   e
            )
        {
            try
            {
                if (SelectedItems.Count == 0)
                {
                    return;
                }

                MPOverride mpOverride = ((OverrideInfo)SelectedItems[0].Tag).m_override;

                if (mpOverride.ManagementPackOverride.GetManagementPack().Sealed)
                {
                    MessageBox.Show("Its not possible to delete an override that is defined in a sealed management pack.");
                    return;
                }

                if (MessageBox.Show("Are you sure you wish to delete the selected override?",
                                    "Delete override",
                                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    mpOverride.ManagementPackOverride.Status = ManagementPackElementStatus.PendingDelete;

                    mpOverride.ManagementPackOverride.GetManagementPack().AcceptChanges();

                    mpOverride.Workflow.RemoveOverride(mpOverride.ManagementPackOverride);

                    Items.Remove(SelectedItems[0]);
                }
            }
            catch (Microsoft.EnterpriseManagement.Common.ManagementPackException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        //---------------------------------------------------------------------
        private void changeTargetMenuItem_Click(
            object      sender,
            EventArgs   e
            )
        {
            try
            {
                if (SelectedItems.Count == 0)
                {
                    return;
                }

                MPOverride mpOverride = ((OverrideInfo)SelectedItems[0].Tag).m_override;

                if (mpOverride.ManagementPackOverride.GetManagementPack().Sealed)
                {
                    MessageBox.Show("Its not possible to modify an override that is defined in a sealed MP");
                    return;
                }

                MPClass mpClass = mpOverride.Target;

                OverrideTargetEditor targetEditor;
                
                targetEditor = new OverrideTargetEditor(m_managementGroup,
                                                        mpClass.ManagementPackClass, 
                                                        mpOverride.ManagementPackOverride.ContextInstance);

                if (targetEditor.ShowDialog(this) == DialogResult.OK)
                {
                    if (targetEditor.IsInstanceContext)
                    {
                        PartialMonitoringObject context = targetEditor.InstanceContext;

                        IList<ManagementPackClass> contextClasses = context.GetMostDerivedClasses();
                        
                        //TODO - dmuscett 2012/11/19 - this works for custom groups, but what for non-groups?
                        // Are we practically changing the override to a most specific one? Need more testing...
                        mpOverride.ManagementPackOverride.Context = contextClasses[contextClasses.Count - 1];
                         
                        mpOverride.ManagementPackOverride.ContextInstance = context.Id;

                    }
                    else
                    {
                        mpOverride.ManagementPackOverride.ContextInstance = null;
                    }

                    mpOverride.ManagementPackOverride.Status = ManagementPackElementStatus.PendingUpdate;

                    mpOverride.ManagementPackOverride.GetManagementPack().AcceptChanges();
                }
            }
            catch (Microsoft.EnterpriseManagement.Common.ManagementPackException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        //---------------------------------------------------------------------
        private void CreateColumns()
        {
            Columns.Clear();

            if (m_columnConfig == OverrideListViewColumnConfig.InstanceColumns)
            {
                Columns.Add("Object Name", 200);
            }

            if (m_columnConfig == OverrideListViewColumnConfig.WorkflowColumns ||
                m_columnConfig == OverrideListViewColumnConfig.InstanceColumns)
            {
                Columns.Add("Workflow name",140);
                Columns.Add("Workflow type",100);
            }

            Columns.Add("Parameter",144);
            Columns.Add("New Value", 114);
            Columns.Add("Creation Time",128);
            Columns.Add("Target",142);
            Columns.Add("Is Enforced",70);
            Columns.Add("Management Pack",130);
            Columns.Add("Sealed MP", 60);
            Columns.Add("Description", 130);
        }

        //---------------------------------------------------------------------
        private void CreateContextMenu()
        {            
            ContextMenuStrip menuStrip = new ContextMenuStrip();

            menuStrip.Items.Add("Change Target", null, changeTargetMenuItem_Click);
            menuStrip.Items.Add("Delete", null, deleteOverrideMenuItem_Click);
            menuStrip.Items.Add("Move to different MP", null, moveToDifferentMPMenuItem_Click);

            if (m_columnConfig == OverrideListViewColumnConfig.InstanceColumns)
            {
                menuStrip.Items.Add("Disable grouping of workflows", null, groupWorkflows_Click);
            }


            ContextMenuStrip = menuStrip;
        }        
    }

    public class ListViewColumnSorter : IComparer
    {
        
        private int ColumnToSort;
        private SortOrder OrderOfSort;
        private CaseInsensitiveComparer ObjectCompare;

        //---------------------------------------------------------------------
        public ListViewColumnSorter()
        {
            ColumnToSort    = 0;        
            OrderOfSort     = SortOrder.None;        
            ObjectCompare   = new CaseInsensitiveComparer();
        }

        //---------------------------------------------------------------------
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

            if (OrderOfSort == SortOrder.Ascending)
            {
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                return (-compareResult);
            }
            else
            {
                return 0;
            }
        }

        //---------------------------------------------------------------------        
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        //---------------------------------------------------------------------        
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }
    }
}
