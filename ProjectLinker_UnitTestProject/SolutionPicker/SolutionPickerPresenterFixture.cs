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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Helper;
using ProjectLinker.SolutionPicker;
using ProjectLinker.UnitTestLibrary.Mocks;
using VSLangProj;

namespace ProjectLinker.Tests.SolutionPicker
{
    /// <summary>
    ///     Summary description for SolutionPickerPresenterTest
    /// </summary>
    [TestClass]
    public class SolutionPickerPresenterFixture
    {
        [TestMethod]
        public void ShouldSetHierarchyNodeOnView()
        {
            MockSolutionPickerView mockView = new MockSolutionPickerView();
            SolutionPickerPresenter presenter = new SolutionPickerPresenter(new MockHierarchyNode(), mockView, new MockHierarchyNode());

            Assert.IsNotNull(mockView.RootHierarchyNode);
        }


        [TestMethod]
        public void ShouldSetViewToCanExitIfProjectNodeSelected()
        {
            MockSolutionPickerView mockView = new MockSolutionPickerView();
            MockHierarchyNode childProjectNode = new MockHierarchyNode {TypeGuid = new Guid(PrjKind.prjKindCSharpProject)};
            MockHierarchyNode targetProjectNode = new MockHierarchyNode {TypeGuid = new Guid(PrjKind.prjKindCSharpProject)};

            SolutionPickerPresenter presenter = new SolutionPickerPresenter(new MockHierarchyNode(), mockView, targetProjectNode);

            mockView.CanExit = false;

            mockView.SelectedNode = childProjectNode;
            mockView.FireSelectedNodeChanged();

            Assert.IsTrue(mockView.CanExit);
        }

        [TestMethod]
        public void ShouldInitialCanExitFalse()
        {
            MockSolutionPickerView mockView = new MockSolutionPickerView();

            SolutionPickerPresenter presenter = new SolutionPickerPresenter(new MockHierarchyNode(), mockView, new MockHierarchyNode());

            Assert.IsFalse(mockView.CanExit);
        }

        [TestMethod]
        public void ShouldNotExitIfSelectedHierachyIsSameAsTarget()
        {
            MockSolutionPickerView mockView = new MockSolutionPickerView();
            MockHierarchyNode childProjectNode = new MockHierarchyNode {TypeGuid = new Guid(PrjKind.prjKindCSharpProject)};

            SolutionPickerPresenter presenter = new SolutionPickerPresenter(new MockHierarchyNode(), mockView, childProjectNode);

            mockView.CanExit = true;

            mockView.SelectedNode = childProjectNode;
            mockView.FireSelectedNodeChanged();

            Assert.IsFalse(mockView.CanExit);
        }

        [TestMethod]
        public void ShouldNotExitIfSelectedHierachyLanguageIsNotSameAsTarget()
        {
            MockSolutionPickerView mockView = new MockSolutionPickerView();
            MockHierarchyNode childProjectNode = new MockHierarchyNode {TypeGuid = new Guid(PrjKind.prjKindCSharpProject)};
            MockHierarchyNode targetProjectNode = new MockHierarchyNode {TypeGuid = new Guid(PrjKind.prjKindVBProject)};

            SolutionPickerPresenter presenter = new SolutionPickerPresenter(new MockHierarchyNode(), mockView, targetProjectNode);

            mockView.CanExit = true;

            mockView.SelectedNode = childProjectNode;
            mockView.FireSelectedNodeChanged();

            Assert.IsFalse(mockView.CanExit);
        }
    }

    internal class MockSolutionPickerView : ISolutionPickerView
    {
        public IHierarchyNode RootHierarchyNode;
        public bool CanExit { get; set; }

        public void SetRootHierarchyNode(IHierarchyNode value)
        {
            RootHierarchyNode = value;
        }


        public IHierarchyNode SelectedNode { get; set; }

        public event EventHandler SelectedNodeChanged;
        public bool CopyProjectItemsByDefault { get; set; }

        public void FireSelectedNodeChanged()
        {
            SelectedNodeChanged(this, EventArgs.Empty);
        }
    }
}