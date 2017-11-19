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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.UnitTestLibrary.Mocks;
using VsWebSite;
using VSLangProj;

namespace ProjectLinker.Helper.Tests
{
    /// <summary>
    ///     Summary description for HierarchyNodeFixture
    /// </summary>
    [TestClass]
    public class ProjectNodeFixture
    {
        private MockVsHierarchy _project;
        private MockVsHierarchy _root;
        private MockVsSolution _vsSolution;

        [TestInitialize]
        public void CreateVsSolution()
        {
            Environment.SetEnvironmentVariable("VSINSTALLDIR", @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community");
            Environment.SetEnvironmentVariable("VisualStudioVersion", @"15.0");
            _root = new MockVsHierarchy();
            _vsSolution = new MockVsSolution(_root);
            _project = new MockVsHierarchy("Project.project");
            _root.AddProject(_project);
        }

        [TestCleanup]
        public void DeleteVsSolution()
        {
            _project = null;
            _vsSolution = null;
            _root = null;
        }

        [TestMethod]
        public void TestAcceptsProjectReferenceToItSelf()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            int before = _project.Children.Count;
            projectNode.AddProjectReference(_project.Guid);
            Assert.AreEqual(before, _project.Children.Count);
        }

        [TestMethod]
        public void TestAddItem()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            string itemName = "item1.cs";
            projectNode.AddItem(itemName);
            string fullItemName = new FileInfo(itemName).FullName;
            Assert.IsTrue(_project.Children.Contains(fullItemName));
        }

        [TestMethod]
        public void TestCanAddItem()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            string itemName = "item1.cs";
            Assert.IsTrue(ProjectNode.CanAddItem(itemName));
            string invalidItemName = "<item1>.cs";
            Assert.IsFalse(ProjectNode.CanAddItem(invalidItemName));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddItemWithEmptyNameThrows()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            string itemName = ".cs";
            string fullItemName = new FileInfo(itemName).FullName;
            projectNode.AddItem(itemName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddItemWithInvalidNameThrows()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            string itemName = "Invalid<Name>";
            projectNode.AddItem(itemName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddItemWithNullNameThrows()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            projectNode.AddItem(null);
        }

        [TestMethod]
        public void TestOpenItem()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            string itemName = "item1.cs";
            HierarchyNode item = projectNode.AddItem(itemName);
            IVsWindowFrame wnd = projectNode.OpenItem(item);
            Assert.IsNotNull(wnd);
        }

        //[TestMethod]
        //public void TestGetMSBuildItem()
        //{
        //	ProjectNode projectNode = new ProjectNode(vsSolution, project.GUID);
        //	string itemName = "item1.cs";
        //	Assert.IsNotNull(projectNode.AddItem(itemName));
        //	Assert.IsNotNull(projectNode.GetBuildItem(itemName));
        //	string itemName2 = ".\\item2.cs";
        //	Assert.IsNotNull(projectNode.AddItem(itemName2));
        //	Assert.IsNotNull(projectNode.GetBuildItem(itemName2));
        //	string itemName3 = ".\\FolderForItem3\\item3.cs";
        //	Assert.IsNotNull(projectNode.AddItem(itemName3));
        //	Assert.IsNotNull(projectNode.GetBuildItem(itemName3));
        //	string itemName4 = "FolderForItem4\\item4.cs";
        //	Assert.IsNotNull(projectNode.AddItem(itemName4));
        //	Assert.IsNotNull(projectNode.GetBuildItem(itemName4));
        //}

