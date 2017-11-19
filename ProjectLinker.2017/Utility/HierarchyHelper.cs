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

using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectLinker.Utility
{
    public class HierarchyHelper : IHierarchyHelper
    {
        /// <summary>
        ///     Gets the item ID for an item inside the project hierarchy.
        /// </summary>
        /// <param name="projectHierarchy">
        ///     Project from which Item Id will be retrieved. This cannot be a solution hierarchy or
        ///     have sub-hierarchies.
        /// </param>
        /// <param name="folderRelativePath">The relative path to the item in the hierarchy.</param>
        /// <returns>The ID of the item, or VSConstants.VSITEMID_NIL if not found.</returns>
        public uint GetItemId(IVsHierarchy projectHierarchy, string folderRelativePath)
        {
            string[] folderPathElements = folderRelativePath.Split(Path.DirectorySeparatorChar);

            if (folderPathElements.Length == 1 && string.IsNullOrEmpty(folderPathElements[0]))
            {
                return VSConstants.VSITEMID_ROOT;
            }

            uint elementItemId = GetItemIdInHierarchy(projectHierarchy, VSConstants.VSITEMID_ROOT, folderPathElements, 0);
            return elementItemId;
        }

        private static uint GetItemIdInHierarchy(IVsHierarchy hierarchy, uint itemid, string[] elementRelativePath, int recursionLevel)
        {
            int hr;
            object pVar;

            if (itemid != VSConstants.VSITEMID_ROOT)
            {
                hr = hierarchy.GetProperty(itemid, (int) __VSHPROPID.VSHPROPID_Name, out pVar);
                ErrorHandler.ThrowOnFailure(hr);
                if ((string) pVar != elementRelativePath[recursionLevel - 1])
                {
                    return VSConstants.VSITEMID_NIL;
                }
                if (elementRelativePath.Length == recursionLevel)
                {
                    return itemid;
                }
            }

            //Get the first child node of the current hierarchy being walked
            hr = hierarchy.GetProperty(itemid,
                (int) __VSHPROPID.VSHPROPID_FirstChild,
                out pVar);
            ErrorHandler.ThrowOnFailure(hr);
            if (VSConstants.S_OK == hr)
            {
                //We are using Depth first search so at each level we recurse to check if the node has any children
                // and then look for siblings.
                uint childId = GetItemId(pVar);
                while (childId != VSConstants.VSITEMID_NIL)
                {
                    uint returnedItemId = GetItemIdInHierarchy(hierarchy, childId, elementRelativePath,
                        recursionLevel + 1);
                    if (returnedItemId != VSConstants.VSITEMID_NIL)
                    {
                        return returnedItemId;
                    }

                    hr = hierarchy.GetProperty(childId,
                        (int) __VSHPROPID.VSHPROPID_NextSibling,
                        out pVar);
                    if (VSConstants.S_OK == hr)
                    {
                        childId = GetItemId(pVar);
                    }
                    else
                    {
                        ErrorHandler.ThrowOnFailure(hr);
                        break;
                    }
                }
            }

            return VSConstants.VSITEMID_NIL;
        }

        /// <summary>
        ///     Gets the item id.
        /// </summary>
        /// <param name="pvar">VARIANT holding an itemid.</param>
        /// <returns>Item Id of the concerned node</returns>
        private static uint GetItemId(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int i) return (uint) i;
            if (pvar is uint u) return u;
            if (pvar is short s) return (uint) s;
            if (pvar is ushort id) return id;
            if (pvar is long l) return (uint) l;
            return VSConstants.VSITEMID_NIL;
        }
    }

    public interface IHierarchyHelper
    {
        uint GetItemId(IVsHierarchy projectHierarchy, string folderRelativePath);
    }
}