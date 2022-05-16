using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    internal sealed class LintFilesCommand : LintFilesCommandBase
    {
        private LintFilesCommand(AsyncPackage package): base(package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(PackageGuids.SolutionExplorerGuid, PackageIds.LintFilesCommand);
                var menuItem = new OleMenuCommand((s, e) => package.JoinableTaskFactory.RunAsync(
                    () => LintItemsSelectedInSolutionExplorerAsync(fixErrors: false)), menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        public static LintFilesCommand Instance { get; private set; }
        public static void Initialize(AsyncPackage package) => Instance = new LintFilesCommand(package);
    }
}
