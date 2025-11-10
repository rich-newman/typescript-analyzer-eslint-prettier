using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Workspace;
using Microsoft.VisualStudio.Workspace.Extensions.VS;
using Task = System.Threading.Tasks.Task;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    [ExportFileContextActionProvider((FileContextActionProviderOptions)VsCommandActionProviderOptions.SupportVsCommands,
        ProviderType, ProviderPriority.Normal, LintableFileContextProviderFactory.LintableFileContextType)]
    public class LintActionProviderFactory : IWorkspaceProviderFactory<IFileContextActionProvider>, IVsCommandActionProvider
    {
        private const string ProviderType = "F77E9835-FB7C-404D-BFB8-A10F4810557B";

        private static readonly Guid ProviderCommandGroup = PackageGuids.FolderViewGuid;
        private static readonly IReadOnlyList<CommandID> SupportedCommands = new List<CommandID>
            {
                new CommandID(PackageGuids.FolderViewGuid, PackageIds.FolderViewLintFilesCommand),
                new CommandID(PackageGuids.FolderViewGuid, PackageIds.FolderViewFixLintErrorsCommand),
            };

        public IFileContextActionProvider CreateProvider(IWorkspace workspaceContext)
        {
            return new LintActionProvider();
        }

        public IReadOnlyCollection<CommandID> GetSupportedVsCommands()
        {
            return SupportedCommands;
        }

        internal class LintActionProvider : IFileContextActionProvider
        {
            private static readonly Guid ActionOutputWindowPane = new Guid("{35F304B6-2329-4A0C-B9BE-92AFAB7AF858}");

            public Task<IReadOnlyList<IFileContextAction>> GetActionsAsync(string filePath, FileContext fileContext, CancellationToken cancellationToken)
            {
                return Task.FromResult<IReadOnlyList<IFileContextAction>>(new IFileContextAction[]
                {
                    new MyContextAction(
                        fileContext,
                        new Tuple<Guid, uint>(ProviderCommandGroup, PackageIds.FolderViewLintFilesCommand),
                        "My Action" + fileContext.DisplayName,
                        async (fCtxt, progress, ct) =>
                        {
                            // Outputs the 'Before Lint' text in the Output window of the client VS, under 'Actions' in the dropdown
                            //await OutputWindowPaneAsync("Before Lint\n");
                            await LintFilesCommandBase.LintItemsSelectedInSolutionExplorerAsync(false, fCtxt.Context.ToString());
                        }),

                    new MyContextAction(
                        fileContext,
                        new Tuple<Guid, uint>(ProviderCommandGroup, PackageIds.FolderViewFixLintErrorsCommand),
                        "My Action" + fileContext.DisplayName,
                        async (fCtxt, progress, ct) =>
                        {
                            await LintFilesCommandBase.LintItemsSelectedInSolutionExplorerAsync(true, fCtxt.Context.ToString());
                        }),
                });
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

            internal class MyContextAction : IFileContextAction, IVsCommandItem
            {
                private readonly Func<FileContext, IProgress<IFileContextActionProgressUpdate>, CancellationToken, Task> executeAction;

                internal MyContextAction(
                    FileContext fileContext,
                    Tuple<Guid, uint> command,
                    string displayName,
                    Func<FileContext, IProgress<IFileContextActionProgressUpdate>, CancellationToken, Task> executeAction)
                {
                    this.executeAction = executeAction;
                    this.Source = fileContext;
                    this.CommandGroup = command.Item1;
                    this.CommandId = command.Item2;
                    this.DisplayName = displayName;
                }

                public Guid CommandGroup { get; }
                public uint CommandId { get; }
                public string DisplayName { get; }
                public FileContext Source { get; }

                public async Task<IFileContextActionResult> ExecuteAsync(IProgress<IFileContextActionProgressUpdate> progress, CancellationToken cancellationToken)
                {
                    await this.executeAction(this.Source, progress, cancellationToken);
                    return new FileContextActionResult(true);
                }
            }
        }
    }
}
