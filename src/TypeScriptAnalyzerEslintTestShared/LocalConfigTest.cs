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
            // The Analyzer default config generates one prettier error with localconfig\test.ts
            settings.EnableLocalConfig = false;
            settings.ShowPrettierErrors = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\test.ts") }, 
                           new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(@"prettier/prettier", result.Errors.ElementAt(0).ErrorCode);
            Assert.AreEqual(@"Replace `'Hello·world')␍⏎` with `""Hello·world"");`", result.Errors.ElementAt(0).Message);
        }

        [TestMethod, TestCategory("Local ESLint Config")]
        public async Task EnableLocalConfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // The local config doesn't enable any rules, so we get no errors
            settings.EnableLocalConfig = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\test.ts") }, 
                           new string[] { });
            Assert.IsFalse(result.HasErrors);
        }


        //[TestMethod, TestCategory("Local ESLint Config")]
        //public async Task EnableBrokenLocalConfig()
        //{
        //    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        //    // The local config doesn't enable any rules, so we get no errors
        //    settings.EnableLocalConfig = true;
        //    LintingResult result = await new Linter(settings)
        //        .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\broken\test.ts") }, new string[] { });
        //    Assert.IsFalse(result.HasErrors);
        //}

        // One problem with coding enableLocalConfig was that if all we do is set overrideConfigFile on the ESLint options object
        // then ESLint still tries to parse all local config, which may not be working. If any of that throws the linting throws.
        // The fix, somewhat counterintuitively since we're trying to use a specific .eslintrc.js file to configure, is to
        // set useEslintrc to false on the ESlint options object.  This test checks that works: if you remove the line in server.js that
        // sets useEslintrc this test should fail.
        [TestMethod, TestCategory("Local ESLint Config")]
        public async Task DisableBrokenLocalConfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // There's a broken .eslintrc.js file in localconfig\broken
            // The Analyzer default config generates one prettier error with localconfig\broken\test.ts
            settings.EnableLocalConfig = false;
            settings.ShowPrettierErrors = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\broken\test.ts") }, 
                           new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(@"prettier/prettier", result.Errors.ElementAt(0).ErrorCode);
            Assert.AreEqual(@"Replace `'Hello·world')␍⏎` with `""Hello·world"");`", result.Errors.ElementAt(0).Message);
        }
    }
}
