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
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectLinker.Utility
{
    public static class VisualStudioExtensibilityExtensions
    {
        public static Guid GetProjectGuid(this IVsHierarchy hierarchy)
        {
            Debug.Assert(hierarchy != null);
            ErrorHandler.ThrowOnFailure(
                hierarchy.GetGuidProperty(
                    VSConstants.VSITEMID_ROOT,
                    (int) __VSHPROPID.VSHPROPID_ProjectIDGuid,
                    out Guid projectGuid));

            return projectGuid;
        }

        public static Project GetProject(this IVsHierarchy hierarchy)
        {
            ErrorHandler.ThrowOnFailure
            (hierarchy.GetProperty(
                VSConstants.VSITEMID_ROOT,
                (int) __VSHPROPID.VSHPROPID_ExtObject,
                out object project));
            return project as Project;
        }
    }
}