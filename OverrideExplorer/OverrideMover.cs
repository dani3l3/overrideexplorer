using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Monitoring;

namespace OverrideExplorer
{
    class OverrideMover
    {
        ManagementPack          m_sourceMp;
        ManagementPack          m_targetMp;
        ManagementPackOverride  m_mpOverride;
        MPWorkflow              m_mpWorkflow;

        //---------------------------------------------------------------------
        internal OverrideMover(
            ManagementPack          sourceMp,
            ManagementPack          targetMp,
            ManagementPackOverride  mpOverride,
            MPWorkflow              mpWorkflow
            )
        {
            m_mpOverride    = mpOverride;
            m_sourceMp      = sourceMp;
            m_targetMp      = targetMp;
            m_mpWorkflow    = mpWorkflow;
        }

        //---------------------------------------------------------------------
        internal ManagementPackOverride PerformMove()
        {
            ManagementPackOverride targetOverride;

            AddDependencyIfNeeded();

            CreateTargetOverride(out targetOverride);

            m_mpOverride.Status = ManagementPackElementStatus.PendingDelete;

            // dmuscett 2012-03-02 first save the NEW one, THEN delete the old one.
            // this can lead to duplicates, but at least prevents the issue where the old one is 
            // removed and the new one fails to apply, effectively deleting it...
            m_targetMp.AcceptChanges();
            m_sourceMp.AcceptChanges();    


            return (targetOverride);
        }

