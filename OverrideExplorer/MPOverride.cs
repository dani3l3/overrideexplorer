using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EnterpriseManagement.Configuration;

namespace OverrideExplorer
{
    internal class MPOverride
    {
        ManagementPackOverride  m_override;
        MPWorkflow              m_workflow;
        MPClass                 m_workflowTarget;

        internal MPOverride(
            ManagementPackOverride  mpOverride,
            MPWorkflow              workflow,
            MPClass                 workflowTarget
            )
        {
            m_override          = mpOverride;
            m_workflow          = workflow;
            m_workflowTarget    = workflowTarget;
        }

        //---------------------------------------------------------------------
        internal ManagementPackOverride ManagementPackOverride
        {
            get
            {
                return (m_override);
            }
        }

        //---------------------------------------------------------------------
        internal MPWorkflow Workflow
        {
            get
            {
                return (m_workflow);
            }
        }

        //---------------------------------------------------------------------
        internal MPClass Target
        {
            get
            {
                return (m_workflowTarget);
            }
        }
    }
}
