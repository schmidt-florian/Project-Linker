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
using System.ComponentModel;

namespace ProjectLinker.Helper.Tests.Design
{
    internal class MockTypeDescriptorContext : ITypeDescriptorContext
    {
        readonly IServiceProvider _serviceProvider;

        public MockTypeDescriptorContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        #endregion

        #region ITypeDescriptorContext Members

        IContainer ITypeDescriptorContext.Container => throw new Exception("The method or operation is not implemented.");

        object ITypeDescriptorContext.Instance => throw new Exception("The method or operation is not implemented.");

        void ITypeDescriptorContext.OnComponentChanged()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool ITypeDescriptorContext.OnComponentChanging()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor => throw new Exception("The method or operation is not implemented.");

        #endregion
    }
}