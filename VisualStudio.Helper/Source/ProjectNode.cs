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
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using EnvDTE;
using Microsoft.Build.Evaluation;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectLinker.Helper.Properties;
using VsWebSite;
using VSLangProj;
using Project = Microsoft.Build.Evaluation.Project;
using ProjectItem = EnvDTE.ProjectItem;

namespace ProjectLinker.Helper
{
    /// <summary>
    ///     Project Node in the current solution
    /// </summary>
    public class ProjectNode : HierarchyNode
    {
        /// <summary>
        ///     Visual Studio Project
        /// </summary>
        private readonly IVsProject _project;

        /// <summary>
        ///     Builds a project node from the project Guid
        /// </summary>
        /// <param name="vsSolution"></param>
        /// <param name="projectGuid"></param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public ProjectNode(IVsSolution vsSolution, Guid projectGuid)
            : base(vsSolution, projectGuid)
        {
            _project = Hierarchy as IVsProject;
            // Commented because it will show up an error dialog before getting back control to the recipe (caller)
            //Debug.Assert(project != null);  
            Debug.Assert(ItemId == VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        ///     Builds a project node from the a parent node
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="itemid"></param>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        private ProjectNode(HierarchyNode parent, uint itemid)
            : base(parent, itemid)
        {
            _project = Hierarchy as IVsProject;
            Debug.Assert(_project != null);
        }

        protected IVsProject Project => _project;

        /// <summary>
        ///     Gets the MSBuild project
        /// </summary>
#pragma warning disable 612, 618
        public Project MSBuildProject
#pragma warning restore 612,618
        {
            get
            {
#pragma warning disable 612,618
                return ProjectCollection.GlobalProjectCollection.GetLoadedProjects(Path).First();
#pragma warning restore 612,618
            }
        }

        ///// <summary>
        ///// Gets the msbuild item
        ///// </summary>
        ///// <param name="includeSpec"></param>
        ///// <returns></returns>
        //public BuildItem GetBuildItem(string includeSpec)
        //{
        //    Guard.ArgumentNotNullOrEmptyString(includeSpec, "includeSpec");
        //    string currentDir = "." + System.IO.Path.DirectorySeparatorChar;
        //    if (includeSpec.StartsWith(currentDir, StringComparison.OrdinalIgnoreCase))
        //    {
        //        includeSpec = includeSpec.Substring(currentDir.Length);
        //    }

        //    Project msbuildProject = MSBuildProject;

        //    foreach (BuildItemGroup group in msbuildProject.b)
        //    {
        //        foreach (BuildItem buildItem in group)
        //        {
        //            if (buildItem.Include == includeSpec)
        //            {
        //                return buildItem;
        //            }
        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        ///     Gets the project reference of a project.
        /// </summary>
        /// <value>The projref of project.</value>
        public string ProjectReferenceOfProject
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                int hr = Solution.GetProjrefOfProject(Hierarchy, out string result);
                Marshal.ThrowExceptionForHR(hr);
                return result;
            }
        }

        /// <summary>
        ///     Gets the VS project.
        /// </summary>
        /// <value>The VS project.</value>
        private EnvDTE.Project VsProject => ExtObject as EnvDTE.Project;

        /// <summary>
        ///     Returns the language of the underlying project, if available.
        /// </summary>
        public string Language
        {
            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            get
            {
                if (VsProject == null)
                {
                    return null;
                }
                if (VsProject.Object is VSProject)
                {
                    return GetLanguageFromProject(((VSProject) VsProject.Object).Project);
                }
                if (VsProject.Object is VSWebSite)
                {
                    return GetLanguageFromProject(((VSWebSite) VsProject.Object).Project);
                }
                return null;
            }
        }

        /// <summary>
        ///     Determines whether this instance [can add item] the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///     <c>true</c> if this instance [can add item] the specified name; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanAddItem(string name)
        {
            Guard.ArgumentNotNullOrEmptyString(name, "name");
            return IsValidFullPathName(name);
        }

