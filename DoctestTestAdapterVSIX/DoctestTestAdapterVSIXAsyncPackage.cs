using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using VS.Common.DoctestTestAdapter;
using VS.Common.DoctestTestAdapter.Options;
using Task = System.Threading.Tasks.Task;

namespace DoctestTestAdapterVSIX
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(DoctestTestAdapterVSIXAsyncPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(GeneralOptionsPage), VS.Common.DoctestTestAdapter.Constants.Options.ToolsOptionName, VS.Common.DoctestTestAdapter.Constants.Options.GeneralCategoryName, 0, 0, true)]
    public sealed class DoctestTestAdapterVSIXAsyncPackage : AsyncPackage
    {
        /// <summary>
        /// DoctestTestAdapterVSIXAsyncPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "d952b4df-4d2a-4549-a5d3-5467ad8762c2";

        private ITestAdapterOptions testAdapterOptions = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctestTestAdapterVSIXAsyncPackage"/> class.
        /// </summary>
        public DoctestTestAdapterVSIXAsyncPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            InitializeOptions();

            Logger.Instance.WriteLine("DoctestTestAdapterVSIXAsyncPackage InitializeAsync was called");
        }

        private void InitializeOptions()
        {
            GeneralOptionsPage generalOptionsPage = (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));
            Debug.Assert(generalOptionsPage != null);

            testAdapterOptions = new TestAdapterOptions(generalOptionsPage);
        }

        #endregion
    }
}
