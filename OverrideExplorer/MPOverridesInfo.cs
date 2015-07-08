using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Configuration;
using System.Collections.ObjectModel;
using Microsoft.EnterpriseManagement.Monitoring;

namespace OverrideExplorer
{
    class MPOverridesInfo
    {
        ManagementGroup                             m_managementGroup;
        Dictionary<Guid, MP>                        m_managementPackCollection;
        Dictionary<Guid, MonitoringRule>            m_rulesCache;
        Dictionary<Guid, ManagementPackMonitor>     m_monitorsCache;
        Dictionary<Guid, MonitoringClass>           m_typesCache;
        Dictionary<Guid, List<MonitoringRule>>      m_typeToRuleMap;
        ReadOnlyCollection<PartialMonitoringObject> m_windowsComputers;
        ReadOnlyCollection<PartialMonitoringObject> m_unixComputers;


        internal delegate void OverrideLoadingProgressDelegate(int percentage,string status);

        public event OverrideLoadingProgressDelegate OverrideLoadingProgress;

        //---------------------------------------------------------------------
        internal MPOverridesInfo(ManagementGroup managementGroup)
        {
            m_managementGroup = managementGroup;
        }

        //---------------------------------------------------------------------
        private void CacheMonitors()
        {
            ReadOnlyCollection<ManagementPackMonitor> monitors;

            monitors = m_managementGroup.GetMonitors(new MonitorCriteria("HasNonCategoryOverride=1"));

            m_monitorsCache = new Dictionary<Guid, ManagementPackMonitor>(monitors.Count);

            foreach (ManagementPackMonitor monitor in monitors)
            {
                m_monitorsCache.Add(monitor.Id, monitor);
            }
        }

        //---------------------------------------------------------------------
        private void CacheRules()
        {
            ReadOnlyCollection<MonitoringRule> rules;

            rules = m_managementGroup.GetMonitoringRules(new MonitoringRuleCriteria("HasNonCategoryOverride=1"));

            m_rulesCache    = new Dictionary<Guid, MonitoringRule>(rules.Count);
            m_typeToRuleMap = new Dictionary<Guid, List<MonitoringRule>>();

            foreach (MonitoringRule rule in rules)
            {
                m_rulesCache.Add(rule.Id, rule);

                if (m_typeToRuleMap.ContainsKey(rule.Target.Id))
                {
                    m_typeToRuleMap[rule.Target.Id].Add(rule);
                }
                else
                {
                    m_typeToRuleMap.Add(rule.Target.Id, new List<MonitoringRule>());
                    m_typeToRuleMap[rule.Target.Id].Add(rule);
                }
            }
        }

        //---------------------------------------------------------------------
        private void CacheTypes()
        {
            ReadOnlyCollection<MonitoringClass> types;

            types = m_managementGroup.GetMonitoringClasses();

            m_typesCache = new Dictionary<Guid, MonitoringClass>(types.Count);

            foreach (MonitoringClass type in types)
            {
                m_typesCache.Add(type.Id, type);
            }
        }

        //---------------------------------------------------------------------
        private void CacheWindowsComputerObjects()
        {
            MonitoringClass windowsComputerClass;
            
            windowsComputerClass = m_managementGroup.GetMonitoringClass(SystemMonitoringClass.WindowsComputer);

            m_windowsComputers = m_managementGroup.GetPartialMonitoringObjects(windowsComputerClass);
        }

        //---------------------------------------------------------------------
        private void CacheUnixComputerObjects()
        {
            MonitoringClass unixComputerClass;

            unixComputerClass = m_managementGroup.GetMonitoringClass(SystemMonitoringClass.UnixComputer);

            m_unixComputers = m_managementGroup.GetPartialMonitoringObjects(unixComputerClass);
        }

        
        //---------------------------------------------------------------------
        internal void LoadOverrides()
        {
            ReadOnlyCollection<ManagementPackOverride> overrides;

            OverrideLoadingProgress(0, "Loading rules");

            CacheRules();

            OverrideLoadingProgress(30, "Loading monitors");

            CacheMonitors();

            OverrideLoadingProgress(40, "Loading types");

            CacheTypes();

            OverrideLoadingProgress(60, "Loading computers");

            CacheWindowsComputerObjects();
            CacheUnixComputerObjects();

            OverrideLoadingProgress(80, "Loading overrides");

            overrides = m_managementGroup.GetMonitoringOverrides();

            m_managementPackCollection = new Dictionary<Guid, MP>();

            foreach (ManagementPackOverride mpOverride in overrides)
            {
                LoadOverrideInfo(mpOverride);
            }

            OverrideLoadingProgress(100, "Done");
        }

