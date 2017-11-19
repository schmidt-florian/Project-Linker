using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectLinker
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The minimum requirement for a class to be considered a valid package for Visual Studio
    ///         is to implement the IVsPackage interface and register itself with the shell.
    ///         This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///         to do it: it derives from the Package class that provides the implementation of the
    ///         IVsPackage interface and uses the registration attributes defined in the framework to
    ///         register itself and its components with the shell. These attributes tell the pkgdef creation
    ///         utility what data to put into .pkgdef file.
    ///     </para>
    ///     <para>
    ///         To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
    ///         &gt; in .vsixmanifest file.
    ///     </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification =
        "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class ProjectLinkerPackage : Package
    {
        /// <summary>
        ///     ProjectLinkerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "220649b8-9113-441e-80e4-b8cb507e6e41";

        #region Package Members

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Trace.WriteLine($"Initialize: {nameof(ProjectLinkerPackage)}");
            ShellPropertyEventHandler.Initialize(this);
            AddProjectLinkCommand.Initialize(this);
            EditLinksCommand.Initialize(this);
        }

        public TInterface GetService<TInterface, TService>() where TInterface : class where TService : class
        {
            return GetService(typeof(TService)) as TInterface;
        }


        public void ShowInformationalMessageBox(string title, string text, bool modal)
        {
            IVsUIShell uiShell = (IVsUIShell) GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            int makeModal = modal ? 1 : 0;


            ErrorHandler.ThrowOnFailure(
                uiShell.ShowMessageBox(0, // Not used but required by api
                    ref clsid, // Not used but required by api
                    title,
                    text,
                    String.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    OLEMSGICON.OLEMSGICON_INFO,
                    makeModal,
                    out result
                )
            );
        }

        #endregion
    }
}