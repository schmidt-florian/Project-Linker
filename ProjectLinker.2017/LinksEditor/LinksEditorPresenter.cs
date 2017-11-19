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
using System.Collections.ObjectModel;
using ProjectLinker.Helper;
using ProjectLinker.Services;

namespace ProjectLinker.LinksEditor
{
    public class LinksEditorPresenter
    {
        private readonly IHierarchyNodeFactory _hierarchyNodeFactory;
        private readonly IProjectLinkTracker _projectLinkTracker;
        private readonly ILinksEditorView _view;

        public LinksEditorPresenter(ILinksEditorView view, IProjectLinkTracker projectLinkTracker, IHierarchyNodeFactory hierarchyNodeFactory)
        {
            _view = view;
            _view.ProjectsUnlinking += (s, e) => UnlinkProjects(_view.SelectedProjectLinkItems);
            _projectLinkTracker = projectLinkTracker;
            _hierarchyNodeFactory = hierarchyNodeFactory;

            SetUpProjectLinks();
        }

        private void UnlinkProjects(IEnumerable<ProjectLinkItem> projectLinks)
        {
            foreach (ProjectLinkItem link in projectLinks)
            {
                _projectLinkTracker.UnlinkProjects(link.SourceProjectGuid, link.TargetProjectGuid);
            }

            SetUpProjectLinks();
        }

        private void SetUpProjectLinks()
        {
            Collection<ProjectLinkItem> projectLinks = new Collection<ProjectLinkItem>();

            foreach (ProjectLink projectLink in _projectLinkTracker.GetProjectLinks())
            {
                ProjectLinkItem item = new ProjectLinkItem
                {
                    SourceProjectName = GetProjectNameFromGuid(projectLink.SourceProjectId),
                    SourceProjectGuid = projectLink.SourceProjectId,
                    TargetProjectName = GetProjectNameFromGuid(projectLink.TargetProjectId),
                    TargetProjectGuid = projectLink.TargetProjectId
                };
                projectLinks.Add(item);
            }

            _view.ProjectLinks = projectLinks;

            IHierarchyNode selectedHierarchyNode = _hierarchyNodeFactory.GetSelectedProject();
            if (selectedHierarchyNode != null)
            {
                string selectedProjectRelativeName = selectedHierarchyNode.SolutionRelativeName;
                foreach (ProjectLinkItem linkItem in projectLinks)
                {
                    if (linkItem.TargetProjectName == selectedProjectRelativeName)
                    {
                        _view.SelectedProjectLinkItems = new Collection<ProjectLinkItem> {linkItem};
                        break;
                    }
                }
            }
        }

        private string GetProjectNameFromGuid(Guid projectGuid)
        {
            IHierarchyNode hierarchyNode = _hierarchyNodeFactory.CreateFromProjectGuid(projectGuid);
            return hierarchyNode.SolutionRelativeName;
        }
    }
}