        //---------------------------------------------------------------------
        private void LoadOverrideInfo(ManagementPackOverride mpOverride)
        {
            try
            {
                ManagementPack mp;
                ManagementPackClass mpClass;

                if (mpOverride is ManagementPackMonitorOverride)
                {
                    ManagementPackMonitorOverride   monitorOverride = (ManagementPackMonitorOverride)mpOverride;
                    ManagementPackMonitor           monitor;

                    if (!m_monitorsCache.ContainsKey(monitorOverride.Monitor.Id))
                    {
                        return;
                    }

                    monitor = m_monitorsCache[monitorOverride.Monitor.Id];

                    //The class to which the monitor is targeted
                    mpClass = m_typesCache[monitor.Target.Id];

                    //The MP in which the monitor is defined
                    mp = monitor.GetManagementPack();

                    if (!m_managementPackCollection.ContainsKey(mp.Id))
                    {
                        m_managementPackCollection.Add(mp.Id, new MP(mp));
                    }

                    m_managementPackCollection[mp.Id].AddOverride(mpClass, monitor, monitorOverride);

                }
                else if (mpOverride is ManagementPackRuleOverride)
                {
                    ManagementPackRuleOverride  ruleOverride = (ManagementPackRuleOverride)mpOverride;
                    ManagementPackRule          rule;

                    if (!m_rulesCache.ContainsKey(ruleOverride.Rule.Id))
                    {
                        return;
                    }

                    rule = m_rulesCache[ruleOverride.Rule.Id];

                    //The class to which the rule is targeted
                    mpClass = m_typesCache[rule.Target.Id];

                    //The MP in which the monitor is defined
                    mp = rule.GetManagementPack();

                    if (!m_managementPackCollection.ContainsKey(mp.Id))
                    {
                        m_managementPackCollection.Add(mp.Id, new MP(mp));
                    }

                    m_managementPackCollection[mp.Id].AddOverride(mpClass, rule, ruleOverride);

                }
                else if (mpOverride is ManagementPackDiscoveryOverride)
                {
                    ManagementPackDiscoveryOverride discoveryOverride   = (ManagementPackDiscoveryOverride)mpOverride;
                    ManagementPackDiscovery         discovery           = discoveryOverride.Discovery.GetElement();

                    mpClass = m_typesCache[discovery.Target.Id];

                    mp = discovery.GetManagementPack();

                    if (!m_managementPackCollection.ContainsKey(mp.Id))
                    {
                        m_managementPackCollection.Add(mp.Id, new MP(mp));
                    }

                    m_managementPackCollection[mp.Id].AddOverride(mpClass, discovery, discoveryOverride);
                }

            }
            catch (Microsoft.EnterpriseManagement.Common.ObjectNotFoundException error)
            {
                //This exception will be thrown for rules/monitors/discoveries/diagnostics/recoveries that are marked "hidden"
            }
        }

        //---------------------------------------------------------------------
        internal Dictionary<Guid, MP> ManagementPacks
        {
            get
            {
                return (m_managementPackCollection);
            }
        }

        //---------------------------------------------------------------------
        internal ReadOnlyCollection<PartialMonitoringObject> WindowsComputers
        {
            get
            {
                return (m_windowsComputers);
            }
        }

        //---------------------------------------------------------------------
        internal ReadOnlyCollection<PartialMonitoringObject> UnixComputers
        {
            get
            {
                return (m_unixComputers);
            }
        }

        //---------------------------------------------------------------------
        internal Dictionary<Guid, MonitoringClass> Types
        {
            get
            {
                return (m_typesCache);
            }
        }

        //---------------------------------------------------------------------
        internal Dictionary<Guid, List<MonitoringRule>> TypeToRuleTable
        {
            get
            {
                return (m_typeToRuleMap);
            }
        }
    }
}
