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
using EnvDTE;

namespace ProjectLinker.Tests.Mocks
{
    internal class MockProjectItemsSyncer : IProjectItemsSynchronizer
    {
        public bool DirectoriesAddedToSourceCalled;
        public bool DirectoriesRemovedFromSourceCalled;
        public bool DirectoriesRenamedInSourceCalled;
        public bool FilesAddedToSourceCalled;
        public bool FilesRemovedFromSourceCalled;
        public bool FilesRenamedInSourceCalled;

        public MockProjectItemsSyncer(Project source, Project target)
        {
            SourceProject = source;
            TargetProject = target;
        }

        public Project SourceProject { get; set; }
        public Project TargetProject { get; set; }

        public void FileAddedToSource(string file)
        {
            FilesAddedToSourceCalled = true;
        }

        public void FileRemovedFromSource(string file)
        {
            FilesRemovedFromSourceCalled = true;
        }

        public void DirectoryAddedToSource(string directory)
        {
            DirectoriesAddedToSourceCalled = true;
        }


        public void DirectoryRemovedFromSource(string directory)
        {
            DirectoriesRemovedFromSourceCalled = true;
        }


        public void FileRenamedInSource(string oldFileName, string newFileName)
        {
            FilesRenamedInSourceCalled = true;
        }

        public void DirectoryRenamedInSource(string oldDirectoryName, string newDirectoryName)
        {
            DirectoriesRenamedInSourceCalled = true;
        }

        public void LinkAllFiles()
        {
            throw new NotImplementedException();
        }
    }
}