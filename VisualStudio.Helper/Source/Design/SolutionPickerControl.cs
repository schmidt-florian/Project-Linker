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
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio;

namespace ProjectLinker.Helper.Design
{
    /// <summary>
    ///     User control that allows selection of a valid target given a filter.
    /// </summary>
    public partial class SolutionPickerControl : UserControl
    {
        private readonly object _childrenTag = new object();
        ISolutionPickerFilter _filter;
        readonly ISolutionPickerFilter _onSelectFilter = new DefaultProjectsOnlyFilter();

        IHierarchyNode _root;

        /// <summary>
        ///     Empty constructor for design-time support.
        /// </summary>
        public SolutionPickerControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Initializes the control receiving the root value and a filter
        ///     to customize the behavior of the control.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="filter"></param>
        public SolutionPickerControl(IHierarchyNode root, ISolutionPickerFilter filter = null)
        {
            Initialize(root, filter);
        }

        /// <summary>
        ///     Gets the target selected in the treeview.
        /// </summary>
        public IHierarchyNode SelectedTarget => (IHierarchyNode) solutionTree.SelectedNode?.Tag;

        /// <summary>
        ///     Event rised whenever the user selects a new node in the tree.
        /// </summary>
        public event EventHandler SelectionChanged;

        private void Initialize(IHierarchyNode rootNode, ISolutionPickerFilter pickerFilter)
        {
            _filter = pickerFilter;
            _root = rootNode;
            InitializeComponent();
            SuspendLayout();
            CreateNode(solutionTree.Nodes, _root);
            solutionTree.Nodes[0].Expand();
            ResumeLayout(false);
        }

        private TreeNode CreateNode(TreeNodeCollection parentCollection, IHierarchyNode hierarchyNode)
        {
            Debug.Assert(hierarchyNode.Icon != null);
            TreeNode node = new TreeNode(hierarchyNode.Name);
            if (!treeIcons.Images.ContainsKey(hierarchyNode.IconKey) && hierarchyNode.Icon != null)
            {
                treeIcons.Images.Add(hierarchyNode.IconKey, hierarchyNode.Icon);
            }
            node.ImageKey = hierarchyNode.IconKey;
            node.SelectedImageKey = hierarchyNode.IconKey;
            node.Name = hierarchyNode.Name;
            node.Tag = hierarchyNode;
            if (hierarchyNode.HasChildren)
            {
                bool filterAll = true;
                foreach (IHierarchyNode child in hierarchyNode.Children)
                {
                    if (!Filter(child))
                    {
                        filterAll = false;
                        break;
                    }
                }
                if (!filterAll)
                {
                    TreeNode firstChildNode = new TreeNode {Tag = _childrenTag};
                    node.Nodes.Add(firstChildNode);
                }
            }
            parentCollection.Add(node);
            return node;
        }

        private void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag == _childrenTag)
            {
                e.Node.Nodes.Remove(e.Node.Nodes[0]);
                IHierarchyNode hierarchyNode = (IHierarchyNode) e.Node.Tag;
                foreach (IHierarchyNode child in hierarchyNode.Children)
                {
                    if (!Filter(child))
                    {
                        CreateNode(e.Node.Nodes, child);
                    }
                }
            }
        }

        private bool Filter(IHierarchyNode node)
        {
            if (_filter != null && _filter.Filter(node))
            {
                return true;
            }
            return false;
        }

        private bool SelectFilter(IHierarchyNode node)
        {
            if (_onSelectFilter != null && _onSelectFilter.Filter(node))
            {
                return true;
            }

            return false;
        }

        private void OnSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (SelectionChanged != null && e.Node.Tag is IHierarchyNode node)
            {
                SelectionChanged(this, new EventArgs());
            }
        }


        internal class DefaultProjectsOnlyFilter : ISolutionPickerFilter
        {
            #region ISolutionPickerFilter Members

            public bool Filter(IHierarchyNode node)
            {
                return !(node.ExtObject is Project) ||
                       node.TypeGuid == VSConstants.GUID_ItemType_VirtualFolder;
            }

            #endregion
        }
    }
}