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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Tests.Mocks;
using ProjectLinker.Utility;

namespace ProjectLinker.Tests.Utility
{
    [TestClass]
    public class DteExtensionsFixture
    {
        [TestMethod]
        public void ShouldReturnTheValueForName()
        {
            MockProperties properties = new MockProperties(null);
            object value = new object();
            properties.PropertiesList.Add(new MockProperty {Name = "TestName", Value = value});

            object result = properties.GetValue<object>("TestName", null);

            Assert.AreSame(value, result);
        }

        [TestMethod]
        public void ShouldReturnDefaultValueIfNameNotFound()
        {
            MockProperties properties = new MockProperties(null);
            object defaultValue = new object();
            properties.PropertiesList.Add(new MockProperty {Name = "TestName", Value = new object()});

            object result = properties.GetValue("DifferentName", defaultValue);

            Assert.AreSame(defaultValue, result);
        }
    }
}