        [TestMethod]
        public void TestFindSubFolder()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            string parentFolder = "Folder" + Guid.NewGuid();
            string subFolderName = "subFolder1";
            Assert.IsNotNull(projectNode.FindSubfolder("\\" + parentFolder + "\\" + subFolderName + "\\" + subFolderName + "\\"));
            HierarchyNode parentFolderNode = projectNode.FindByName(parentFolder);
            Assert.IsNotNull(parentFolderNode);
            HierarchyNode subFolder1Node = parentFolderNode.FindByName(subFolderName);
            Assert.IsNotNull(subFolder1Node);
            HierarchyNode subFolder2Node = subFolder1Node.FindByName(subFolderName);
            Assert.IsNotNull(subFolder2Node);
        }

        [TestMethod]
        public void TestCreateOrFindFolder()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            Assert.AreSame(projectNode, projectNode.FindOrCreateFolder("."));
            Assert.AreSame(projectNode, projectNode.FindSubfolder("."));
            Assert.AreSame(projectNode, projectNode.FindSubfolder(".\\."));
            Assert.AreSame(projectNode, projectNode.FindSubfolder(".\\.\\"));
            Assert.AreSame(projectNode, projectNode.FindSubfolder("\\"));
            Assert.AreSame(projectNode, projectNode.FindSubfolder("\\."));
            Assert.AreSame(projectNode, projectNode.FindSubfolder("\\.\\"));
            Assert.AreSame(projectNode, projectNode.FindSubfolder("\\.\\."));
        }

        [TestMethod]
        public void ShouldReturnCodeModelForNormalProject()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);

            string language = projectNode.Language;
            Assert.AreEqual(CodeModelLanguageConstants.vsCMLanguageCSharp, language);
        }

        [TestMethod]
        public void ShouldReturnCodeModelForWebProject()
        {
            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            CreateWebsiteProject();

            string language = projectNode.Language;
            Assert.AreEqual(CodeModelLanguageConstants.vsCMLanguageCSharp, language);
        }


        [TestMethod]
        public void ShouldAddReferencesToVsProject()
        {
            string assemblyPath = @"c:\blah\some.dll";

            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            projectNode.AddAssemblyReference(assemblyPath);

            string pathFromMock = ((VSProject) ((MockEnvDteProject) _project.ExtObject).Object).References.Item(0).Path;

            Assert.AreEqual(assemblyPath, pathFromMock);
        }

        [TestMethod]
        public void ShouldAddProjectReferencestoVsProject()
        {
            MockVsHierarchy refProjHier = new MockVsHierarchy("refedproj.proj");
            _root.AddProject(refProjHier);
            Project projToRef = refProjHier.ExtObject as Project;
            Assert.IsNotNull(projToRef);

            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            projectNode.AddProjectReference(refProjHier.Guid);

            Project projectFromMock = ((VSProject) ((MockEnvDteProject) _project.ExtObject).Object).References.Item(0).SourceProject;

            Assert.AreSame(projToRef, projectFromMock);
        }

        [TestMethod]
        public void ShouldAddReferenceToWebSiteProject()
        {
            string assemblyPath = @"c:\blah\some.dll";

            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            CreateWebsiteProject();

            projectNode.AddAssemblyReference(assemblyPath);

            string pathFromMock = ((VSWebSite) ((MockEnvDteProject) _project.ExtObject).Object).References.Item(0).FullPath;

            Assert.AreEqual(assemblyPath, pathFromMock);
        }

        [TestMethod]
        public void ShouldAddProjectReferencesToWebSiteProject()
        {
            MockVsHierarchy refProjHier = new MockVsHierarchy("refedproj.proj");
            _root.AddProject(refProjHier);
            Project projToRef = refProjHier.ExtObject as Project;
            Assert.IsNotNull(projToRef);

            ProjectNode projectNode = new ProjectNode(_vsSolution, _project.Guid);
            CreateWebsiteProject();

            projectNode.AddProjectReference(refProjHier.Guid);

            Project projectFromMock = ((VSWebSite) ((MockEnvDteProject) _project.ExtObject).Object).References.Item(0).ReferencedProject;

            Assert.AreSame(projToRef, projectFromMock);
        }

        private void CreateWebsiteProject()
        {
            MockEnvDteWebSite webSiteProj = new MockEnvDteWebSite();
            ((MockEnvDteProject) _project.ExtObject).Object = webSiteProj;
            webSiteProj.Project = (Project) _project.ExtObject;
        }
    }
}