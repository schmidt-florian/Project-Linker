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
using System.IO;
using EnvDTE;
using VSLangProj;

namespace ProjectLinker.Tests.Mocks
{
    class MockProject : Project
    {
        static readonly Random Random = new Random();
        public MockGlobals Globals = new MockGlobals();
        public MockProjectItems ProjectItems;
        public MockProperties Properties;

        public MockProject()
        {
            Properties = new MockProperties(this);
            FullName = string.Format(@"c:\mockProjectPath\{0}\{0}.csproj", Random.Next());
            ProjectItems = new MockProjectItems(this);
        }

        public MockProject(string fullName)
            : this()
        {
            FullName = fullName;
        }

        public string FullName { get; set; }

        Globals Project.Globals => Globals;

        Properties Project.Properties => Properties;

        ProjectItems Project.ProjectItems => ProjectItems;

        public string Name
        {
            get => Path.GetFileNameWithoutExtension(FullName);
            set => throw new NotImplementedException();
        }

        string Project.UniqueName => FullName;

        CodeModel Project.CodeModel => throw new NotImplementedException();

        Projects Project.Collection => throw new NotImplementedException();

        ConfigurationManager Project.ConfigurationManager => throw new NotImplementedException();

        DTE Project.DTE => throw new NotImplementedException();

        void Project.Delete()
        {
            throw new NotImplementedException();
        }

        string Project.ExtenderCATID => throw new NotImplementedException();

        object Project.ExtenderNames => throw new NotImplementedException();

        string Project.FileName => throw new NotImplementedException();

        bool Project.IsDirty
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        string Project.Kind => PrjKind.prjKindCSharpProject;

        object Project.Object => throw new NotImplementedException();

        ProjectItem Project.ParentProjectItem => throw new NotImplementedException();

        void Project.Save(string fileName)
        {
            throw new NotImplementedException();
        }

        void Project.SaveAs(string newFileName)
        {
            throw new NotImplementedException();
        }

        bool Project.Saved
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        object Project.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }
    }
}