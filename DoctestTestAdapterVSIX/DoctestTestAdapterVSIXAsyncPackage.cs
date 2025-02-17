using EnvDTE;
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
using VS.Common.DoctestTestAdapter.IO;
using VS.Common.DoctestTestAdapter.Options;
using VS.Common.DoctestTestAdapter.Packages;
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
    [Guid(VS.Common.DoctestTestAdapter.Constants.Package.GuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(GeneralOptionsPage), VS.Common.DoctestTestAdapter.Constants.Options.ToolsOptionName, VS.Common.DoctestTestAdapter.Constants.Options.GeneralCategoryName, 0, 0, true)]
    public sealed class DoctestTestAdapterVSIXAsyncPackage : AsyncPackage, ITestAdapterPackage
    {
        private ITestAdapterOptions testAdapterOptions = null;
        private string solutionDirectory = string.Empty;

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
            //TODO_comfyjase_17/02/2025: Comment back in once the thread exception has been fixed to fix compiler warning VSTHRD010.
            //ThreadHelper.ThrowIfNotOnUIThread();

            GeneralOptionsPage generalOptionsPage = (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));
            Debug.Assert(generalOptionsPage != null);

            IVsSolution solution = (IVsSolution)GetService(typeof(SVsSolution));
            Debug.Assert(solution != null);

            solution.GetSolutionInfo(out solutionDirectory, out string solutionName, out string solutionDirectory2);
            Debug.Assert(!string.IsNullOrEmpty(solutionDirectory));
            
            testAdapterOptions = new TestAdapterOptions(solutionDirectory, generalOptionsPage);
            Logger.Instance.CacheTestAdapterOptions(testAdapterOptions);
        }

        public ITestAdapterOptions TestAdapterOptions
        {
            get { return testAdapterOptions; }
        }


        #endregion
    }
}
