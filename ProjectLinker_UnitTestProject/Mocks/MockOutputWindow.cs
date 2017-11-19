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

namespace ProjectLinker.Tests.Mocks
{
    internal class MockOutputWindow : IVsOutputWindow
    {
        public Guid GetPaneArgumentGuidPane;
        public bool GetPaneCalled;
        public MockOutputWindowPane GetPaneReturnValue = new MockOutputWindowPane();

        public int GetPane(ref Guid rguidPane, out IVsOutputWindowPane ppPane)
        {
            GetPaneCalled = true;
            GetPaneArgumentGuidPane = rguidPane;
            ppPane = GetPaneReturnValue;
            return VSConstants.S_OK;
        }

        #region Implementation of IVsOutputWindow

        int IVsOutputWindow.CreatePane(ref Guid rguidPane, string pszPaneName, int fInitVisible, int fClearWithSolution)
        {
            throw new NotImplementedException();
        }

        int IVsOutputWindow.DeletePane(ref Guid rguidPane)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}