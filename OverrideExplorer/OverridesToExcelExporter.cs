using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement;
using System.IO;
using System.Data;

namespace OverrideExplorer
{
    internal class OverridesToExcelExporter
    {
        Dictionary<Guid, MP>    m_managementPackCollection;
        ManagementGroup         m_managementGroup;
        StringBuilder           m_contents;

        //---------------------------------------------------------------------
        internal OverridesToExcelExporter(
            Dictionary<Guid, MP> mpCollection,
            ManagementGroup      managementGroup
            )
        {
            m_managementPackCollection = mpCollection;
            m_managementGroup = managementGroup;

            m_contents = new StringBuilder();

            GenerateHeader(managementGroup.Name);
        }


        //---------------------------------------------------------------------
        private void GenerateHeader(string managementGroupName)
        {
            m_contents.Append("<?xml version=\"1.0\"?>\n");
            m_contents.Append("<?mso-application progid=\"Excel.Sheet\"?>\n");
            m_contents.Append("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
            m_contents.Append("xmlns:o=\"urn:schemas-microsoft-com:office:office\" ");
            m_contents.Append("xmlns:x=\"urn:schemas-microsoft-com:office:excel\" ");
            m_contents.Append("xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" ");
            m_contents.Append("xmlns:html=\"http://www.w3.org/TR/REC-html40\">\n");
            m_contents.Append("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
            m_contents.Append("</DocumentProperties>");
            m_contents.Append("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">\n");
            m_contents.Append("<ProtectStructure>False</ProtectStructure>\n");
            m_contents.Append("<ProtectWindows>False</ProtectWindows>\n");
            m_contents.Append("</ExcelWorkbook>\n");

            m_contents.Append("<Styles>\n");
            m_contents.Append("<Style ss:ID=\"Default\" ss:Name=\"Normal\">\n");
            m_contents.Append("<Alignment ss:Vertical=\"Bottom\"/>\n");
            m_contents.Append("<Borders/>\n");
            m_contents.Append("<Font/>\n");
            m_contents.Append("<Interior/>\n");
            m_contents.Append("<NumberFormat/>\n");
            m_contents.Append("<Protection/>\n");
            m_contents.Append("</Style>\n");
            m_contents.Append("<Style ss:ID=\"s21\">\n");
            m_contents.Append("<Font ss:Bold=\"1\"/>\n");
            m_contents.Append("<Alignment ss:Vertical=\"Bottom\"/>\n");
            m_contents.Append("</Style>\n");
            m_contents.Append("<Style ss:ID=\"s28\">\n");
            m_contents.Append("<Borders>\n");
            m_contents.Append("<Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\n");
            m_contents.Append("<Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\n");
            m_contents.Append("<Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\n");
            m_contents.Append("<Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\n");
            m_contents.Append("</Borders>\n");
            m_contents.Append("<Font x:Family=\"Swiss\" ss:Bold=\"1\"/>\n");
            m_contents.Append("<Interior ss:Color=\"#FFCC00\" ss:Pattern=\"Solid\"/>\n");
            m_contents.Append("</Style>\n");
            m_contents.Append("</Styles>\n");

            //start the sheet and table
            m_contents.AppendFormat(@"<Worksheet ss:Name=""Overrides - {0}"">", managementGroupName);
            m_contents.Append("<Table>");
        }


        //---------------------------------------------------------------------
        internal void WriteTableHeader()
        {
            m_contents.AppendLine("<ss:Column ss:Width=\"80\"/>");
            m_contents.AppendLine("<Row>");
            
            //hard-coded list of columns we need
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Management Pack");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Class");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Workflow Type");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Workflow Name");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Override Parameter");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Override Value");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Context");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Is Enforced");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Is Sealed");
            m_contents.AppendFormat(@"<Cell ss:StyleID=""s28""><Data ss:Type=""String"">{0}</Data></Cell>", "Time Added");

            m_contents.AppendLine("</Row>");
        }



        //---------------------------------------------------------------------
        private void WriteTableRows(MP mp)
        {
            foreach (var mpClass in mp.Classes)
            {
                foreach (var workflowPair in mpClass.Value.MPWorkflows)
                {
                    // this is a piece which will be used on each row pertaining to this workflow
                    string workflowType = string.Empty;

                    if (workflowPair.Value.Workflow is ManagementPackMonitor)
                    {
                        workflowType = "Monitor";
                    }
                    else if (workflowPair.Value.Workflow is ManagementPackRule)
                    {
                        workflowType = "Rule";
                    }
                    else if (workflowPair.Value.Workflow is ManagementPackDiscovery)
                    {
                        workflowType = "Discovery";
                    }
                    else
                    {
                        throw new ApplicationException("Unknown element type");
                    }

                    // this is a piece which will be used on each row pertaining to this workflow
                    string workflowName = Common.GetBestElementName(workflowPair.Value.Workflow);

                    foreach (ManagementPackOverride mpOverride in workflowPair.Value.Overrides)
                    {
                        string parameter;
                        string newValue;

                        Common.RetrieveParameterInfo(mpOverride, out parameter, out newValue);

                        // let's start outputting the row
                        m_contents.AppendLine("<Row>");

                        // CDATA added dmuscett march 8th 2012 to MPViewer because Exchange MP contains invalid characters such as "<" and ">" and that was breaking the XML file format
                        // it is safe to leave it here too :-)
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", mp.Name); //"Management Pack"

                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", mp.Name); //"Class"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", workflowType); //"Workflow Type"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", workflowName); //"Workflow Name"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", parameter); //"Override Parameter"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", newValue); //"Override Value"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", Common.RetrieveContext(mpOverride, m_managementGroup)); //"Context"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", mpOverride.Enforced.ToString()); //"Is Enforced"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", Common.GetManagementPackSealedFlag(mpOverride.GetManagementPack())); //"Is Sealed"
                        m_contents.AppendFormat(@"<Cell><Data ss:Type=""String""><![CDATA[{0}]]></Data></Cell>", mpOverride.TimeAdded.ToLocalTime().ToString()); //"Time Added"

                        m_contents.AppendLine("</Row>");

                    }
                }
            }
        }



        //---------------------------------------------------------------------
        internal void WriteOverridesToExcel(MP mp)
        {
            WriteTableRows(mp);
        }


        //---------------------------------------------------------------------
        internal void FinalizeTableAndWorksheet()
        {
            m_contents.Append("</Table>");
            m_contents.AppendFormat("</Worksheet>");
        }


        //---------------------------------------------------------------------
        internal void SaveToFile(string filePath)
        {
            m_contents.Append("</Workbook>");

            TextWriter tw = new StreamWriter(filePath);

            tw.Write(m_contents.ToString());

            tw.Close();
        }
    }
}
