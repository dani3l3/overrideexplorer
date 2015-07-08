using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace OverrideExplorer
{
    public partial class ProgressDialog : Form
    {
        //---------------------------------------------------------------------
        public ProgressDialog()
        {
            InitializeComponent();
        }

        //---------------------------------------------------------------------
        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;
            progressBar.Value   = 0;
        }

        //---------------------------------------------------------------------
        internal void UpdateProgress(
            ProgressInfo progressInfo
            )
        {
            lblCurrentOperation.Text = progressInfo.Status;
            progressBar.Value        = progressInfo.PercentageComplete;
        }
    }

    //---------------------------------------------------------------------
    class ProgressInfo
    {
        string  m_status;
        int     m_percentageComplete;

        //---------------------------------------------------------------------
        public ProgressInfo(
            string  status,
            int     percentageComplete
            )
        {
            Debug.Assert(status.Length > 0);

            m_status                = status;
            m_percentageComplete    = percentageComplete;
        }

        //---------------------------------------------------------------------
        public string Status
        {
            get
            {
                return (m_status);
            }
        }

        //---------------------------------------------------------------------
        public int PercentageComplete
        {
            get
            {
                return (m_percentageComplete);
            }
        }
    }
}