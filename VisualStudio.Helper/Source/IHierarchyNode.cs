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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace ProjectLinker.Helper
{
    public interface IHierarchyNode
    {
        Icon Icon { get; }

        /// <summary>
        ///     Name of this node
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Returns the Key to index icons in an image collection
        /// </summary>
        string IconKey { get; }

        /// <summary>
        ///     Returns true is there is al least one child under this node
        /// </summary>
        bool HasChildren { get; }

        IEnumerable<IHierarchyNode> Children { get; }

        /// <summary>
        ///     Returns the TypeGUID
        /// </summary>
        Guid TypeGuid { get; }

        /// <summary>
        ///     Returns the Project GUID
        /// </summary>
        Guid ProjectGuid { get; }

        /// <summary>
        ///     Returns the extensibility object
        /// </summary>
        object ExtObject { get; }

        /// <summary>
        ///     Returns true if the current node is the solution root
        /// </summary>
        bool IsSolution { get; }

        uint ItemId { get; }
        string SolutionRelativeName { get; }

        /// <summary>
        ///     Queries the type T to the internal hierarchy object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        T GetObject<T>()
            where T : class;
    }
}