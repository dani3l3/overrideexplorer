using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EnterpriseManagement.Monitoring;
using System.Collections.ObjectModel;
using Microsoft.EnterpriseManagement.Common;


namespace OverrideExplorer
{
    class Common
    {
        //---------------------------------------------------------------------
        internal static string RetrieveContext(
            ManagementPackOverride mpOverrride,
            ManagementGroup        managementGroup
            )
        {
            string context = "Unknown";

            if (mpOverrride.ContextInstance == null)
            {
                string className;

                className = GetBestElementName(managementGroup.GetMonitoringClass(mpOverrride.Context.Id));

                context = string.Format("All Instances of type {0}", className);
            }
            else
            {
                PartialMonitoringObject monitoringObject;

                try
                {
                    try
                    {
                        monitoringObject = managementGroup.GetPartialMonitoringObject((Guid)mpOverrride.ContextInstance);
                        context = monitoringObject.DisplayName;
                    }
                    catch (Microsoft.EnterpriseManagement.Common.MonitoringException exception)
                    {
                        managementGroup.Reconnect();
                        monitoringObject = managementGroup.GetMonitoringObject((Guid)mpOverrride.ContextInstance);
                    }
                }
                catch (Microsoft.EnterpriseManagement.Common.MonitoringException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }

            return (context);
        }

        //---------------------------------------------------------------------
        internal static void RetrieveParameterInfo(
            ManagementPackOverride      mpOverrride,
            out string                  parameter,
            out string                  newValue
            )
        {
            parameter   = string.Empty;
            newValue    = string.Empty;

            if (mpOverrride is ManagementPackMonitorConfigurationOverride)
            {
                ManagementPackMonitorConfigurationOverride monitorConfigOverride = (ManagementPackMonitorConfigurationOverride)mpOverrride;

                parameter   = monitorConfigOverride.Parameter;
                newValue    = monitorConfigOverride.Value;
            }
            else if (mpOverrride is ManagementPackMonitorPropertyOverride)
            {
                ManagementPackMonitorPropertyOverride monitorPropertyOverride = (ManagementPackMonitorPropertyOverride)mpOverrride;

                parameter   = monitorPropertyOverride.Property.ToString();
                newValue    = monitorPropertyOverride.Value;
            }
            else if (mpOverrride is ManagementPackRuleConfigurationOverride)
            {
                ManagementPackRuleConfigurationOverride ruleConfigOverride = (ManagementPackRuleConfigurationOverride)mpOverrride;

                parameter   = ruleConfigOverride.Parameter;
                newValue    = ruleConfigOverride.Value;
            }
            else if (mpOverrride is ManagementPackRulePropertyOverride)
            {
                ManagementPackRulePropertyOverride rulePropertyOverride = (ManagementPackRulePropertyOverride)mpOverrride;

                parameter = rulePropertyOverride.Property.ToString();
                newValue = rulePropertyOverride.Value;
            }
            else if (mpOverrride is ManagementPackDiscoveryConfigurationOverride)
            {
                ManagementPackDiscoveryConfigurationOverride discoveryConfigurationOverride = (ManagementPackDiscoveryConfigurationOverride)mpOverrride;

                parameter   = discoveryConfigurationOverride.Parameter;
                newValue    = discoveryConfigurationOverride.Value;
            }
            else if (mpOverrride is ManagementPackDiscoveryPropertyOverride)
            {
                ManagementPackDiscoveryPropertyOverride discoveryPropertyOverride = (ManagementPackDiscoveryPropertyOverride)mpOverrride;

                parameter   = discoveryPropertyOverride.Property.ToString();
                newValue    = discoveryPropertyOverride.Value;
            }
            else
            {
                throw new ApplicationException("Unknown override type");
            }

        }       

        //---------------------------------------------------------------------
        private static List<MonitoringDiscoveryResultantOverrideSet> RetrieveDiscoveryOverrides(
            PartialMonitoringObject monitoringObject
            )
        {
            ReadOnlyCollection<MonitoringDiscovery>         discoveries;
            List<MonitoringDiscoveryResultantOverrideSet>   listOfOverrides;
            
            listOfOverrides = new List<MonitoringDiscoveryResultantOverrideSet>();

            discoveries = monitoringObject.GetMonitoringDiscoveries(new MonitoringDiscoveryCriteria("HasNonCategoryOverride=1"));

            foreach (MonitoringDiscovery discovery in discoveries)
            {
                listOfOverrides.Add(monitoringObject.GetResultantOverrides(discovery));
            }

            return (listOfOverrides);
        }

        //---------------------------------------------------------------------
        internal static List<MPOverride> RetrieveRuleOverrides(
            PartialMonitoringObject                 monitoringObject,
            Dictionary<Guid,MonitoringClass>        typeLookupTable,
            Dictionary<Guid,List<MonitoringRule>>   typeToMonitoringRulesTable
            )
        {
            List<MonitoringRule> rules = new List<MonitoringRule>();            

            string classesGuids = string.Empty;

            foreach (MonitoringClass monitoringClass in monitoringObject.GetMonitoringClasses())
            {
                if (typeToMonitoringRulesTable.ContainsKey(monitoringClass.Id))
                {
                    rules.AddRange(typeToMonitoringRulesTable[monitoringClass.Id]);
                }
            }           

            return GenerateEffectiveOverrrideList(monitoringObject,
                                                  new ReadOnlyCollection<MonitoringRule>(rules),
                                                  typeLookupTable);
        }

        //---------------------------------------------------------------------
        internal static List<MPOverride> RetrieveMonitorOverrides(
            PartialMonitoringObject                 monitoringObject,
            Dictionary<Guid, MonitoringClass>       typeLookupTable
            )
        {
            List<ManagementPackMonitor> monitors = new List<ManagementPackMonitor>();

            RetrieveApplicableMonitors(monitoringObject, monitors, null);

            return GenerateEffectiveOverrrideList(monitoringObject,
                                                  new ReadOnlyCollection<ManagementPackMonitor>(monitors),
                                                  typeLookupTable);
        }

        //---------------------------------------------------------------------
        internal static List<MPOverride> RetrieveDiscoveryOverrides(
            PartialMonitoringObject monitoringObject,
            Dictionary<Guid, MonitoringClass> typeLookupTable
            )
        {
            return GenerateEffectiveOverrrideList(monitoringObject,
                                                  monitoringObject.GetMonitoringDiscoveries(new MonitoringDiscoveryCriteria("HasNonCategoryOverride=1")),
                                                  typeLookupTable);

        }

        //---------------------------------------------------------------------
        private static void RetrieveApplicableMonitors(
            PartialMonitoringObject                         monitoringObject,
            List<ManagementPackMonitor>                     monitors,
            MonitoringHierarchyNode<ManagementPackMonitor>  hierarchyNode
            )
        {
            if (hierarchyNode == null)
            {
                hierarchyNode = monitoringObject.GetMonitorHierarchy();
                monitors.Add(hierarchyNode.Item);
            }

            foreach (MonitoringHierarchyNode<ManagementPackMonitor> node in hierarchyNode.ChildNodes)
            {
                monitors.Add(node.Item);

                RetrieveApplicableMonitors(monitoringObject, monitors, node);
            }
        }

        //---------------------------------------------------------------------
        private static List<MPOverride> GenerateEffectiveOverrrideList(
            PartialMonitoringObject                 monitoringObject,
            ReadOnlyCollection<MonitoringRule>      rules,
            Dictionary<Guid, MonitoringClass>       typeLookupTable
            )
        {
            List<MPOverride> overrides = new List<MPOverride>();

            foreach (MonitoringRule rule in rules)
            {
                MonitoringRuleResultantOverrideSet overrideSet = monitoringObject.GetResultantOverrides(rule);

                MPWorkflow workflow = new MPWorkflow(rule);

                foreach (ResultantOverride<MonitoringRuleConfigurationOverride> ruleOverride in overrideSet.ResultantConfigurationOverrides.Values)
                {
                    MonitoringClass target = typeLookupTable[rule.Target.Id];

                    workflow.AddOverride(ruleOverride.EffectiveOverride);

                    MPOverride mpOverride = new MPOverride(ruleOverride.EffectiveOverride,
                                                           workflow,
                                                           new MPClass(target));
                    overrides.Add(mpOverride);
                }

                foreach (ResultantOverride<ManagementPackOverride> ruleOverride in overrideSet.ResultantPropertyOverrides.Values)
                {
                    MonitoringClass target = typeLookupTable[rule.Target.Id];

                    workflow.AddOverride(ruleOverride.EffectiveOverride);

                    MPOverride mpOverride = new MPOverride(ruleOverride.EffectiveOverride,
                                                           workflow,
                                                           new MPClass(target));
                    overrides.Add(mpOverride);
                }
            }

            return (overrides);
        }

        //---------------------------------------------------------------------
        private static List<MPOverride> GenerateEffectiveOverrrideList(
            PartialMonitoringObject                     monitoringObject,
            ReadOnlyCollection<ManagementPackMonitor>   monitors,
            Dictionary<Guid, MonitoringClass>           typeLookupTable
            )
        {
            List<MPOverride> overrides = new List<MPOverride>();

            foreach (ManagementPackMonitor monitor in monitors)
            {
                // if we do this check which I commented out, it only works for Windows computer, not for linux ones...
                // I am not sure why the check was done in the first place... what was it preventing from doing?
                // was it just an optimization to speed things up, or was it protecting the UI from dealing with some weird monitors?

                //if (!DoesMonitorHaveNonCategoryOverride(monitor))
                //{
                //    continue;
                //}

                MonitorResultantOverrideSet overrideSet = monitoringObject.GetResultantOverrides(monitor);

                MPWorkflow workflow = new MPWorkflow(monitor);

                foreach (ResultantOverride<MonitorConfigurationOverride> monitorOverride in overrideSet.ResultantConfigurationOverrides.Values)
                {
                    MonitoringClass target = typeLookupTable[monitor.Target.Id];

                    workflow.AddOverride(monitorOverride.EffectiveOverride);

                    MPOverride mpOverride = new MPOverride(monitorOverride.EffectiveOverride,
                                                           workflow,
                                                           new MPClass(target));
                    overrides.Add(mpOverride);
                }

                foreach (ResultantOverride<ManagementPackOverride> monitorOverride in overrideSet.ResultantPropertyOverrides.Values)
                {
                    MonitoringClass target = typeLookupTable[monitor.Target.Id];

                    workflow.AddOverride(monitorOverride.EffectiveOverride);

                    MPOverride mpOverride = new MPOverride(monitorOverride.EffectiveOverride,
                                                           workflow,
                                                           new MPClass(target));
                    overrides.Add(mpOverride);
                }
            }

            return (overrides);
        }

        //---------------------------------------------------------------------
        private static List<MPOverride> GenerateEffectiveOverrrideList(
            PartialMonitoringObject                         monitoringObject,
            ReadOnlyCollection<MonitoringDiscovery>         discoveries,
            Dictionary<Guid, MonitoringClass>               typeLookupTable
            )
        {
            List<MPOverride> overrides = new List<MPOverride>();

            foreach (MonitoringDiscovery discovery in discoveries)
            {                
                MonitoringDiscoveryResultantOverrideSet overrideSet = monitoringObject.GetResultantOverrides(discovery);

                MPWorkflow workflow = new MPWorkflow(discovery);

                foreach (ResultantOverride<MonitoringDiscoveryConfigurationOverride> discoveryOverride in overrideSet.ResultantConfigurationOverrides.Values)
                {
                    MonitoringClass target = typeLookupTable[discovery.Target.Id];

                    workflow.AddOverride(discoveryOverride.EffectiveOverride);

                    MPOverride mpOverride = new MPOverride(discoveryOverride.EffectiveOverride,
                                                           workflow,
                                                           new MPClass(target));
                    overrides.Add(mpOverride);
                }

                foreach (ResultantOverride<ManagementPackOverride> discoveryOverride in overrideSet.ResultantPropertyOverrides.Values)
                {
                    MonitoringClass target = typeLookupTable[discovery.Target.Id];

                    workflow.AddOverride(discoveryOverride.EffectiveOverride);

                    MPOverride mpOverride = new MPOverride(discoveryOverride.EffectiveOverride,
                                                           workflow,
                                                           new MPClass(target));
                    overrides.Add(mpOverride);
                }
            }

            return (overrides);
        }

        //---------------------------------------------------------------------
        internal static bool DoesMonitorHaveNonCategoryOverride(
            ManagementPackMonitor monitor
            )
        {
            if(monitor is UnitMonitor)
            {
                return ((UnitMonitor)monitor).HasNonCategoryOverride;
            }
            else if(monitor is InternalRollupMonitor)
            {
                return ((InternalRollupMonitor)monitor).HasNonCategoryOverride;
            }
            else if (monitor is ExternalRollupMonitor)
            {
                return ((ExternalRollupMonitor)monitor).HasNonCategoryOverride;
            }
            else
            {
                throw new ApplicationException("Unknown monitor type " + monitor.GetType());
            }
        }

        //---------------------------------------------------------------------
        internal static string GetBestElementName(
            ManagementPackElement element
            )
        {
            if (element.DisplayName != null && element.DisplayName.Length > 0)
            {
                return (element.DisplayName);
            }
            else
            {
                return (element.Name);
            }
        }

        //---------------------------------------------------------------------
        internal static string GetBestMonitoringObjectName(
            PartialMonitoringObject monitoringObject
            )
        {
            if (monitoringObject.DisplayName != null && monitoringObject.DisplayName.Length > 0)
            {
                return (monitoringObject.DisplayName);
            }
            else
            {
                return (monitoringObject.Name);
            }
        }

        //---------------------------------------------------------------------
        internal static string GetBestManagementPackName(
            ManagementPack managementPack
            )
        {
            if (managementPack.DisplayName != null && managementPack.DisplayName.Length > 0)
            {
                return (managementPack.DisplayName);
            }
            else
            {
                return (managementPack.Name);
            }
        }


        internal static string GetManagementPackSealedFlag(
         ManagementPack managementPack
         )
        {
                return managementPack.Sealed.ToString();
        }
 
    
    }
}
