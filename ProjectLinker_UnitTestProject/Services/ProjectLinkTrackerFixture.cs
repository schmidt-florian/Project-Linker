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
using System.Linq;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectLinker.Services;
using ProjectLinker.Tests.Mocks;
using ProjectLinker.UnitTestLibrary.Mocks;
using MockVsHierarchy = ProjectLinker.Tests.Mocks.MockVsHierarchy;

namespace ProjectLinker.Tests.Services
{
    [TestClass]
    public class ProjectLinkTrackerFixture
    {
        [TestMethod]
        public void ShouldRegisterInDocumentTracker()
        {
            MockDocumentTracker documentTracker = new MockDocumentTracker();

            ProjectLinkTracker tracker = new ProjectLinkTracker(documentTracker, new MockIVsSolution(), null);

            Assert.IsTrue(documentTracker.AdviseTrackProjectDocumentsEventsCalled);
            Assert.AreSame(tracker, documentTracker.AdviseTrackProjectDocumentsEventsArgumentEventSink);
        }

        [TestMethod]
        public void ShouldRegisterInSolutionEvents()
        {
            MockIVsSolution vsSolution = new MockIVsSolution();

            ProjectLinkTracker tracker = new ProjectLinkTracker(new MockDocumentTracker(), vsSolution, null);

            Assert.IsTrue(vsSolution.AdviseSolutionEventsCalled);
        }

        [TestMethod]
        public void ShouldAddNewItemsToSync()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy project1VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project2VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy.GetPropertyProjectIdGuidValue, project2VsHierarchy.GetPropertyProjectIdGuidValue);

