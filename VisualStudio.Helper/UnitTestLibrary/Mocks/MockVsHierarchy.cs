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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSLangProj;
using Constants = EnvDTE.Constants;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Project = Microsoft.Build.Evaluation.Project;
using ProjectItem = EnvDTE.ProjectItem;

namespace ProjectLinker.UnitTestLibrary.Mocks
{
    public class MockVsHierarchy : IVsHierarchy, IVsProject, IVsUIHierarchy, VSProject, References, IVsProject2
    {
        readonly List<string> _folders;
        readonly MockVsHierarchy _parent;
#pragma warning disable 612,618
        readonly Project _project;
#pragma warning restore 612,618

        public MockVsHierarchy()
            : this(0, "<Solution>", Guid.Empty, null)
        {
        }

        public MockVsHierarchy(string name)
            : this(0, name, Guid.NewGuid(), null)
        {
        }

        public MockVsHierarchy(string name, MockVsHierarchy parent)
            : this(0, name, Guid.NewGuid(), parent)
        {
        }

        public MockVsHierarchy(string name, Guid guid)
            : this(0, name, guid, null)
        {
        }

        public MockVsHierarchy(int children)
            : this(children, "Project.Project", Guid.NewGuid(), null)
        {
        }

        public MockVsHierarchy(int childrenSize, string name, Guid guid, MockVsHierarchy parent)
        {
            if (parent == null && MockVsSolution.Solution != null && MockVsSolution.Solution.Root != null)
            {
                _parent = MockVsSolution.Solution.Root;
            }
            else
            {
                _parent = parent;
            }
            Guid = guid;
            SubProjects = new List<MockVsHierarchy>();
            FileName = name;
            Children = new List<string>();
            _folders = new List<string>();
            AddChildren(childrenSize);
            if (guid != Guid.Empty && !Directory.Exists(FileName))
            {
#pragma warning disable 612,618
                _project = new Project(ProjectCollection.GlobalProjectCollection);
#pragma warning restore 612,618
                _project.Save(FileName);
                FileName = _project.ProjectFileLocation.File;
            }

            ExtObject = new MockEnvDteProject(this);
        }

        public List<MockVsHierarchy> SubProjects { get; }

        public List<string> Children { get; }

        public string FileName { get; }

        public string Name
        {
            get
            {
                if (FileName.StartsWith("<"))
                {
                    return FileName;
                }
                return new FileInfo(FileName).Name;
            }
        }

        public Guid Guid { get; }

        public Guid TypeGuid { get; set; }

        public object ExtObject { get; set; }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public void AddChild(string child)
        {
            Children.Add(child);
        }

        public void AddFolder(string child)
        {
            _folders.Add(child);
        }

        public void AddChildren(int childrenSize)
        {
            for (int i = 0; i < childrenSize; i++)
            {
                Children.Add($"Child{i}");
            }
        }

        public void AddProject(MockVsHierarchy project)
        {
            Assert.AreNotSame(this, project);
            SubProjects.Add(project);
            MockVsSolution.Solution.RegisterProjectInSolution(project);
        }

        public void RemoveProject(MockVsHierarchy project)
        {
            SubProjects.Remove(project);
            MockVsSolution.Solution.UnregisterProjectInSolution(project);
        }

        #region IVsHierarchy Members

        public int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            pdwCookie = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int Close()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int GetCanonicalName(uint itemid, out string pbstrName)
        {
            pbstrName = string.Empty;
            return VSConstants.E_NOTIMPL;
        }

        public int GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            __VSHPROPID vshPropId = (__VSHPROPID) propid;
            switch (vshPropId)
            {
                case __VSHPROPID.VSHPROPID_ProjectIDGuid:
                    if (itemid == VSConstants.VSITEMID_ROOT)
                    {
                        pguid = Guid;
                        return VSConstants.S_OK;
                    }
                    break;

                case __VSHPROPID.VSHPROPID_TypeGuid:
                    if (itemid == VSConstants.VSITEMID_ROOT)
                    {
                        pguid = TypeGuid;
                    }
                    else if (IsProject(itemid))
                    {
                        pguid = GetProject(itemid).TypeGuid;
                    }
                    else if (IsChild(itemid))
                    {
                        pguid = VSConstants.GUID_ItemType_PhysicalFile;
                    }
                    else if (IsFolder(itemid))
                    {
                        pguid = VSConstants.GUID_ItemType_PhysicalFolder;
                    }
                    else
                    {
                        pguid = Guid.Empty;
                        return VSConstants.S_FALSE;
                    }

                    return VSConstants.S_OK;
            }


