using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    /// <summary>
    /// Tests linting of individual .ts and .tsx files when use tsconfig.json is false
    /// </summary>
    [TestClass]
    public class ESLintTest
    {
        public TestContext TestContext { get; set; } = null;

        [TestMethod, TestCategory("ESLint")]
        public async Task StandardTs()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            LintingResult result = await new Linter(MockSettings.Instance)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\a.ts") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(11, result.Errors);
            Assert.AreEqual("Unexpected var, use let or const instead.", result.Errors.First().Message);
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task StandardTsFixErrors()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            try
            {
                File.Copy(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\a.ts"),
                          Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\aTest.ts"), true);
                // Same as standard test above, except we fix, so should get rid of some of those errors
                LintingResult result = await new Linter(MockSettings.Instance, true)
                    .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\aTest.ts") }, new string[] { });
                Assert.IsTrue(result.HasErrors);
                Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
                Assert.HasCount(5, result.Errors);
                Assert.AreEqual("Expected '===' and instead saw '=='.", result.Errors.First().Message);
                string actual = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\aTest.ts"));
                string expected = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\aFixed.ts"));
                // normalize by removing space indents and using Windows line breaks
                actual = System.Text.RegularExpressions.Regex.Replace(actual, "\r?\n\\s+", "\r\n");
                expected = System.Text.RegularExpressions.Regex.Replace(expected, "\r?\n\\s+", "\r\n");
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                File.Delete(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\aTest.ts"));
            }
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task StandardTsNoErrors()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            LintingResult result = await new Linter(MockSettings.Instance)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\e.ts") }, new string[] { });
            Assert.IsFalse(result.HasErrors);
            Assert.IsEmpty(result.Errors);
            Assert.IsFalse(string.IsNullOrEmpty(result.FileNames.First()), "File name is empty");
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task StandardTsx()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            LintingResult result = await new Linter(MockSettings.Instance)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\c.tsx") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(3, result.Errors);
            Assert.AreEqual("Missing return type on function.", result.Errors.First().Message);
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task StandardTsxFixErrors()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            try
            {
                File.Copy(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\c.tsx"),
                          Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\cTest.tsx"), true);
                LintingResult result = await new Linter(MockSettings.Instance, true)
                    .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\cTest.tsx") }, new string[] { });
                Assert.IsTrue(result.HasErrors);
                Assert.HasCount(2, result.Errors);
                string actual = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\cTest.tsx"));
                string expected = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\cFixed.tsx"));
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                File.Delete(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\cTest.tsx"));
            }
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task StandardTsxNoErrors()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            LintingResult result = await new Linter(MockSettings.Instance)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\d.tsx") }, new string[] { });
            Assert.IsFalse(result.HasErrors);
            Assert.IsEmpty(result.Errors);
            Assert.IsFalse(string.IsNullOrEmpty(result.FileNames.First()), "File name is empty");
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task TsFileNotExist()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            LintingResult result = await new Linter(MockSettings.Instance)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\doesntexist.ts") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
        }

        [TestMethod, TestCategory("ESLint")]
        public async Task TsxFileNotExist()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            LintingResult result = await new Linter(MockSettings.Instance)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"eslint\doesntexist.tsx") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
        }
    }
}
