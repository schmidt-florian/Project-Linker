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
using System.Text.RegularExpressions;

namespace ProjectLinker
{
    public class RegexProjectItemsFilter : IProjectItemsFilter
    {
        private readonly List<Regex> _excludeFilters;

        public RegexProjectItemsFilter(IEnumerable<string> filterPatterns)
        {
            if (filterPatterns == null)
                throw new ArgumentNullException(nameof(filterPatterns));

            _excludeFilters = new List<Regex>();
            foreach (string filterPattern in filterPatterns)
            {
                _excludeFilters.Add(new Regex(filterPattern, RegexOptions.IgnoreCase));
            }
        }

        public bool CanBeSynchronized(string relativePath)
        {
            foreach (Regex filter in _excludeFilters)
            {
                if (filter.Match(relativePath).Success)
                    return false;
            }

            return true;
        }
    }
}