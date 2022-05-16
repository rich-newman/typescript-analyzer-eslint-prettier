using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using TypeScriptAnalyzerEslintLinter;
using Microsoft.VisualStudio.Threading;

namespace TypeScriptAnalyzerEslintVsix
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", TypeScriptAnalyzerEslintLinter.Constants.VERSION, IconResourceID = 400)]
    [ProvideOptionPage(typeof(Settings), "TypeScript Analyzer", "ESLint", 101, 111, true, new[] { "eslint" }, ProvidesLocalizedCategoryName = false)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(PackageGuids.guidVSPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class Package : AsyncPackage
    {
        public static DTE2 Dte;
        public static ISettings Settings;
        public static TaggerProvider TaggerProvider;
        public static JoinableTaskFactory Jtf;
        private SolutionEvents events;
        public static List<Tuple<IWpfTextView, ITextDocument>> UnhandledStartUpFiles = new List<Tuple<IWpfTextView, ITextDocument>>();

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
//#if DEBUG
//            VsShellUtilities.ShowMessageBox(
//                this,
//                "InitializeAsync was called",
//                "",
//                OLEMSGICON.OLEMSGICON_INFO,
//                OLEMSGBUTTON.OLEMSGBUTTON_OK,
//                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
//#endif
            Logger.Initialize(this, Vsix.Name);
//#if DEBUG
            Logger.Log("TypeScript Analyzer (ESLint) initialization called");
//#endif
            Jtf = JoinableTaskFactory;
            Dte = await GetServiceAsync(typeof(DTE)) as DTE2;
            Microsoft.Assumes.Present(Dte);
            Settings = (Settings)GetDialogPage(typeof(Settings));
            Settings.Initialize();

            events = Dte.Events.SolutionEvents;
            events.AfterClosing += delegate { ErrorListDataSource.Instance.CleanAllErrors(); }; // Called on UI thread

            //Logger.Initialize(this, Vsix.Name);

            bool isSolutionLoaded = await IsSolutionLoadedAsync();
            if (isSolutionLoaded) await HandleOpenSolutionAsync();

            LintFilesCommand.Initialize(this);
            FixLintErrorsCommand.Initialize(this);
            CleanErrorsCommand.Initialize(this);
            LintActiveFileCommand.Initialize(this);
            FixActiveFileCommand.Initialize(this);
            BuildEventsListener.Initialize(this);
            EditConfigFilesCommand.Initialize(this);
            ResetConfigFilesCommand.Initialize(this);
#if GCTESTING
            SetWeakReferenceCommand.Initialize(this);
            DisplayWeakReferenceCommand.Initialize(this);
#endif
            base.Initialize();
        }

        private async System.Threading.Tasks.Task<bool> IsSolutionLoadedAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsSolution solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            Microsoft.Assumes.Present(solService);
            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object value));
            return value is bool isSolOpen && isSolOpen;
        }

        private async System.Threading.Tasks.Task HandleOpenSolutionAsync(object sender = null, EventArgs e = null)
        {
            foreach (Tuple<IWpfTextView, ITextDocument> tuple in UnhandledStartUpFiles)
            {
                await FileListener.OnFileOpenedAsync(tuple.Item1, tuple.Item2);
            }
            UnhandledStartUpFiles.Clear();
        }

        public IVsOutputWindow GetIVsOutputWindow()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return (IVsOutputWindow)GetService(typeof(SVsOutputWindow));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Linter.Server.Down();
            base.Dispose(true);
        }
    }
}