using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using EnvDTE;
using EnvDTE100;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectLinker.Helper;
using ProjectLinker.Services;

namespace ProjectLinker
{
    public class ServiceCreator
    {
        public static ServiceCreator Instance;
        private readonly ProjectLinkerPackage _package;

        private ServiceCreator(ProjectLinkerPackage package)
        {
            _package = package;

            InitializeProjectLinkTracker();
        }

        public static void Initialize(ProjectLinkerPackage package)
        {
            Instance = new ServiceCreator(package);
        }

        private void InitializeProjectLinkTracker()
        {
            CreateOutputWindow();

            object tracker = CreateProjectLinkTracker(_package, typeof(ISProjectLinkTracker));

            if (tracker != null)
            {
                Trace.WriteLine($"AddService: {nameof(ISProjectLinkTracker)} {tracker}");
                Trace.WriteLine($"AddService: {nameof(IHierarchyNodeFactory)}");

                ((IServiceContainer) _package).AddService(typeof(ISProjectLinkTracker), tracker, true);
                ((IServiceContainer) _package).AddService(typeof(IHierarchyNodeFactory), new HierarchyNodeFactory(_package));
            }
            else
            {
                Debug.Fail("ProjectLinkTracker creation failed");
            }
        }

        private void CreateOutputWindow()
        {
            const int initiallyVisible = 1;
            const int clearWhenSolutionUnloads = 1;

            Guid outputPaneGuid = ProjectLinkerGuids.GuidProjectLinkerOutputPane;
            IVsOutputWindow outputWindow = _package.GetService<IVsOutputWindow, SVsOutputWindow>();
            if (outputWindow == null)
                throw new ProjectLinkerException(Resources.FailedToCreateOutputWindow);

            if (ErrorHandler.Failed(outputWindow.GetPane(ref outputPaneGuid, out IVsOutputWindowPane existingPane)) || existingPane == null)
            {
                ErrorHandler.ThrowOnFailure(outputWindow.CreatePane(ref outputPaneGuid,
                    Resources.LoggerOutputPaneTitle,
                    initiallyVisible,
                    clearWhenSolutionUnloads)
                );
            }
        }

        private object CreateProjectLinkTracker(IServiceContainer container, Type serviceType)
        {
            if (container != _package)
            {
                return null;
            }

            try
            {
                if (typeof(ISProjectLinkTracker) == serviceType)
                {
                    IVsTrackProjectDocuments2 trackProjectDocuments = _package.GetService<IVsTrackProjectDocuments2, SVsTrackProjectDocuments>();
                    IVsSolution solution = _package.GetService<IVsSolution, SVsSolution>();
                    if (solution == null)
                    {
                        _package.ShowInformationalMessageBox("Solution null", "Solution null", false);
                    }
                    IVsOutputWindow outputWindow = _package.GetService<IVsOutputWindow, SVsOutputWindow>();
                    DTE dte = _package.GetService<DTE, SDTE>();
                    Solution4 dteSolution = null;
                    if (dte != null)
                    {
                        dteSolution = (Solution4) dte.Solution;
                    }

                    return new ProjectLinkTracker(trackProjectDocuments, solution, new OutputWindowLogger(outputWindow), dteSolution);
                }
            }
            catch (NotImplementedException nex)
            {
                _package.ShowInformationalMessageBox(Resources.ProjectLinkerCaption, nex.Message + nex.StackTrace, true);
            }
            catch (Exception ex)
            {
                _package.ShowInformationalMessageBox(Resources.ProjectLinkerCaption, ex.Message, true);
            }

            return null;
        }
    }
}