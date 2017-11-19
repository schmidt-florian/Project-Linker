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
using ProjectLinker.Helper;

namespace ProjectLinker.SolutionPicker
{
    public class SolutionPickerPresenter
    {
        private readonly IHierarchyNode _targetProject;
        private readonly ISolutionPickerView _view;

        public SolutionPickerPresenter(IHierarchyNode solutionNode, ISolutionPickerView view, IHierarchyNode targetProject)
        {
            _view = view;
            _targetProject = targetProject;
            view.CanExit = false;
            view.SetRootHierarchyNode(solutionNode);
            view.SelectedNodeChanged += view_SelectedNodeChanged;
        }

        void view_SelectedNodeChanged(object sender, EventArgs e)
        {
            IHierarchyNode node = _view.SelectedNode;
            bool isValidSelection = node.TypeGuid == _targetProject.TypeGuid;

            isValidSelection &= node.ProjectGuid != _targetProject.ProjectGuid;

            _view.CanExit = isValidSelection;
        }
    }
}