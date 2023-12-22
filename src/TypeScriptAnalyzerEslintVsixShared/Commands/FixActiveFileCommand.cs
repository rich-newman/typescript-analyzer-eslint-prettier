using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    internal class FixActiveFileCommand : LintActiveFileCommandBase
    {
        private FixActiveFileCommand(AsyncPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException("package");
            if (ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(PackageGuids.CodeFileGuid, PackageIds.FixActiveFileCommand);
                var menuItem = new OleMenuCommand((s, e) => package.JoinableTaskFactory.RunAsync(
                    () => LintActiveFileAsync(fixErrors: true)).Forget(), menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        public static void Initialize(AsyncPackage package) => Instance = new FixActiveFileCommand(package);
    }
}