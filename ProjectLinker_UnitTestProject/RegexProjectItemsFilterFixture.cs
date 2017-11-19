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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectLinker.Tests
{
    [TestClass]
    public class RegexProjectItemsFilterFixture
    {
        [TestMethod]
        public void ShouldCreateFilter()
        {
            RegexProjectItemsFilter filter = new RegexProjectItemsFilter(new[] {".*"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowOnInvalidArgument()
        {
            RegexProjectItemsFilter filter = new RegexProjectItemsFilter(null);
        }

        [TestMethod]
        public void ShouldFilterOutItem()
        {
            RegexProjectItemsFilter filter = new RegexProjectItemsFilter(new[] {@"\.xaml"});
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\myTestString.xaml"));
        }

        [TestMethod]
        public void ShouldFilterInItem()
        {
            RegexProjectItemsFilter filter = new RegexProjectItemsFilter(new[] {@"\.xaml"});
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\myTestString.cs"));
        }
    }
}