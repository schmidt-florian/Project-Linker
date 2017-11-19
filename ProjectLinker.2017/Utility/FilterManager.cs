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
using System.Text;
using EnvDTE;

namespace ProjectLinker.Utility
{
    public static class FilterManager
    {
        private const string ProjectLinkerExcludeFilterKey = "ProjectLinkerExcludeFilter";
        private const char ExpressionsSeparator = ';';

        private static readonly string[] DefaultFilterExpressions =
        {
            @"\\?desktop(\\.*)?$", @"\\?silverlight(\\.*)?$", @"\.desktop", @"\.silverlight", @"\.xaml", @"^service references(\\.*)?$",
            @"\.clientconfig", @"^web references(\\.*)?$"
        };

        public static IProjectItemsFilter GetFilterForProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (!project.Globals.get_VariableExists(ProjectLinkerExcludeFilterKey))
            {
                SetFilterForProject(project, DefaultFilterExpressions);
            }
            string excludeFilterExpressions = (string) project.Globals[ProjectLinkerExcludeFilterKey];

            IProjectItemsFilter filter = new RegexProjectItemsFilter(excludeFilterExpressions.Split(new[] {';'},
                StringSplitOptions.RemoveEmptyEntries));

            return filter;
        }

        public static void SetFilterForProject(Project project, IEnumerable<string> filterExpressions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string filterExpression in filterExpressions)
            {
                if (sb.Length > 0)
                {
                    sb.Append(ExpressionsSeparator);
                }
                sb.Append(filterExpression);
            }

            project.Globals[ProjectLinkerExcludeFilterKey] = sb.ToString();
            project.Globals.set_VariablePersists(ProjectLinkerExcludeFilterKey, true);
        }
    }
}