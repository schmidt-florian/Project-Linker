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

namespace ProjectLinker.UnitTestLibrary.Mocks
{
    public class MockServiceProvider : IServiceProvider
    {
        readonly Dictionary<Type, object> _services;

        public MockServiceProvider()
        {
            _services = new Dictionary<Type, object>();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (_services.ContainsKey(serviceType))
            {
                return _services[serviceType];
            }
            return null;
        }

        #endregion

        public void AddService(Type serviceType, object serviceInstance)
        {
            _services.Add(serviceType, serviceInstance);
        }
    }
}