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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectLinker.Helper;
using ProjectLinker.Helper.Design;

namespace ProjectLinker.SolutionPicker
{
    public partial class SolutionPickerView : Form, ISolutionPickerView
    {
        private bool _canExit;
        private SolutionPickerControl _pickerControl;

        //FxCop: Presenter attaches to ISolutionPickerView and events once created.
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId =
            "Microsoft.Practices.ProjectLinker.SolutionPicker.SolutionPickerPresenter")]
        public SolutionPickerView(IServiceProvider provider, IHierarchyNode targetProject)
        {
            InitializeComponent();
            IVsSolution solution = provider.GetService(typeof(SVsSolution)) as IVsSolution;
            new SolutionPickerPresenter(new HierarchyNode(solution), this, targetProject);
            CenterToParent();
        }

        public void SetRootHierarchyNode(IHierarchyNode value)
        {
            UpdatePickerControl(value);
        }

        public bool CanExit
        {
            get => _canExit;
            set
            {
                _canExit = value;
                buttonOK.Enabled = _canExit;
            }
        }

        public IHierarchyNode SelectedNode => _pickerControl.SelectedTarget;

        public event EventHandler SelectedNodeChanged;


        public bool CopyProjectItemsByDefault
        {
            get => cbxCopyItems.Checked;
            set => cbxCopyItems.Checked = value;
        }

        private void UpdatePickerControl(IHierarchyNode rootNode)
        {
            if (_pickerControl != null)
            {
                contentPanel.Controls.Remove(_pickerControl);
            }

            _pickerControl = new SolutionPickerControl(rootNode, new OnlyProjectsFilter())
            {
                Left = 0,
                Top = 0,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Size = contentPanel.ClientSize
            };
            _pickerControl.SelectionChanged += _pickerControl_SelectionChanged;

            contentPanel.Controls.Add(_pickerControl);
        }

        void _pickerControl_SelectionChanged(object sender, EventArgs e)
        {
            EventHandler selectedNodeChangedHandler = SelectedNodeChanged;
            selectedNodeChangedHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}