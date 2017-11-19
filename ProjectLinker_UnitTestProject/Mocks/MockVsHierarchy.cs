//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace ProjectLinker.Tests.Mocks
{
    internal class MockVsHierarchy : IVsProject, IVsHierarchy
    {
        public VSADDITEMOPERATION AddItemArgumentAddItemOperation;
        public string[] AddItemArgumentArrayFilesToOpen;
        public uint AddItemArgumentFilesToOpen;
        public uint AddItemArgumentItemidLoc;
        public string AddItemArgumentItemName;
        public bool AddItemCalled;
        public uint GetPropertyArgumentItemId;
        public int GetPropertyArgumentPropId;
        public bool GetPropertyCalled;
        public Guid GetPropertyProjectIdGuidValue = Guid.NewGuid();
        public string GetPropertyProjectName;
        public MockProject GetPropertyProjectValue = new MockProject();

        public int GetProperty(uint itemid, int propid, out object pvar)
        {
            GetPropertyCalled = true;
            GetPropertyArgumentItemId = itemid;
            GetPropertyArgumentPropId = propid;
            switch (propid)
            {
                case (int) __VSHPROPID.VSHPROPID_ExtObject:
                    pvar = GetPropertyProjectValue;
                    break;
                case (int) __VSHPROPID.VSHPROPID_ProjectIDGuid:
                    pvar = GetPropertyProjectIdGuidValue;
                    break;
                case (int) __VSHPROPID.VSHPROPID_Name:
                    pvar = GetPropertyProjectName;
                    break;
                default:
                    pvar = null;
                    break;
            }
            return 0;
        }

        public int AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen, string[] rgpszFilesToOpen,
            IntPtr hwndDlgOwner, VSADDRESULT[] pResult)
        {
            AddItemCalled = true;

            AddItemArgumentItemidLoc = itemidLoc;
            AddItemArgumentAddItemOperation = dwAddItemOperation;
            AddItemArgumentItemName = pszItemName;
            AddItemArgumentFilesToOpen = cFilesToOpen;
            AddItemArgumentArrayFilesToOpen = rgpszFilesToOpen;

            return VSConstants.S_OK;
        }

        #region IVsProject, IVsHierarchy members

        public int IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid)
        {
            throw new NotImplementedException();
        }

        public int GetMkDocument(uint itemid, out string pbstrMkDocument)
        {
            throw new NotImplementedException();
        }

        public int OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        {
            throw new NotImplementedException();
        }

        public int GetItemContext(uint itemid, out IServiceProvider ppSp)
        {
            throw new NotImplementedException();
        }

        public int GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName)
        {
            throw new NotImplementedException();
        }

        public int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            throw new NotImplementedException();
        }

        public int Close()
        {
            throw new NotImplementedException();
        }

        public int GetCanonicalName(uint itemid, out string pbstrName)
        {
            throw new NotImplementedException();
        }

        public int GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            GetProperty(itemid, propid, out object retVal);
            pguid = (Guid) retVal;
            return VSConstants.S_OK;
        }

        public int GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
        {
            throw new NotImplementedException();
        }

        public int GetSite(out IServiceProvider ppSp)
        {
            throw new NotImplementedException();
        }

        public int ParseCanonicalName(string pszName, out uint pitemid)
        {
            throw new NotImplementedException();
        }

        public int QueryClose(out int pfCanClose)
        {
            throw new NotImplementedException();
        }

        public int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            throw new NotImplementedException();
        }

        public int SetProperty(uint itemid, int propid, object var)
        {
            throw new NotImplementedException();
        }

        public int SetSite(IServiceProvider psp)
        {
            throw new NotImplementedException();
        }

        public int UnadviseHierarchyEvents(uint dwCookie)
        {
            throw new NotImplementedException();
        }

        public int Unused0()
        {
            throw new NotImplementedException();
        }

        public int Unused1()
        {
            throw new NotImplementedException();
        }

        public int Unused2()
        {
            throw new NotImplementedException();
        }

        public int Unused3()
        {
            throw new NotImplementedException();
        }

        public int Unused4()
        {
            throw new NotImplementedException();
        }

        #endregion IVsProject, IVsHierarchy members
    }
}