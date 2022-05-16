using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintVsix
{
    internal class LintFilesCommandBase
    {
        protected readonly AsyncPackage package;
        protected IServiceProvider ServiceProvider => package;

        internal LintFilesCommandBase(AsyncPackage package) 
        { 
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package ?? throw new ArgumentNullException("package");
        }

        protected void BeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                ((OleMenuCommand)sender).Visible = Package.Settings.ESLintEnable && LintableFiles.AreAllSelectedItemsLintable();
            }
            catch (Exception ex)
            {
                Logger.LogAndWarn(ex);
            }
        }

        // We provide a path iff we are linting from the folder view, when SelectedItems (painfully) doesn't work in Solution Explorer
        internal static async System.Threading.Tasks.Task LintItemsSelectedInSolutionExplorerAsync(bool fixErrors, string path = null)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            try
            {
                Benchmark.Start();
                if (!LinterService.IsLinterEnabled)
                {
                    Package.Dte.StatusBar.Text = "TypeScript Analyzer (ESlint) is not enabled in Tools/Options";
                }
                // Use a sledgehammer to crack the nut of unsaved files when we lint in Solution Explorer: save before we start.
                // The nutcracker is to identify unsaved files and lint with the current text in their text buffers, rather than what's
                // on disk.
                // The flag below stops the linting on a save that usually happens in the FileListener.  I don't think it's
                // possible to stop a lint after a fix though: the TextChanged event fires after this method has completely exited.  It
                // only happens if the file is open in the editor of course.
                FileListener.EventLintingSuspended = true;
                Package.Dte.ExecuteCommand("File.SaveAll");
                if (fixErrors) await Task.Delay(300);
                if (path != null)
                    await LintSelectedItemInFolderViewAsync(fixErrors, path);
                else
                    await LintSelectedItemsAsync(fixErrors, Package.Dte.ToolWindows.SolutionExplorer.SelectedItems
                                                            as UIHierarchyItem[]);
            }
            catch (Exception ex)
            {
                await Logger.LogAndWarnAsync(ex);
                Linter.Server.Down();
            }
            finally { 
                Benchmark.End();
                FileListener.EventLintingSuspended = false; 
            }
        }

        internal static async System.Threading.Tasks.Task<bool> LintSelectedItemsAsync(bool fixErrors, UIHierarchyItem[] selectedItems)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            // TODO if multiple items are selected in Solution Explorer (=> selectedItems.Length > 1) then it's probably better to run
            // multiple calls to the linter each with its own configuration.  It's a bit of an edge case as I can't really imagine
            // anyone would do that very often.
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] files = LintFileLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);
            string[] projectFiles = Package.Settings.UseTsConfig ?  
                TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap) :
                new string[] { };
            if (files.Any() || projectFiles.Any())
            {
                string localInstallPath = LocalNodeModulesLocations.FindLocalInstallFromPath(files.Length > 0 ? files[0] : projectFiles[0]);
                bool clearAllErrors = AnyItemNotLintableSingleFile(selectedItems);
                return await LinterService.LintAsync(fileNames: files, projectFileNames: projectFiles, text: null, localInstallPath: localInstallPath,
                    fileToProjectMap: fileToProjectMap, showErrorList: true, clearAllErrors: clearAllErrors, fixErrors: fixErrors, isCalledFromBuild: false);
            }
            else
            {
                Package.Dte.StatusBar.Text = $"No files found to {(fixErrors ? "fix" : "lint")}";
                return false;
            }
        }

        internal static async System.Threading.Tasks.Task<bool> LintSelectedItemInFolderViewAsync(bool fixErrors, string path)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] files = LintFileLocations.FindPathsFromSelectedItemInFolderView(path, fileToProjectMap);
            string[] projectFiles = Package.Settings.UseTsConfig ? 
                TsconfigLocations.FindPathsFromSelectedItemInFolderView(path, fileToProjectMap) : 
                new string[] { };
            if (files.Any() || projectFiles.Any())
            {
                string localInstallPath = LocalNodeModulesLocations.FindLocalInstallFromPath(files.Length > 0 ? files[0] : projectFiles[0]);
                bool clearAllErrors = System.IO.Directory.Exists(path) || 
                    (Package.Settings.UseTsConfig && LintableFiles.IsLintableTsconfig(path));  // Clear errors if linting a folder or tsconfig.json
                return await LinterService.LintAsync(fileNames: files, projectFileNames: projectFiles, text: null, localInstallPath: localInstallPath,
                    fileToProjectMap: fileToProjectMap, showErrorList: true, clearAllErrors: clearAllErrors, fixErrors: fixErrors, isCalledFromBuild: false);
            }
            else
            {
                Package.Dte.StatusBar.Text = $"No files found to {(fixErrors ? "fix" : "lint")}";
                return false;
            }
        }

        private static bool AnyItemNotLintableSingleFile(UIHierarchyItem[] items)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (UIHierarchyItem selItem in items)
            {
                if (!(selItem.Object is ProjectItem item &&
                    item.GetFullPath() is string projectItemPath &&
                    LintableFiles.IsLintableFile(projectItemPath)))
                    return true;
            }
            return false;
        }
    }

}