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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using EnvDTE;
using EnvDTE100;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectLinker.Utility;
using Constants = EnvDTE.Constants;

namespace ProjectLinker.Services
{
    public partial class ProjectLinkTracker : IProjectLinkTracker, ISProjectLinkTracker, IVsTrackProjectDocumentsEvents2, IVsSolutionEvents
    {
        private const string ProjectLinkReferenceKey = "ProjectLinkReference";

        //FxCop: False positive, ProjectLinkTracker requests advise on this interface.
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")] private readonly IVsTrackProjectDocuments2 _documentTracker;

        private readonly ILogger _logger;
        private readonly List<KeyValuePair<Guid, Project>> _pendingLinks = new List<KeyValuePair<Guid, Project>>();
        private readonly IVsSolution _solution;
        private uint _solutionCookie;
        private uint _trackProjectDocumentsCookie;


        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "dte")]
        public ProjectLinkTracker(IVsTrackProjectDocuments2 documentTracker, IVsSolution solution, ILogger logger, Solution4 dteSolution = null)
        {
            logger?.Log(string.Format(CultureInfo.CurrentCulture, Resources.InitializingProjectTracker));

            _documentTracker = documentTracker;
            _solution = solution;
            _logger = logger;
            ErrorHandler.ThrowOnFailure(documentTracker.AdviseTrackProjectDocumentsEvents(this, out _trackProjectDocumentsCookie));
            ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(this, out _solutionCookie));

            RestoreLinks(dteSolution);
        }

        protected Collection<IProjectItemsSynchronizer> Synchronizers { get; } = new Collection<IProjectItemsSynchronizer>();


        public void AddProjectLink(Guid sourceProject, Guid targetProject)
        {
            ErrorHandler.ThrowOnFailure(_solution.GetProjectOfGuid(ref sourceProject, out IVsHierarchy sourceProjectHierarchy));
            ErrorHandler.ThrowOnFailure(_solution.GetProjectOfGuid(ref targetProject, out IVsHierarchy targetProjectHierarchy));
            AddProjectLink(sourceProjectHierarchy, targetProjectHierarchy);
        }

        public IEnumerable<ProjectLink> GetProjectLinks()
        {
            List<ProjectLink> list = new List<ProjectLink>();
            foreach (IProjectItemsSynchronizer syncher in Synchronizers)
            {
                ProjectLink link = new ProjectLink
                {
                    SourceProjectId = syncher.SourceProject.GetProjectGuid(_solution),
                    TargetProjectId = syncher.TargetProject.GetProjectGuid(_solution)
                };
                list.Add(link);
            }
            return list;
        }

        public void UnlinkProjects(Guid sourceProject, Guid targetProject)
        {
            IProjectItemsSynchronizer syncher =
                Synchronizers.FirstOrDefault(
                    s =>
                        s.SourceProject.GetProjectGuid(_solution) == sourceProject &&
                        s.TargetProject.GetProjectGuid(_solution) == targetProject);

            if (syncher != null)
            {
                syncher.TargetProject.Globals.set_VariablePersists(ProjectLinkReferenceKey, false);
                Synchronizers.Remove(syncher);

                _logger.Log(string.Format(CultureInfo.CurrentCulture, Resources.ProjectsSuccessfullyUnlinked, syncher.TargetProject.Name,
                    syncher.SourceProject.Name));
            }
        }


        public void LinkAllProjectItems(Guid sourceProject, Guid targetProject)
        {
            IProjectItemsSynchronizer syncher =
                Synchronizers.FirstOrDefault(
                    s =>
                        s.SourceProject.GetProjectGuid(_solution) == sourceProject &&
                        s.TargetProject.GetProjectGuid(_solution) == targetProject);

            if (syncher != null)
            {
                syncher.LinkAllFiles();

                _logger.Log(string.Format(CultureInfo.CurrentCulture, Resources.ProjectsSuccessfullyLinkedAll, syncher.TargetProject.Name,
                    syncher.SourceProject.Name));
            }
        }


        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            Trace.WriteLine("OnAfterOpenProject");
            RestoreLinksForHierarchy(pHierarchy);

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            Trace.WriteLine("OnBeforeCloseProject");
            RemoveLinksForHierarchy(pHierarchy);
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments,
            VSADDFILEFLAGS[] rgFlags)
        {
            TraceAdviseEvent("OnAfterAddFilesEx", rgpszMkDocuments);
            Debug.Assert(cProjects <= 1, "Multiple projects is not expected.");
            for (int projectIndex = 0; projectIndex < cProjects; projectIndex++)
            {
                foreach (IProjectItemsSynchronizer syncher in Synchronizers.Where(s => AreProjectsEqual(s.SourceProject, rgpProjects[projectIndex])))
                {
                    Trace.WriteLine("Item added in monitored project");

                    string[] newFiles = GetDocumentsForProject(rgpszMkDocuments, projectIndex, cProjects, rgFirstIndices);

                    newFiles.ForEach(syncher.FileAddedToSource);
                }
            }
            return VSConstants.S_OK;
        }

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments,
            VSADDDIRECTORYFLAGS[] rgFlags)
        {
            TraceAdviseEvent("OnAfterAddDirectoriesEx", rgpszMkDocuments);
            Debug.Assert(cProjects <= 1, "Multiple projects is not expected.");
            for (int projectIndex = 0; projectIndex < cProjects; projectIndex++)
            {
                foreach (IProjectItemsSynchronizer syncher in Synchronizers.Where(s => AreProjectsEqual(s.SourceProject, rgpProjects[projectIndex])))
                {
                    Trace.WriteLine("Directory added in monitored project");

                    string[] newDirectories = GetDocumentsForProject(rgpszMkDocuments, projectIndex, cProjects, rgFirstIndices);

                    newDirectories.ForEach(syncher.DirectoryAddedToSource);
                }
            }
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments,
            VSREMOVEFILEFLAGS[] rgFlags)
        {
            TraceAdviseEvent("OnAfterRemoveFiles", rgpszMkDocuments);
            Debug.Assert(cProjects <= 1, "Multiple projects is not expected.");
            for (int projectIndex = 0; projectIndex < cProjects; projectIndex++)
            {
                foreach (IProjectItemsSynchronizer syncher in Synchronizers.Where(s => AreProjectsEqual(s.SourceProject, rgpProjects[projectIndex])))
                {
                    Trace.WriteLine("File removed in monitored project");

                    string[] removedFiles = GetDocumentsForProject(rgpszMkDocuments, projectIndex, cProjects, rgFirstIndices);

                    removedFiles.ForEach(syncher.FileRemovedFromSource);
                }
            }
            return VSConstants.S_OK;
        }


        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices,
            string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            TraceAdviseEvent("OnAfterRemoveDirectories", rgpszMkDocuments);
            Debug.Assert(cProjects <= 1, "Multiple projects is not expected.");
            for (int projectIndex = 0; projectIndex < cProjects; projectIndex++)
            {
                foreach (IProjectItemsSynchronizer syncher in Synchronizers.Where(s => AreProjectsEqual(s.SourceProject, rgpProjects[projectIndex])))
                {
                    Trace.WriteLine("Directory removed in monitored project");

                    string[] removedDocuments = GetDocumentsForProject(rgpszMkDocuments, projectIndex, cProjects, rgFirstIndices);

                    removedDocuments.ForEach(syncher.DirectoryRemovedFromSource);
                }
            }
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames,
            string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            TraceAdviseEvent("OnAfterRenameFiles", rgszMkOldNames);
            Debug.Assert(cProjects <= 1, "Multiple projects is not expected.");
            for (int projectIndex = 0; projectIndex < cProjects; projectIndex++)
            {
                foreach (IProjectItemsSynchronizer syncher in Synchronizers.Where(s => AreProjectsEqual(s.SourceProject, rgpProjects[projectIndex])))
                {
                    Trace.WriteLine("File renamed in monitored project");

                    string[] oldDocuments = GetDocumentsForProject(rgszMkOldNames, projectIndex, cProjects, rgFirstIndices);
                    string[] newDocuments = GetDocumentsForProject(rgszMkNewNames, projectIndex, cProjects, rgFirstIndices);

                    for (int documentIndex = 0; documentIndex < oldDocuments.Length; documentIndex++)
                    {
                        if (rgFlags[documentIndex] == VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_Directory)
                        {
                            syncher.DirectoryRenamedInSource(oldDocuments[documentIndex], newDocuments[documentIndex]);
                        }
                        else
                        {
                            syncher.FileRenamedInSource(oldDocuments[documentIndex], newDocuments[documentIndex]);
                        }
                    }
                }
            }
            return VSConstants.S_OK;
        }

        private void RestoreLinks(Solution4 dteSolution)
        {
            if (dteSolution != null)
            {
                foreach (Project project in GetProjectsInSolution(dteSolution))
                {
                    ErrorHandler.ThrowOnFailure(_solution.GetProjectOfUniqueName(project.UniqueName, out IVsHierarchy hierarchy));
                    RestoreLinksForHierarchy(hierarchy);
                }
            }
        }

        private static IEnumerable<Project> GetProjectsInSolution(Solution4 solution)
        {
            List<Project> projects = new List<Project>();

            foreach (Project project in solution.Projects)
            {
                GetProjects(project, projects);
            }

            return projects;
        }

        private static void GetProjects(Project project, List<Project> projects)
        {
            //const string SolutionFolderGuid = "66a26720-8fb5-11d2-aa7e-00c04f688dde"; THIS LINE HAD THE WRONG CONSTANT         
            // Don't add the MiscFilesProject to the list of projects
            if (project.UniqueName == Constants.vsMiscFilesProjectUniqueName)
                return;

            if (project.Kind.Equals(Constants.vsProjectKindSolutionItems, StringComparison.OrdinalIgnoreCase)
            ) //USING ENVDTE CONSTANT TO CHECK IF THE ITEM IS A SOLUTION FOLDER
            {
                foreach (ProjectItem projectItem in project.ProjectItems)
                {
                    if (projectItem.SubProject != null)
                    {
                        GetProjects(projectItem.SubProject, projects);
                    }
                }
            }
            else
            {
                projects.Add(project);
            }
        }

        public void AddProjectLink(IVsHierarchy sourceProject, IVsHierarchy targetProject)
        {
            if (LinkExists(sourceProject, targetProject))
            {
                throw new ProjectLinkerException(Resources.ProjectFilesAlreadyLinked);
            }

            Project targetDteProject = targetProject.GetProject();

            AddSyncher(sourceProject.GetProject(), targetDteProject);

            Guid sourceProjectId = sourceProject.GetProjectGuid();
            targetDteProject.Globals[ProjectLinkReferenceKey] = sourceProjectId.ToString();
            targetDteProject.Globals.set_VariablePersists(ProjectLinkReferenceKey, true);
        }

        private void AddSyncher(Project sourceProject, Project targetProject)
        {
            Project linkedSourceProject = GetProjectSource(sourceProject);
            //if (linkedSourceProject != null)
            //{
            //    throw new ProjectLinkerException(String.Format(CultureInfo.CurrentCulture,
            //                                                   Resources.SourceProjectIsAlreadyLinkedAsTarget,
            //                                                   targetProject.Name, sourceProject.Name, linkedSourceProject.Name));
            //}

            linkedSourceProject = GetProjectSource(targetProject);
            if (linkedSourceProject != null)
            {
                throw new ProjectLinkerException(String.Format(CultureInfo.CurrentCulture,
                    Resources.TargetProjectIsAlreadyLinkedAsTarget,
                    targetProject.Name, sourceProject.Name, linkedSourceProject.Name));
            }

            //var linkedTargetProjects = GetProjectTargets(targetProject);
            //if (linkedTargetProjects.Count() > 0)
            //{
            //    var targetsName = from targets in linkedTargetProjects select targets.Name;
            //    StringBuilder sb = new StringBuilder();
            //    targetsName.ForEach(s =>
            //                            {
            //                                if (sb.Length > 0) sb.Append(", ");
            //                                sb.Append("'");
            //                                sb.Append(s);
            //                                sb.Append("'");
            //                            }
            //        );
            //    throw new ProjectLinkerException(String.Format(CultureInfo.CurrentCulture,
            //                                                   Resources.TargetProjectIsAlreadyLinkedAsSource,
            //                                                   targetProject.Name, sourceProject.Name, sb));
            //}

            Synchronizers.Add(CreateSynchronizer(sourceProject, targetProject));
        }

        private Project GetProjectSource(Project targetProject)
        {
            IProjectItemsSynchronizer synchronizer = Synchronizers.FirstOrDefault(x => x.TargetProject == targetProject);
            return synchronizer?.SourceProject;
        }

        private IEnumerable<Project> GetProjectTargets(Project sourceProject)
        {
            return from s in Synchronizers
                where s.SourceProject == sourceProject
                select s.TargetProject;
        }

        protected bool LinkExists(IVsHierarchy sourceProject, IVsHierarchy targetProject)
        {
            Project srcProject = sourceProject.GetProject();
            Project tgtProject = targetProject.GetProject();
            IProjectItemsSynchronizer first = null;
            foreach (IProjectItemsSynchronizer x in Synchronizers)
            {
                if (x.SourceProject == srcProject && x.TargetProject == tgtProject)
                {
                    first = x;
                    break;
                }
            }
            return first != null;
        }

        private static bool AreProjectsEqual(Project project, IVsProject vsProject)
        {
            return project == ((IVsHierarchy) vsProject).GetProject();
        }

        private static string[] GetDocumentsForProject(string[] documents, int currentProjectIndex, int totalProjects, int[] firstIndices)
        {
            int startIndex = firstIndices[currentProjectIndex];
            int arrayLength;
            if (currentProjectIndex == totalProjects - 1)
            {
                arrayLength = documents.Length - startIndex;
            }
            else
            {
                arrayLength = firstIndices[currentProjectIndex + 1] - startIndex;
            }

            string[] newDocuments = new string[arrayLength];
            Array.Copy(documents, startIndex, newDocuments, 0, arrayLength);
            return newDocuments;
        }

        [Conditional("DEBUG")]
        private static void TraceAdviseEvent(string methodName, string[] documents)
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, methodName, "{0}: "));
            if (documents != null)
            {
                foreach (string document in documents)
                {
                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "  - {0}", document));
                }
            }
        }

        private void RestoreLinksForHierarchy(IVsHierarchy hierarchy)
        {
            try
            {
                Guid projectId = hierarchy.GetProjectGuid();
                Project currentDteProject = hierarchy.GetProject();

                Project targetProject = DequeueProjectLink(projectId);
                while (targetProject != null)
                {
                    try
                    {
                        AddSyncher(currentDteProject, targetProject);
                        _logger.Log(string.Format(CultureInfo.CurrentCulture, Resources.ProjectsSuccessfullyRelinked, targetProject.Name,
                            currentDteProject.Name));
                    }
                    catch (ProjectLinkerException projectLinkerEx)
                    {
                        _logger.Log(projectLinkerEx.Message);
                    }
                    targetProject = DequeueProjectLink(projectId);
                }

                if (currentDteProject?.Globals != null && currentDteProject.Globals.get_VariableExists(ProjectLinkReferenceKey))
                {
                    Guid sourceProjectId = new Guid(currentDteProject.Globals[ProjectLinkReferenceKey] as string);
                    int hr = _solution.GetProjectOfGuid(ref sourceProjectId, out IVsHierarchy sourceHierarchy);
                    if (hr == VSConstants.S_OK && sourceHierarchy != null)
                    {
                        try
                        {
                            Project sourceDteProject = sourceHierarchy.GetProject();
                            AddSyncher(sourceDteProject, currentDteProject);
                            _logger.Log(string.Format(CultureInfo.CurrentCulture, Resources.ProjectsSuccessfullyRelinked, currentDteProject.Name,
                                sourceDteProject.Name));
                        }
                        catch (NotImplementedException nex)
                        {
                            EnqueuePendingLink(sourceProjectId, currentDteProject);
                            _logger.Log(nex.Message);
                        }
                        catch (Exception projectLinkerEx)
                        {
                            _logger.Log(projectLinkerEx.Message);
                        }
                    }
                    else
                    {
                        EnqueuePendingLink(sourceProjectId, currentDteProject);
                    }
                }
            }
            catch (NotImplementedException nex)
            {
                _logger.Log(nex.Message);
            }
        }

        private void RemoveLinksForHierarchy(IVsHierarchy hierarchy)
        {
            Project project = hierarchy.GetProject();
            RemovePendingLinksToProject(project);
            foreach (IProjectItemsSynchronizer synch in Synchronizers.Where(s => s.SourceProject == project || s.TargetProject == project)
                .ToList())
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Removing project link between {0} and {1}",
                    synch.SourceProject.FullName, synch.TargetProject.FullName));
                if (synch.SourceProject == project)
                {
                    EnqueuePendingLink(hierarchy.GetProjectGuid(), synch.TargetProject);
                }
                Synchronizers.Remove(synch);

                if (synch.SourceProject == project)
                {
                    _logger.Log(string.Format(CultureInfo.CurrentCulture, Resources.ProjectsUnlinkedOnSourceClose, synch.TargetProject.Name,
                        synch.SourceProject.Name));
                }
                else
                {
                    _logger.Log(string.Format(CultureInfo.CurrentCulture, Resources.ProjectsUnlinkedOnTargetClose, synch.TargetProject.Name,
                        synch.SourceProject.Name));
                }
            }
        }

        private Project DequeueProjectLink(Guid projectId)
        {
            KeyValuePair<Guid, Project> item = _pendingLinks.FirstOrDefault(x => x.Key == projectId);
            if (default(KeyValuePair<Guid, Project>).Equals(item))
                return null;

            _pendingLinks.Remove(item);
            return item.Value;
        }

        private void EnqueuePendingLink(Guid projectId, Project targetProject)
        {
            _pendingLinks.Add(new KeyValuePair<Guid, Project>(projectId, targetProject));
            _logger.Log(string.Format(CultureInfo.CurrentCulture, Resources.TargetProjectWaitingForSource, targetProject.Name, projectId));
        }

        private void RemovePendingLinksToProject(Project targetProject)
        {
            foreach (KeyValuePair<Guid, Project> link in _pendingLinks.Where(x => x.Value == targetProject).ToList())
            {
                _pendingLinks.Remove(link);
            }
        }

        protected IProjectItemsSynchronizer CreateSynchronizer(Project sourceProject, Project targetProject)
        {
            IProjectItemsFilter projectItemsFilter = FilterManager.GetFilterForProject(targetProject);
            return new ProjectItemsSynchronizer(sourceProject, targetProject, _logger, _solution, projectItemsFilter);
        }
    }

    public static class EnumerableUtilities
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
                action(item);
        }
    }
}