            pguid = Guid.Empty;
            return VSConstants.DISP_E_MEMBERNOTFOUND;
        }

        public int GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
        {
            if (itemid == VSConstants.VSITEMID_ROOT || itemid == VSConstants.VSITEMID_NIL)
            {
                ppHierarchyNested = IntPtr.Zero;
                pitemidNested = VSConstants.VSITEMID_NIL;
            }
            else if (IsProject(itemid))
            {
                itemid = itemid - (uint)Children.Count;
                ppHierarchyNested = Marshal.GetIUnknownForObject(SubProjects[(int) itemid]);
                pitemidNested = VSConstants.VSITEMID_ROOT;
            }
            else
            {
                ppHierarchyNested = IntPtr.Zero;
                pitemidNested = VSConstants.VSITEMID_NIL;
            }
            return VSConstants.S_OK;
        }

        private bool IsChild(uint itemId)
        {
            return itemId != VSConstants.VSITEMID_ROOT &&
                   itemId < HierarchyChildrenCount &&
                   itemId < Children.Count;
        }

        private bool IsFolder(uint itemId)
        {
            return itemId != VSConstants.VSITEMID_ROOT &&
                   itemId < HierarchyChildrenCount &&
                   itemId >= Children.Count &&
                   itemId < Children.Count + _folders.Count;
        }

        private bool IsProject(uint itemId)
        {
            return itemId != VSConstants.VSITEMID_ROOT &&
                   itemId < HierarchyChildrenCount &&
                   itemId >= Children.Count + _folders.Count;
        }


        private int HierarchyChildrenCount => Children.Count + _folders.Count + SubProjects.Count;

        public int GetProperty(uint itemid, int propid, out object pvar)
        {
            pvar = null;
            __VSHPROPID propId = (__VSHPROPID) propid;
            switch (propId)
            {
                case __VSHPROPID.VSHPROPID_ExtObject:
                {
                    if (itemid == VSConstants.VSITEMID_ROOT)
                    {
                        pvar = ExtObject;
                        return VSConstants.S_OK;
                    }

                    if (IsChild(itemid))
                    {
                        pvar = new MockEnvDteProjectItem(this)
                        {
                            Name = new FileInfo(GetChild(itemid)).FullName
                        };
                        return VSConstants.S_OK;
                    }
                    if (IsFolder(itemid))
                    {
                        pvar = new MockEnvDteProjectItem(this)
                        {
                            Name = new FileInfo(GetFolder(itemid)).FullName,
                            Kind = Constants.vsProjectItemKindPhysicalFolder
                        };
                        return VSConstants.S_OK;
                    }
                    break;
                }
                case __VSHPROPID.VSHPROPID_SaveName:
                {
                    if (itemid == VSConstants.VSITEMID_ROOT)
                    {
                        pvar = FileName;
                    }
                    else if (IsChild(itemid))
                    {
                        pvar = new FileInfo(GetChild(itemid)).FullName;
                    }
                    else if (IsFolder(itemid))
                    {
                        pvar = new FileInfo(GetFolder(itemid)).FullName;
                    }
                    else if (IsProject(itemid))
                    {
                        pvar = GetProject(itemid).FileName;
                    }

                    return VSConstants.S_OK;
                }
                case __VSHPROPID.VSHPROPID_Name:
                {
                    if (itemid == VSConstants.VSITEMID_ROOT)
                    {
                        pvar = Name;
                    }
                    else if (IsChild(itemid))
                    {
                        pvar = new FileInfo(GetChild(itemid)).Name;
                    }
                    else if (IsFolder(itemid))
                    {
                        pvar = new FileInfo(GetFolder(itemid)).Name;
                    }
                    else if (IsProject(itemid))
                    {
                        pvar = GetProject(itemid).Name;
                    }
                    return VSConstants.S_OK;
                }
                case __VSHPROPID.VSHPROPID_FirstVisibleChild:
                case __VSHPROPID.VSHPROPID_FirstChild:
                {
                    if (itemid == VSConstants.VSITEMID_ROOT && HierarchyChildrenCount > 0)
                    {
                        pvar = 0;
                        return VSConstants.S_OK;
                    }
                    pvar = VSConstants.VSITEMID_NIL;
                    return VSConstants.S_OK;
                }
                case __VSHPROPID.VSHPROPID_NextSibling:
                case __VSHPROPID.VSHPROPID_NextVisibleSibling:
                {
                    if (itemid + 1 < HierarchyChildrenCount)
                    {
                        pvar = itemid + 1;
                        return VSConstants.S_OK;
                    }
                    pvar = VSConstants.VSITEMID_NIL;
                    return VSConstants.S_OK;
                }
                case __VSHPROPID.VSHPROPID_ProjectDir:
                {
                    pvar = Directory.GetCurrentDirectory();
                    return VSConstants.S_OK;
                }
                case __VSHPROPID.VSHPROPID_ParentHierarchy:
                {
                    pvar = _parent;
                    return VSConstants.S_OK;
                }
            }
            return VSConstants.DISP_E_MEMBERNOTFOUND;
        }

