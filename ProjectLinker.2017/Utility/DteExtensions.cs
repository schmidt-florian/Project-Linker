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
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectLinker.Utility
{
    //FxCop: Naming consistent with Visual Studio
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dte")]
    public static class DteExtensions
    {
        public static T GetValue<T>(this Properties properties, string propertyName, T defaultValue)
        {
            foreach (Property prop in properties)
            {
                if (prop.Name == propertyName)
                {
                    return (T) prop.Value;
                }
            }
            return defaultValue;
        }

        //FxCop: Naming consistent with Visual Studio
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Vs")]
        public static IVsHierarchy GetVsHierarchy(this Project project, IVsSolution solution)
        {
            int hr = solution.GetProjectOfUniqueName(project.FullName, out IVsHierarchy hierarchy);
            ErrorHandler.ThrowOnFailure(hr);
            return hierarchy;
        }

        public static Guid GetProjectGuid(this Project project, IVsSolution solution)
        {
            return project.GetVsHierarchy(solution).GetProjectGuid();
        }
    }
}