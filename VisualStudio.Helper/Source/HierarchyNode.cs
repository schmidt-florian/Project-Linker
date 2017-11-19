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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectLinker.Helper.Design;
using ProjectLinker.Helper.Properties;

namespace ProjectLinker.Helper
{
    /// <summary>
    ///     Node in the solution explorer
    /// </summary>
    [TypeConverter(typeof(HierarchyNodeConverter))]
    public class HierarchyNode : IDisposable, IHierarchyNode
    {
        /// <summary>
        ///     hierarchy object
        /// </summary>
        private IVsHierarchy _hierarchy;


        private Icon _icon;

        private string _iconKey;

        /// <summary>
        ///     item id
        /// </summary>
        private uint _itemId;

        /// <summary>
        ///     Solution service
        /// </summary>
        private IVsSolution _solution;

        /// <summary>
        ///     Constructs a HierarchyNode at the solution root
        /// </summary>
        /// <param name="vsSolution"></param>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode(IVsSolution vsSolution)
            : this(vsSolution, Guid.Empty)
        {
        }

        /// <summary>
        ///     Constructs a HierarchyNode given the unique string identifier
        /// </summary>
        /// <param name="vsSolution"></param>
        /// <param name="projectUniqueName"></param>
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId =
            "Microsoft.VisualStudio.Shell.Interop.IVsSolution.GetProjectOfUniqueName(System.String,Microsoft.VisualStudio.Shell.Interop.IVsHierarchy@)")]
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode(IVsSolution vsSolution, string projectUniqueName)
        {
            Guard.ArgumentNotNull(vsSolution, "vsSolution");
            Guard.ArgumentNotNullOrEmptyString(projectUniqueName, "projectUniqueName");

            IVsHierarchy rootHierarchy;
            if (projectUniqueName.StartsWith("{", StringComparison.OrdinalIgnoreCase) &&
                projectUniqueName.EndsWith("}", StringComparison.OrdinalIgnoreCase))
            {
                projectUniqueName = projectUniqueName.Substring(1, projectUniqueName.Length - 2);
            }

            if (projectUniqueName.Length == Guid.Empty.ToString().Length &&
                projectUniqueName.Split('-').Length == 5)
            {
                Guid projectGuid = new Guid(projectUniqueName);
                int hr = vsSolution.GetProjectOfGuid(ref projectGuid, out rootHierarchy);
                Marshal.ThrowExceptionForHR(hr);
            }
            else
            {
                int hr = vsSolution.GetProjectOfUniqueName(projectUniqueName, out rootHierarchy);
                if (rootHierarchy == null)
                {
                    //Thrown if Project doesn't exist on solution
                    throw new InvalidOperationException(
                        String.Format(CultureInfo.CurrentCulture, Resources.InvalidProjectUniqueName, projectUniqueName));
                }
            }
            Init(vsSolution, rootHierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        ///     Constructs a HierarchyNode given the projectGuid
        /// </summary>
        /// <param name="vsSolution"></param>
        /// <param name="projectGuid"></param>
        // FXCOP: False positive
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode(IVsSolution vsSolution, Guid projectGuid)
        {
            Guard.ArgumentNotNull(vsSolution, "vsSolution");

            int hr = vsSolution.GetProjectOfGuid(ref projectGuid, out IVsHierarchy rootHierarchy);
            if (rootHierarchy == null)
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.CurrentCulture, Resources.ProjectDoesNotExist, projectGuid.ToString("b")),
                    Marshal.GetExceptionForHR(hr));
            }
            Init(vsSolution, rootHierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        ///     Constructs a hierarchy node at the root level of hierarchy
        /// </summary>
        /// <param name="vsSolution"></param>
        /// <param name="hierarchy"></param>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode(IVsSolution vsSolution, IVsHierarchy hierarchy)
        {
            Guard.ArgumentNotNull(vsSolution, "vsSolution");

            Init(vsSolution, hierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        ///     Builds a child HierarchyNode from the parent node
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childId"></param>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode(HierarchyNode parent, uint childId)
        {
            Guard.ArgumentNotNull(parent, "parent");

            Init(parent._solution, parent._hierarchy, childId);
        }

        /// <summary>
        ///     Returns true if this a root node of another node
        /// </summary>
        public bool IsRoot => VSConstants.VSITEMID_ROOT == _itemId;

        /// <summary>
        ///     Document cookie
        /// </summary>
        public uint DocCookie => GetProperty<uint>(__VSHPROPID.VSHPROPID_ItemDocCookie);

        /// <summary>
        ///     Name of this node
        /// </summary>

        public string CanonicalName
        {
            [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
            get
            {
                int hr = _hierarchy.GetCanonicalName(_itemId, out string name);
                Marshal.ThrowExceptionForHR(hr);
                if (name != null)
                {
                    return name;
                }
                return string.Empty;
            }
        }

        /// <summary>
        ///     Returns the unique string that identifies this node in the solution
        /// </summary>
        public string UniqueName
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                int hr = _solution.GetUniqueNameOfProject(_hierarchy, out string uniqueName);
                Marshal.ThrowExceptionForHR(hr);
                return uniqueName;
            }
        }

        /// <summary>
        ///     Icon handle of the node
        /// </summary>
        public IntPtr IconHandle => new IntPtr(GetProperty<int>(__VSHPROPID.VSHPROPID_IconHandle));

        /// <summary>
        ///     Icon index of the node
        /// </summary>
        public int IconIndex => GetProperty<int>(__VSHPROPID.VSHPROPID_IconIndex);

        /// <summary>
        ///     True if the Icon index of the node is valid
        /// </summary>
        public bool HasIconIndex => HasProperty(__VSHPROPID.VSHPROPID_IconIndex);

        /// <summary>
        ///     StateIcon index of the node
        /// </summary>
        public int StateIconIndex => GetProperty<int>(__VSHPROPID.VSHPROPID_StateIconIndex);

        /// <summary>
        ///     OverlayIcon index of the node
        /// </summary>
        public int OverlayIconIndex => GetProperty<int>(__VSHPROPID.VSHPROPID_OverlayIconIndex);

        /// <summary>
        ///     Imagelist Handle
        /// </summary>
        public IntPtr ImageListHandle => new IntPtr(GetProperty<int>(__VSHPROPID.VSHPROPID_IconImgList));

        protected IVsHierarchy Hierarchy => _hierarchy;

        protected IVsSolution Solution => _solution;

        /// <summary>
        ///     Returns the item id of the first child
        /// </summary>
        public uint FirstChildId => GetItemId(GetProperty<object>(IsSolution ? __VSHPROPID.VSHPROPID_FirstVisibleChild : __VSHPROPID.VSHPROPID_FirstChild));

        /// <summary>
        ///     Returns the file name of the hierarcynode
        /// </summary>
        public string SaveName => GetProperty<string>(__VSHPROPID.VSHPROPID_SaveName);

        /// <summary>
        ///     Returns the project directory
        /// </summary>
        public string ProjectDir => GetProperty<string>(__VSHPROPID.VSHPROPID_ProjectDir);

        /// <summary>
        ///     Returns the full path
        /// </summary>
        /// <returns></returns>
        public string Path
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                if (_hierarchy is IVsProject project)
                {
                    int hr = project.GetMkDocument(_itemId, out string path);
                    Marshal.ThrowExceptionForHR(hr);
                    return path;
                }
                return string.Empty;
            }
        }

        public string RelativePath
        {
            get
            {
                if (IsRoot)
                {
                    return ProjectDir;
                }
                if (ParentNode != null)
                {
                    return System.IO.Path.Combine(ParentNode.RelativePath, Name);
                }
                return Name;
            }
        }

        public HierarchyNode Parent
        {
            get
            {
                if (!IsRoot)
                {
                    return new HierarchyNode(_solution, _hierarchy);
                }
                IVsHierarchy vsHierarchy = GetProperty<IVsHierarchy>(__VSHPROPID.VSHPROPID_ParentHierarchy, VSConstants.VSITEMID_ROOT);
                if (vsHierarchy == null)
                {
                    return null;
                }
                return new HierarchyNode(_solution, vsHierarchy);
            }
        }

        public HierarchyNode ParentNode
        {
            get
            {
                HierarchyNode parent = Parent;
                if (parent == this)
                    return parent;
                parent = parent.RecursiveFind(x => x.Children.FirstOrDefault(child => child.ItemId == ItemId) != null);
                return parent;
            }
        }

        /// <summary>
        ///     Queries the type T to the internal hierarchy object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T GetObject<T>()
            where T : class
        {
            return _hierarchy as T;
        }

        /// <summary>
        ///     Name of this node
        /// </summary>
        public string Name => GetProperty<string>(__VSHPROPID.VSHPROPID_Name);

        /// <summary>
        ///     Returns the Project GUID
        /// </summary>
        public Guid ProjectGuid
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")] get { return GetGuidProperty(__VSHPROPID.VSHPROPID_ProjectIDGuid); }
        }

        /// <summary>
        ///     Returns true if the current node is the solution root
        /// </summary>
        public bool IsSolution => Parent == null;

        /// <summary>
        ///     Returns the TypeGUID
        /// </summary>
        public Guid TypeGuid
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                // If the root node is a solution, then there is no TypeGuid
                if (IsSolution)
                {
                    return Guid.Empty;
                }
                return GetGuidProperty(__VSHPROPID.VSHPROPID_TypeGuid, ItemId);
            }
        }

        /// <summary>
        ///     Returns the Key to index icons in an image collection
        /// </summary>
        public string IconKey
        {
            get
            {
                if (_iconKey == null)
                {
                    if (HasIconIndex)
                    {
                        _iconKey = TypeGuid.ToString("b", CultureInfo.InvariantCulture) + "." + IconIndex.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (IsValidFullPathName(SaveName))
                    {
                        _iconKey = new FileInfo(SaveName).Extension;
                    }
                    else
                    {
                        _iconKey = string.Empty;
                    }
                }
                return _iconKey;
            }
        }

        public uint ItemId => _itemId;

        /// <summary>
        ///     Returns the icon of the node
        /// </summary>
        public Icon Icon
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                if (_icon == null)
                {
                    if (ImageListHandle != IntPtr.Zero && HasIconIndex)
                    {
                        IntPtr hIcon = NativeMethods.ImageList_GetIcon(ImageListHandle, IconIndex, 0);
                        _icon = Icon.FromHandle(hIcon);
                    }
                    else if (IconHandle != IntPtr.Zero)
                    {
                        _icon = Icon.FromHandle(IconHandle);
                    }
                    else if (IsValidFullPathName(SaveName))
                    {
                        // The following comes from http://support.microsoft.com/kb/319350/
                        NativeMethods.Shfileinfo shinfo = new NativeMethods.Shfileinfo();
                        NativeMethods.SHGetFileInfo(
                            new FileInfo(SaveName).Extension,
                            NativeMethods.FileAttributeNormal,
                            ref shinfo, (uint) Marshal.SizeOf(shinfo),
                            NativeMethods.ShgfiUsefileattributes | NativeMethods.ShgfiIcon | NativeMethods.ShgfiSmallicon);
                        if (shinfo.hIcon != IntPtr.Zero)
                        {
                            _icon = Icon.FromHandle(shinfo.hIcon);
                        }
                    }
                }
                return _icon;
            }
        }

        /// <summary>
        ///     Returns true is there is al least one child under this node
        /// </summary>
        public bool HasChildren => FirstChildId != VSConstants.VSITEMID_NIL;

        /// <summary>
        ///     Returns the extensibility object
        /// </summary>
        public object ExtObject => GetProperty<object>(__VSHPROPID.VSHPROPID_ExtObject);

        public IEnumerable<IHierarchyNode> Children => new HierarchyNodeCollection(this);

        public string SolutionRelativeName
        {
            get
            {
                if (IsSolution)
                {
                    return string.Empty;
                }
                string parentRelativeName = ParentNode?.SolutionRelativeName;
                if (!String.IsNullOrEmpty(parentRelativeName))
                    return parentRelativeName + "\\" + Name;

                return Name;
            }
        }

        protected static bool IsValidFullPathName(string fileName)
        {
            //Debug.Assert(fileName != null);
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            int i = fileName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
            if (i == -1)
            {
                return IsValidFileName(fileName);
            }
            string pathPart = fileName.Substring(0, i + 1);
            if (IsValidPath(pathPart))
            {
                string filePart = fileName.Substring(i + 1);
                return IsValidFileName(filePart);
            }
            return false;
        }

        protected static bool IsValidPath(string pathPart)
        {
            //Debug.Assert(pathPart != null);
            if (string.IsNullOrEmpty(pathPart))
            {
                return true;
            }
            foreach (char c in System.IO.Path.GetInvalidPathChars())
            {
                if (pathPart.IndexOf(c) != -1)
                {
                    return false;
                }
            }
            return true;
        }

        protected static bool IsValidFileName(string filePart)
        {
            //Debug.Assert(filePart != null);
            if (string.IsNullOrEmpty(filePart))
            {
                return false;
            }
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                if (filePart.IndexOf(c) != -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Gets the next child id from the passed childId
        /// </summary>
        /// <param name="childId"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId =
            "Microsoft.VisualStudio.Shell.Interop.IVsHierarchy.GetProperty(System.UInt32,System.Int32,System.Object@)")]
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public uint GetNextChildId(uint childId)
        {
            // NOTE: to work around a bug with the Solution implementation of VSHPROPID_NextSibling,
            // we keep track of the recursion level. If we are asking for the next sibling under
            // the Solution, we use VSHPROPID_NextVisibleSibling instead of _NextSibling. 
            // In VS 2005 and earlier, the Solution improperly enumerates all nested projects
            // in the Solution (at any depth) as if they are immediate children of the Solution.
            // Its implementation   _NextVisibleSibling is correct however, and given that there is
            // not a feature to hide a SolutionFolder or a Project, thus _NextVisibleSibling is 
            // expected to return the identical results as _NextSibling.
            _hierarchy.GetProperty(childId,
                (int) (IsSolution ? __VSHPROPID.VSHPROPID_NextVisibleSibling : __VSHPROPID.VSHPROPID_NextSibling),
                out object nextChild);
            return GetItemId(nextChild);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private Guid GetGuidProperty(__VSHPROPID propId, uint itemid = VSConstants.VSITEMID_ROOT)
        {
            int hr = _hierarchy.GetGuidProperty(itemid, (int) propId, out Guid guid);
            // in case of failure, we simply trace the error and return silently with an empty guid
            // so the caller can resolve what to do without blowing out execution with an exception
            if (hr != 0) Trace.TraceError(Marshal.GetExceptionForHR(hr).ToString());
            return guid;
        }

        private bool HasProperty(__VSHPROPID propId)
        {
            int hr = _hierarchy.GetProperty(_itemId, (int) propId, out object value);
            if (hr != VSConstants.S_OK || value == null)
            {
                return false;
            }
            return true;
        }

        private T GetProperty<T>(__VSHPROPID propId, uint itemid)
        {
            int hr = _hierarchy.GetProperty(itemid, (int) propId, out object value);
            if (hr != VSConstants.S_OK || value == null)
            {
                return default(T);
            }
            return (T) value;
        }

        private T GetProperty<T>(__VSHPROPID propId)
        {
            return GetProperty<T>(propId, _itemId);
        }

        private static uint GetItemId(object pvar)
        {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int i) return (uint) i;
            if (pvar is uint u) return u;
            if (pvar is short s) return (uint) s;
            if (pvar is ushort @ushort) return @ushort;
            if (pvar is long l) return (uint) l;
            return VSConstants.VSITEMID_NIL;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode CreateSolutionFolder(string folderName)
        {
            Guard.ArgumentNotNullOrEmptyString(folderName, "folderName");
            Guid solutionFolderGuid = new Guid(Constants.SolutionFolderType);
            Guid iidProject = typeof(IVsHierarchy).GUID;
            int hr = _solution.CreateProject(
                ref solutionFolderGuid,
                null,
                null,
                folderName,
                0,
                ref iidProject,
                out IntPtr ptr);
            if (hr == VSConstants.S_OK && ptr != IntPtr.Zero)
            {
                IVsHierarchy vsHierarchy = (IVsHierarchy) Marshal.GetObjectForIUnknown(ptr);
                Debug.Assert(vsHierarchy != null);
                return new HierarchyNode(_solution, vsHierarchy);
            }
            return null;
        }

        public HierarchyNode Find(Predicate<HierarchyNode> func)
        {
            foreach (HierarchyNode child in Children)
            {
                if (func(child))
                {
                    return child;
                }
            }
            return null;
        }

        // FXCOP: False positive
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "child")]
        public void ForEach(Action<HierarchyNode> func)
        {
            foreach (HierarchyNode child in Children)
            {
                func(child);
            }
        }

        public void RecursiveForEach(Action<HierarchyNode> func)
        {
            func(this);
            foreach (HierarchyNode child in Children)
            {
                child.RecursiveForEach(func);
            }
        }

        public HierarchyNode RecursiveFind(Predicate<HierarchyNode> func)
        {
            if (func(this))
            {
                return this;
            }
            foreach (HierarchyNode child in Children)
            {
                HierarchyNode found = child.RecursiveFind(func);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public HierarchyNode FindByName(string name)
        {
            return Find(node => node.Name != null && node.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public HierarchyNode RecursiveFindByName(string name)
        {
            if (name.IndexOf(System.IO.Path.DirectorySeparatorChar) == -1)
            {
                return RecursiveFind(node => node.Name != null && node.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            HierarchyNode folder = null;
            foreach (string part in name.Split(new[] {System.IO.Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries))
            {
                folder = folder == null ? FindByName(part) : folder.FindByName(part);
                if (folder == null)
                {
                    break;
                }
            }
            return folder;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode FindOrCreateSolutionFolder(string name)
        {
            HierarchyNode folder = FindByName(name) ?? CreateSolutionFolder(name);
            return folder;
        }

        public void Remove()
        {
            Debug.Assert(Parent != null);
            Parent.RemoveItem(_itemId);
        }

        private bool RemoveItem(uint vsItemId)
        {
            if (!(_hierarchy is IVsProject2 vsProject))
            {
                return false;
            }
            int hr = vsProject.RemoveItem(0, vsItemId, out int result);
            return hr == VSConstants.S_OK && result == 1;
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private void Init(IVsSolution vsSolution, IVsHierarchy vsHierarchy, uint vsItemId)
        {
            _solution = vsSolution;
            int hr;
            if (vsHierarchy == null)
            {
                Guid emptyGuid = Guid.Empty;
                hr = _solution.GetProjectOfGuid(ref emptyGuid, out _hierarchy);
                Marshal.ThrowExceptionForHR(hr);
            }
            else
            {
                _hierarchy = vsHierarchy;
            }
            _itemId = vsItemId;

            Guid hierGuid = typeof(IVsHierarchy).GUID;

            // Check first if this node has a nested hierarchy. If so, then there really are two 
            // identities for this node: 1. hierarchy/itemid 2. nestedHierarchy/nestedItemId.
            // We will convert this node using the inner nestedHierarchy/nestedItemId identity.
            hr = _hierarchy.GetNestedHierarchy(_itemId, ref hierGuid, out IntPtr nestedHierarchyObj, out uint nestedItemId);
            if (VSConstants.S_OK == hr && IntPtr.Zero != nestedHierarchyObj)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHierarchyObj) as IVsHierarchy;
                Marshal.Release(nestedHierarchyObj); // we are responsible to release the refcount on the out IntPtr parameter
                if (nestedHierarchy != null)
                {
                    _hierarchy = nestedHierarchy;
                    _itemId = nestedItemId;
                }
            }
        }

        #region NativeMethods class

        private sealed class NativeMethods
        {
            public const uint FileAttributeNormal = 0x00000080;
            public const uint ShgfiUsefileattributes = 0x000000010; // use passed dwFileAttribute
            public const uint ShgfiIcon = 0x100;
            public const uint ShgfiLargeicon = 0x0; // 'Large icon
            public const uint ShgfiSmallicon = 0x1; // 'Small icon

            private NativeMethods()
            {
            }

            [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Shfileinfo psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("comctl32.dll")]
            public static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

            [StructLayout(LayoutKind.Sequential)]
            public struct Shfileinfo
            {
                public readonly IntPtr hIcon;
                public readonly IntPtr iIcon;
                public readonly uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public readonly string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public readonly string szTypeName;
            }

            //[DllImport("comctl32.dll")]
            //public extern static int ImageList_GetImageCount(IntPtr himl);
        }

        #endregion

        #region IDisposable Members

        private bool _disposed;

        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.

                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.
            }
            _disposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~HierarchyNode()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion
    }
}