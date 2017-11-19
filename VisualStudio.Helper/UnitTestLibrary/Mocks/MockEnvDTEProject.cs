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
using EnvDTE;
using VsWebSite;
using VSLangProj;
using PrjKind = VSLangProj.PrjKind;

namespace ProjectLinker.UnitTestLibrary.Mocks
{
    public class MockEnvDteProject : Project
    {
        CodeModel _codeModel = new MockCodeModel(CodeModelLanguageConstants.vsCMLanguageCSharp);
        private readonly MockEnvDteProjectItems _projectItems;
        private string _projectKind = PrjKind.prjKindCSharpProject;
        readonly MockProjectProperties _projectProperties = new MockProjectProperties();

        public MockEnvDteProject(MockVsHierarchy hierarchy)
        {
            Hierarchy = hierarchy;
            _projectProperties.Add("RootNamespace", "Namespace1");

            Object = new MockEnvDteVsProject(this);
            _projectItems = new MockEnvDteProjectItems(hierarchy);
        }


        public MockVsHierarchy Hierarchy { get; }

        public void SetCodeModel(CodeModel codeModel)
        {
            _codeModel = codeModel;
        }

        public void SetKind(string projectKind)
        {
            _projectKind = projectKind;
        }

        #region Project Members

        CodeModel Project.CodeModel => _codeModel;

        Projects Project.Collection => throw new Exception("The method or operation is not implemented.");

        ConfigurationManager Project.ConfigurationManager => throw new Exception("The method or operation is not implemented.");

        DTE Project.DTE => throw new Exception("The method or operation is not implemented.");

        void Project.Delete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        string Project.ExtenderCATID => throw new Exception("The method or operation is not implemented.");

        object Project.ExtenderNames => throw new Exception("The method or operation is not implemented.");

        string Project.FileName => throw new Exception("The method or operation is not implemented.");

        string Project.FullName => throw new Exception("The method or operation is not implemented.");

        Globals Project.Globals { get; } = new MockGlobals();

        bool Project.IsDirty
        {
            get => throw new Exception("The method or operation is not implemented.");
            set => throw new Exception("The method or operation is not implemented.");
        }

        string Project.Kind => _projectKind;

        string Project.Name
        {
            get => throw new Exception("The method or operation is not implemented.");
            set => throw new Exception("The method or operation is not implemented.");
        }

        public object Object { get; set; }

        ProjectItem Project.ParentProjectItem => throw new Exception("The method or operation is not implemented.");

        ProjectItems Project.ProjectItems => _projectItems;

        Properties Project.Properties => _projectProperties;

