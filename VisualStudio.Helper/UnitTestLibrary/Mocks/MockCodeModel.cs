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

namespace ProjectLinker.UnitTestLibrary.Mocks
{
    public class MockCodeModel : CodeModel
    {
        public MockCodeModel(string language)
        {
            Language = language;
        }

        #region CodeModel Members

        public CodeAttribute AddAttribute(string name, object location, string value, object position)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeClass AddClass(string name, object location, object position, object bases, object implementedInterfaces, vsCMAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeDelegate AddDelegate(string name, object location, object type, object position, vsCMAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeEnum AddEnum(string name, object location, object position, object bases, vsCMAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeFunction AddFunction(string name, object location, vsCMFunction kind, object type, object position, vsCMAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeInterface AddInterface(string name, object location, object position, object bases, vsCMAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeNamespace AddNamespace(string name, object location, object position)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeStruct AddStruct(string name, object location, object position, object bases, object implementedInterfaces, vsCMAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeVariable AddVariable(string name, object location, object type, object position, vsCMAccess access)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeElements CodeElements => throw new Exception("The method or operation is not implemented.");

        public CodeType CodeTypeFromFullName(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public CodeTypeRef CreateCodeTypeRef(object type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DTE DTE => throw new Exception("The method or operation is not implemented.");

        public bool IsCaseSensitive => throw new Exception("The method or operation is not implemented.");

        public bool IsValidID(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string Language { get; internal set; }

        public Project Parent => throw new Exception("The method or operation is not implemented.");

        public void Remove(object element)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}