using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
using System.Collections.ObjectModel;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Diagnostics;

namespace OverrideExplorer
{
    public partial class OverrideTargetEditor : Form
    {
        ManagementGroup                             m_managementGroup;
        ManagementPackClass                         m_managementPackClass;
        Guid?                                       m_overrideContextInstance;
        ReadOnlyCollection<MonitoringObjectGroup>   m_groups;
        ReadOnlyCollection<PartialMonitoringObject> m_monitoringObjects;
        PartialMonitoringObject                     m_currentMonitoringObject;
        bool                                        m_isContextCurrentlyGroup;
        
        //---------------------------------------------------------------------
        public OverrideTargetEditor(
            ManagementGroup             managementGroup,
            ManagementPackClass         managementPackClass,
            Guid?                       overrideContextInstance
            )
        {
            InitializeComponent();

            m_managementGroup           = managementGroup;
            m_managementPackClass       = managementPackClass;
            m_overrideContextInstance   = overrideContextInstance;
            m_isContextCurrentlyGroup   = false;
        }

        //---------------------------------------------------------------------
        private void groupsComboBox_DropDown(object sender, EventArgs e)
        {
            if (m_groups == null)
            {
                m_groups = m_managementGroup.GetRootMonitoringObjectGroups();

                groupsComboBox.Items.Clear();

                foreach (MonitoringObjectGroup group in m_groups)
                {
                    groupsComboBox.Items.Add(group);
                }
            }

            if (m_currentMonitoringObject != null && m_isContextCurrentlyGroup == true)
            {
                groupsComboBox.SelectedItem = m_currentMonitoringObject;
            }
        }

        //---------------------------------------------------------------------
        private void instancesComboBox_DropDown(object sender, EventArgs e)
        {
            if (m_monitoringObjects == null)
            {
                MonitoringClass monitoringClass = m_managementGroup.GetMonitoringClass(m_managementPackClass.Id);

                m_monitoringObjects = m_managementGroup.GetPartialMonitoringObjects(monitoringClass);

                instancesComboBox.Items.Clear();

                foreach (PartialMonitoringObject monitoringObject in m_monitoringObjects)
                {
                    instancesComboBox.Items.Add(monitoringObject);
                }

                if (m_currentMonitoringObject != null && m_isContextCurrentlyGroup == false)
                {
                    instancesComboBox.SelectedItem = m_currentMonitoringObject;
                }
            }
        }

        //---------------------------------------------------------------------
        private void overrideTargetEditor_Load(object sender, EventArgs e)
        {
            if (m_overrideContextInstance == null)
            {
                allInstancesRadioButton.Checked = true;
            }
            else
            {
                PartialMonitoringObject             monitoringObject;

                monitoringObject = m_managementGroup.GetPartialMonitoringObject((Guid)m_overrideContextInstance);
                    
                m_isContextCurrentlyGroup = IsGroup(monitoringObject);

                if (m_isContextCurrentlyGroup)
                {
                    groupRadioButton.Checked = true;
                    groupsComboBox.Items.Clear();
                    groupsComboBox.Items.Add(monitoringObject);
                    groupsComboBox.SelectedItem = monitoringObject;
                }
                else
                {
                    instanceRadioButton.Checked = true;
                    instancesComboBox.Items.Clear();
                    instancesComboBox.Items.Add(monitoringObject);
                    groupsComboBox.SelectedItem = monitoringObject;
                }

                m_currentMonitoringObject = monitoringObject;
            }
        }

        //---------------------------------------------------------------------
        private void AdjustComboBoxesEnabledProperty()
        {
            if (allInstancesRadioButton.Checked)
            {
                groupsComboBox.Enabled      = false;
                instancesComboBox.Enabled   = false;
            }
            if (groupRadioButton.Checked)
            {
                groupsComboBox.Enabled      = true;
                instancesComboBox.Enabled   = false;
            }
            if (instanceRadioButton.Checked)
            {
                groupsComboBox.Enabled      = false;
                instancesComboBox.Enabled   = true;
            }
        }

        //---------------------------------------------------------------------
        private void radiobutton_CheckChanged(object sender, EventArgs e)
        {
            AdjustComboBoxesEnabledProperty();
        }

        //---------------------------------------------------------------------
        private bool IsGroup(PartialMonitoringObject monitoringObject)
        {
            ReadOnlyCollection<MonitoringClass> monitoringObjectClasses;
            MonitoringClass groupClass;
            bool            isGroup = false;
                
            groupClass = m_managementGroup.GetMonitoringClass(SystemMonitoringClass.Group);

            monitoringObjectClasses = monitoringObject.GetMonitoringClasses();

            foreach (MonitoringClass monitoringClass in monitoringObjectClasses)
            {
                if (monitoringClass.Id == groupClass.Id)
                {
                    isGroup = true;
                }
            }

            isGroup = (m_isContextCurrentlyGroup || (monitoringObjectClasses.Count == 1 && monitoringObjectClasses[0].Singleton));

            return (isGroup);
        }

        //---------------------------------------------------------------------
        internal bool IsInstanceContext
        {
            get
            {
                return (!allInstancesRadioButton.Checked);
            }
        }

        //---------------------------------------------------------------------
        internal PartialMonitoringObject InstanceContext
        {
            get
            {
                Debug.Assert(IsInstanceContext);

                if (groupRadioButton.Checked)
                {
                    return ((PartialMonitoringObject)groupsComboBox.SelectedItem);
                }
                else
                {
                    return ((PartialMonitoringObject)instancesComboBox.SelectedItem);
                }
            }
        }

        //---------------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            
            if (groupRadioButton.Checked)
            {
                if (groupsComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select a group as the override target");
                    DialogResult = DialogResult.None;
                }
            }

            if (instanceRadioButton.Checked)
            {
                if (instancesComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Please select an instance as the override target");
                    DialogResult = DialogResult.None;
                }
            }            
        }
    }
}