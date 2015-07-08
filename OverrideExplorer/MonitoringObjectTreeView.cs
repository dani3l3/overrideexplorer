using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement;
using System.Collections.ObjectModel;
using Microsoft.EnterpriseManagement.Monitoring;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using System.Drawing;
using System.IO;

namespace OverrideExplorer
{
    class MonitoringObjectTreeView : TreeView
    {
        Dictionary<Guid, ReadOnlyCollection<MonitoringImageReference>>  m_classIdToMonitoringImageReferences;
        Dictionary<Guid, MonitoringImageObject>                         m_imageIdToImageMap;
        ManagementGroup                                                 m_managementGroup;
        ImageList                                                       m_imageList;

        //---------------------------------------------------------------------
        internal void Initialize(
            ManagementGroup                             managementGroup,
            ReadOnlyCollection<PartialMonitoringObject> rootWindowsMonitoringObjects,
            ReadOnlyCollection<PartialMonitoringObject> rootUnixMonitoringObjects
            )
        {
            m_managementGroup                       = managementGroup;
            m_classIdToMonitoringImageReferences    = new Dictionary<Guid, ReadOnlyCollection<MonitoringImageReference>>();
            m_imageIdToImageMap                     = new Dictionary<Guid, MonitoringImageObject>();

            LoadImageList();

            ImageList = m_imageList;

            CreateRootNodes(rootWindowsMonitoringObjects,rootUnixMonitoringObjects);
        }

        //---------------------------------------------------------------------
        private void CreateRootNodes(
            ReadOnlyCollection<PartialMonitoringObject> rootWindowsMonitoringObjects,
            ReadOnlyCollection<PartialMonitoringObject> rootUnixMonitoringObjects
            )
        {
            List<PartialMonitoringObject> monitoringObjectList = new List<PartialMonitoringObject>(rootWindowsMonitoringObjects);
            monitoringObjectList.AddRange(new List<PartialMonitoringObject>(rootUnixMonitoringObjects));

            monitoringObjectList.Sort(new MonitoringObjectComparer());

            BeginUpdate();

            foreach (PartialMonitoringObject monitoringObject in monitoringObjectList)
            {
                AddMonitoringObjectTreeNode(monitoringObject, null);
            }

            EndUpdate();
        }

        //---------------------------------------------------------------------
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes[0] is PlaceholderTreeNode)
            {
                e.Node.Nodes.Clear();

                PartialMonitoringObject parentMonitoringObject = (PartialMonitoringObject)e.Node.Tag;

                ReadOnlyCollection<PartialMonitoringObject> monitoringObjects;

                try
                {
                    monitoringObjects = parentMonitoringObject.GetRelatedPartialMonitoringObjects(TraversalDepth.OneLevel);
                }
                catch (Microsoft.EnterpriseManagement.Common.ServerDisconnectedException)
                {
                    m_managementGroup.Reconnect();

                    monitoringObjects = parentMonitoringObject.GetRelatedPartialMonitoringObjects(TraversalDepth.OneLevel);
                }

                List<PartialMonitoringObject> monitoringObjectList = new List<PartialMonitoringObject>(monitoringObjects);
                               
                monitoringObjectList.Sort(new MonitoringObjectComparer());

                BeginUpdate();

                foreach (PartialMonitoringObject monitoringObject in monitoringObjectList)
                {
                    AddMonitoringObjectTreeNode(monitoringObject, e.Node);
                }

                EndUpdate();
            }
            
