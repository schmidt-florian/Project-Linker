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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Tests.Mocks;
using ProjectLinker.Utility;
using Constants = EnvDTE.Constants;

namespace ProjectLinker.Tests
{
    [TestClass]
    public class ProjectItemsSyncherFixture
    {
        [TestMethod]
        public void ShouldAddItemToTargetWhenAddingToSourceProject()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockLogger mockLogger = new MockLogger();
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("ABC.txt"));

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            string fileToAdd = Path.Combine(@"c:\mockPath1", @"ABC.txt");

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "ABC.txt");
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "added");
            StringAssert.Contains(mockLogger.MessageLog[0], "ABC.txt");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldAddMultipleItemsToTarget()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("ABC.txt"));
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("123.txt"));

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            string fileToAdd1 = Path.Combine(@"c:\mockPath1", @"ABC.txt");
            string fileToAdd2 = Path.Combine(@"c:\mockPath1", @"123.txt");

            syncher.FileAddedToSource(fileToAdd1);
            syncher.FileAddedToSource(fileToAdd2);

            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(2, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "ABC.txt");
            StringAssert.EndsWith(targetProject.ProjectItems.Item(1).Name, "123.txt");
        }

        [TestMethod]
        public void ShouldAddFileInCorrectSubfolder()
        {
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem sourceFolder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            sourceFolder.ProjectItems.AddProjectItem(new MockProjectItem("MyFile.txt"));

            MockProjectItem targetFolder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            targetProject.ProjectItems.AddProjectItem(targetFolder);

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            string fileToAdd = @"c:\mockPath1\MyFolder\MyFile.txt";

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetFolder.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetFolder.ProjectItems.Count);
            StringAssert.EndsWith(targetFolder.ProjectItems.Item(0).Name, "MyFile.txt");
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "added");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyFolder\MyFile.txt");
        }

        [TestMethod]
        public void ShouldRemoveLinkedFileWhenDeletingFromSource()
        {
            string sourceFile = @"c:\mockPath1\MyClass.cs";
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem targetFile = new MockProjectItem(sourceFile, true);
            targetProject.ProjectItems.AddProjectItem(targetFile);

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyClass.cs");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldNotRemoveFileIfTargetFileIsNotALink()
        {
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem targetFile = new MockProjectItem("MyClass.cs") {Kind = Constants.vsProjectItemKindPhysicalFile};
            targetProject.ProjectItems.AddProjectItem(targetFile);

            string sourceFile = Path.Combine(@"c:\mockPath1", @"MyClass.cs");
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsFalse(targetFile.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "not linked");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyClass.cs");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldRemoveLinkedFileInSubFolderWhenDeletingFromSource()
        {
            string sourceFile = @"c:\mockPath1\SubFolder\MyClass.cs";
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem subFolder = new MockProjectItem("SubFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            MockProjectItem targetFile = new MockProjectItem(sourceFile, true);
            subFolder.ProjectItems.AddProjectItem(targetFile);
            targetProject.ProjectItems.AddProjectItem(subFolder);

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
        }

        [TestMethod]
        public void ShouldRenameLinkedFile()
        {
            string oldSourceFile = Path.Combine(@"c:\mockPath1", @"MyOldFilename.cs");
            string newSourceFile = Path.Combine(@"c:\mockPath1", @"MyNewFilename.cs");

            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("MyNewFilename.cs"));
            MockProjectItem targetFile = new MockProjectItem(oldSourceFile, true);
            targetProject.ProjectItems.AddProjectItem(targetFile);

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRenamedInSource(oldSourceFile, newSourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.IsNotNull(targetProject.ProjectItems.FirstOrDefault(x => x.Name.EndsWith("MyNewFilename.cs")));
        }

        [TestMethod]
        public void ShouldCreateFolderStructureWhenAddingLinkedFile()
        {
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem sourceFolder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            sourceFolder.ProjectItems.AddProjectItem(new MockProjectItem("MyFile.txt"));

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            string fileToAdd = @"c:\mockPath1\MyFolder\MyFile.txt";

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsNotNull(targetProject.ProjectItems.Item(0));
            Assert.AreEqual(targetProject.ProjectItems.Item(0).Name, "MyFolder");
            Assert.IsNotNull(targetProject.ProjectItems.Item(0).ProjectItems);
            Assert.AreEqual(1, targetProject.ProjectItems.Item(0).ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).ProjectItems.Item(0).Name, "MyFile.txt");
            Assert.IsTrue(mockLogger.MessageLog.Count > 1);
            string loggedMessage = mockLogger.MessageLog.FirstOrDefault(x => x.IndexOf("folder", StringComparison.OrdinalIgnoreCase) >= 0);
            Assert.IsNotNull(loggedMessage);
            StringAssert.Contains(loggedMessage, "created");
            Assert.AreEqual(-1, loggedMessage.IndexOf("MyFile.txt"));
        }

        [TestMethod]
        public void ShouldRemoveLinkedFolderWhenDeletingFolderFromSource()
        {
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem folder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            targetProject.ProjectItems.AddProjectItem(folder);

            string sourceFolder = Path.Combine(@"c:\mockPath1", @"MyFolder");
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.DirectoryRemovedFromSource(sourceFolder);

            Assert.IsTrue(folder.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyFolder");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldAddDirectoryToTargetWhenAddingToSourceProject()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\MyFolder\";

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFolderCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "MyFolder");
        }

        [TestMethod]
        public void ShouldAddSubDirectoryToTargetWhenAddingToSourceProject()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();

            targetProject.ProjectItems.AddFolder("Folder1", Constants.vsProjectItemKindPhysicalFolder);
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\Folder1\Folder2";

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFolderCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Item(0).ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).ProjectItems.Item(0).Name, "Folder2");
        }

        [TestMethod]
        public void ShouldNotRemoveLinkedFolderWithNestedFilesWhenDeletingFolderFromSource()
        {
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();

            MockProjectItem folder = new MockProjectItem("Folder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            MockProjectItem subFolder = new MockProjectItem("SubFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            subFolder.ProjectItems.AddProjectItem(new MockProjectItem("File.txt") {Kind = Constants.vsProjectItemKindPhysicalFile});
            folder.ProjectItems.AddProjectItem(subFolder);

            targetProject.ProjectItems.AddProjectItem(folder);

            string sourceFolder = Path.Combine(@"c:\mockPath1", @"Folder");
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.DirectoryRemovedFromSource(sourceFolder);

            Assert.IsFalse(folder.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "not removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"Folder");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void RemoveDirectoryWithEmptySubFolderShouldDeleteWholeFolderStructure()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem folder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            MockProjectItem subFolder = new MockProjectItem("EmptySubFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            folder.ProjectItems.AddProjectItem(subFolder);
            targetProject.ProjectItems.AddProjectItem(folder);

            string sourceFolder = Path.Combine(@"c:\mockPath1", @"MyFolder");
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.DirectoryRemovedFromSource(sourceFolder);

            Assert.IsTrue(folder.DeleteCalled);
        }

        [TestMethod]
        public void ShouldIncludeDirectoryInTargetWhenItsPresentInFileSystem()
        {
            MockIVsSolution mockSolution = new MockIVsSolution();
            MockVsHierarchy mockTargetVsHierarchy = new MockVsHierarchy();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject(@"c:\mockPath2\project1.csproj");
            mockTargetVsHierarchy.GetPropertyProjectValue = targetProject;
            mockSolution.Hierarchies.Add(mockTargetVsHierarchy);
            MockHierarchyHelper mockHierarchyHelper = new MockHierarchyHelper();
            MockLogger mockLogger = new MockLogger();
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, mockSolution, mockHierarchyHelper,
                new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\MyFolder\";
            targetProject.ProjectItems.ThrowOnAddFolder = true;

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(mockTargetVsHierarchy.AddItemCalled);
            Assert.AreEqual(VSConstants.VSITEMID_ROOT, mockTargetVsHierarchy.AddItemArgumentItemidLoc);
            Assert.AreEqual(string.Empty, mockTargetVsHierarchy.AddItemArgumentItemName);
            Assert.AreEqual<uint>(1, mockTargetVsHierarchy.AddItemArgumentFilesToOpen);
            Assert.AreEqual(@"c:\mockPath2\MyFolder", mockTargetVsHierarchy.AddItemArgumentArrayFilesToOpen[0]);
        }

        [TestMethod]
        public void ShouldIncludeSubDirectoryInTargetWhenItsPresentInFileSystem()
        {
            MockIVsSolution mockSolution = new MockIVsSolution();
            MockVsHierarchy mockTargetVsHierarchy = new MockVsHierarchy();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject(@"c:\mockPath2\project1.csproj");
            mockTargetVsHierarchy.GetPropertyProjectValue = targetProject;
            mockSolution.Hierarchies.Add(mockTargetVsHierarchy);

            targetProject.ProjectItems.AddFolder("Folder1", Constants.vsProjectItemKindPhysicalFolder);
            MockProjectItem folder1 = targetProject.ProjectItems.Item(0) as MockProjectItem;
            folder1.ProjectItems.ThrowOnAddFolder = true;

            MockHierarchyHelper mockHierarchyHelper = new MockHierarchyHelper();
            MockLogger mockLogger = new MockLogger();
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, mockSolution, mockHierarchyHelper,
                new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\Folder1\Folder2\";

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsTrue(mockTargetVsHierarchy.AddItemCalled);
            Assert.AreEqual(VSConstants.VSITEMID_ROOT, mockTargetVsHierarchy.AddItemArgumentItemidLoc);
            Assert.AreEqual(string.Empty, mockTargetVsHierarchy.AddItemArgumentItemName);
            Assert.AreEqual<uint>(1, mockTargetVsHierarchy.AddItemArgumentFilesToOpen);
            Assert.AreEqual(@"c:\mockPath2\Folder1\Folder2", mockTargetVsHierarchy.AddItemArgumentArrayFilesToOpen[0]);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "already exists");
            StringAssert.Contains(mockLogger.MessageLog[0], "included");
            StringAssert.Contains(mockLogger.MessageLog[0], @"Folder2");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(COMException))]
        public void ShouldThrowIfComExceptionErrorCodeIsNotTheExpected()
        {
            MockIVsSolution mockSolution = new MockIVsSolution();
            MockVsHierarchy mockTargetVsHierarchy = new MockVsHierarchy();
            mockSolution.Hierarchies.Add(mockTargetVsHierarchy);
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject(@"c:\mockPath2\project1.csproj");
            targetProject.ProjectItems.ThrowOnAddFolder = true;
            targetProject.ProjectItems.ErrorCode = VSConstants.S_FALSE;

            MockHierarchyHelper mockHierarchyHelper = new MockHierarchyHelper();
            MockLogger mockLogger = new MockLogger();
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, mockSolution, mockHierarchyHelper,
                new MockProjectItemsFilter());
            string directoryToAdd = @"c:\mockPath1\MyFolder\";

            syncher.DirectoryAddedToSource(directoryToAdd);
        }

        [TestMethod]
        public void ShouldCallRemoveAndAddDirectoryWhenDirectoryIsRenamedInSource()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem folder = new MockProjectItem("oldFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            targetProject.ProjectItems.AddProjectItem(folder);

            string oldFolderName = Path.Combine(@"c:\mockPath1", @"oldFolder");
            string newFolderName = @"c:\mockPath1\newFolder\";

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, new MockProjectItemsFilter());

            syncher.DirectoryRenamedInSource(oldFolderName, newFolderName);

            Assert.IsTrue(folder.DeleteCalled);
            Assert.IsTrue(targetProject.ProjectItems.AddFolderCalled);
        }

        [TestMethod]
        public void ShouldFilterAddedFileOnSource()
        {
            string fileToAdd = Path.Combine(@"c:\mockPath1", @"ABC.xaml");
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            sourceProject.ProjectItems.AddProjectItem(new MockProjectItem("ABC.xaml"));
            MockProject targetProject = new MockProject();
            MockProjectItemsFilter mockProjectItemsFilter = new MockProjectItemsFilter {IsSynchronizableReturnValue = false};
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, null, mockProjectItemsFilter);
            Assert.IsFalse(mockProjectItemsFilter.IsSynchronizableCalled);

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsFalse(targetProject.ProjectItems.AddFromFileCalled);
            Assert.IsTrue(mockProjectItemsFilter.IsSynchronizableCalled);
        }

        [TestMethod]
        public void ShouldFilterAddedDirectoryOnSource()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();

            MockProjectItemsFilter mockProjectItemsFilter = new MockProjectItemsFilter {IsSynchronizableReturnValue = false};
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, new MockLogger(), null, null, mockProjectItemsFilter);
            string directoryToAdd = Path.Combine(@"c:\mockPath1", "MyFolder");

            syncher.DirectoryAddedToSource(directoryToAdd);

            Assert.IsFalse(targetProject.ProjectItems.AddFolderCalled);
            Assert.IsTrue(mockProjectItemsFilter.IsSynchronizableCalled);
        }

        [TestMethod]
        public void ShouldLogMessageIfFileDoesNotExistInTargetProject()
        {
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();

            string sourceFile = Path.Combine(@"c:\mockPath1", @"MyClass.cs");
            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());

            syncher.FileRemovedFromSource(sourceFile);

            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "not linked");
            StringAssert.Contains(mockLogger.MessageLog[0], @"MyClass.cs");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldAddItemToTargetWhenAddingLinkedFileToSourceProject()
        {
            string fileToAdd = @"c:\alternativeExternalPath\file.txt";
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockLogger mockLogger = new MockLogger();
            MockProjectItem linkedSourceFile = new MockProjectItem("file.txt");
            linkedSourceFile.MockProperties.PropertiesList.Add(new MockProperty("FullPath", fileToAdd));
            sourceProject.ProjectItems.AddProjectItem(linkedSourceFile);

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetProject.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetProject.ProjectItems.Count);
            StringAssert.EndsWith(targetProject.ProjectItems.Item(0).Name, "file.txt");
        }

        [TestMethod]
        public void ShouldAddFileThatIsLinkInCorrectSubfolder()
        {
            string fileToAdd = @"c:\alternativeExternalPath\file.txt";
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem sourceFolder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            MockProjectItem linkedSourceFile = new MockProjectItem("file.txt");
            linkedSourceFile.MockProperties.PropertiesList.Add(new MockProperty("FullPath", fileToAdd));
            sourceFolder.ProjectItems.AddProjectItem(linkedSourceFile);

            MockProjectItem targetFolder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            targetProject.ProjectItems.AddProjectItem(targetFolder);

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(targetFolder.ProjectItems.AddFromFileCalled);
            Assert.AreEqual(1, targetFolder.ProjectItems.Count);
            StringAssert.EndsWith(targetFolder.ProjectItems.Item(0).Name, "file.txt");
        }

        [TestMethod]
        public void ShouldRemoveLinkedFileEvenWhenSourceIsALink()
        {
            string sourceFile = @"c:\alternativeExternalPath\file.txt";
            MockLogger mockLogger = new MockLogger();
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockProjectItem targetFile = new MockProjectItem(sourceFile, true);
            targetProject.ProjectItems.AddProjectItem(targetFile);

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, new MockProjectItemsFilter());
            Assert.AreEqual(1, targetProject.ProjectItems.Count);

            syncher.FileRemovedFromSource(sourceFile);

            Assert.IsTrue(targetFile.DeleteCalled);
            Assert.AreEqual(1, mockLogger.MessageLog.Count);
            StringAssert.Contains(mockLogger.MessageLog[0], "removed");
            StringAssert.Contains(mockLogger.MessageLog[0], @"file.txt");
            StringAssert.Contains(mockLogger.MessageLog[0], targetProject.Name);
        }

        [TestMethod]
        public void ShouldUseRelativePathWhenEvaluatingFilter()
        {
            MockProject sourceProject = new MockProject(@"c:\mockPath1\project1.csproj");
            MockProject targetProject = new MockProject();
            MockLogger mockLogger = new MockLogger();
            MockProjectItemsFilter mockFilter = new MockProjectItemsFilter();
            MockProjectItem sourceFolder = new MockProjectItem("MyFolder") {Kind = Constants.vsProjectItemKindPhysicalFolder};
            sourceProject.ProjectItems.AddProjectItem(sourceFolder);
            sourceFolder.ProjectItems.AddProjectItem(new MockProjectItem("ABC.txt"));

            ProjectItemsSynchronizer syncher = new ProjectItemsSynchronizer(sourceProject, targetProject, mockLogger, null, mockFilter);
            string fileToAdd = @"c:\mockPath1\MyFolder\ABC.txt";

            syncher.FileAddedToSource(fileToAdd);

            Assert.IsTrue(mockFilter.IsSynchronizableCalled);
            Assert.AreEqual(@"MyFolder\ABC.txt", mockFilter.IsSynchronizableArgument);
        }

        /*
         * Delete a folder that contains files excluded from the project (analyze best option)
         */
    }

    internal class MockProjectItemsFilter : IProjectItemsFilter
    {
        public string IsSynchronizableArgument;
        public bool IsSynchronizableCalled;
        public bool IsSynchronizableReturnValue = true;

        public bool CanBeSynchronized(string relativePath)
        {
            IsSynchronizableCalled = true;
            IsSynchronizableArgument = relativePath;
            return IsSynchronizableReturnValue;
        }
    }


    internal class MockHierarchyHelper : IHierarchyHelper
    {
        public uint GetItemId(IVsHierarchy projectHierarchy, string folderRelativePath)
        {
            return VSConstants.VSITEMID_ROOT;
        }
    }
}