            Assert.IsTrue(tracker.ProjectsAreLinked(project1VsHierarchy, project2VsHierarchy));
            Assert.IsFalse(tracker.ProjectsAreLinked(project2VsHierarchy, project1VsHierarchy));
        }

        [TestMethod]
        public void ShouldTrackMultipleProjects()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy project1VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project2VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
            tracker.AddProjectLink(project1VsHierarchy, project3VsHierarchy);

            Assert.IsTrue(tracker.ProjectsAreLinked(project1VsHierarchy, project2VsHierarchy));
            Assert.IsTrue(tracker.ProjectsAreLinked(project1VsHierarchy, project3VsHierarchy));
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldThrowIfAddingSameLink()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy project1VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project2VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
        }

        [TestMethod]
        public void ShouldDispatchAddToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            MockVsHierarchy project1 = new MockVsHierarchy();
            MockVsHierarchy project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterAddFilesEx(1, 1, new[] {project1}, new[] {0}, new[] {"File1.txt"},
                new[] {VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags});

            Assert.IsTrue(syncher.FilesAddedToSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchOnlyToMatchingSynchers()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            MockVsHierarchy project1 = new MockVsHierarchy();
            MockVsHierarchy project2 = new MockVsHierarchy();
            MockVsHierarchy project3 = new MockVsHierarchy();

            MockProjectItemsSyncer syncherMatching = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);
            MockProjectItemsSyncer syncherNonMatching =
                new MockProjectItemsSyncer(project3.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncherMatching);
            tracker.AddProjectSyncer(syncherNonMatching);

            tracker.OnAfterAddFilesEx(1, 1, new[] {project1}, new[] {0}, new[] {"File1.txt"},
                new[] {VSADDFILEFLAGS.VSADDFILEFLAGS_NoFlags});

            Assert.IsTrue(syncherMatching.FilesAddedToSourceCalled);
            Assert.IsFalse(syncherNonMatching.FilesAddedToSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchRemoveToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            MockVsHierarchy project1 = new MockVsHierarchy();
            MockVsHierarchy project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterRemoveFiles(1, 1, new[] {project1}, new[] {0}, new[] {"File1.txt"},
                new[] {VSREMOVEFILEFLAGS.VSREMOVEFILEFLAGS_NoFlags});

            Assert.IsTrue(syncher.FilesRemovedFromSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchRenameFilesToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            MockVsHierarchy project1 = new MockVsHierarchy();
            MockVsHierarchy project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterRenameFiles(1, 2, new[] {project1}, new[] {0}, new[] {"oldFileName", "oldDirectoryName"},
                new[] {"newFileName", "newDirectoryName"},
                new[] {VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_NoFlags, VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_Directory});

            Assert.IsTrue(syncher.FilesRenamedInSourceCalled);
        }


        [TestMethod]
        public void ShouldDispatchAddDirectoriesToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            MockVsHierarchy project1 = new MockVsHierarchy();
            MockVsHierarchy project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterAddDirectoriesEx(1, 1, new[] {project1}, new[] {0}, new[] {"Myfolder"},
                new[] {VSADDDIRECTORYFLAGS.VSADDDIRECTORYFLAGS_NoFlags});

            Assert.IsTrue(syncher.DirectoriesAddedToSourceCalled);
        }

        [TestMethod]
        public void ShouldDispatchRemoveDirectoriesToSyncher()
        {
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), new MockIVsSolution());
            MockVsHierarchy project1 = new MockVsHierarchy();
            MockVsHierarchy project2 = new MockVsHierarchy();

            MockProjectItemsSyncer syncher = new MockProjectItemsSyncer(project1.GetPropertyProjectValue, project2.GetPropertyProjectValue);

            tracker.AddProjectSyncer(syncher);

            tracker.OnAfterRemoveDirectories(1, 1, new[] {project1}, new[] {0}, new[] {"Myfolder"},
                new[] {VSREMOVEDIRECTORYFLAGS.VSREMOVEDIRECTORYFLAGS_NoFlags});

            Assert.IsTrue(syncher.DirectoriesRemovedFromSourceCalled);
        }

        [TestMethod]
        public void ShouldCreateSyncherWhenLoadingLinkedTargetProjectAfterSourceProject()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] =
                sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();

            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            tracker.OnAfterOpenProject(targetHierarchy, 0);

            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldCreateSyncherWhenLoadingLinkedSourceProjectAfterTargetProject()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] =
                sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();

            tracker.OnAfterOpenProject(targetHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void AddProjectLinkAddsPersistInfoToTargetProject()
        {
            MockIVsSolution solution = new MockIVsSolution();
            ProjectLinkTracker tracker = new ProjectLinkTracker(new MockDocumentTracker(), solution, null);
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();

            tracker.AddProjectLink(sourceHierarchy, targetHierarchy);

            Assert.IsTrue(targetHierarchy.GetPropertyProjectValue.Globals.Dictionary.ContainsKey("ProjectLinkReference"));
            Assert.AreEqual(sourceHierarchy.GetPropertyProjectIdGuidValue.ToString(),
                targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"]);
            Assert.IsTrue(targetHierarchy.GetPropertyProjectValue.Globals.SetVariablePersistsCalled);
        }

        [TestMethod]
        public void ShouldStopTrackingIfTargetProjectIsClosed()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] =
                sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);
            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            tracker.OnBeforeCloseProject(targetHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldStopTrackingIfSourceProjectIsClosed()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] =
                sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);
            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));

            tracker.OnBeforeCloseProject(sourceHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldRestartTrackingIfSourceProjectIsClosedAndReopened()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] =
                sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);

            tracker.OnBeforeCloseProject(sourceHierarchy, 0);
            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsTrue(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        public void ShouldNotRestartTrackingIfSourceAndTargetProjectsAreClosedAndSourceGetsReopened()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] =
                sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();
            solution.Hierarchies.Add(sourceHierarchy);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);
            tracker.OnAfterOpenProject(targetHierarchy, 0);

            tracker.OnBeforeCloseProject(sourceHierarchy, 0);
            tracker.OnBeforeCloseProject(targetHierarchy, 0);
            tracker.OnAfterOpenProject(sourceHierarchy, 0);

            Assert.IsFalse(tracker.ProjectsAreLinked(sourceHierarchy, targetHierarchy));
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldNotLinkIfSourceIsAlreadyLinkedAsTarget()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy project1VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project2VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);
            tracker.AddProjectLink(project2VsHierarchy, project3VsHierarchy);
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldNotLinkTwiceTheSameTarget()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy project1VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project2VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project3VsHierarchy);
            tracker.AddProjectLink(project2VsHierarchy, project3VsHierarchy);
        }

        [TestMethod]
        [ExpectedException(typeof(ProjectLinkerException))]
        public void ShouldNotLinkIfTargetIsAlreadyASource()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy project1VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project2VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project3VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);
            solution.Hierarchies.Add(project3VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project3VsHierarchy);
            tracker.AddProjectLink(project2VsHierarchy, project1VsHierarchy);
        }

        [TestMethod]
        public void ShouldGetEmptyLinkedProjectList()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);

            IEnumerable<ProjectLink> projectLinks = tracker.GetProjectLinks();

            Assert.IsNotNull(projectLinks);
            Assert.AreEqual(0, projectLinks.Count());
        }

        [TestMethod]
        public void ShouldGetLinkedProject()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy project1VsHierarchy = new MockVsHierarchy();
            MockVsHierarchy project2VsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(project1VsHierarchy);
            solution.Hierarchies.Add(project2VsHierarchy);

            tracker.AddProjectLink(project1VsHierarchy, project2VsHierarchy);

            IEnumerable<ProjectLink> projectLinks = tracker.GetProjectLinks();

            Assert.AreEqual(1, projectLinks.Count());
            ProjectLink projectLink = projectLinks.ElementAt(0);
            Assert.AreEqual(project1VsHierarchy.GetPropertyProjectIdGuidValue, projectLink.SourceProjectId);
            Assert.AreEqual(project2VsHierarchy.GetPropertyProjectIdGuidValue, projectLink.TargetProjectId);
        }

        [TestMethod]
        public void ShouldUnlinkProjects()
        {
            MockIVsSolution solution = new MockIVsSolution();
            TestableProjectLinkTracker tracker = new TestableProjectLinkTracker(new MockDocumentTracker(), solution);
            MockVsHierarchy sourceVsHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetVsHierarchy = new MockVsHierarchy();
            solution.Hierarchies.Add(sourceVsHierarchy);
            solution.Hierarchies.Add(targetVsHierarchy);
            tracker.AddProjectLink(sourceVsHierarchy, targetVsHierarchy);
            targetVsHierarchy.GetPropertyProjectValue.Globals.SetVariablePersistsCalled = false;

            tracker.UnlinkProjects(sourceVsHierarchy.GetPropertyProjectIdGuidValue, targetVsHierarchy.GetPropertyProjectIdGuidValue);

            Assert.AreEqual(0, tracker.GetProjectLinks().Count());
            Assert.IsTrue(targetVsHierarchy.GetPropertyProjectValue.Globals.SetVariablePersistsCalled);
            Assert.IsFalse(targetVsHierarchy.GetPropertyProjectValue.Globals.SetVariablePersistsArgumentValue);
        }

        [TestMethod]
        public void ShouldRestoreLinksOnServiceInitialization()
        {
            MockIVsSolution solution = new MockIVsSolution();
            MockVsHierarchy sourceHierarchy = new MockVsHierarchy();
            MockVsHierarchy targetHierarchy = new MockVsHierarchy();
            targetHierarchy.GetPropertyProjectValue.Globals.Dictionary["ProjectLinkReference"] =
                sourceHierarchy.GetPropertyProjectIdGuidValue.ToString();

            solution.Hierarchies.Add(sourceHierarchy);
            solution.Hierarchies.Add(targetHierarchy);

            MockSolution dteSolution = new MockSolution();
            dteSolution.Projects.List.Add(sourceHierarchy.GetPropertyProjectValue);
            dteSolution.Projects.List.Add(targetHierarchy.GetPropertyProjectValue);

            ProjectLinkTracker tracker = new ProjectLinkTracker(new MockDocumentTracker(), solution, new MockLogger(), dteSolution);

            List<ProjectLink> links = tracker.GetProjectLinks().ToList();
            Assert.AreEqual(1, links.Count);
            Assert.AreEqual(sourceHierarchy.GetPropertyProjectIdGuidValue, links[0].SourceProjectId);
            Assert.AreEqual(targetHierarchy.GetPropertyProjectIdGuidValue, links[0].TargetProjectId);
        }
    }

    internal class MockSolution : Solution4
    {
        public MockProjects Projects = new MockProjects();

        Projects Solution4.Projects => Projects;

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region _Solution Members

        Projects _Solution.Projects => throw new NotImplementedException();

        #endregion

        #region _Solution Members

        Project _Solution.AddFromFile(string fileName, bool exclusive)
        {
            throw new NotImplementedException();
        }

        Project _Solution.AddFromTemplate(string fileName, string destination, string projectName, bool exclusive)
        {
            throw new NotImplementedException();
        }

        AddIns _Solution.AddIns => throw new NotImplementedException();

        void _Solution.Close(bool saveFirst)
        {
            throw new NotImplementedException();
        }

        int _Solution.Count => throw new NotImplementedException();

        void _Solution.Create(string destination, string name)
        {
            throw new NotImplementedException();
        }

        DTE _Solution.DTE => throw new NotImplementedException();

        string _Solution.ExtenderCATID => throw new NotImplementedException();

        object _Solution.ExtenderNames => throw new NotImplementedException();

        string _Solution.FileName => throw new NotImplementedException();

        ProjectItem _Solution.FindProjectItem(string fileName)
        {
            throw new NotImplementedException();
        }

        string _Solution.FullName => throw new NotImplementedException();

        IEnumerator _Solution.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        Globals _Solution.Globals => throw new NotImplementedException();

        bool _Solution.IsDirty
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        bool _Solution.IsOpen => throw new NotImplementedException();

        Project _Solution.Item(object index)
        {
            throw new NotImplementedException();
        }

        void _Solution.Open(string fileName)
        {
            throw new NotImplementedException();
        }

        DTE _Solution.Parent => throw new NotImplementedException();

        string _Solution.ProjectItemsTemplatePath(string projectKind)
        {
            throw new NotImplementedException();
        }

        Properties _Solution.Properties => throw new NotImplementedException();

        void _Solution.Remove(Project proj)
        {
            throw new NotImplementedException();
        }

        void _Solution.SaveAs(string fileName)
        {
            throw new NotImplementedException();
        }

        bool _Solution.Saved
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        SolutionBuild _Solution.SolutionBuild => throw new NotImplementedException();

        object _Solution.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }

        string _Solution.get_TemplatePath(string projectType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Solution4 Members

        Project Solution4.AddFromFile(string fileName, bool exclusive = true)
        {
            throw new NotImplementedException();
        }

        Project Solution4.AddFromTemplate(string fileName, string destination, string projectName, bool exclusive = true)
        {
            throw new NotImplementedException();
        }

        public Project AddFromTemplateEx(string fileName, string destination, string projectName, string solutionName, bool exclusive = true,
            uint options = 0)
        {
            throw new NotImplementedException();
        }

        AddIns Solution4.AddIns => throw new NotImplementedException();

        public Project AddSolutionFolder(string name)
        {
            throw new NotImplementedException();
        }

        void Solution4.Close(bool saveFirst = true)
        {
            throw new NotImplementedException();
        }

        int Solution4.Count => throw new NotImplementedException();

        void Solution4.Create(string destination, string name)
        {
            throw new NotImplementedException();
        }

        DTE Solution4.DTE => throw new NotImplementedException();

        string Solution4.ExtenderCATID => throw new NotImplementedException();

        object Solution4.ExtenderNames => throw new NotImplementedException();

        string Solution4.FileName => throw new NotImplementedException();

        ProjectItem Solution4.FindProjectItem(string fileName)
        {
            throw new NotImplementedException();
        }

        string Solution4.FullName => throw new NotImplementedException();

        IEnumerator Solution4.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public string GetProjectItemTemplate(string templateName, string language)
        {
            throw new NotImplementedException();
        }

        public Templates GetProjectItemTemplates(string language, string customDataSignature)
        {
            throw new NotImplementedException();
        }

        public string GetProjectTemplate(string templateName, string language)
        {
            throw new NotImplementedException();
        }

        Globals Solution4.Globals => throw new NotImplementedException();

        bool Solution4.IsDirty
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        bool Solution4.IsOpen => throw new NotImplementedException();

        Project Solution4.Item(object index)
        {
            throw new NotImplementedException();
        }

        void Solution4.Open(string fileName)
        {
            throw new NotImplementedException();
        }

        DTE Solution4.Parent => throw new NotImplementedException();

        string Solution4.ProjectItemsTemplatePath(string projectKind)
        {
            throw new NotImplementedException();
        }


        Properties Solution4.Properties => throw new NotImplementedException();

        void Solution4.Remove(Project proj)
        {
            throw new NotImplementedException();
        }

        void Solution4.SaveAs(string fileName)
        {
            throw new NotImplementedException();
        }

        bool Solution4.Saved
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        SolutionBuild Solution4.SolutionBuild => throw new NotImplementedException();

        object Solution4.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }

        string Solution4.get_TemplatePath(string projectType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Solution3 Members

        Project Solution3.AddFromFile(string fileName, bool exclusive = false)
        {
            throw new NotImplementedException();
        }

        Project Solution3.AddFromTemplate(string fileName, string destination, string projectName, bool exclusive = false)
        {
            throw new NotImplementedException();
        }

        AddIns Solution3.AddIns => throw new NotImplementedException();

        void Solution3.Close(bool saveFirst = false)
        {
            throw new NotImplementedException();
        }

        int Solution3.Count => throw new NotImplementedException();

        void Solution3.Create(string destination, string name)
        {
            throw new NotImplementedException();
        }

        DTE Solution3.DTE => throw new NotImplementedException();

        string Solution3.ExtenderCATID => throw new NotImplementedException();

        object Solution3.ExtenderNames => throw new NotImplementedException();

        string Solution3.FileName => throw new NotImplementedException();

        ProjectItem Solution3.FindProjectItem(string fileName)
        {
            throw new NotImplementedException();
        }

        string Solution3.FullName => throw new NotImplementedException();

        IEnumerator Solution3.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        Globals Solution3.Globals => throw new NotImplementedException();

        bool Solution3.IsDirty
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        bool Solution3.IsOpen => throw new NotImplementedException();

        Project Solution3.Item(object index)
        {
            throw new NotImplementedException();
        }

        void Solution3.Open(string fileName)
        {
            throw new NotImplementedException();
        }

        DTE Solution3.Parent => throw new NotImplementedException();

        string Solution3.ProjectItemsTemplatePath(string projectKind)
        {
            throw new NotImplementedException();
        }

        Projects Solution3.Projects => throw new NotImplementedException();

        Properties Solution3.Properties => throw new NotImplementedException();

        void Solution3.Remove(Project proj)
        {
            throw new NotImplementedException();
        }

        void Solution3.SaveAs(string fileName)
        {
            throw new NotImplementedException();
        }

        bool Solution3.Saved
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        SolutionBuild Solution3.SolutionBuild => throw new NotImplementedException();

        object Solution3.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }

        string Solution3.get_TemplatePath(string projectType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Solution2 Members

        Project Solution2.AddFromFile(string fileName, bool exclusive = false)
        {
            throw new NotImplementedException();
        }

        Project Solution2.AddFromTemplate(string fileName, string destination, string projectName, bool exclusive = false)
        {
            throw new NotImplementedException();
        }

        AddIns Solution2.AddIns => throw new NotImplementedException();

        void Solution2.Close(bool saveFirst = false)
        {
            throw new NotImplementedException();
        }

        int Solution2.Count => throw new NotImplementedException();

        void Solution2.Create(string destination, string name)
        {
            throw new NotImplementedException();
        }

        DTE Solution2.DTE => throw new NotImplementedException();

        string Solution2.ExtenderCATID => throw new NotImplementedException();

        object Solution2.ExtenderNames => throw new NotImplementedException();

        string Solution2.FileName => throw new NotImplementedException();

        ProjectItem Solution2.FindProjectItem(string fileName)
        {
            throw new NotImplementedException();
        }

        string Solution2.FullName => throw new NotImplementedException();

        IEnumerator Solution2.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        Globals Solution2.Globals => throw new NotImplementedException();

        bool Solution2.IsDirty
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        bool Solution2.IsOpen => throw new NotImplementedException();

        Project Solution2.Item(object index)
        {
            throw new NotImplementedException();
        }

        void Solution2.Open(string fileName)
        {
            throw new NotImplementedException();
        }

        DTE Solution2.Parent => throw new NotImplementedException();

        string Solution2.ProjectItemsTemplatePath(string projectKind)
        {
            throw new NotImplementedException();
        }

        Projects Solution2.Projects => throw new NotImplementedException();

        Properties Solution2.Properties => throw new NotImplementedException();

        void Solution2.Remove(Project proj)
        {
            throw new NotImplementedException();
        }

        void Solution2.SaveAs(string fileName)
        {
            throw new NotImplementedException();
        }

        bool Solution2.Saved
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        SolutionBuild Solution2.SolutionBuild => throw new NotImplementedException();

        object Solution2.get_Extender(string extenderName)
        {
            throw new NotImplementedException();
        }

        string Solution2.get_TemplatePath(string projectType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class MockProjects : Projects
    {
        public List<Project> List = new List<Project>();

        #region Projects Members

        public int Count => throw new NotImplementedException();

        public DTE DTE => throw new NotImplementedException();

        public IEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public Project Item(object index)
        {
            throw new NotImplementedException();
        }

        public string Kind => throw new NotImplementedException();

        public DTE Parent => throw new NotImplementedException();

        public Properties Properties => throw new NotImplementedException();

        #endregion
    }

    internal class TestableProjectLinkTracker : ProjectLinkTracker
    {
        public TestableProjectLinkTracker(IVsTrackProjectDocuments2 documentTracker, IVsSolution solution)
            : base(documentTracker, solution, new MockLogger())
        {
        }

        public bool ProjectsAreLinked(IVsHierarchy sourceProject, IVsHierarchy targetProject)
        {
            return LinkExists(sourceProject, targetProject);
        }

        public void AddProjectSyncer(IProjectItemsSynchronizer syncher)
        {
            Synchronizers.Add(syncher);
        }
    }
}