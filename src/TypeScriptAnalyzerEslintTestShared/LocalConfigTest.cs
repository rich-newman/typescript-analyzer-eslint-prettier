using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    [TestClass]
    public class LocalConfigTest
    {
        private static MockSettings settings = null;

        public TestContext TestContext { get; set; } = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () => { await ClassInitializeAsync(testContext); });
        }

        // These tests assume the default .eslintrc.js is the one installed with the Analyzer:
        // the class initialize methods below make sure this is true whilst trying to keep any
        // changes that have been made
        public static async Task ClassInitializeAsync(TestContext testContext)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(testContext.CancellationToken);
            string userConfigFolder = Linter.GetUserConfigFolder();
            string defaultUserConfigFile = Path.Combine(userConfigFolder, ".eslintrc.js");
            RenameFile(defaultUserConfigFile, defaultUserConfigFile + "bak");
            string defaultEslintrcFile =
                Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\defaulteslintrc\.eslintrc.js");
            File.Copy(defaultEslintrcFile, defaultUserConfigFile);
        }

        // Renames without throwing if files don't exist or already exist: deletes an existing file with the target name,
        // does nothing if the source file doesn't exist
        private static void RenameFile(string source, string target)
        {
            if (File.Exists(source))
            {
                File.Delete(target);
                File.Copy(source, target);
                File.Delete(source);
            }
        }

#if MSTEST_V3
        [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
#else
        [ClassCleanup]
#endif
        public static void ClassCleanup(TestContext testContext)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () => { await ClassCleanupAsync(testContext); });
        }

        public static async Task ClassCleanupAsync(TestContext testContext)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(testContext.CancellationToken);
            string userConfigFolder = Linter.GetUserConfigFolder();
            string defaultUserConfigFile = Path.Combine(userConfigFolder, ".eslintrc.js");
            RenameFile(defaultUserConfigFile + "bak", defaultUserConfigFile);
        }

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
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // The Analyzer default config generates one prettier error with localconfig\test.ts
            settings.EnableLocalConfig = false;
            settings.ShowPrettierErrors = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\test.ts") }, 
                           new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(1, result.Errors);
            Assert.AreEqual(@"prettier/prettier", result.Errors.ElementAt(0).ErrorCode);
            Assert.AreEqual(@"Replace `'Hello·world')␍⏎` with `""Hello·world"");`", result.Errors.ElementAt(0).Message);
        }

        [TestMethod, TestCategory("Local ESLint Config")]
        public async Task EnableLocalConfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // The local config doesn't enable any rules, so we get no errors
            settings.EnableLocalConfig = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\test.ts") }, 
                           new string[] { });
            Assert.IsFalse(result.HasErrors);
        }

        // One problem with coding enableLocalConfig was that if all we do is set overrideConfigFile on the ESLint options object
        // then ESLint still tries to parse all local config, which may not be working. If any of that throws the linting throws.
        // The fix, somewhat counterintuitively since we're trying to use a specific .eslintrc.js file to configure, is to
        // set useEslintrc to false on the ESlint options object.  This test checks that works: if you remove the line in server.js that
        // sets useEslintrc this test should fail.
        [TestMethod, TestCategory("Local ESLint Config")]
        public async Task DisableBrokenLocalConfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // There's a broken .eslintrc.js file in localconfig\broken
            // The Analyzer default config generates one prettier error with localconfig\broken\test.ts
            settings.EnableLocalConfig = false;
            settings.ShowPrettierErrors = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\broken\test.ts") }, 
                           new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(1, result.Errors);
            Assert.AreEqual(@"prettier/prettier", result.Errors.ElementAt(0).ErrorCode);
            Assert.AreEqual(@"Replace `'Hello·world')␍⏎` with `""Hello·world"");`", result.Errors.ElementAt(0).Message);
        }

        [TestMethod, TestCategory("Local ESLint Config")]
        public async Task EnableBrokenLocalConfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // The local config is broken: we get an error that it's broken if we try to use it
            settings.EnableLocalConfig = true;
            settings.ShowPrettierErrors = true;
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localconfig\broken\test.ts") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(1, result.Errors);
            Assert.AreEqual(@"eslint", result.Errors.ElementAt(0).ErrorCode);
            Assert.StartsWith(@"ESLint webserver error. For more details see Output window: Cannot read config file", result.Errors.ElementAt(0).Message);
        }
    }
}