        private MockVsHierarchy GetProject(uint itemid)
        {
            uint projectItemId = itemid - ((uint)Children.Count + (uint)_folders.Count);
            return SubProjects[(int) projectItemId];
        }

        private string GetChild(uint itemid)
        {
            return Children[(int) itemid];
        }

        private string GetFolder(uint itemid)
        {
            uint folderItemId = itemid - (uint)Children.Count;
            return _folders[(int) folderItemId];
        }

        public int GetSite(out IServiceProvider ppSp)
        {
            ppSp = null;
            return VSConstants.E_NOTIMPL;
        }

        public int ParseCanonicalName(string pszName, out uint pitemid)
        {
            pitemid = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int QueryClose(out int pfCanClose)
        {
            pfCanClose = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetProperty(uint itemid, int propid, object var)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SetSite(IServiceProvider psp)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int UnadviseHierarchyEvents(uint dwCookie)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused0()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused1()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused2()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused3()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Unused4()
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsProject Members

        int IVsProject.AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen,
            string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult)
        {
            if (Directory.Exists(rgpszFilesToOpen[0]))
            {
                AddProject(new MockVsHierarchy(rgpszFilesToOpen[0], this));
            }
            else
            {
                Children.Add(rgpszFilesToOpen[0]);
                if (_project != null)
                {
                    FileInfo itemFileInfo = new FileInfo(rgpszFilesToOpen[0]);
                    _project.Save(FileName);
                    FileInfo projectFileInfo = new FileInfo(_project.ProjectFileLocation.File);
                    string itemName = itemFileInfo.FullName.Substring(projectFileInfo.Directory.FullName.Length + 1);
                    _project.AddItem("Compile", itemName);
                    _project.Save(FileName);
                }
            }
            return VSConstants.S_OK;
        }

        int IVsProject.GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName)
        {
            pbstrItemName = string.Empty;
            return VSConstants.E_NOTIMPL;
        }

        int IVsProject.GetItemContext(uint itemid, out IServiceProvider ppSp)
        {
            ppSp = null;
            return VSConstants.E_NOTIMPL;
        }

        int IVsProject.GetMkDocument(uint itemid, out string pbstrMkDocument)
        {
            pbstrMkDocument = null;
            if (itemid == VSConstants.VSITEMID_ROOT)
            {
                pbstrMkDocument = FileName;
            }
            else if (IsChild(itemid))
            {
                pbstrMkDocument = GetChild(itemid);
            }
            else if (IsFolder(itemid))
            {
                pbstrMkDocument = GetFolder(itemid);
            }
            return VSConstants.S_OK;
        }

