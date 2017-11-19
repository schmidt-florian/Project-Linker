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
    public class ShellPropertyEventHandler : IVsShellPropertyEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly ProjectLinkerPackage _package;

        uint _cookie;

        public ShellPropertyEventHandler(ProjectLinkerPackage package)
        {
            _package = package;


            // set an eventlistener for shell property changes 
            IVsShell shellService = _package.GetService<IVsShell, SVsShell>();
            if (shellService != null)
                ErrorHandler.ThrowOnFailure(shellService.AdviseShellPropertyChanges(this, out _cookie));
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static ShellPropertyEventHandler Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        public int OnShellPropertyChange(int propid, object var)
        {
            if ((bool) var == false)
            {
                //zombie state dependent code/initialization
                CreateOutputWindow();

                object tracker = CreateProjectLinkTracker(_package, typeof(ISProjectLinkTracker));
                if (tracker != null)
                {
                    Debug.Assert(tracker != null, "ProjectLinkTracker creation failed");

                    Trace.WriteLine($"AddService: {nameof(ISProjectLinkTracker)} {tracker}");
                    Trace.WriteLine($"AddService: {nameof(IHierarchyNodeFactory)}");

                    ((IServiceContainer) ServiceProvider).AddService(typeof(ISProjectLinkTracker), tracker, true);
                    ((IServiceContainer) ServiceProvider).AddService(typeof(IHierarchyNodeFactory), new HierarchyNodeFactory(_package));

                    // eventlistener no longer needed
                    IVsShell shellService = _package.GetService<IVsShell, SVsShell>();
                    if (shellService != null)
                        ErrorHandler.ThrowOnFailure(shellService.UnadviseShellPropertyChanges(_cookie));

                    _cookie = 0;
                }
            }
            return VSConstants.S_OK;
        }


        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(ProjectLinkerPackage package)
        {
            Instance = new ShellPropertyEventHandler(package);
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