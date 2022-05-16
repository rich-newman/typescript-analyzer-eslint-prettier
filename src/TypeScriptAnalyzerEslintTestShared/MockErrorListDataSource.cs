using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptAnalyzerEslintVsix;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintTest
{
    public class MockErrorListDataSource : IErrorListDataSource
    {
        private readonly Dictionary<string, IGrouping<string, LintingError>> snapshots = new Dictionary<string, IGrouping<string, LintingError>>();
        public Dictionary<string, IGrouping<string, LintingError>> Snapshots => snapshots;

        public void AddErrors(IEnumerable<LintingError> errors, Dictionary<string, string> fileToProjectMap)
        {
            if (errors == null || !errors.Any()) return;
            IEnumerable<LintingError> cleanErrors = errors.Where(e => e != null && !string.IsNullOrEmpty(e.FileName));
            foreach (IGrouping<string, LintingError> error in cleanErrors.GroupBy(t => t.FileName))
            {
                snapshots[error.Key] = error;
            }
        }

        public void BringToFront() { }
        public void CleanAllErrors() => snapshots.Clear();

        public void CleanErrors(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                if (snapshots.ContainsKey(file)) snapshots.Remove(file);
            }
        }

        public void CleanNonLintableFileErrors()
        {
            throw new NotImplementedException();
        }

        public void CleanPrettierErrors()
        {
            throw new NotImplementedException();
        }

        public bool HasErrors() => snapshots.Count > 0;
        public bool HasErrors(string fileName) => snapshots.ContainsKey(fileName);

        public bool HasPrettierErrors()
        {
            throw new NotImplementedException();
        }
    }
}
