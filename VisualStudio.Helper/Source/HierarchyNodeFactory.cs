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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectLinker.Helper
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class HierarchyNodeFactory : IHierarchyNodeFactory
    {
        private readonly IServiceProvider _provider;
        private readonly IVsSolution _solution;

        public HierarchyNodeFactory(IServiceProvider provider)
        {
            _provider = provider;
            _solution = provider.GetService(typeof(SVsSolution)) as IVsSolution;
        }

        public IHierarchyNode CreateFromProjectGuid(Guid projectGuid)
        {
            return new HierarchyNode(_solution, projectGuid);
        }

        public IHierarchyNode GetSelectedProject()
        {
            IVsUIHierarchyWindow uiWindow = VsShellUtilities.GetUIHierarchyWindow(_provider, new Guid(ToolWindowGuids.SolutionExplorer));
            uint pItemId;
            IVsMultiItemSelect itemSelection;
            int hr = uiWindow.GetCurrentSelection(out IntPtr pHier, out pItemId, out itemSelection);

            if (hr != VSConstants.S_OK)
            {
                throw new HierarchyNodeException("Could not retrieve tool window.");
            }

            IVsHierarchy selectedHier = Marshal.GetObjectForIUnknown(pHier) as IVsHierarchy;
            Debug.Assert(selectedHier != null);

            HierarchyNode node = new HierarchyNode(_solution, selectedHier);

            // TODO: Should we Marshall.Release selectedHier?

            return node;
        }
    }

    public interface IHierarchyNodeFactory
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        IHierarchyNode CreateFromProjectGuid(Guid projectGuid);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IHierarchyNode GetSelectedProject();
    }
}