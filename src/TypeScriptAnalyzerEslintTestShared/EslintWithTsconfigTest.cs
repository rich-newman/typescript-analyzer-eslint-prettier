using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    /// <summary>
    /// Tests linting of individual tsconfig.json files when use tsconfig.json is true
    /// </summary>
    [TestClass]
    public class EslintWithTsconfigTest
    {
        [TestMethod, TestCategory("ESLint with tsconfig")]
        public async Task BasicLint()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // .eslintrc.js is the usual file but has had no-unnecessary-type-assertion added to allow us to test the type checking:
            // this needs type checking via a tsconfig or it won't work
            // Note that eslint is very unforgiving: if you try to lint individual files with a type checking rule in your config it will
            // throw and not lint anything.  You have to give it tsconfig.json files.  This meant we had to duplicate the tsconfig artifact
            // folder here.  We have artifacts/tsconfig, which is used for the LintEndToEndTest.cs tests which test the same files with
            // and without using tsconfig.json, but can't have a type checking rule as a result.  Also we have
            // artifacts/tsconfigTypeInfoRule.  This is exactly the same files as artifacts/tsconfig except the .eslintrc.js file has
            // a type-checking rule, so we can't test individual files with the folder.  artifacts/tsconfigTypeInfoRule is used here.

            // Arrange
            string mainProjectTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/multiple/tsconfig.json");
            string[] fileNames = new string[] { mainProjectTsconfig };

            // Act
            LintingResult result = await new Linter(new MockSettings() { UseTsConfig = true }).LintAsync(new string[] { }, fileNames);

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(378, result.Errors);

            // file3.ts is excluded from this tsconfig.json, in spite of being in the VS project.
            // It has errors but they shouldn't appear here
            IList<LintingError> file3Errors = GetErrorsForFile("file3.ts", result.Errors);
            Assert.IsEmpty(file3Errors);

            // file2.ts is the reverse of the above: it's included in the tsconfig.json file, but is not in the VS project
            // It should have 3 errors, one of them our no-unnecessary-type-assertion
            IList<LintingError> file2Errors = GetErrorsForFile("file2.ts", result.Errors);
            Assert.HasCount(3, file2Errors);
            LintingError noUnnecessaryTypeAssertionError = file2Errors.First(le => le.ErrorCode == "@typescript-eslint/no-unnecessary-type-assertion");
            Assert.IsNotNull(noUnnecessaryTypeAssertionError);
            Assert.AreEqual(2, noUnnecessaryTypeAssertionError.LineNumber);
            Assert.AreEqual(12, noUnnecessaryTypeAssertionError.ColumnNumber);
        }

        [TestMethod, TestCategory("ESLint with tsconfig")]
        public async Task LintWithDuplicateErrors()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();           // Arrange
            string topTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/multiple/tsconfig.json");
            string folderbTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/multiple/b/tsconfig.json");
            string[] fileNames = new string[] { topTsconfig, folderbTsconfig };

            // Act
            LintingResult result = await new Linter(new MockSettings() { UseTsConfig = true }).LintAsync(new string[] { }, fileNames);

            // Assert
            // file2 is in both tsconfigs.  It has 3 errors.  With the old code we got duplicates in the Error List, and here.
            Assert.IsNotNull(result);
            Assert.HasCount(380, result.Errors);

            IList<LintingError> file2Errors = GetErrorsForFile("file2.ts", result.Errors);
            Assert.HasCount(3, file2Errors);
            LintingError noUnnecessaryTypeAssertionError = file2Errors.First(le => le.ErrorCode == "@typescript-eslint/no-unnecessary-type-assertion");
            Assert.IsNotNull(noUnnecessaryTypeAssertionError);
            Assert.AreEqual(2, noUnnecessaryTypeAssertionError.LineNumber);
            Assert.AreEqual(12, noUnnecessaryTypeAssertionError.ColumnNumber);
        }

        [TestMethod, TestCategory("ESLint with tsconfig")]
        public async Task LintAll()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Arrange
            string topTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/multiple/tsconfig.json");
            string folderaTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/multiple/a/tsconfig.json");
            string folderbTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/multiple/b/tsconfig.json");
            // The tsconfig.json below isn't in a VS project, so couldn't be linted from the UI - can be here
            string folderbTsconfigEmptyProject = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/none/b/tsconfig.json");
            string[] fileNames = new string[] { topTsconfig, folderaTsconfig, folderbTsconfig, folderbTsconfigEmptyProject };

            // Act
            LintingResult result = await new Linter(new MockSettings() { UseTsConfig = true }).LintAsync(new string[] { }, fileNames);

            // Assert
            // file2 is in both tsconfigs.  It has 4 errors.  With the old code we got 8 in the Error List, and here.
            Assert.IsNotNull(result);
            Assert.HasCount(382, result.Errors);

            IList<LintingError> file1Errors = GetErrorsForFile("file1.ts", result.Errors);
            Assert.HasCount(2, file1Errors);

            IList<LintingError> file2Errors = GetErrorsForFile("file2.ts", result.Errors);
            Assert.HasCount(3, file2Errors);
            LintingError noUnnecessaryTypeAssertionError = file2Errors.First(le => le.ErrorCode == "@typescript-eslint/no-unnecessary-type-assertion");
            Assert.IsNotNull(noUnnecessaryTypeAssertionError);
            Assert.AreEqual(2, noUnnecessaryTypeAssertionError.LineNumber);
            Assert.AreEqual(12, noUnnecessaryTypeAssertionError.ColumnNumber);

            IList<LintingError> file3Errors = GetErrorsForFile("file3.ts", result.Errors);
            Assert.HasCount(2, file3Errors);

            IList<LintingError> file4Errors = GetErrorsForFile("file4.ts", result.Errors);
            Assert.HasCount(2, file4Errors);

            IList<LintingError> file5Errors = GetErrorsForFile("file5.ts", result.Errors);
            Assert.HasCount(2, file5Errors);

            IList<LintingError> file6TsxErrors = GetErrorsForFile("file6.tsx", result.Errors);
            Assert.HasCount(3, file6TsxErrors);

            IList<LintingError> file7NestedErrors = GetErrorsForFile("file7.ts", result.Errors);
            Assert.HasCount(2, file7NestedErrors);

            IList<LintingError> testErrors = GetErrorsForFile("test.ts", result.Errors);
            Assert.HasCount(3, testErrors);
        }

        private IList<LintingError> GetErrorsForFile(string fileName, IEnumerable<LintingError> allErrors)
        {
            List<LintingError> result = new List<LintingError>();
            foreach (LintingError lintingError in allErrors)
            {
                if (lintingError.FileName.EndsWith(fileName)) result.Add(lintingError);
            }
            return result;
        }
    }
}