        //---------------------------------------------------------------------
        private void CreateTargetOverride(
            out ManagementPackOverride  targetOverride
            )
        {
            targetOverride = null;

            if (m_mpOverride is ManagementPackMonitorConfigurationOverride)
            {
                ManagementPackMonitorConfigurationOverride sourceMonitorConfigOverride = (ManagementPackMonitorConfigurationOverride)m_mpOverride;
                ManagementPackMonitorConfigurationOverride targetMonitorConfigOverride = new ManagementPackMonitorConfigurationOverride(m_targetMp, m_mpOverride.Name);

                targetMonitorConfigOverride.Monitor     = sourceMonitorConfigOverride.Monitor;
                targetMonitorConfigOverride.Parameter   = sourceMonitorConfigOverride.Parameter;
                targetOverride                          = targetMonitorConfigOverride;

            }
            else if (m_mpOverride is ManagementPackMonitorPropertyOverride)
            {
                ManagementPackMonitorPropertyOverride sourceMonitorPropertyOverride = (ManagementPackMonitorPropertyOverride)m_mpOverride;
                ManagementPackMonitorPropertyOverride targetMonitorPropertyOverride = new ManagementPackMonitorPropertyOverride(m_targetMp, m_mpOverride.Name);

                targetMonitorPropertyOverride.Monitor   = sourceMonitorPropertyOverride.Monitor;
                targetMonitorPropertyOverride.Property  = sourceMonitorPropertyOverride.Property;
                targetOverride                          = targetMonitorPropertyOverride;

            }
            else if (m_mpOverride is ManagementPackRuleConfigurationOverride)
            {
                ManagementPackRuleConfigurationOverride sourceRuleConfigOverride = (ManagementPackRuleConfigurationOverride)m_mpOverride;
                ManagementPackRuleConfigurationOverride targetRuleConfigOverride = new ManagementPackRuleConfigurationOverride(m_targetMp, m_mpOverride.Name);

                targetRuleConfigOverride.Module     = sourceRuleConfigOverride.Module;
                targetRuleConfigOverride.Parameter  = sourceRuleConfigOverride.Parameter;
                targetRuleConfigOverride.Rule       = sourceRuleConfigOverride.Rule;
                targetOverride                      = targetRuleConfigOverride;
            }
            else if (m_mpOverride is ManagementPackRulePropertyOverride)
            {
                ManagementPackRulePropertyOverride sourceRulePropertyOverride = (ManagementPackRulePropertyOverride)m_mpOverride;
                ManagementPackRulePropertyOverride targetRulePropertyOverride = new ManagementPackRulePropertyOverride(m_targetMp, m_mpOverride.Name);

                targetRulePropertyOverride.Property = sourceRulePropertyOverride.Property;
                targetRulePropertyOverride.Rule     = sourceRulePropertyOverride.Rule;
                targetOverride                      = targetRulePropertyOverride;

            }
            else if (m_mpOverride is ManagementPackDiscoveryConfigurationOverride)
            {
                ManagementPackDiscoveryConfigurationOverride sourceDiscoveryConfigurationOverride = (ManagementPackDiscoveryConfigurationOverride)m_mpOverride;
                ManagementPackDiscoveryConfigurationOverride targetDiscoveryConfigurationOverride = new ManagementPackDiscoveryConfigurationOverride(m_targetMp, m_mpOverride.Name);

                targetDiscoveryConfigurationOverride.Discovery  = sourceDiscoveryConfigurationOverride.Discovery;
                targetDiscoveryConfigurationOverride.Module     = sourceDiscoveryConfigurationOverride.Module;
                targetDiscoveryConfigurationOverride.Parameter  = sourceDiscoveryConfigurationOverride.Parameter;
                targetOverride                                  = targetDiscoveryConfigurationOverride;

            }
            else if (m_mpOverride is ManagementPackDiscoveryPropertyOverride)
            {
                ManagementPackDiscoveryPropertyOverride sourceDiscoveryPropertyOverride = (ManagementPackDiscoveryPropertyOverride)m_mpOverride;
                ManagementPackDiscoveryPropertyOverride targetDiscoveryPropertyOverride = new ManagementPackDiscoveryPropertyOverride(m_targetMp, m_mpOverride.Name);

                targetDiscoveryPropertyOverride.Discovery   = sourceDiscoveryPropertyOverride.Discovery;
                targetDiscoveryPropertyOverride.Property    = sourceDiscoveryPropertyOverride.Property;
                targetOverride                              = targetDiscoveryPropertyOverride;
            }
            else
            {
                throw new ApplicationException("Unknown override type");
            }

            targetOverride.Comment          = m_mpOverride.Comment;
            targetOverride.Context          = m_mpOverride.Context;
            targetOverride.ContextInstance  = m_mpOverride.ContextInstance;
            targetOverride.Description      = m_mpOverride.Description;
            
            // dmuscett 2012-03-02 in 2012 this has become mandatory, apparently....
            if (!String.IsNullOrEmpty(m_mpOverride.DisplayName))
                targetOverride.DisplayName      = m_mpOverride.DisplayName;
            else
                targetOverride.DisplayName = "";
            
            targetOverride.Enforced         = m_mpOverride.Enforced;

            targetOverride.LanguageCode     = m_mpOverride.LanguageCode;
            targetOverride.LastModified     = m_mpOverride.LastModified;

            targetOverride.TimeAdded        = m_mpOverride.TimeAdded;
            targetOverride.Value            = m_mpOverride.Value;

            targetOverride.Status           = ManagementPackElementStatus.PendingAdd;

            if (targetOverride.DisplayName == null)
            {
                targetOverride.DisplayName = m_mpOverride.Name;
            }
        }

        //---------------------------------------------------------------------
        private void AddDependencyIfNeeded()
        {
            ManagementPack  workflowMP      = m_mpWorkflow.Workflow.GetManagementPack();
            bool            addRefRequired  = true;

            if (workflowMP.Id == m_targetMp.Id)
            {
                //The workflow is defined in the same MP as the new override mp, no need to add dependency
                addRefRequired = false;
            }
            else
            {
                foreach (KeyValuePair<string, ManagementPackReference> existingMPRef in m_targetMp.References)
                {
                    if (existingMPRef.Value.KeyToken    == workflowMP.KeyToken &&
                        existingMPRef.Value.Version     == workflowMP.Version &&
                        existingMPRef.Value.Name        == workflowMP.Name)
                    {
                        addRefRequired = false;
                        break;
                    }
                }
            }

            if (addRefRequired)
            {
                ManagementPackReference mpRef = new ManagementPackReference(workflowMP);

                m_targetMp.References.Add(workflowMP.Name, mpRef);
                m_targetMp.AcceptChanges();
            }
        }
    }
}
