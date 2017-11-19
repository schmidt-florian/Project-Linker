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

using EnvDTE;

namespace ProjectLinker
{
    public interface IProjectItemsSynchronizer
    {
        Project SourceProject { get; }
        Project TargetProject { get; }
        void FileAddedToSource(string file);
        void FileRemovedFromSource(string file);
        void FileRenamedInSource(string oldFileName, string newFileName);
        void DirectoryAddedToSource(string directory);
        void DirectoryRemovedFromSource(string directory);
        void DirectoryRenamedInSource(string oldDirectoryName, string newDirectoryName);
        void LinkAllFiles();
    }
}