using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    internal class LintActiveFileCommand : LintActiveFileCommandBase
    {

        private LintActiveFileCommand(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException("package");
            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(PackageGuids.CodeFileGuid, PackageIds.LintActiveFileCommand);
                var menuItem = new OleMenuCommand((s, e) => package.JoinableTaskFactory.RunAsync(
                    () => LintActiveFileAsync(fixErrors: false)), menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        public static void Initialize(AsyncPackage package) => Instance = new LintActiveFileCommand(package);

    }
}
