using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptAnalyzerEslintLinter;
using System.Windows;
using System.Threading.Tasks;

namespace TypeScriptAnalyzerEslintVsix
{
    class ErrorListService
    {
        public static async Task ProcessLintingResultsAsync(LintingResult result, bool clearAllErrors, bool showErrorList, 
            bool isFixing, Dictionary<string, string> fileToProjectMap)
        {
            // Called on worker thread unless we're running on a build when we are on the UI thread
            // We have several possibilities re files and filters:
            // 1.  We're not using tsconfig.  In this case fileNames is the list of files we're linting, clearAllErrors is true unless
            //     we're linting an individual file or files, in which case we want to clear errors for those files only.  In this case
            //     fileToProjectMap contains the list of files we are linting.
            // 2.  We're using tsconfig but have selected an individual file or files to lint in Solution Explorer or are saving/opening an
            //     individual file. In this case fileNames contains the relevant tsconfig(s) to pass to ESLint, clearAllErrors is false and
            //     fileToProjectMap contains the names of the files we are linting.  These are the only errors we want to update.
            // 3.  We're using tsconfig and have selected a folder/project/solution to lint in Solution Explorer.  In this case fileNames
            //     again contains the tsconfig file names we will pass to ESLint.  clearAllErrors is true.  We want to update all errors
            //     for the project/solution.
            //bool useFilter = Package.Settings.UseTsConfig && !clearAllErrors; // Case 2
            //bool tsConfigNoFilter = Package.Settings.UseTsConfig && clearAllErrors;  // Case 3
            IEnumerable<LintingError> allErrors = clearAllErrors ? result.Errors : 
                result.Errors.Where(e => fileToProjectMap.ContainsKey(e.FileName) || e.FileName == "eslint");
            // lintedFilesWithNoErrors is used to clear previous errors for files with no errors remaining in cases 1 and 2
            IEnumerable<string> lintedFilesWithNoErrors = clearAllErrors ? Enumerable.Empty<string>() :
               fileToProjectMap.Keys.Where(f => !allErrors.Select(e => e.FileName).Contains(f, StringComparer.OrdinalIgnoreCase));
            await UpdateErrorListDataSourceAsync(allErrors, showErrorList, lintedFilesWithNoErrors, 
                                                 isFixing, clearAllErrors, fileToProjectMap);
        }

        private static async Task UpdateErrorListDataSourceAsync(IEnumerable<LintingError> allErrors, bool showErrorList,
            IEnumerable<string> lintedFilesWithNoErrors, bool isFixing, bool clearAllErrors, Dictionary<string, string> fileToProjectMap)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            if (clearAllErrors)
            {
                Benchmark.Log("Before CleanAllErrors");
                ErrorListDataSource.Instance.CleanAllErrors();
                Benchmark.Log("After CleanAllErrors");
            }
            else
            {
                Benchmark.Log("Before CleanErrors, using linted files with no errors");
                ErrorListDataSource.Instance.CleanErrors(lintedFilesWithNoErrors);
                ErrorListDataSource.Instance.CleanErrors(new string[] {"eslint"});
                Benchmark.Log("After CleanErrors");
            }
            if (allErrors.Any())
            {
                Benchmark.Log($"Before AddErrors, allErrors.Count={allErrors.Count()}");
                //Logger.Log($"Before AddErrors, fileToProjectMap first: {(fileToProjectMap.Keys.Count() > 0 ? fileToProjectMap.Keys.First() : "")}");
                ErrorListDataSource.Instance.AddErrors(allErrors, fileToProjectMap);
                if (showErrorList)
                {
                    Benchmark.Log("Before BringToFront");
                    ErrorListDataSource.Instance.BringToFront();
                    Benchmark.Log("After BringToFront");
                }
            }
            Benchmark.Log("Before RefreshTags");
            Package.TaggerProvider?.RefreshTags(isFixing);
            Benchmark.Log("After RefreshTags");
        }
    }
}