        int IVsProject.IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid)
        {
            uint i = 0;
            foreach (string doc in Children)
            {
                if (doc == pszMkDocument)
                {
                    pfFound = 1;
                    pitemid = i;
                    return VSConstants.S_OK;
                }
                i++;
            }
            pitemid = VSConstants.VSITEMID_NIL;
            pfFound = 0;
            return VSConstants.S_OK;
        }

        int IVsProject.OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        {
            ppWindowFrame = new MockVsWindowFrame();
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsUIHierarchy Members

        int IVsUIHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.GetProperty(uint itemid, int propid, out object pvar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.GetSite(out IServiceProvider ppSp)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.QueryClose(out int pfCanClose)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.SetProperty(uint itemid, int propid, object var)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.SetSite(IServiceProvider psp)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.UnadviseHierarchyEvents(uint dwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.Unused0()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.Unused1()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.Unused2()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.Unused3()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IVsUIHierarchy.Unused4()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region VSProject Members

        ProjectItem VSProject.AddWebReference(string bstrUrl)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        BuildManager VSProject.BuildManager => throw new Exception("The method or operation is not implemented.");

        void VSProject.CopyProject(string bstrDestFolder, string bstrDestUncPath, prjCopyProjectOption copyProjectOption, string bstrUsername,
            string bstrPassword)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        ProjectItem VSProject.CreateWebReferencesFolder()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        DTE VSProject.DTE => throw new Exception("The method or operation is not implemented.");

        VSProjectEvents VSProject.Events => throw new Exception("The method or operation is not implemented.");

        void VSProject.Exec(prjExecCommand command, int bSuppressUI, object varIn, out object pVarOut)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void VSProject.GenerateKeyPairFiles(string strPublicPrivateFile, string strPublicOnlyFile)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        string VSProject.GetUniqueFilename(object pDispatch, string bstrRoot, string bstrDesiredExt)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        Imports VSProject.Imports => throw new Exception("The method or operation is not implemented.");

        EnvDTE.Project VSProject.Project => ExtObject as EnvDTE.Project;

        References VSProject.References => this;

        void VSProject.Refresh()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        string VSProject.TemplatePath => throw new Exception("The method or operation is not implemented.");

        ProjectItem VSProject.WebReferencesFolder => throw new Exception("The method or operation is not implemented.");

        bool VSProject.WorkOffline
        {
            get => throw new Exception("The method or operation is not implemented.");
            set => throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region References Members

        Reference References.Add(string bstrPath)
        {
            if (!Children.Contains(bstrPath))
            {
                Children.Add(bstrPath);
            }
            return null;
        }

        Reference References.AddActiveX(string bstrTypeLibGuid, int lMajorVer, int lMinorVer, int lLocaleId, string bstrWrapperTool)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        Reference References.AddProject(EnvDTE.Project pProject)
        {
            MockEnvDteProject project = (MockEnvDteProject) pProject;
            if (!Children.Contains(project.Hierarchy.FileName))
            {
                Children.Add(project.Hierarchy.FileName);
            }
            return null;
        }

        EnvDTE.Project References.ContainingProject => throw new Exception("The method or operation is not implemented.");

        int References.Count => throw new Exception("The method or operation is not implemented.");

        DTE References.DTE => throw new Exception("The method or operation is not implemented.");

        Reference References.Find(string bstrIdentity)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IEnumerator References.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        Reference References.Item(object index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object References.Parent => throw new Exception("The method or operation is not implemented.");

        #endregion


        #region IVsProject2 Members

        int IVsProject2.AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen,
            string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult)
        {
            return ((IVsProject) this).AddItem(itemidLoc, dwAddItemOperation, pszItemName, cFilesToOpen, rgpszFilesToOpen, hwndDlgOwner, pResult);
        }

        int IVsProject2.GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName)
        {
            return ((IVsProject) this).GenerateUniqueItemName(itemidLoc, pszExt, pszSuggestedRoot, out pbstrItemName);
        }

        int IVsProject2.GetItemContext(uint itemid, out IServiceProvider ppSp)
        {
            return ((IVsProject) this).GetItemContext(itemid, out ppSp);
        }

        int IVsProject2.GetMkDocument(uint itemid, out string pbstrMkDocument)
        {
            return ((IVsProject) this).GetMkDocument(itemid, out pbstrMkDocument);
        }

        int IVsProject2.IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid)
        {
            return ((IVsProject) this).IsDocumentInProject(pszMkDocument, out pfFound, pdwPriority, out pitemid);
        }

        int IVsProject2.OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        {
            return ((IVsProject) this).OpenItem(itemid, ref rguidLogicalView, punkDocDataExisting, out ppWindowFrame);
        }

        int IVsProject2.RemoveItem(uint dwReserved, uint itemid, out int pfResult)
        {
            if (itemid < HierarchyChildrenCount)
            {
                if (IsChild(itemid))
                {
                    Children.RemoveAt((int) itemid);
                }
                else
                {
                    MockVsHierarchy project = GetProject(itemid);
                    RemoveProject(project);
                }
                pfResult = 1;
                return VSConstants.S_OK;
            }
            pfResult = 0;
            return VSConstants.E_FAIL;
        }

        int IVsProject2.ReopenItem(uint itemid, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView,
            IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}