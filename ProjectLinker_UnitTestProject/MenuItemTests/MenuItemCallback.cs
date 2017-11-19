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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectLinker.Tests.MenuItemTests
{
    [TestClass]
    public class MenuItemTest
    {
        ///// <summary>
        ///// Verify that a new menu command object gets added to the OleMenuCommandService. 
        ///// This action takes place In the Initialize method of the Package object
        ///// </summary>
        //[TestMethod]
        //public void InitializeMenuCommand()
        //{
        //    // Create the package
        //    IVsPackage package = new ProjectLinkerPackage() as IVsPackage;
        //    Assert.IsNotNull(package, "The object does not implement IVsPackage");

        //    // Create a basic service provider
        //    OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
        //    serviceProvider.AddService(typeof(SVsTrackProjectDocuments), new MockDocumentTracker(), true);
        //    serviceProvider.AddService(typeof(SVsSolution), new MockIVsSolution(), true);

        //    // Site the package
        //    Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

        //    //Verify that the menu command can be found
        //    CommandID menuCommandID = new CommandID(Microsoft.ProjectLinker.GuidList.guidProjectLinkerCmdSet, (int)Microsoft.ProjectLinker.PkgCmdIDList.cmdidMyCommand);
        //    System.Reflection.MethodInfo info = typeof(Package).GetMethod("GetService", BindingFlags.Instance | BindingFlags.NonPublic);
        //    Assert.IsNotNull(info);
        //    OleMenuCommandService mcs = info.Invoke(package, new object[] { (typeof(IMenuCommandService)) }) as OleMenuCommandService;
        //    Assert.IsNotNull(mcs.FindCommand(menuCommandID));
        //}

        //[TestMethod]
        //public void MenuItemCallback()
        //{
        //    // Create the package
        //    IVsPackage package = new ProjectLinkerPackage() as IVsPackage;
        //    Assert.IsNotNull(package, "The object does not implement IVsPackage");

        //    // Create a basic service provider
        //    OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
        //    serviceProvider.AddService(typeof(SVsTrackProjectDocuments), new MockDocumentTracker(), true);
        //    serviceProvider.AddService(typeof(SVsSolution), new MockIVsSolution(), true);

        //    // Create a UIShell service mock and proffer the service so that it can called from the MenuItemCallback method
        //    BaseMock uishellMock = UIShellServiceMock.GetUiShellInstance();
        //    serviceProvider.AddService(typeof(SVsUIShell), uishellMock, true);

        //    // Site the package
        //    Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

        //    //Invoke private method on package class and observe that the method does not throw
        //    System.Reflection.MethodInfo info = package.GetType().GetMethod("MenuItemCallback", BindingFlags.Instance | BindingFlags.NonPublic);
        //    Assert.IsNotNull(info, "Failed to get the private method MenuItemCallback throug refplection");
        //    info.Invoke(package, new object[] { null, null });

        //    //Clean up services
        //    serviceProvider.RemoveService(typeof(SVsUIShell));

        //}
    }
}