using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Monitoring.Security;

namespace OverrideExplorer
{
    public partial class ConnectionDialog : Form
    {
        ManagementGroup m_managementGroup;

        //---------------------------------------------------------------------
        public ConnectionDialog()
        {
            InitializeComponent();
        }

        //---------------------------------------------------------------------
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                ManagementGroupConnectionSettings settings = new ManagementGroupConnectionSettings(txtRmsServerName.Text);

                settings.CacheMode = Microsoft.EnterpriseManagement.Common.CacheMode.Configuration;

                m_managementGroup   = new ManagementGroup(settings);
                DialogResult        = DialogResult.OK;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                DialogResult = DialogResult.None;
            }            
        }

        //---------------------------------------------------------------------
        internal ManagementGroup ManagementGroup
        {
            get
            {
                return (m_managementGroup);
            }
        }
    }
}