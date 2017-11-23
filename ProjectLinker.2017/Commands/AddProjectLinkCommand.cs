using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ProjectLinker.Helper;
using ProjectLinker.Services;
using ProjectLinker.SolutionPicker;

namespace ProjectLinker.Commands
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class AddProjectLinkCommand
    {
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("54522b3b-7790-4c8e-9ca3-1f37900f7638");

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly ProjectLinkerPackage _package;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddProjectLinkCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AddProjectLinkCommand(ProjectLinkerPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                CommandID menuCommandID = new CommandID(CommandSet, CommandId);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static AddProjectLinkCommand Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(ProjectLinkerPackage package)
        {
            Instance = new AddProjectLinkCommand(package);
        }

        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            IVsSolution solution = _package.GetService<IVsSolution, SVsSolution>();

            // As the "Edit Links" command is added on the Project menu even when no solution is opened, it must
            // be validated that a solution exists (in case it doesn't, GetSolutionInfo() returns 3 nulled strings).
            ErrorHandler.ThrowOnFailure(solution.GetSolutionInfo(out string s1, out string s2, out string s3));
            if (s1 != null && s2 != null && s3 != null)
            {
                IVsUIShell uiShell = _package.GetService<IVsUIShell, SVsUIShell>();

                ErrorHandler.ThrowOnFailure(uiShell.GetDialogOwnerHwnd(out IntPtr parentHwnd));

                HierarchyNodeFactory hierarchyNodeFactory = new HierarchyNodeFactory(ServiceProvider);
                IHierarchyNode targetProject = hierarchyNodeFactory.GetSelectedProject();
                SolutionPickerView solutionPicker = new SolutionPickerView(ServiceProvider, targetProject);

                DialogResult result = solutionPicker.ShowDialog(new WindowHandleAdapter(parentHwnd));

                if (result == DialogResult.OK)
                {
                    try
                    {
                        IHierarchyNode sourceProject = solutionPicker.SelectedNode;

                        IProjectLinkTracker linker = _package.GetService<IProjectLinkTracker, ISProjectLinkTracker>();

                        // todo
                        linker.AddProjectLink(sourceProject.ProjectGuid, targetProject.ProjectGuid);
                        if (solutionPicker.CopyProjectItemsByDefault)
                        {
                            linker.LinkAllProjectItems(sourceProject.ProjectGuid, targetProject.ProjectGuid);
                        }
                        _package.ShowInformationalMessageBox(
                            Resources.ProjectLinkerCaption,
                            Resources.ProjectsSuccessfullyLinked,
                            false);
                    }
                    catch (ProjectLinkerException ex)
                    {
                        _package.ShowInformationalMessageBox(Resources.ProjectLinkerCaption, ex.Message, true);
                    }
                }
            }
        }
    }
}