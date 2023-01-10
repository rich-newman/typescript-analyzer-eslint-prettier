using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TypeScriptAnalyzerEslintLinter;
using TypeScriptAnalyzerEslintTest;

namespace TypeScriptAnalyzerEslintTest
{
    [TestClass]
    public class LocalConfigTest
    {
        private static MockSettings settings = null;

        [TestInitialize]
        public void TestInitialize()
        {
            AssemblyMethods.MockServiceProvider.Reset();
            TypeScriptAnalyzerEslintVsix.Package.Jtf = ThreadHelper.JoinableTaskFactory;
            settings = new MockSettings();
            TypeScriptAnalyzerEslintVsix.Package.Settings = settings;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TypeScriptAnalyzerEslintVsix.Package.Jtf = null;
            settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
        }

        [TestMethod, TestCategory("Local ESLint Config")]
        public async Task DisableLocalConfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // The Analyzer default config generates two errors with localconfig\test.ts
            settings.EnableLocalConfig = false;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\test.ts") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(2, result.Errors.Count);
            Assert.AreEqual(@"@typescript-eslint/quotes", result.Errors.ElementAt(0).ErrorCode);
            Assert.AreEqual("Strings must use doublequote.", result.Errors.ElementAt(0).Message);
            Assert.AreEqual(@"@typescript-eslint/semi", result.Errors.ElementAt(1).ErrorCode);
            Assert.AreEqual("Missing semicolon.", result.Errors.ElementAt(1).Message);
        }

        [TestMethod, TestCategory("Local ESLint Config")]
        public async Task EnableLocalConfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // The local config doesn't enable any rules, so we get no errors
            settings.EnableLocalConfig = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\test.ts") }, new string[] { });
            Assert.IsFalse(result.HasErrors);
        }
    }
}
