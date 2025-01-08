using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    internal sealed class FixLintErrorsCommand : LintFilesCommandBase
    {
        private FixLintErrorsCommand(AsyncPackage package) : base(package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(PackageGuids.SolutionExplorerGuid, PackageIds.FixLintErrorsCommand);
                var menuItem = new OleMenuCommand((s, e) => package.JoinableTaskFactory.RunAsync(
                    () => LintItemsSelectedInSolutionExplorerAsync(fixErrors: true)).Forget(), menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        public static FixLintErrorsCommand Instance { get; private set; }
        public static void Initialize(AsyncPackage package) => Instance = new FixLintErrorsCommand(package);
    }
}
