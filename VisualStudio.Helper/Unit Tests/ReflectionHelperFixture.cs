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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectLinker.Helper.Tests
{
    [TestClass]
    public class ReflectionHelperFixture
    {
        [TestMethod]
        public void GetAttributeHandlesNull()
        {
            const bool inherit = false;
            MockAttributeProvider provider = new MockAttributeProvider(null, inherit);
            object result = ReflectionHelper.GetAttribute<object>(provider, inherit);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAttributeReturnsNullIfMultipleFound()
        {
            const bool inherit = false;
            MyAttribute attrib = new MyAttribute();
            MockAttributeProvider provider = new MockAttributeProvider(new object[] {attrib, attrib}, inherit);
            MyAttribute result = ReflectionHelper.GetAttribute<MyAttribute>(provider, inherit);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAttributeReturnsOnlyIfOneIsPresent()
        {
            const bool inherit = false;
            MyAttribute attrib = new MyAttribute();
            MockAttributeProvider provider = new MockAttributeProvider(new object[] {attrib}, inherit);
            MyAttribute result = ReflectionHelper.GetAttribute<MyAttribute>(provider, inherit);
            Assert.AreSame(attrib, result);
        }

        [TestMethod]
        public void GetAttributesHandlesNull()
        {
            const bool inherit = false;
            MockAttributeProvider provider = new MockAttributeProvider(null, inherit);
            object[] result = ReflectionHelper.GetAttributes<object>(provider, inherit);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void GetAttributesReturnsNotEmpty()
        {
            const bool inherit = false;
            MyAttribute attrib = new MyAttribute();
            object[] attribs = {attrib, attrib};
            MockAttributeProvider provider = new MockAttributeProvider(attribs, inherit);
            MyAttribute[] result = ReflectionHelper.GetAttributes<MyAttribute>(provider, inherit);
            Assert.AreEqual(attribs.Length, result.Length);
        }

        [TestMethod]
        public void GetAttributesReturnsOnlyOne()
        {
            const bool inherit = false;
            MyAttribute attrib = new MyAttribute();
            object[] attribs = {attrib};
            MockAttributeProvider provider = new MockAttributeProvider(attribs, inherit);
            MyAttribute[] result = ReflectionHelper.GetAttributes<MyAttribute>(provider, inherit);
            Assert.AreEqual(attribs.Length, result.Length);
        }

        [TestMethod]
        public void GetTypeByInterfaceReturns()
        {
            List<Type> seedTypeList = new List<Type> {typeof(MockAttributeProvider)};

            IList<Type> foundTypes = ReflectionHelper.GetTypesByInterface(
                seedTypeList,
                typeof(ICustomAttributeProvider));

            Assert.AreEqual(1, foundTypes.Count);
            Assert.AreEqual(typeof(MockAttributeProvider), foundTypes[0]);
        }

        [TestMethod]
        public void CanGetProviderFromPropertyMember()
        {
            ICustomAttributeProvider provider = ReflectionHelper.GetProvider<MyType>("Element");

            Assert.IsNotNull(provider);
        }

        #region Internal classes 

        internal class MockAttributeProvider : ICustomAttributeProvider
        {
            readonly object[] _attribs;
            readonly bool _inherit;

            public MockAttributeProvider(object[] attribs, bool inherit)
            {
                _attribs = attribs;
                _inherit = inherit;
            }

            #region ICustomAttributeProvider Members

            object[] ICustomAttributeProvider.GetCustomAttributes(bool inherit)
            {
                Assert.AreEqual(_inherit, inherit);
                return _attribs;
            }

            object[] ICustomAttributeProvider.GetCustomAttributes(Type attributeType, bool inherit)
            {
                Assert.AreEqual(_inherit, inherit);
                return _attribs;
            }

            bool ICustomAttributeProvider.IsDefined(Type attributeType, bool inherit)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        internal class MyAttribute
        {
        }

        internal class MyType
        {
            public string Element { get; } = "";
        }

        #endregion
    }
}