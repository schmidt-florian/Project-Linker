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
using System.IO;
using EnvDTE;

namespace ProjectLinker.UnitTestLibrary.Mocks
{
    public class MockEnvDteProjectItems : ProjectItems
    {
        private readonly MockVsHierarchy _parentHierarchy;

        public MockEnvDteProjectItems(MockVsHierarchy parentHierarchy)
        {
            _parentHierarchy = parentHierarchy;
        }

        public ProjectItem Item(object index)
        {
            throw new NotImplementedException();
        }

        IEnumerator ProjectItems.GetEnumerator()
        {
            return new MockProjectItemsEnumerator(_parentHierarchy);
        }

        public ProjectItem AddFromFile(string fileName)
        {
            MockEnvDteProjectItem newItem = new MockEnvDteProjectItem(_parentHierarchy) {Name = fileName};
            _parentHierarchy.AddChild(Path.GetFileName(fileName));
            return newItem;
        }

        public ProjectItem AddFolder(string name, string kind)
        {
            MockEnvDteProjectItem newItem = new MockEnvDteProjectItem(_parentHierarchy)
            {
                Name = name,
                Kind = Constants.vsProjectItemKindPhysicalFolder
            };
            _parentHierarchy.AddFolder(Path.GetFileName(name));
            return newItem;
        }

        public ProjectItem AddFromTemplate(string fileName, string name)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromDirectory(string directory)
        {
            throw new NotImplementedException();
        }

        public ProjectItem AddFromFileCopy(string filePath)
        {
            throw new NotImplementedException();
        }

        public object Parent => null;

        public int Count => _parentHierarchy.Children.Count;

        public DTE DTE => throw new NotImplementedException();

        public string Kind => throw new NotImplementedException();

        public Project ContainingProject => throw new NotImplementedException();

        public IEnumerator GetEnumerator()
        {
            return new MockProjectItemsEnumerator(_parentHierarchy);
        }

        private class MockProjectItemsEnumerator : IEnumerator<MockEnvDteProjectItem>
        {
            private readonly MockVsHierarchy _hierarchy;
            private List<string>.Enumerator _hierarchyChildrenEnumerator;

            public MockProjectItemsEnumerator(MockVsHierarchy hierarchy)
            {
                _hierarchy = hierarchy;
                _hierarchyChildrenEnumerator = hierarchy.Children.GetEnumerator();
            }

            public bool MoveNext()
            {
                return _hierarchyChildrenEnumerator.MoveNext();
            }

            public void Reset()
            {
                _hierarchyChildrenEnumerator.Dispose();
                _hierarchyChildrenEnumerator = _hierarchy.Children.GetEnumerator();
            }

            object IEnumerator.Current => Current;

            public MockEnvDteProjectItem Current => new MockEnvDteProjectItem(_hierarchy) {Name = _hierarchyChildrenEnumerator.Current};

            public void Dispose()
            {
                _hierarchyChildrenEnumerator.Dispose();
            }
        }
    }
}