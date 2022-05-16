using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintVsix
{
    internal static class LinterService
    {
        private static bool defaultsCreated;

        public static bool IsLinterEnabled => Package.Settings.ESLintEnable;

        public static async Task LintTextAsync(string text, string filePath, bool fixErrors = false)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            Benchmark.Start();
            Benchmark.Log("In LinterService.LintTextAsync, path=" + (filePath ?? ""));
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(filePath)) return;
            string[] tsconfigs = (Package.Settings.UseTsConfig && LintableFiles.IsTypeScriptFileExtension(filePath)) ?
                TsconfigLocations.FindParentTsconfigs(filePath) : new string[] { };
            if (tsconfigs == null) return;  // Using tsconfig.json but can't find one
            string localInstallPath = LocalNodeModulesLocations.FindLocalInstallFromPath(filePath);
            Dictionary<string, string> fileToProjectMap = CreateFileToProjectMap(filePath); // Needs UI thread
            await LintAsync(new string[] { filePath }, tsconfigs, text, localInstallPath: localInstallPath,
                fileToProjectMap: fileToProjectMap,
                showErrorList: false, clearAllErrors: false, fixErrors: fixErrors, isCalledFromBuild: false);
            Benchmark.End();
        }

        private static Dictionary<string, string> CreateFileToProjectMap(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                { { filePath, ProjectExtensions.GetProjectNameFromFilePath(filePath) } };
        }

        public static async Task<bool> LintAsync(string[] fileNames, string[] projectFileNames, string text, string localInstallPath,
            Dictionary<string, string> fileToProjectMap, bool showErrorList, bool clearAllErrors, bool fixErrors,
            bool isCalledFromBuild)
        {
#if DEBUG
            if (fileNames.Length == 0 && projectFileNames.Length == 0)
                throw new Exception("LinterService.LintAsync called with empty fileNames list");
#endif
            bool hasVSErrors = false;
            try
            {
                await ErrorListDataSource.ClearFilesClosedWhileLintingAsync();
                await UpdateStatusBarAsync("Analyzing...", true);
                await TaskScheduler.Default;
                await CopyResourceFilesToUserProfileAsync(false);
                Linter linter = new Linter(Package.Settings, fixErrors, Logger.LogAndWarnAsync, localInstallPath);
                LintingResult result = await linter.LintAsync(fileNames, projectFileNames, text, isCalledFromBuild);
                if (result != null)
                {
                    await ErrorListService.ProcessLintingResultsAsync(result, clearAllErrors, showErrorList, fixErrors, fileToProjectMap);
                    hasVSErrors = result.HasVsErrors;
                }
            }
            finally
            {
                await UpdateStatusBarAsync();
            }
            return hasVSErrors;
        }

        private static async Task UpdateStatusBarAsync(string text = null, bool animate = false)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            if (text == null)
                Package.Dte.StatusBar.Clear();
            else
                Package.Dte.StatusBar.Text = text;
            Package.Dte.StatusBar.Animate(animate, vsStatusAnimation.vsStatusAnimationGeneral);
        }

        public static async Task CopyResourceFilesToUserProfileAsync(bool force = false)
        {
            // Not sure about the defaultsCreated flag here: if you delete your own .eslintrc.js whilst
            // VS is running we're going to fail until you restart
            if (!defaultsCreated || force)
            {
                defaultsCreated = true;
                try
                {
                    string sourceFolder = GetVsixFolder();
                    string destFolder = Linter.GetUserConfigFolder();
                    if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                    foreach (string sourceFile in Directory.EnumerateFiles(sourceFolder))
                    {
                        string fileName = Path.GetFileName(sourceFile);
                        string destFile = Path.Combine(destFolder, fileName);
                        if (force || !File.Exists(destFile))
                            File.Copy(sourceFile, destFile, true);
                    }
                }
                catch (Exception ex)
                {
                    defaultsCreated = false;
                    await Logger.LogAsync(ex);
                }
            }
        }

        private static string GetVsixFolder()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string root = Path.GetDirectoryName(assembly);
            return Path.Combine(root, "Resources\\Defaults");
        }
    }
}