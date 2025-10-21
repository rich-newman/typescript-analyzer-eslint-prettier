using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    [TestClass]
    public class EsLintTextTest
    {
        public TestContext TestContext { get; set; } = null;

        [TestMethod, TestCategory("ESLint with text")]
        public async Task BasicLint()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            string text = @"
debugger;
";
            LintingResult result = await new Linter(MockSettings.Instance)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\a.ts") }, new string[] { }
                , text, false);
            Assert.IsTrue(result.HasErrors);
            Assert.HasCount(1, result.Errors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.EndsWith(@"eslint\a.ts", result.Errors.First().FileName);
            Assert.AreEqual(1, result.Errors.First().LineNumber);
            Assert.AreEqual("Unexpected 'debugger' statement.", result.Errors.First().Message);
        }

        [TestMethod, TestCategory("ESLint with text")]
        public async Task BasicLintWithTsconfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // .eslintrc.js is the usual file but has had no-unnecessary-type-assertion added to allow us to test the type checking:
            // this needs type checking via a tsconfig or it won't work

            // Arrange
            string mainProjectTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), 
                @"tsconfigTypeInfoRule/multiple/tsconfig.json");
            string[] projectFiles = new string[] { mainProjectTsconfig };
            string file = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfigTypeInfoRule/multiple/b/file2.ts");
            string[] files = new string[] { file };
            string text = @"const foo = 3;
const bar = foo!;
const baz = foo!";

            // Act
            LintingResult result = await new Linter(new MockSettings() { UseTsConfig = true }).LintAsync(files, projectFiles, text, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(6, result.Errors);  // The underlying file has 3 errors, text above has 6

            // Should have 6 errors, one of them our no-unnecessary-type-assertion
            IList<LintingError> file2Errors = GetErrorsForFile("file2.ts", result.Errors);
            Assert.HasCount(6, file2Errors);
            LintingError noUnnecessaryTypeAssertionError = file2Errors.First(le => 
                le.ErrorCode == "@typescript-eslint/no-unnecessary-type-assertion");
            Assert.IsNotNull(noUnnecessaryTypeAssertionError);
            Assert.AreEqual(1, noUnnecessaryTypeAssertionError.LineNumber);
            Assert.AreEqual(12, noUnnecessaryTypeAssertionError.ColumnNumber);
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
