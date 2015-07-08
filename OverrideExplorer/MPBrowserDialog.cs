using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OverrideExplorer
{
    public partial class MPBrowserDialog : Form
    {
        ManagementGroup m_managementGroup;

        //---------------------------------------------------------------------
        public MPBrowserDialog(ManagementGroup managementGroup)
        {
            InitializeComponent();

            m_managementGroup = managementGroup;
        }

        //---------------------------------------------------------------------
        private void MPBrowserDialog_Load(object sender, EventArgs e)
        {
            ReadOnlyCollection<ManagementPack> managementPacks;
            ManagementPackCriteria             criteria = new ManagementPackCriteria("Sealed=0");

            try
            {
                managementPacks = m_managementGroup.GetManagementPacks(criteria);
            }
            catch (Microsoft.EnterpriseManagement.Common.ServerDisconnectedException)
            {
                m_managementGroup.Reconnect();

                managementPacks = m_managementGroup.GetManagementPacks(criteria);
            }

            lstManagementPacks.BeginUpdate();

            foreach (ManagementPack mp in managementPacks)
            {
                ListViewItem listViewItem = new ListViewItem();

                listViewItem.Text   = Common.GetBestManagementPackName(mp);              
                listViewItem.Tag    = mp;
                
                listViewItem.SubItems.Add(mp.Version.ToString());

                lstManagementPacks.Items.Add(listViewItem);

                lstManagementPacks.Sort();
            }

            lstManagementPacks.EndUpdate();
        }

        //---------------------------------------------------------------------
        internal ManagementPack ManagementPack
        {
            get
            {
                Debug.Assert(lstManagementPacks.SelectedItems.Count == 1);
                return ((ManagementPack)lstManagementPacks.SelectedItems[0].Tag);
            }
        }

        //---------------------------------------------------------------------
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstManagementPacks.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one management pack");
                DialogResult = DialogResult.None;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }

        //---------------------------------------------------------------------
        private void lstManagementPacks_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = true;
        }
    }
}