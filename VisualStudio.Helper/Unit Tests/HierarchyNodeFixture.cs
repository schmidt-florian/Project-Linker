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
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.UnitTestLibrary.Mocks;

namespace ProjectLinker.Helper.Tests
{
    /// <summary>
    ///     Summary description for HierarchyNodeFixture
    /// </summary>
    [TestClass]
    public class HierarchyNodeFixture
    {
        [TestInitialize]
        public void TestInit()
        {
            Environment.SetEnvironmentVariable("VSINSTALLDIR", @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community");
            Environment.SetEnvironmentVariable("VisualStudioVersion", @"15.0");
        }


        [TestMethod]
        public void EnumerationWalking()
        {
            int children = 3;
            MockVsHierarchy hierarchy = new MockVsHierarchy(children);
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode node = new HierarchyNode(solution);
            int i = 0;
            foreach (HierarchyNode child in node.Children)
            {
                ++i;
            }
            Assert.AreEqual(children, i, "Invalid number of children");
        }

        [TestMethod]
        public void NamePropertySetCorrectly()
        {
            string name = "MyName";
            MockVsHierarchy hierarchy = new MockVsHierarchy(name, Guid.NewGuid());
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode node = new HierarchyNode(solution);
            Assert.AreEqual(name, node.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConstructorThrowsIfProjectIsInvalid()
        {
            new HierarchyNode(new MockVsSolution(new MockVsHierarchy(0)), Guid.NewGuid());
        }

        [TestMethod]
        public void ForEachWalksAllChildren()
        {
            int children = 3;
            MockVsHierarchy hierarchy = new MockVsHierarchy(children);
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode node = new HierarchyNode(solution);
            int i = 0;

            node.ForEach(delegate { i++; });

            Assert.AreEqual(children, i, "Incorrect number of nodes walked");
        }

        [TestMethod]
        public void RecursiveForEachWalksAllChildrenAndParent()
        {
            int children = 3;
            MockVsHierarchy hierarchy = new MockVsHierarchy(children);
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode node = new HierarchyNode(solution);

            int i = 0;

            node.RecursiveForEach(delegate(HierarchyNode child)
            {
                Trace.WriteLine(child.Name);
                i++;
            });

            Assert.AreEqual(children + 1, i, "Incorrect number of nodes walked");
        }

        [TestMethod]
        public void FindByName()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            string child1 = "Child1";
            string child2 = "Child2";
            string child3 = "Child3";
            string child4 = "Child4";
            hierarchy.AddChild(child1);
            hierarchy.AddChild(child2);
            hierarchy.AddChild(child3);
            HierarchyNode node = new HierarchyNode(solution);
            Assert.IsNotNull(node.FindByName(child1));
            Assert.IsNotNull(node.FindByName(child2));
            Assert.IsNotNull(node.FindByName(child3));
            Assert.IsNull(node.FindByName(child4));
        }

        [TestMethod]
        public void RecursiveFindByName()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            MockVsHierarchy project1 = new MockVsHierarchy("Project1.project");
            hierarchy.AddProject(project1);
            string child1 = "Child1";
            project1.AddChild(child1);
            MockVsHierarchy project2 = new MockVsHierarchy("Project2.project");
            hierarchy.AddProject(project2);
            string child2 = "Child2";
            project2.AddChild(child2);
            string child3 = "Child3";
            project2.AddChild(child3);
            string child4 = "Child4";

            HierarchyNode node = new HierarchyNode(solution);
            Assert.IsNull(node.FindByName(child1));
            Assert.IsNull(node.FindByName(child2));
            Assert.IsNull(node.FindByName(child3));
            Assert.IsNull(node.FindByName(child4));
            Assert.IsNotNull(node.RecursiveFindByName(child1));
            Assert.IsNotNull(node.RecursiveFindByName(child2));
            Assert.IsNotNull(node.RecursiveFindByName(child3));
            Assert.IsNull(node.RecursiveFindByName(child4));
        }

        [TestMethod]
        public void RecursiveFindByNameIgnoreCase()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            MockVsHierarchy project1 = new MockVsHierarchy("Project1.project");
            hierarchy.AddProject(project1);
            string child1 = "Child1";
            project1.AddChild(child1);
            MockVsHierarchy project2 = new MockVsHierarchy("Project2.project");
            hierarchy.AddProject(project2);
            string child2 = "ChIlD2.cd";
            project2.AddChild(child2);
            string child3 = "ChildThree3";
            project2.AddChild(child3);
            string child4 = "Child4NotAdded";

            HierarchyNode node = new HierarchyNode(solution);
            Assert.IsNull(node.FindByName(child1));
            Assert.IsNull(node.FindByName(child2));
            Assert.IsNull(node.FindByName(child3));
            Assert.IsNull(node.FindByName(child4));
            Assert.IsNotNull(node.RecursiveFindByName(child1.ToLowerInvariant()));
            Assert.IsNotNull(node.RecursiveFindByName(child2.ToUpperInvariant()));
            Assert.IsNotNull(node.RecursiveFindByName(CodeIdentifier.MakeCamel(child3)));
            Assert.IsNull(node.RecursiveFindByName(child4));
        }

