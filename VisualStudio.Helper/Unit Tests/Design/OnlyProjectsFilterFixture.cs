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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Helper.Design;
using ProjectLinker.UnitTestLibrary.Mocks;

namespace ProjectLinker.Helper.Tests.Design
{
    [TestClass]
    public class OnlyProjectsFilterFixture
    {
        [TestMethod]
        public void ShouldFilterOutChildren()
        {
            MockHierarchyNode solutionNode = new MockHierarchyNode {IsSolution = true};
            MockHierarchyNode projectNode = new MockHierarchyNode {ExtObject = new MockProject()};
            MockHierarchyNode itemNode = new MockHierarchyNode {ExtObject = new object()};

            OnlyProjectsFilter target = new OnlyProjectsFilter();
            Assert.IsFalse(target.Filter(solutionNode));
            Assert.IsTrue(target.Filter(itemNode));
            Assert.IsFalse(target.Filter(projectNode));
        }

        internal class MockProject : Project
        {
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

            string Project.FullName => throw new NotImplementedException();

            Globals Project.Globals => throw new NotImplementedException();

            bool Project.IsDirty
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            string Project.Kind => throw new NotImplementedException();

            string Project.Name
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            object Project.Object => throw new NotImplementedException();

            ProjectItem Project.ParentProjectItem => throw new NotImplementedException();

            ProjectItems Project.ProjectItems => throw new NotImplementedException();

            EnvDTE.Properties Project.Properties => throw new NotImplementedException();

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

            string Project.UniqueName => throw new NotImplementedException();

            object Project.get_Extender(string extenderName)
            {
                throw new NotImplementedException();
            }
        }
    }
}