using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement;

namespace OverrideExplorer
{
    internal class OverridesToXmlExporter
    {
        Dictionary<Guid, MP>    m_managementPackCollection;
        ManagementGroup         m_managementGroup;

        //---------------------------------------------------------------------
        internal OverridesToXmlExporter(
            Dictionary<Guid, MP> mpCollection,
            ManagementGroup      managementGroup
            )
        {
            m_managementPackCollection = mpCollection;
            m_managementGroup = managementGroup;
        }
        
        //---------------------------------------------------------------------
        internal void Export(string filePath)
        {
            XmlDocument document = new XmlDocument();
            XmlElement overridesElement;

            overridesElement = document.CreateElement("Overrides");

            document.AppendChild(overridesElement);

            foreach (KeyValuePair<Guid, MP> mpPair in m_managementPackCollection)
            {
                XmlElement mpElement = document.CreateElement("ManagementPack");

                mpElement.SetAttribute("Name", mpPair.Value.Name);

                AddClassToXmlDocument(mpPair.Value, mpElement, document);

                overridesElement.AppendChild(mpElement);
            }

            document.Save(filePath);
        }

        //---------------------------------------------------------------------
        private void AddClassToXmlDocument(MP mp, XmlElement mpElement, XmlDocument document)
        {
            foreach (KeyValuePair<Guid, MPClass> classPair in mp.Classes)
            {
                XmlElement classElement = document.CreateElement("Type");

                classElement.SetAttribute("Name", classPair.Value.Name);

                mpElement.AppendChild(classElement);

                AddWorkflowsToXmlDocument(classPair.Value, classElement, document);
            }
        }

        //---------------------------------------------------------------------
        private void AddWorkflowsToXmlDocument(MPClass mpClass, XmlElement classElement, XmlDocument document)
        {
            foreach (KeyValuePair<Guid, MPWorkflow> workflowPair in mpClass.MPWorkflows)
            {
                XmlElement workflowElement;

                if (workflowPair.Value.Workflow is ManagementPackMonitor)
                {
                    workflowElement = document.CreateElement("Monitor");
                }
                else if (workflowPair.Value.Workflow is ManagementPackRule)
                {
                    workflowElement = document.CreateElement("Rule");
                }
                else if (workflowPair.Value.Workflow is ManagementPackDiscovery)
                {
                    workflowElement = document.CreateElement("Discovery");
                }
                else
                {
                    throw new ApplicationException("Unknown element type");
                }

                workflowElement.SetAttribute("Name", Common.GetBestElementName(workflowPair.Value.Workflow));

                classElement.AppendChild(workflowElement);

                AddOverridesToXmlDocument(workflowPair.Value, workflowElement, document);
            }
        }

        //---------------------------------------------------------------------
        private void AddOverridesToXmlDocument(
            MPWorkflow mpWorkflow,
            XmlElement workflowElement,
            XmlDocument document
            )
        {
            foreach (ManagementPackOverride mpOverride in mpWorkflow.Overrides)
            {
                XmlElement  overrideElement = document.CreateElement("Override");
                string      parameter;
                string      newValue;

                Common.RetrieveParameterInfo(mpOverride, out parameter, out newValue);

                overrideElement.SetAttribute("Parameter", parameter);
                overrideElement.SetAttribute("NewValue", newValue);
                overrideElement.SetAttribute("TimeAdded", mpOverride.TimeAdded.ToLocalTime().ToString());
                overrideElement.SetAttribute("Context", Common.RetrieveContext(mpOverride, m_managementGroup));
                overrideElement.SetAttribute("IsEnforced", mpOverride.Enforced.ToString());
                overrideElement.SetAttribute("ManagementPack", mpOverride.GetManagementPack().Name);
                overrideElement.SetAttribute("IsSealed", Common.GetManagementPackSealedFlag(mpOverride.GetManagementPack()));

                workflowElement.AppendChild(overrideElement);

            }
        }
    }
}
