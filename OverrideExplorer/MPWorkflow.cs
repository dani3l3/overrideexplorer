using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EnterpriseManagement.Configuration;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OverrideExplorer
{
    class MPWorkflow
    {
        List<ManagementPackOverride> m_overrides;
        ManagementPackElement        m_mpElement;

        //---------------------------------------------------------------------
        internal MPWorkflow(ManagementPackElement mpElement)
        {
            m_overrides     = new List<ManagementPackOverride>();
            m_mpElement     = mpElement;
        }

        //---------------------------------------------------------------------
        internal void AddOverride(ManagementPackOverride mpOverride)
        {
            Debug.Assert(mpOverride != null);

            m_overrides.Add(mpOverride);
        }

        //---------------------------------------------------------------------
        internal void RemoveOverride(ManagementPackOverride mpOverrideToDelete)
        {
            foreach (ManagementPackOverride mpOverride in m_overrides)
            {
                if (mpOverride.Id == mpOverrideToDelete.Id)
                {
                    m_overrides.Remove(mpOverride);
                    break;
                }
            }
        }

        //---------------------------------------------------------------------
        internal ReadOnlyCollection<ManagementPackOverride> Overrides
        {
            get
            {
                return (new ReadOnlyCollection<ManagementPackOverride>(m_overrides));
            }
        }

        //---------------------------------------------------------------------
        internal ManagementPackElement Workflow
        {
            get
            {
                return (m_mpElement);
            }
        }
    }
}
