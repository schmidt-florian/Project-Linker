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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Tests.Mocks;
using ProjectLinker.Utility;

namespace ProjectLinker.Tests.Utility
{
    [TestClass]
    public class VsipExtensionsFixture
    {
        [TestMethod]
        public void ShouldReturnProjectGuid()
        {
            MockVsHierarchy mockHierarchy = new MockVsHierarchy {GetPropertyProjectIdGuidValue = Guid.NewGuid()};

            Guid result = mockHierarchy.GetProjectGuid();

            Assert.IsTrue(mockHierarchy.GetPropertyCalled);
            Assert.AreEqual(VSConstants.VSITEMID_ROOT, mockHierarchy.GetPropertyArgumentItemId);
            Assert.AreEqual((int) __VSHPROPID.VSHPROPID_ProjectIDGuid, mockHierarchy.GetPropertyArgumentPropId);
            Assert.AreEqual(result, result);
        }
    }
}