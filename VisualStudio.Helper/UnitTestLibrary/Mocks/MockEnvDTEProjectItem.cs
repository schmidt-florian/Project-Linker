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
using EnvDTE;

namespace ProjectLinker.UnitTestLibrary.Mocks
{
    public class MockEnvDteProjectItem : ProjectItem
    {
        public MockEnvDteProjectItem(MockVsHierarchy parentHierarchy)
        {
            Collection = new MockEnvDteProjectItems(parentHierarchy);
        }

        public bool SaveAs(string newFileName)
        {
            throw new NotImplementedException();
        }

        public Window Open(string viewKind)
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public void ExpandView()
        {
            throw new NotImplementedException();
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public bool IsDirty
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public short FileCount => throw new NotImplementedException();

        public string Name { get; set; }

        public ProjectItems Collection { get; set; }

        public Properties Properties => throw new NotImplementedException();

        public DTE DTE => throw new NotImplementedException();

        public string Kind { get; set; }

        public ProjectItems ProjectItems => throw new NotImplementedException();

        public object Object => throw new NotImplementedException();

        public object ExtenderNames => throw new NotImplementedException();

        public string ExtenderCATID => throw new NotImplementedException();

        public bool Saved
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ConfigurationManager ConfigurationManager => throw new NotImplementedException();

        public FileCodeModel FileCodeModel => throw new NotImplementedException();

        public Document Document => throw new NotImplementedException();

        public Project SubProject => throw new NotImplementedException();

        public Project ContainingProject => throw new NotImplementedException();

        public string get_FileNames(short index)
        {
            return Name;
        }

        public bool get_IsOpen(string viewKind)
        {
            throw new NotImplementedException();
        }

        public object get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }
    }
}