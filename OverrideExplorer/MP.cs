using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EnterpriseManagement.Configuration;
using System.Diagnostics;

namespace OverrideExplorer
{
    class MP
    {
        ManagementPack              m_mp;
        Dictionary<Guid, MPClass>   m_classCollection;

        //---------------------------------------------------------------------
        internal MP(ManagementPack mp)
        {
            Debug.Assert(mp != null);

            m_mp = mp;

            m_classCollection = new Dictionary<Guid, MPClass>();
        }

        //---------------------------------------------------------------------
        internal void AddOverride(
            ManagementPackClass             mpClass,
            ManagementPackElement           overridenElement,
            ManagementPackOverride          mpOverride
            )
        {
            Debug.Assert(mpClass != null);
            Debug.Assert(overridenElement != null);
            Debug.Assert(mpOverride != null);

            if (!m_classCollection.ContainsKey(mpClass.Id))
            {
                m_classCollection.Add(mpClass.Id, new MPClass(mpClass));
            }

            m_classCollection[mpClass.Id].AddOverride(mpOverride, overridenElement);
        }

        //---------------------------------------------------------------------
        internal int NumberOfOverrides
        {
            get
            {
                int numberOfOverrides = 0;

                foreach (KeyValuePair<Guid, MPClass> mpClass in m_classCollection)
                {
                    foreach (KeyValuePair<Guid, MPWorkflow> workflow in mpClass.Value.MPWorkflows)
                    {
                        numberOfOverrides += workflow.Value.Overrides.Count;
                    }
                }

                return (numberOfOverrides);
            }
        }

        //---------------------------------------------------------------------
        internal string Name
        {
            get
            {
                return (Common.GetBestManagementPackName(m_mp));
            }
        }

        //added dmuscett 2012-03-02
        internal string Sealed
        {
            get
            {
                return Common.GetManagementPackSealedFlag(m_mp);
            }
        }

        //---------------------------------------------------------------------
        internal Dictionary<Guid, MPClass> Classes
        {
            get
            {
                return (m_classCollection);
            }
        }

    }
}
