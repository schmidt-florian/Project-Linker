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
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Utility;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace ProjectLinker.Tests.Utility
{
    [TestClass]
    public class HierarchyHelperFixture
    {
        [TestMethod]
        public void ShouldReturnRootId()
        {
            HierarchyHelper hierarchyHelper = new HierarchyHelper();
            MockVsHierarchy2 mockHierarchy = new MockVsHierarchy2();
            uint itemId = hierarchyHelper.GetItemId(mockHierarchy, "");

            Assert.AreEqual(VSConstants.VSITEMID_ROOT, itemId);
        }

        [TestMethod]
        public void ShouldReturnIdOfFolderIfIsFirstChild()
        {
            HierarchyHelper hierarchyHelper = new HierarchyHelper();
            MockVsHierarchy2 mockHierarchy = new MockVsHierarchy2();
            Node folderNode = new Node {Name = "folder"};
            Node rootNode = new Node(true) {FirstChild = folderNode};
            mockHierarchy.AddNode(rootNode);

            uint itemId = hierarchyHelper.GetItemId(mockHierarchy, "folder");

            Assert.AreEqual(folderNode.ItemId, itemId);
        }

        [TestMethod]
        public void ShouldReturnIdOfFolderIfIsNextSiblingOfFirstChild()
        {
            HierarchyHelper hierarchyHelper = new HierarchyHelper();
            MockVsHierarchy2 mockHierarchy = new MockVsHierarchy2();
            Node folderNode = new Node {Name = "folder"};
            Node firstChildNode = new Node {Name = "otherItem", NextSibling = folderNode};
            Node rootNode = new Node(true) {FirstChild = firstChildNode};
            mockHierarchy.AddNode(rootNode);

            uint itemId = hierarchyHelper.GetItemId(mockHierarchy, "folder");

            Assert.AreEqual(folderNode.ItemId, itemId);
        }

        [TestMethod]
        public void ShouldReturnIdOfFolderIfInSubFolder()
        {
            HierarchyHelper hierarchyHelper = new HierarchyHelper();
            MockVsHierarchy2 mockHierarchy = new MockVsHierarchy2();
            Node subFolderNode = new Node {Name = "subfolder"};
            Node folderNode = new Node {Name = "folder", FirstChild = subFolderNode};
            Node firstChildNode = new Node {Name = "otherItem", NextSibling = folderNode};
            Node rootNode = new Node(true) {FirstChild = firstChildNode};
            mockHierarchy.AddNode(rootNode);

            uint itemId = hierarchyHelper.GetItemId(mockHierarchy, "folder\\subfolder");

            Assert.AreEqual(subFolderNode.ItemId, itemId);
        }

        [TestMethod]
        public void ShouldReturnIdOfFolderIfInSubFolderAsSecondSibling()
        {
            HierarchyHelper hierarchyHelper = new HierarchyHelper();
            MockVsHierarchy2 mockHierarchy = new MockVsHierarchy2();
            Node subFolderNode = new Node {Name = "subfolder"};
            Node folderNode = new Node {Name = "folder", FirstChild = subFolderNode};
            Node aSiblingNode = new Node {Name = "otherItem", NextSibling = folderNode};
            Node firstChildNode = new Node {Name = "yetanotherItem", NextSibling = aSiblingNode};
            Node rootNode = new Node(true) {FirstChild = firstChildNode};
            mockHierarchy.AddNode(rootNode);

            uint itemId = hierarchyHelper.GetItemId(mockHierarchy, "folder\\subfolder");

            Assert.AreEqual(subFolderNode.ItemId, itemId);
        }

        [TestMethod]
        public void ShouldReturnNilIfElementDoesntExists()
        {
            HierarchyHelper hierarchyHelper = new HierarchyHelper();
            MockVsHierarchy2 mockHierarchy = new MockVsHierarchy2();
            Node firstChildNode = new Node {Name = "anotherItem"};
            Node rootNode = new Node(true) {FirstChild = firstChildNode};
            mockHierarchy.AddNode(rootNode);

            uint itemId = hierarchyHelper.GetItemId(mockHierarchy, "folder\\subfolder");

            Assert.AreEqual(VSConstants.VSITEMID_NIL, itemId);
        }
    }

    public class Node
    {
        private static uint _currentItemId = 1;
        public Node FirstChild;
        public string Name;
        public Node NextSibling;

        public Node() : this(false)
        {
        }

        public Node(bool isRoot)
        {
            ItemId = isRoot ? VSConstants.VSITEMID_ROOT : _currentItemId++;
        }

        public uint ItemId { get; private set; }
    }

    internal class MockVsHierarchy2 : IVsHierarchy
    {
        private readonly Dictionary<uint, Node> _nodes = new Dictionary<uint, Node>();

        public int GetProperty(uint itemid, int propid, out object pvar)
        {
            Node node = _nodes[itemid];
            switch (propid)
            {
                case (int) __VSHPROPID.VSHPROPID_FirstChild:
                    pvar = node.FirstChild?.ItemId ?? VSConstants.VSITEMID_NIL;
                    break;
                case (int) __VSHPROPID.VSHPROPID_NextSibling:
                    pvar = node.NextSibling?.ItemId ?? VSConstants.VSITEMID_NIL;
                    break;
                case (int) __VSHPROPID.VSHPROPID_Name:
                    pvar = node.Name;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return VSConstants.S_OK;
        }

        public void AddNode(Node node)
        {
            _nodes.Add(node.ItemId, node);
            if (node.FirstChild != null)
            {
                AddNode(node.FirstChild);
            }
            if (node.NextSibling != null)
            {
                AddNode(node.NextSibling);
            }
        }

        #region IVsHierarchy

        int IVsHierarchy.SetSite(IServiceProvider psp)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.GetSite(out IServiceProvider ppSp)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.QueryClose(out int pfCanClose)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Close()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.SetProperty(uint itemid, int propid, object var)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused0()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.UnadviseHierarchyEvents(uint dwCookie)
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused1()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused2()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused3()
        {
            throw new NotImplementedException();
        }

        int IVsHierarchy.Unused4()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}