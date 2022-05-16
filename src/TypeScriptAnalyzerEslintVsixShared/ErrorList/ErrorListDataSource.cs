using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using TypeScriptAnalyzerEslintLinter;
using EnvDTE;
using System.Diagnostics;

namespace TypeScriptAnalyzerEslintVsix
{
    internal interface IErrorListDataSource
    {
        void AddErrors(IEnumerable<LintingError> errors, Dictionary<string, string> fileToProjectMap);
        void CleanErrors(IEnumerable<string> files);
        void BringToFront();
        void CleanAllErrors();
        void CleanNonLintableFileErrors();
        void CleanPrettierErrors();
        bool HasErrors();
        bool HasErrors(string fileName);
        bool HasPrettierErrors();
    }

    internal class ErrorListDataSource : ITableDataSource, IErrorListDataSource
    {
        private static IErrorListDataSource instance;
        private readonly List<SinkManager> managers = new List<SinkManager>();

        private static readonly Dictionary<string, TableEntriesSnapshot> snapshots = 
            new Dictionary<string, TableEntriesSnapshot>(StringComparer.OrdinalIgnoreCase);

        public static Dictionary<string, TableEntriesSnapshot> Snapshots
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                return snapshots;
            }
        }

        [Import]
        private ITableManagerProvider TableManagerProvider { get; set; } = null;

        private ErrorListDataSource()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var compositionService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            Microsoft.Assumes.Present(compositionService);
            compositionService.DefaultCompositionService.SatisfyImportsOnce(this);

            ITableManager manager = TableManagerProvider.GetTableManager(StandardTables.ErrorsTable);
            manager.AddSource(this, StandardTableColumnDefinitions.DetailsExpander,
                                    StandardTableColumnDefinitions.ErrorSeverity, StandardTableColumnDefinitions.ErrorCode,
                                    StandardTableColumnDefinitions.ErrorSource, StandardTableColumnDefinitions.BuildTool,
                                    StandardTableColumnDefinitions.ErrorCategory,
                                    StandardTableColumnDefinitions.Text, StandardTableColumnDefinitions.DocumentName,
                                    StandardTableColumnDefinitions.Line, StandardTableColumnDefinitions.Column);
        }

        internal static void InjectMockErrorListDataSource(IErrorListDataSource instance) => ErrorListDataSource.instance = instance;

        public static IErrorListDataSource Instance
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (instance == null)
                    instance = new ErrorListDataSource();

                return instance;
            }
        }

        #region ITableDataSource members
        public string SourceTypeIdentifier
        {
            get { return StandardTableDataSources.ErrorTableDataSource; }
        }

        public string Identifier
        {
            get { return PackageGuids.guidVSPackageString; }
        }

        public string DisplayName
        {
            get { return Vsix.Name; }
        }

        public IDisposable Subscribe(ITableDataSink sink)
        {
            // Not necessarily called on UI thread
            return new SinkManager(this, sink);
        }
        #endregion

        public void AddSinkManager(SinkManager manager)
        {
            // Not necessarily called from UI thread if called from ErrorDataListSource.Subscribe/new SinkManager
            managers.Add(manager);
        }

        public void RemoveSinkManager(SinkManager manager)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            managers.Remove(manager);
        }

        public void UpdateAllSinks()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (var manager in managers)
                manager.UpdateSink(Snapshots.Values);
        }

        public void AddErrors(IEnumerable<LintingError> errors, Dictionary<string, string> fileToProjectMap)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (errors == null || !errors.Any()) return;
            var cleanErrors = errors.Where(e => e != null && !string.IsNullOrEmpty(e.FileName));
            //DebugDumpMap(fileToProjectMap);
            CreateSnapshots(fileToProjectMap, cleanErrors);
            UpdateAllSinks();
            Benchmark.Log("After UpdateAllSinks");
        }

        // Only access from UI thread
        // See comment in FileListener.TextviewClosed
        public static HashSet<string> FilesClosedWhileLinting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public static async System.Threading.Tasks.Task ClearFilesClosedWhileLintingAsync()
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            FilesClosedWhileLinting.Clear();
        }

        private static void CreateSnapshots(Dictionary<string, string> fileToProjectMap, IEnumerable<LintingError> cleanErrors)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (IGrouping<string, LintingError> error in cleanErrors.GroupBy(t => t.FileName))
            {
                if (FilesClosedWhileLinting.Contains(error.Key)) continue;
                fileToProjectMap.TryGetValue(error.Key, out string projectName);
                if (projectName == null)
                {
                    ProjectItem item = Package.Dte.Solution.FindProjectItem(error.Key);
                    projectName = item != null && item.Properties != null && item.ContainingProject != null ?
                        item.ContainingProject.Name : "";
                    fileToProjectMap.Add(error.Key, projectName);
                }
                TableEntriesSnapshot snapshot = new TableEntriesSnapshot(error.Key, projectName, error);
                Snapshots[error.Key] = snapshot;
            }
        }

        //[Conditional("DEBUG")]
        //private void DebugDumpMap(Dictionary<string, string> map)
        //{
        //    foreach (var item in map)
        //        Debug.WriteLine(item.Key + ":" + item.Value);
        //}

        public void CleanNonLintableFileErrors()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<string> fileNames = new List<string>();
            foreach (string fileName in Snapshots.Keys)
            {
                if (!(LintableFiles.IsLintableFile(fileName) || fileName == "eslint")) 
                    fileNames.Add(fileName);
            }
            CleanErrors(fileNames);
        }

        public void CleanErrors(IEnumerable<string> filesEnum)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string[] files = filesEnum.ToArray();  // Only evaluate once
            Benchmark.Log($"In CleanErrors: There are {files.Length}  files, {Snapshots.Count} snapshots, {managers.Count} managers.");
            foreach (string file in files)
            {
                if (Snapshots.ContainsKey(file))
                {
                    Snapshots[file].Dispose();
                    Snapshots.Remove(file);
                }
            }

            foreach (var manager in managers)
                manager.RemoveSnapshots(files);

            UpdateAllSinks();
        }

        public void CleanAllErrors()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (string file in Snapshots.Keys)
            {
                var snapshot = Snapshots[file];
                if (snapshot != null) snapshot.Dispose();
            }
            Snapshots.Clear();
            foreach (var manager in managers)
                manager.Clear();

            UpdateAllSinks();
        }

        public void BringToFront()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Package.Dte.ExecuteCommand("View.ErrorList");
        }

        public bool HasErrors()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return Snapshots.Count > 0;
        }

        public bool HasErrors(string fileName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return Snapshots.ContainsKey(fileName);
        }

        public bool HasPrettierErrors()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (KeyValuePair<string, TableEntriesSnapshot> item in Snapshots)
            {
                if (HasPrettierErrors(item)) return true;
            }
            return false;
        }

        private bool HasPrettierErrors(KeyValuePair<string, TableEntriesSnapshot> item)
        {
            foreach(LintingError lintingError in item.Value.Errors)
            {
                if (IsPrettierError(lintingError)) return true;
            }
            return false;
        }

        private bool IsPrettierError(LintingError lintingError)
        {
            return lintingError.ErrorCode == "prettier/prettier";
        }

        public void CleanPrettierErrors()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<LintingError> nonPrettierErrorsToRecreate = new List<LintingError>();
            List<string> filesToClean = new List<string>();
            foreach (KeyValuePair<string, TableEntriesSnapshot> item in Snapshots)
            {
                if (HasPrettierErrors(item))
                {
                    filesToClean.Add(item.Key);
                    foreach (LintingError lintingError in item.Value.Errors)
                    {
                        if (!IsPrettierError(lintingError)) nonPrettierErrorsToRecreate.Add(lintingError);
                    }
                }
            }
            CleanErrors(filesToClean);
            AddErrors(nonPrettierErrorsToRecreate, new Dictionary<string, string>());
        }
    }
}