        [TestMethod]
        public void FindOrCreateSolutionFolder()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode node = new HierarchyNode(solution);
            string folderName = "SlnItems";
            HierarchyNode folder = node.FindOrCreateSolutionFolder(folderName);
            Assert.IsNotNull(folder);
            Assert.AreEqual(folderName, folder.Name);
        }

        [TestMethod]
        public void TestRelativePath()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            MockVsHierarchy folder1 = new MockVsHierarchy("folder1");
            hierarchy.AddProject(folder1);
            string child1 = "subFolder1";
            folder1.AddChild(child1);
            HierarchyNode rootNode = new HierarchyNode(solution);
            HierarchyNode folder1Node = rootNode.FindByName("folder1");
            HierarchyNode child1Node = folder1Node.FindByName(child1);
            Assert.IsNotNull(child1Node.RelativePath);
            Assert.AreEqual(Path.Combine(Directory.GetCurrentDirectory(), child1), child1Node.RelativePath);
        }

        [TestMethod]
        public void RemoveItem()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            MockVsHierarchy project = new MockVsHierarchy("Project1.project");
            hierarchy.AddProject(project);
            ProjectNode projectNode = new ProjectNode(solution, project.Guid);
            string itemName = "item1";
            HierarchyNode node = projectNode.AddItem(itemName);
            Assert.IsNotNull(projectNode.FindByName(itemName));
            node.Remove();
            Assert.IsNull(projectNode.FindByName(itemName));
        }

        [TestMethod]
        public void TestFileHasIcon()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            string docName = "Doc1.doc";
            hierarchy.AddChild(docName);
            HierarchyNode slnNode = new HierarchyNode(solution);
            HierarchyNode node = slnNode.FindByName(docName);
            Assert.IsNotNull(node.Icon);
        }

        [TestMethod]
        public void TestHasChildrenChanges()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode slnNode = new HierarchyNode(solution);
            Assert.IsFalse(slnNode.HasChildren);
            string docName = "Doc1.doc";
            hierarchy.AddChild(docName);
            Assert.IsTrue(slnNode.HasChildren);
        }

        [TestMethod]
        public void TestHasProperty()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode slnNode = new HierarchyNode(solution);
            Assert.IsFalse(slnNode.HasIconIndex);
        }

        [TestMethod]
        public void TestGetObject()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            HierarchyNode slnNode = new HierarchyNode(solution);
            Assert.AreSame(hierarchy, slnNode.GetObject<MockVsHierarchy>());
        }

        [TestMethod]
        public void ShouldReturnTypeGuidForSolutionFile()
        {
            MockVsHierarchy root = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(root);
            string childName = "Child1.txt";
            root.AddChild(childName);

            HierarchyNode slnNode = new HierarchyNode(solution);
            HierarchyNode childNode = slnNode.FindByName(childName);

            Assert.AreEqual(VSConstants.GUID_ItemType_PhysicalFile, childNode.TypeGuid);
        }

        [TestMethod]
        public void ShouldReturnTypeGuidForSolutionFolder()
        {
            MockVsHierarchy hierarchy = new MockVsHierarchy();
            MockVsSolution solution = new MockVsSolution(hierarchy);
            MockVsHierarchy project = new MockVsHierarchy("Project1.project") {TypeGuid = VSConstants.GUID_ItemType_VirtualFolder};
            hierarchy.AddProject(project);

            HierarchyNode slnNode = new HierarchyNode(solution);
            HierarchyNode prjNode = slnNode.FindByName("Project1.project");

            Assert.AreEqual(VSConstants.GUID_ItemType_VirtualFolder, prjNode.TypeGuid);
        }
    }
}