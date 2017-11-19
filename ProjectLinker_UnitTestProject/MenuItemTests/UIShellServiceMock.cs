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
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;

namespace ProjectLinker.Tests.MenuItemTests
{
    static class UIShellServiceMock
    {
        private static GenericMockFactory _uiShellFactory;

        #region UiShell Getters

        /// <summary>
        ///     Returns an IVsUiShell that does not implement any methods
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetUiShellInstance()
        {
            if (_uiShellFactory == null)
            {
                _uiShellFactory = new GenericMockFactory("UiShell", new[] {typeof(IVsUIShell), typeof(IVsUIShellOpenDocument)});
            }
            BaseMock uiShell = _uiShellFactory.GetInstance();
            return uiShell;
        }

        /// <summary>
        ///     Get an IVsUiShell that implements SetWaitCursor, SaveDocDataToFile, ShowMessageBox
        /// </summary>
        /// <returns>uishell mock</returns>
        internal static BaseMock GetUiShellInstance0()
        {
            BaseMock uiShell = GetUiShellInstance();
            string name = $"{typeof(IVsUIShell).FullName}.{"SetWaitCursor"}";
            uiShell.AddMethodCallback(name, SetWaitCursorCallBack);

            name = $"{typeof(IVsUIShell).FullName}.{"SaveDocDataToFile"}";
            uiShell.AddMethodCallback(name, SaveDocDataToFileCallBack);

            name = $"{typeof(IVsUIShell).FullName}.{"ShowMessageBox"}";
            uiShell.AddMethodCallback(name, ShowMessageBoxCallBack);
            return uiShell;
        }

        #endregion

        #region Callbacks

        private static void SetWaitCursorCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void SaveDocDataToFileCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void ShowMessageBoxCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
            arguments.SetParameter(10, (int) DialogResult.Yes);
        }

        #endregion
    }
}