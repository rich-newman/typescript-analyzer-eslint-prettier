#if VS2022
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Workspace;
//using Microsoft.VisualStudio.Workspace.Extensions.VS;
using Task = System.Threading.Tasks.Task;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    [ExportFileContextActionProvider(
        ProviderType,
        ProviderPriority.Normal,
        LintableFileContextProviderFactory.LintableFileContextType)]
    internal sealed class LintActionProviderFactory : IWorkspaceProviderFactory<IFileContextActionProvider>
    {
        private const string ProviderType = "F77E9835-FB7C-404D-BFB8-A10F4810557B";

        public IFileContextActionProvider CreateProvider(IWorkspace workspaceContext)
            => new LintActionProvider();


        internal sealed class LintActionProvider : IFileContextActionProvider
        {
            private static readonly Guid ActionOutputWindowPane = new Guid("{35F304B6-2329-4A0C-B9BE-92AFAB7AF858}");

            public Task<IReadOnlyList<IFileContextAction>> GetActionsAsync(
                string filePath, FileContext fileContext, CancellationToken cancellationToken)
            {
                var actions = new List<IFileContextAction>
                {
                    new MyContextAction(fileContext, "Lint " + fileContext.DisplayName,
                        async (ctx, progress, ct) =>
                        {
                            await OutputWindowPaneAsync("Test of messaging before linting");
                            await LintFilesCommandBase.LintItemsSelectedInSolutionExplorerAsync(false, ctx.Context.ToString());
                        }),
                    new MyContextAction(fileContext, "Fix lint errors in " + fileContext.DisplayName,
                        async (ctx, progress, ct) =>
                        {
                            await LintFilesCommandBase.LintItemsSelectedInSolutionExplorerAsync(true, ctx.Context.ToString());
                        })
                };

                return Task.FromResult<IReadOnlyList<IFileContextAction>>(actions);
            }

            internal static async Task OutputWindowPaneAsync(string message)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                IVsOutputWindowPane outputPane = null;
                if (ServiceProvider.GlobalProvider.GetService(typeof(SVsOutputWindow)) is IVsOutputWindow outputWindow
                    && ErrorHandler.Failed(outputWindow.GetPane(ActionOutputWindowPane, out outputPane)))
                {
                    if (ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell)) is IVsUIShell vsUiShell)
                    {
                        uint flags = (uint)__VSFINDTOOLWIN.FTW_fForceCreate;
                        vsUiShell.FindToolWindow(flags, VSConstants.StandardToolWindows.Output, out IVsWindowFrame windowFrame);
                        windowFrame.Show();
                    }

                    outputWindow.CreatePane(ActionOutputWindowPane, "Actions", 1, 1);
                    outputWindow.GetPane(ActionOutputWindowPane, out outputPane);
                    outputPane.Activate();
                }

                outputPane?.OutputStringThreadSafe(message);
            }

            internal sealed class MyContextAction : IFileContextAction
            {
                private readonly Func<FileContext, IProgress<IFileContextActionProgressUpdate>, CancellationToken, Task> _execute;

                public MyContextAction(FileContext source, string displayName,
                    Func<FileContext, IProgress<IFileContextActionProgressUpdate>, CancellationToken, Task> execute)
                {
                    Source = source;
                    DisplayName = displayName;
                    _execute = execute;
                }

                public string DisplayName { get; }
                public FileContext Source { get; }

                public async Task<IFileContextActionResult> ExecuteAsync(
                    IProgress<IFileContextActionProgressUpdate> progress,
                    CancellationToken cancellationToken)
                {
                    await _execute(Source, progress, cancellationToken);
                    return new FileContextActionResult(true);
                }
            }
        }
    }
}
#endif