        /// <summary>
        ///     Adds a new item in the project
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public HierarchyNode AddItem(string name)
        {
            Guard.ArgumentNotNullOrEmptyString(name, "name");
            if (!CanAddItem(name))
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.InvalidFileName,
                        name));
            }
            FileInfo fileInfo;
            string subFolder = string.Empty;
            if (System.IO.Path.IsPathRooted(name))
            {
                fileInfo = new FileInfo(name);
            }
            else
            {
                fileInfo = new FileInfo(System.IO.Path.Combine(ProjectDir, name));
                int subFolderIndex = name.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                if (subFolderIndex != -1)
                {
                    subFolder = name.Substring(0, subFolderIndex);
                }
            }
            if (fileInfo.Name.Equals(fileInfo.Extension, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        Resources.CannotCreateItemWithEmptyName));
            }
            if (!File.Exists(fileInfo.FullName))
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
                File.Create(fileInfo.FullName).Dispose();
            }
            VSDOCUMENTPRIORITY docPri = VSDOCUMENTPRIORITY.DP_Standard;
            int hr = _project.IsDocumentInProject(fileInfo.FullName, out int found, new[] {docPri}, out uint itemId);
            Marshal.ThrowExceptionForHR(hr);
            if (found == 0)
            {
                VSADDRESULT result = VSADDRESULT.ADDRESULT_Cancel;
                uint folderId = ItemId;
                HierarchyNode subFolderNode = FindSubfolder(subFolder);
                if (subFolderNode != null)
                {
                    folderId = subFolderNode.ItemId;
                }
                hr = _project.AddItem(folderId,
                    VSADDITEMOPERATION.VSADDITEMOP_OPENFILE,
                    fileInfo.Name, 1, new[] {fileInfo.FullName},
                    IntPtr.Zero, new[] {result});
                Marshal.ThrowExceptionForHR(hr);
            }
            hr = _project.IsDocumentInProject(fileInfo.FullName, out found, new[] {docPri}, out itemId);
            Marshal.ThrowExceptionForHR(hr);
            if (found == 1)
            {
                return new HierarchyNode(this, itemId);
            }
            return null;
        }

        /// <summary>
        ///     Finds the sub folder.
        /// </summary>
        /// <param name="subfolder">The sub folder.</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public ProjectNode FindSubfolder(string subfolder)
        {
            if (!string.IsNullOrEmpty(subfolder))
            {
                string[] folders = subfolder.Split(new[] {System.IO.Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
                ProjectNode folderNode = this;
                foreach (string folder in folders)
                {
                    folderNode = folderNode.FindOrCreateFolder(folder);
                    if (folderNode == null)
                    {
                        break;
                    }
                }
                return folderNode;
            }
            return null;
        }

        /// <summary>
        ///     Finds or create a folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public ProjectNode FindOrCreateFolder(string folderName)
        {
            if (string.IsNullOrEmpty(folderName) || folderName == ".")
            {
                return this;
            }
            DirectoryInfo di = new DirectoryInfo(System.IO.Path.Combine(RelativePath, folderName));
            HierarchyNode subFolder = FindByName(di.Name);
            if (subFolder == null)
            {
                if (!Directory.Exists(di.FullName))
                {
                    Directory.CreateDirectory(di.FullName);
                }
                VSADDRESULT result = VSADDRESULT.ADDRESULT_Cancel;
                int hr = _project.AddItem(ItemId,
                    VSADDITEMOPERATION.VSADDITEMOP_OPENFILE, di.Name,
                    1, new[] {di.FullName},
                    IntPtr.Zero, new[] {result});
                Marshal.ThrowExceptionForHR(hr);
            }
            subFolder = FindByName(di.Name);
            if (subFolder != null)
            {
                return new ProjectNode(subFolder, subFolder.ItemId);
            }
            return null;
        }

        /// <summary>
        ///     Opens an item using the default view
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public IVsWindowFrame OpenItem(HierarchyNode child)
        {
            Guard.ArgumentNotNull(child, "child");

            Guid logicalView = VSConstants.LOGVIEWID_Primary;
            IntPtr existingDocData = IntPtr.Zero;
            int hr = _project.OpenItem(child.ItemId, ref logicalView, existingDocData, out IVsWindowFrame windowFrame);
            Marshal.ThrowExceptionForHR(hr);
            return windowFrame;
        }

        /// <summary>
        ///     Adds the assembly reference.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        public void AddAssemblyReference(string assemblyPath)
        {
            Guard.ArgumentNotNullOrEmptyString(assemblyPath, "assemblyPath");

            if (VsProject != null)
            {
                if (VsProject.Object is VSProject project)
                {
                    References references = project.References;
                    references?.Add(assemblyPath);
                }
                else
                {
                    AssemblyReferences references = (VsProject.Object as VSWebSite)?.References;

                    if (references != null)
                    {
                        if (System.IO.Path.IsPathRooted(assemblyPath))
                        {
                            references.AddFromFile(assemblyPath);
                        }
                        else
                        {
                            references.AddFromGAC(assemblyPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Adds the project reference.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        public void AddProjectReference(Guid projectId)
        {
            if (ProjectGuid == projectId)
            {
                return;
            }

            ProjectNode referencedProject = new ProjectNode(Solution, projectId);

            if (VsProject != null)
            {
                if (VsProject.Object is VSProject project)
                {
                    References references = project.References;

                    if (references != null && referencedProject.ExtObject is EnvDTE.Project)
                    {
                        references.AddProject(referencedProject.ExtObject as EnvDTE.Project);
                    }
                }
                else
                {
                    AssemblyReferences references = (VsProject.Object as VSWebSite)?.References;

                    if (references != null && referencedProject.ExtObject is EnvDTE.Project)
                    {
                        try
                        {
                            references.AddFromProject(referencedProject.ExtObject as EnvDTE.Project);
                        }
                        catch (COMException)
                        {
                            //Web projects throws exceptions if the reference already exists
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the evaluated property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public string GetEvaluatedProperty(string propertyName)
        {
            return GetEvaluatedProperty(VsProject, propertyName, false);
        }

        /// <summary>
        ///     Gets the evaluated property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="throwIfNotFound">When 'true', it thows an error if the property wasn't found.</param>
        /// <returns></returns>
        public string GetEvaluatedProperty(string propertyName, bool throwIfNotFound)
        {
            return GetEvaluatedProperty(VsProject, propertyName, throwIfNotFound);
        }

        /// <summary>
        ///     Gets the evaluated property.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="throwIfNotFound">When 'true', it throws an error if the property wasn't found.</param>
        /// <returns></returns>
        public static string GetEvaluatedProperty(EnvDTE.Project project, string propertyName, bool throwIfNotFound = false)
        {
            if (project == null)
            {
                return string.Empty;
            }
            Guard.ArgumentNotNullOrEmptyString(propertyName, "propertyName");

            string value = string.Empty;
            foreach (Property prop in project.Properties)
            {
                if (prop.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = prop.Value.ToString();
                    break;
                }
            }

            if (throwIfNotFound)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.PropertyNameNotFound, propertyName, project.Name));
            }
            return value;
        }


        /// <summary>
        ///     Gets the language from project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        /// \
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static string GetLanguageFromProject(EnvDTE.Project project)
        {
            Guard.ArgumentNotNull(project, "project");

            if (project.CodeModel != null)
            {
                return project.CodeModel.Language;
            }

            CodeDomProvider provider = DteHelper.GetCodeDomProvider(project);
            if (provider is CSharpCodeProvider)
            {
                return CodeModelLanguageConstants.vsCMLanguageCSharp;
            }
            if (provider is VBCodeProvider)
            {
                return CodeModelLanguageConstants.vsCMLanguageVB;
            }
            return null;
        }
    }

    internal static class DteHelper
    {
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static CodeDomProvider GetCodeDomProvider(EnvDTE.Project project)
        {
            if (project != null)
            {
                return
                    CodeDomProvider.CreateProvider(CodeDomProvider.GetLanguageFromExtension(GetDefaultExtension(project)));
            }
            return CodeDomProvider.CreateProvider("C#");
        }

        private static string GetDefaultExtension(EnvDTE.Project project)
        {
            if (!IsWebProject(project))
            {
                return GetDefaultExtensionFromNonWebProject(project);
            }
            if (!IsWebCSharpProject(project))
            {
                return ".vb";
            }
            return ".cs";
        }

        // FxCop: The reason for project properties retrieval failures may be varied.
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        // FxCop: Need to try target as different types.
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static bool IsWebCSharpProject(object target)
        {
            EnvDTE.Project containingProject = null;
            if (target is EnvDTE.Project project)
            {
                containingProject = project;
            }
            else if (target is ProjectItem)
            {
                containingProject = ((ProjectItem) target).ContainingProject;
            }
            if (containingProject != null && IsWebProject(containingProject) && containingProject.Properties != null)
            {
                try
                {
                    Property property = containingProject.Properties.Item("CurrentWebsiteLanguage");
                    return property.Value != null && property.Value.ToString().Equals("Visual C#", StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception exception)
                {
                    Trace.TraceError(exception.ToString());
                    return false;
                }
            }
            return false;
        }

        private static string GetDefaultExtensionFromNonWebProject(EnvDTE.Project project)
        {
            Guard.ArgumentNotNull(project, "project");
            if (project.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
            {
                return ".cs";
            }
            if (project.Kind != "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}")
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedProjectKind,
                    new object[] {project.Name}));
            }
            return ".vb";
        }


        private static bool IsWebProject(EnvDTE.Project project)
        {
            return project.Kind == "{E24C65DC-7377-472b-9ABA-BC803B73C61A}";
        }
    }
}