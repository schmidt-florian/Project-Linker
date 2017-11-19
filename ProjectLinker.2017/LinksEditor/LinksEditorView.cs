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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using ProjectLinker.Helper;
using ProjectLinker.Services;

namespace ProjectLinker.LinksEditor
{
    public partial class LinksEditorView : Form, ILinksEditorView
    {
        private readonly IProjectLinkTracker _projectLinkTracker;
        private readonly IServiceProvider _provider;

        public LinksEditorView(IProjectLinkTracker projectLinkTracker, IServiceProvider provider)
        {
            _projectLinkTracker = projectLinkTracker;
            _provider = provider;
            InitializeComponent();
        }

        public event EventHandler ProjectsUnlinking;

        // FxCop: This sets the data context for the view from the presenter
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<ProjectLinkItem> ProjectLinks
        {
            set => gridProjectLinks.DataSource = value;
        }

        //FxCop: Allows selected content to be set from outside.  Consider moving this to a method instead of property
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public Collection<ProjectLinkItem> SelectedProjectLinkItems
        {
            get
            {
                Collection<ProjectLinkItem> selectedProjectLinks = new Collection<ProjectLinkItem>();
                foreach (DataGridViewRow row in gridProjectLinks.SelectedRows)
                {
                    selectedProjectLinks.Add(row.DataBoundItem as ProjectLinkItem);
                }

                return selectedProjectLinks;
            }

            set
            {
                foreach (DataGridViewRow row in gridProjectLinks.Rows)
                {
                    row.Selected = value.Contains((ProjectLinkItem) row.DataBoundItem);
                }
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Presenter is passed to view which and then monitors view activity based on events.
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId =
            "Microsoft.Practices.ProjectLinker.LinksEditor.LinksEditorPresenter")]
        private void LinksEditorView_Load(object sender, EventArgs e)
        {
            new LinksEditorPresenter(this, _projectLinkTracker, new HierarchyNodeFactory(_provider));
        }

        private void gridProjectLinks_SelectionChanged(object sender, EventArgs e)
        {
            buttonUnbind.Enabled = gridProjectLinks.SelectedCells.Count > 0;
        }

        private void buttonUnbind_Click(object sender, EventArgs e)
        {
            ProjectsUnlinking?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface ILinksEditorView
    {
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly"),
         SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        Collection<ProjectLinkItem> ProjectLinks { set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        Collection<ProjectLinkItem> SelectedProjectLinkItems { get; set; }

        event EventHandler ProjectsUnlinking;
    }
}