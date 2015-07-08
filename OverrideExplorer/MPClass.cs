using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EnterpriseManagement.Configuration;
using System.Collections.ObjectModel;

namespace OverrideExplorer
{
    class MPClass
    {
        Dictionary<Guid,MPWorkflow>     m_mpWorkflows;
        ManagementPackClass             m_mpClass;

        //---------------------------------------------------------------------
        internal MPClass(ManagementPackClass mpClass)
        {
            m_mpClass           = mpClass;
            m_mpWorkflows       = new Dictionary<Guid, MPWorkflow>();
        }
       
        //---------------------------------------------------------------------
        internal Dictionary<Guid, MPWorkflow> MPWorkflows
        {
            get
            {
                return (m_mpWorkflows);
            }
        }

        //---------------------------------------------------------------------
        internal string Name
        {
            get
            {
                return (Common.GetBestElementName(m_mpClass));                
            }
        }

        //---------------------------------------------------------------------
        internal int NumberOfOverrides
        {
            get
            {
                int numberOfOverrides = 0;

                foreach (MPWorkflow workflow in m_mpWorkflows.Values)
                {
                    numberOfOverrides += workflow.Overrides.Count;
                }

                return (numberOfOverrides);
            }
        }

        //---------------------------------------------------------------------
        internal ManagementPackClass ManagementPackClass
        {
            get
            {
                return (m_mpClass);
            }
        }

        //---------------------------------------------------------------------
        internal void AddOverride(
            ManagementPackOverride  mpOverride,
            ManagementPackElement   overridenElement
            )
        {
            if (!m_mpWorkflows.ContainsKey(overridenElement.Id))
            {
                m_mpWorkflows.Add(overridenElement.Id, new MPWorkflow(overridenElement));
            }

            m_mpWorkflows[overridenElement.Id].AddOverride(mpOverride);
        }
    }
}
