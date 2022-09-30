using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptAnalyzerEslintLinter;
using I = Microsoft.VisualStudio.VSConstants.VSStd97CmdID;

namespace TypeScriptAnalyzerEslintVsix
{
    internal class BuildEventsListener : BuildEventsBase
    {
        private readonly AsyncPackage package;
        private readonly CommandEvents commandEvents;

        internal BuildEventsListener(Microsoft.VisualStudio.Shell.Package package) : base(package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = (AsyncPackage)package;
            commandEvents = Package.Dte.Events.CommandEvents;
            commandEvents.BeforeExecute += CommandEvents_BeforeExecute;
            base.StartListeningForChanges();
        }

        private bool isBuilding = false;
        private bool isBuildingSolution = true;
        private readonly HashSet<int> buildIds = new HashSet<int> { (int)I.BuildSln, (int)I.RebuildSln, (int)I.BuildCtx, (int)I.RebuildCtx,
                                                            (int)I.BuildSel, (int)I.RebuildSel, (int)I.BatchBuildDlg, (int)I.Start,
                                                            (int)I.StartNoDebug };
        private readonly HashSet<int> buildSolutionIds = new HashSet<int> { (int)I.BuildSln, (int)I.RebuildSln, (int)I.BatchBuildDlg,
                                                                    (int)I.Start, (int)I.StartNoDebug };
        private void CommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            // TODO There's also Build1-9, Rebuild1-9 and BuildLast/RebuildLast.  
            // 1-9 can be called from the build menu if on an item outside a project.
            // These are really edge cases and a bit of work to implement as we need to map them to projects and tell the linter
            // TODO Batch build lints everything, not just the built projects
            if (!Guid.StartsWith("{5E") || Guid != "{5EFC7975-14BC-11CF-9B2B-00AA00573819}" || !buildIds.Contains(ID)) return;
            isBuilding = true;
            isBuildingSolution = buildSolutionIds.Contains(ID);
        }

        public override int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            FileListener.FixOnSaveSuspended = false;
            return base.UpdateSolution_Done(fSucceeded, fModified, fCancelCommand);
        }

        // Fires after any save events for unsaved files, so we can reset the fix on save suspended here
        public override int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            FileListener.FixOnSaveSuspended = false;
            return base.UpdateSolution_StartUpdate(ref pfCancelUpdate);
        }


        public override int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            try
            {
                if (isBuilding && Package.Settings.ESLintEnable) 
                    FileListener.FixOnSaveSuspended = true;  // Event linting re-enabled in UpdateSolution_Done
                if (!isBuilding || !Package.Settings.RunOnBuild || !Package.Settings.ESLintEnable)
                    return VSConstants.S_OK;
#pragma warning disable VSTHRD102
                bool cancelBuild = package.JoinableTaskFactory.Run(() => LintBuildSelectionAsync(isBuildingSolution));
#pragma warning restore VSTHRD102
                pfCancelUpdate = cancelBuild ? 1 : 0;
                ThreadHelper.ThrowIfNotOnUIThread();
                if (cancelBuild)
                {
                    string message = "Build failed because of ESLint Errors";
                    Package.Dte.StatusBar.Text = message;
                    Logger.Log(message);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                isBuilding = false;
            }
            return VSConstants.S_OK;
        }

        protected async System.Threading.Tasks.Task<bool> LintBuildSelectionAsync(bool isBuildingSolution)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            try
            {
                if (!LinterService.IsLinterEnabled) return false;
                FileListener.EventLintingSuspended = true;
                Package.Dte.ExecuteCommand("File.SaveAll");
                await System.Threading.Tasks.Task.Delay(Settings.SaveDelay);
                UIHierarchyItem[] selectedItems = BuildSelectedItems.Get(isBuildingSolution);
                Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string[] files = LintFileLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);
                string[] projectFiles = Package.Settings.UseTsConfig ?
                    TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap) :
                    new string[] { };
                if (!files.Any() && !projectFiles.Any()) return false;
                string localInstallPath =
                    LocalNodeModulesLocations.FindLocalInstallFromPath(files.Length > 0 ? files[0] : projectFiles[0]);
                return await LinterService.LintAsync(fileNames: files.ToArray(), projectFileNames: projectFiles.ToArray(), text: null,
                    localInstallPath: localInstallPath, fileToProjectMap: fileToProjectMap, showErrorList: true, clearAllErrors: true, fixErrors: false, isCalledFromBuild: true);
            }
            catch (Exception ex)
            {
                await Logger.LogAndWarnAsync(ex);
                Linter.Server.Down();
                return false;
            }
            finally
            {
                FileListener.EventLintingSuspended = false;
            }
        }

        public static BuildEventsListener Instance { get; private set; }
        public static void Initialize(Microsoft.VisualStudio.Shell.Package package) => Instance = new BuildEventsListener(package);

    }
}
