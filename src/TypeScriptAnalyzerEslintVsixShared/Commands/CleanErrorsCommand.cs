using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    internal sealed class CleanErrorsCommand
    {
        private readonly Microsoft.VisualStudio.Shell.Package package;
        private readonly BuildEvents events;

        private CleanErrorsCommand(Microsoft.VisualStudio.Shell.Package package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package;
            events = Package.Dte.Events.BuildEvents;
            events.OnBuildBegin += OnBuildBegin;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            Microsoft.Assumes.Present(commandService);

            var menuCommandID = new CommandID(PackageGuids.SolutionExplorerGuid, PackageIds.CleanErrorsCommand);
            var menuItem = new OleMenuCommand(CleanErrors, menuCommandID);
            menuItem.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(menuItem);

            var folderViewMenuCommandID = new CommandID(PackageGuids.FolderViewGuid, PackageIds.FolderViewCleanErrorsCommand);
            var folderViewMenuItem = new OleMenuCommand(CleanErrors, folderViewMenuCommandID);
            folderViewMenuItem.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(folderViewMenuItem);

            var codeFileMenuCommandID = new CommandID(PackageGuids.CodeFileGuid, PackageIds.CodeFileCleanErrorsCommand);
            var codeFileViewMenuItem = new OleMenuCommand(CleanErrors, codeFileMenuCommandID);
            codeFileViewMenuItem.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(codeFileViewMenuItem);

            var errorListMenuCommandID = new CommandID(PackageGuids.ErrorListGuid, PackageIds.ErrorListCleanErrorsCommand);
            var errorListViewMenuItem = new OleMenuCommand(CleanErrors, errorListMenuCommandID);
            errorListViewMenuItem.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(errorListViewMenuItem);
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Action == vsBuildAction.vsBuildActionClean ||
               (Action == vsBuildAction.vsBuildActionRebuildAll && !Package.Settings.RunOnBuild))
            {
                ErrorListDataSource.Instance.CleanAllErrors();
            }
        }

        public static CleanErrorsCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        public static void Initialize(Microsoft.VisualStudio.Shell.Package package)
        {
            Instance = new CleanErrorsCommand(package);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var button = (OleMenuCommand)sender;
            button.Visible = ErrorListDataSource.Instance.HasErrors();
        }

        private void CleanErrors(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ErrorListDataSource.Instance.CleanAllErrors();
            Package.TaggerProvider?.RefreshTags();
        }
    }
}