            base.OnBeforeExpand(e);
        }

        //---------------------------------------------------------------------
        private void AddMonitoringObjectTreeNode(PartialMonitoringObject monitoringObject, TreeNode parentNode)
        {
            TreeNode    node        = new TreeNode();
            string      imageKey    = GetImageKey(monitoringObject.GetLeastDerivedNonAbstractMonitoringClass());

            node.Text = Common.GetBestMonitoringObjectName(monitoringObject);

            node.SelectedImageKey   = imageKey;
            node.ImageKey           = imageKey;
            node.Tag                = monitoringObject;

            if (parentNode == null)
            {
                Nodes.Add(node);
            }
            else
            {
                parentNode.Nodes.Add(node);
            }

            PlaceholderTreeNode placeHolderNode = new PlaceholderTreeNode(node);
        }

        //---------------------------------------------------------------------
        private string GetImageKey(
            MonitoringClass monitoringClass
            )
        {
            ReadOnlyCollection<MonitoringImageReference> imageRefs;

            if (m_classIdToMonitoringImageReferences.ContainsKey(monitoringClass.Id))
            {
                imageRefs = m_classIdToMonitoringImageReferences[monitoringClass.Id];
            }
            else
            {
                imageRefs = monitoringClass.GetMonitoringImageReferences();
                m_classIdToMonitoringImageReferences.Add(monitoringClass.Id, imageRefs);
            }
            
            string imageKey = string.Empty;

            foreach (ManagementPackImageReference imageRef in imageRefs)
            {
                MonitoringImageObject imageObj;
                
                if(m_imageIdToImageMap.ContainsKey(imageRef.ImageID.Id))
                {
                    imageObj = m_imageIdToImageMap[imageRef.ImageID.Id];
                }
                else
                {
                    imageObj = m_managementGroup.GetMonitoringImageObject(imageRef.ImageID.Id);
                    m_imageIdToImageMap.Add(imageRef.ImageID.Id,imageObj);
                }                

                Bitmap classImage = new Bitmap(imageObj.ImageData);

                if (classImage.Height == 16 && classImage.Width == 16)
                {
                    imageKey = imageRef.ImageID.Id.ToString();
                    break;
                }
            }

            if (imageKey == string.Empty)
            {
                if (monitoringClass.Base == null)
                {
                    return (imageKey);
                }

                MonitoringClass parentMonitoringClass = m_managementGroup.GetMonitoringClass(monitoringClass.Base.Id);

                imageKey = GetImageKey(parentMonitoringClass);
            }

            return (imageKey);
        }

        //---------------------------------------------------------------------
        private void LoadImageList()
        {
            ReadOnlyCollection<MonitoringImageObject> monitoringImages = m_managementGroup.GetMonitoringImageObjects();

            m_imageList = new ImageList();

            foreach (MonitoringImageObject imageObj in monitoringImages)
            {

                MemoryStream m_imageStream = new MemoryStream();

                if (imageObj.ImageData != null)
                {
                    try
                    {
                        Bitmap bitmap = new Bitmap(imageObj.ImageData);

                        if (bitmap.Width == 16 && bitmap.Height == 16)
                        {
                            m_imageList.Images.Add(imageObj.Id.ToString(), bitmap);
                        }
                    }
                    catch
                    {
                        //dmuscett 2012-03-02 nothing to do, really... just necessary until I figure out why certain icons break this w/ 2012 SDK... 
                    }

                }
            }
        }
    }

    //---------------------------------------------------------------------
    class MonitoringObjectComparer : IComparer<PartialMonitoringObject>
    {
        //---------------------------------------------------------------------
        public MonitoringObjectComparer()
        {
        }

        //---------------------------------------------------------------------
        public int Compare(PartialMonitoringObject x, PartialMonitoringObject y)
        {
            string lhs = string.IsNullOrEmpty(x.DisplayName) ? x.Name : x.DisplayName;
            string rhs = string.IsNullOrEmpty(y.DisplayName) ? y.Name : y.DisplayName;
            return string.Compare(lhs, rhs);
        }
    }

    //---------------------------------------------------------------------
    internal class PlaceholderTreeNode : TreeNode
    {
        internal PlaceholderTreeNode(TreeNode parentNode)
        {
            parentNode.Nodes.Add(this);
        }
    }
}
