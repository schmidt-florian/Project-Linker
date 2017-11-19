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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EnvDTE;

namespace ProjectLinker.Tests.Mocks
{
    class MockProjectItems : ProjectItems, IEnumerable<ProjectItem>
    {
        public bool AddFolderCalled;
        public bool AddFromFileCalled;
        public int ErrorCode = -2147467259;
        private readonly List<ProjectItem> _projectItems = new List<ProjectItem>();
        public bool ThrowOnAddFolder;

        public MockProjectItems(object parent)
        {
            Parent = parent;
        }

        IEnumerator<ProjectItem> IEnumerable<ProjectItem>.GetEnumerator()
        {
            foreach (ProjectItem item in _projectItems)
            {
                yield return item;
            }
        }

        public ProjectItem AddFolder(string name, string kind)
        {
            AddFolderCalled = true;
            if (ThrowOnAddFolder)
                throw new COMException("Folder Exists in File System (from MockProjectItems)", ErrorCode);

            MockProjectItem projectItem = new MockProjectItem(name) {Kind = kind};
            AddProjectItem(projectItem);
            return projectItem;
        }

        public ProjectItem AddFromDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromFile(string fileName)
        {
            AddFromFileCalled = true;
            AddProjectItem(new MockProjectItem(fileName));
            ;
            return null;
        }

        public ProjectItem AddFromFileCopy(string filePath)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromTemplate(string fileName, string name)
        {
            throw new NotImplementedException();
        }

        public Project ContainingProject => throw new NotImplementedException();

        public int Count => _projectItems.Count;

        public DTE DTE => throw new NotImplementedException();

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<ProjectItem>) this).GetEnumerator();
        }

        public ProjectItem Item(object index)
        {
            if (index is int)
            {
                return _projectItems[(int) index];
            }
            if (index is string)
            {
                MockProjectItem item = new MockProjectItem("Stub mock created dynamically by calling Item(string)");
                AddProjectItem(item);
                return item;
            }
            return null;
        }

        public string Kind => throw new NotImplementedException();

        public object Parent { get; set; }

        public void AddProjectItem(MockProjectItem projectItem)
        {
            if (projectItem.Collection != null)
                throw new InvalidOperationException("Invalid use of the mock object");
            projectItem.Collection = this;
            _projectItems.Add(projectItem);
        }
    }
}