        void Project.Save(string fileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void Project.SaveAs(string newFileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool Project.Saved
        {
            get => throw new Exception("The method or operation is not implemented.");
            set => throw new Exception("The method or operation is not implemented.");
        }

        string Project.UniqueName => throw new Exception("The method or operation is not implemented.");

        object Project.get_Extender(string extenderName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private class MockGlobals : Globals
        {
            DTE Globals.DTE => throw new NotImplementedException();

            object Globals.Parent => throw new NotImplementedException();

            object Globals.VariableNames => throw new NotImplementedException();

            bool Globals.get_VariableExists(string name)
            {
                throw new NotImplementedException();
            }

            bool Globals.get_VariablePersists(string variableName)
            {
                throw new NotImplementedException();
            }

            void Globals.set_VariablePersists(string variableName, bool pVal)
            {
            }

            object Globals.this[string variableName]
            {
                get { throw new NotImplementedException(); }
                set { }
            }
        }

        #endregion
    }

    public class MockProjectProperties : Properties
    {
        readonly List<Property> _propertiesList = new List<Property>();


        public Property Add(string name, string value)
        {
            MockProperty prop = new MockProperty
            {
                Name = name,
                Value = value
            };
            _propertiesList.Add(prop);

            return prop;
        }

        #region Properties Members

        public object Application => throw new Exception("The method or operation is not implemented.");

        public int Count => throw new Exception("The method or operation is not implemented.");

        public DTE DTE => throw new Exception("The method or operation is not implemented.");

        public IEnumerator GetEnumerator()
        {
            return _propertiesList.GetEnumerator();
        }

        public Property Item(object index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object Parent => throw new Exception("The method or operation is not implemented.");

        #endregion
    }

    public class MockProperty : Property
    {
        #region Property Members

        public object Application => throw new Exception("The method or operation is not implemented.");

        public Properties Collection => throw new Exception("The method or operation is not implemented.");

        public DTE DTE => throw new Exception("The method or operation is not implemented.");

        public string Name { set; get; }

        public short NumIndices => throw new Exception("The method or operation is not implemented.");

        public object Object
        {
            get => throw new Exception("The method or operation is not implemented.");
            set => throw new Exception("The method or operation is not implemented.");
        }

        public Properties Parent => throw new Exception("The method or operation is not implemented.");

        public object Value { get; set; }

        public object get_IndexedValue(object index1, object index2, object index3, object index4)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void let_Value(object lppvReturn)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void set_IndexedValue(object index1, object index2, object index3, object index4, object val)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    public class MockEnvDteWebSite : VSWebSite
    {
        private readonly MockVsWebSiteAssemblyReferences _references = new MockVsWebSiteAssemblyReferences();

        #region VSWebSite Members

        public ProjectItem AddFromTemplate(string bstrRelFolderUrl, string bstrWizardName, string bstrLanguage, string bstrItemName,
            bool bUseCodeSeparation, string bstrMasterPage, string bstrDocType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeFolders CodeFolders => throw new Exception("The method or operation is not implemented.");

        public DTE DTE => throw new Exception("The method or operation is not implemented.");

        public string EnsureServerRunning()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetUniqueFilename(string bstrFolder, string bstrRoot, string bstrDesiredExt)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PreCompileWeb(string bstrCompilePath, bool bUpdateable)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Project Project { get; set; }

        public AssemblyReferences References => _references;

        public void Refresh()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string TemplatePath => throw new Exception("The method or operation is not implemented.");

        public string URL => throw new Exception("The method or operation is not implemented.");

        public string UserTemplatePath => throw new Exception("The method or operation is not implemented.");

        public VSWebSiteEvents VSWebSiteEvents => throw new Exception("The method or operation is not implemented.");

        public void WaitUntilReady()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public WebReferences WebReferences => throw new Exception("The method or operation is not implemented.");

        public WebServices WebServices => throw new Exception("The method or operation is not implemented.");

        #endregion
    }

    public class MockEnvDteVsProject : VSProject
    {
        readonly MockVsProjectReferences _references = new MockVsProjectReferences();

        public MockEnvDteVsProject(Project envDteProject)
        {
            Project = envDteProject;
        }

        #region VSProject Members

        public ProjectItem AddWebReference(string bstrUrl)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public BuildManager BuildManager => throw new Exception("The method or operation is not implemented.");

        public void CopyProject(string bstrDestFolder, string bstrDestUncPath, prjCopyProjectOption copyProjectOption, string bstrUsername,
            string bstrPassword)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ProjectItem CreateWebReferencesFolder()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DTE DTE => throw new Exception("The method or operation is not implemented.");

        public VSProjectEvents Events => throw new Exception("The method or operation is not implemented.");

        public void Exec(prjExecCommand command, int bSuppressUI, object varIn, out object pVarOut)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void GenerateKeyPairFiles(string strPublicPrivateFile, string strPublicOnlyFile)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetUniqueFilename(object pDispatch, string bstrRoot, string bstrDesiredExt)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Imports Imports => throw new Exception("The method or operation is not implemented.");

        public Project Project { get; set; }

        public References References => _references;

        public void Refresh()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string TemplatePath => throw new Exception("The method or operation is not implemented.");

        public ProjectItem WebReferencesFolder => throw new Exception("The method or operation is not implemented.");

        public bool WorkOffline
        {
            get => throw new Exception("The method or operation is not implemented.");
            set => throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    internal class MockVsWebSiteAssemblyReferences : AssemblyReferences
    {
        readonly List<MockVsWebSiteAssemblyReference> _references = new List<MockVsWebSiteAssemblyReference>();

        internal class MockVsWebSiteAssemblyReference : AssemblyReference
        {
            #region AssemblyReference Members

            public Project ContainingProject => throw new Exception("The method or operation is not implemented.");

            public DTE DTE => throw new Exception("The method or operation is not implemented.");

            public string FullPath { get; set; } = string.Empty;

            public string Name => throw new Exception("The method or operation is not implemented.");

            public AssemblyReferenceType ReferenceKind => throw new Exception("The method or operation is not implemented.");

            public Project ReferencedProject { get; set; }

            public void Remove()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public string StrongName => throw new Exception("The method or operation is not implemented.");

            #endregion
        }

        #region AssemblyReferences Members

        public AssemblyReference AddFromFile(string bstrPath)
        {
            MockVsWebSiteAssemblyReference reference = new MockVsWebSiteAssemblyReference {FullPath = bstrPath};
            _references.Add(reference);
            return reference;
        }

        public AssemblyReference AddFromGAC(string bstrAssemblyName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddFromProject(Project pProj)
        {
            MockVsWebSiteAssemblyReference reference = new MockVsWebSiteAssemblyReference {ReferencedProject = pProj};
            _references.Add(reference);
        }

        public Project ContainingProject => throw new Exception("The method or operation is not implemented.");

        public int Count => throw new Exception("The method or operation is not implemented.");

        public DTE DTE => throw new Exception("The method or operation is not implemented.");

        public IEnumerator GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public AssemblyReference Item(object index)
        {
            return _references[(int) index];
        }

        #endregion
    }

    internal class MockVsProjectReferences : References
    {
        readonly List<Reference> _references = new List<Reference>();

        internal class MockVsReference : Reference
        {
            #region Reference Members

            public int BuildNumber => throw new Exception("The method or operation is not implemented.");

            public References Collection => throw new Exception("The method or operation is not implemented.");

            public Project ContainingProject => throw new Exception("The method or operation is not implemented.");

            public bool CopyLocal
            {
                get => throw new Exception("The method or operation is not implemented.");
                set => throw new Exception("The method or operation is not implemented.");
            }

            public string Culture => throw new Exception("The method or operation is not implemented.");

            public DTE DTE => throw new Exception("The method or operation is not implemented.");

            public string Description => throw new Exception("The method or operation is not implemented.");

            public string ExtenderCATID => throw new Exception("The method or operation is not implemented.");

            public object ExtenderNames => throw new Exception("The method or operation is not implemented.");

            public string Identity => throw new Exception("The method or operation is not implemented.");

            public int MajorVersion => throw new Exception("The method or operation is not implemented.");

            public int MinorVersion => throw new Exception("The method or operation is not implemented.");

            public string Name => throw new Exception("The method or operation is not implemented.");

            public string Path { get; set; }

            public string PublicKeyToken => throw new Exception("The method or operation is not implemented.");

            public void Remove()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public int RevisionNumber => throw new Exception("The method or operation is not implemented.");

            public Project SourceProject { get; set; }

            public bool StrongName => throw new Exception("The method or operation is not implemented.");

            public prjReferenceType Type => throw new Exception("The method or operation is not implemented.");

            public string Version => throw new Exception("The method or operation is not implemented.");

            public object get_Extender(string extenderName)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #region References Members

        public Reference Add(string bstrPath)
        {
            MockVsReference reference = new MockVsReference {Path = bstrPath};
            _references.Add(reference);
            return reference;
        }

        public Reference AddActiveX(string bstrTypeLibGuid, int lMajorVer, int lMinorVer, int lLocaleId, string bstrWrapperTool)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Reference AddProject(Project pProject)
        {
            MockVsReference reference = new MockVsReference {SourceProject = pProject};
            _references.Add(reference);
            return reference;
        }

        public Project ContainingProject => throw new Exception("The method or operation is not implemented.");

        public int Count => throw new Exception("The method or operation is not implemented.");

        public DTE DTE => throw new Exception("The method or operation is not implemented.");

        public Reference Find(string bstrIdentity)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerator GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Reference Item(object index)
        {
            return _references[(int) index];
        }

        public object Parent => throw new Exception("The method or operation is not implemented.");

        #endregion
    }
}