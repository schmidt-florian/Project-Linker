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
using ProjectLinker.Tests.Mocks;
using ProjectLinker.Utility;

namespace ProjectLinker.Tests.Utility
{
    [TestClass]
    public class FilterManagerFixture
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ShouldValidateArgument()
        {
            FilterManager.GetFilterForProject(null);
        }

        [TestMethod]
        public void ShouldRetrieveDefaultFilter()
        {
            MockProject mockProject = new MockProject();

            IProjectItemsFilter result = FilterManager.GetFilterForProject(mockProject);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RegexProjectItemsFilter));
        }

        [TestMethod]
        public void DefaultFilterShouldFilterOutFiles()
        {
            MockProject mockProject = new MockProject();

            IProjectItemsFilter filter = FilterManager.GetFilterForProject(mockProject);

            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.silverlight.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.desktop"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder.Silverlight\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder.desktop\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\silverlight\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\Desktop\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.xaml"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\Service References\abc.xaml"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\Web References\abc.xaml"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\Service References\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\Web References\abc.cs"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.clientconfig"));
        }

        [TestMethod]
        public void ShouldRetrievePersistedFilters()
        {
            MockProject mockProject = new MockProject();
            mockProject.Globals.Dictionary["ProjectLinkerExcludeFilter"] = @"\.excludeMe;\.excludeMeToo";

            IProjectItemsFilter filter = FilterManager.GetFilterForProject(mockProject);

            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.silverlight.excludeMe"));
            Assert.IsFalse(filter.CanBeSynchronized(@"c:\MyFolder\abc.excludeMeToo"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\abc.silverlight.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\abc.desktop"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder.Silverlight\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder.desktop\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\silverlight\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\Desktop\abc.cs"));
            Assert.IsTrue(filter.CanBeSynchronized(@"c:\MyFolder\abc.xaml"));
        }

        [TestMethod]
        public void ShouldPersistDefaultFiltersOnProject()
        {
            MockProject mockProject = new MockProject();
            string defaultFilterExpressions =
                @"\\?desktop(\\.*)?$;\\?silverlight(\\.*)?$;\.desktop;\.silverlight;\.xaml;^service references(\\.*)?$;\.clientconfig;^web references(\\.*)?$";

            IProjectItemsFilter filter = FilterManager.GetFilterForProject(mockProject);

            Assert.IsTrue(mockProject.Globals.SetVariablePersistsCalled);
            Assert.AreEqual(defaultFilterExpressions, mockProject.Globals.Dictionary["ProjectLinkerExcludeFilter"]);
        }
    }
}