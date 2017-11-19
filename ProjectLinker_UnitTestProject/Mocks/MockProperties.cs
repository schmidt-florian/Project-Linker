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
using System.Linq;
using EnvDTE;
using ProjectLinker.Utility;

namespace ProjectLinker.Tests.Mocks
{
    class MockProperties : Properties
    {
        public List<Property> PropertiesList = new List<Property>();

        public MockProperties(object parent)
        {
            Parent = parent;
        }

        public object Application => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public DTE DTE => throw new NotImplementedException();

        public IEnumerator GetEnumerator()
        {
            EnsureFullPathProperty();
            foreach (Property property in PropertiesList)
            {
                yield return property;
            }
        }

        public Property Item(object index)
        {
            EnsureFullPathProperty();
            throw new NotImplementedException();
        }

        public object Parent { get; set; }

        private void EnsureFullPathProperty()
        {
            if (Parent is ProjectItem parentProjectItem && PropertiesList.FirstOrDefault(x => x.Name == "FullPath") == null)
            {
                MockProperty fullpath = new MockProperty {Name = "FullPath"};
                if (parentProjectItem.Collection.Parent is ProjectItem projectItem)
                {
                    fullpath.Value = Path.Combine(projectItem.Properties.GetValue("FullPath", string.Empty), parentProjectItem.Name);
                }
                else
                {
                    fullpath.Value = Path.Combine(Path.GetDirectoryName(((Project) parentProjectItem.Collection.Parent).FullName), parentProjectItem.Name);
                }
                PropertiesList.Add(fullpath);
            }
        }
    }

    class MockProperty : Property
    {
        public MockProperty()
        {
        }

        public MockProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public object Value { get; set; }

        public object Application => throw new NotImplementedException();

        public Properties Collection => throw new NotImplementedException();

        public DTE DTE => throw new NotImplementedException();


        public short NumIndices => throw new NotImplementedException();

        public object Object
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Properties Parent => throw new NotImplementedException();

        public object get_IndexedValue(object index1, object index2, object index3, object index4)
        {
            throw new NotImplementedException();
        }

        public void let_Value(object lppvReturn)
        {
            throw new NotImplementedException();
        }

        public void set_IndexedValue(object index1, object index2, object index3, object index4, object val)
        {
            throw new NotImplementedException();
        }
    }
}