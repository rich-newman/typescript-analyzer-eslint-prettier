using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintVsix;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    [TestClass]
    public class PrettierTest
    {
        public TestContext TestContext { get; set; } = null;

        [TestInitialize]
        public void TestInitialize()
        {
            AssemblyMethods.MockServiceProvider.Reset();
            TypeScriptAnalyzerEslintVsix.Package.Jtf = ThreadHelper.JoinableTaskFactory;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TypeScriptAnalyzerEslintVsix.Package.Jtf = null;
        }

        [TestMethod, TestCategory("Prettier")]
        public async Task StandardTs()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // prettier is enabled by the .eslintrc.js in artifacts/prettier containing the appropriate rule
            // The EnablePrettier setting only applies if you're using the default configs in Users
            MockSettings settings = new MockSettings { ShowPrettierErrors = true };
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\a.ts") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(HasPrettierErrors(result.Errors));
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(19, result.Errors);
            Assert.AreEqual("no-var", result.Errors.ElementAt(0).ErrorCode);
            Assert.AreEqual("Unexpected var, use let or const instead.", result.Errors.ElementAt(0).Message);
            Assert.AreEqual("prettier/prettier", result.Errors.ElementAt(1).ErrorCode);
            Assert.AreEqual("Insert `;`", result.Errors.ElementAt(1).Message);
        }

        [TestMethod, TestCategory("Prettier")]
        public async Task StandardTsDontShowPrettier()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // We set ShowPrettierErrors to false and they should be filtered out even though we're running with prettier
            MockSettings settings = new MockSettings { ShowPrettierErrors = false };
            LintingResult result = await new Linter(settings)
                .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\a.ts") }, new string[] { });
            Assert.IsTrue(result.HasErrors);
            Assert.IsFalse(HasPrettierErrors(result.Errors));
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(11, result.Errors);
            Assert.AreEqual("no-var", result.Errors.First().ErrorCode);
            Assert.AreEqual("Unexpected var, use let or const instead.", result.Errors.First().Message);
        }

        private bool HasPrettierErrors(HashSet<LintingError> errors) => errors.Any(e => e.ErrorCode == "prettier/prettier");

        [TestMethod, TestCategory("Prettier")]
        public async Task StandardTsFixErrors()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            try
            {
                File.Copy(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\a.ts"), 
                    Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\aTest.ts"), true);
                // Same as standard test above, except we fix, so should get rid of some of those errors
                LintingResult result = await new Linter(MockSettings.Instance, true)
                    .LintAsync(new string[] { Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\aTest.ts") }, new string[] { });
                Assert.IsTrue(result.HasErrors);
                Assert.IsFalse(HasPrettierErrors(result.Errors)); // They should have all be fixed
                Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
                Assert.HasCount(5, result.Errors);
                Assert.AreEqual("Expected '===' and instead saw '=='.", result.Errors.First().Message);
                string actual = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\aTest.ts"));
                string expected = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\aFixed.ts"));
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                File.Delete(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\aTest.ts"));
            }
        }

        [TestMethod, TestCategory("Prettier")]
        public async Task StandardTsconfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            // As for tsconfig tests in EslintWithConfigTest we add rule @typescript-eslint/no-unnecessary-type-assertion
            // into our .eslintrc.js and hand the linter a tsconfig.json file to lint.  This points at file a.ts only, which
            // contains code that has the error, as well as prettier errors.
            MockSettings settings = new MockSettings { ShowPrettierErrors = true, UseTsConfig = true };
            string mainProjectTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\tsconfig.json");
            LintingResult result = await new Linter(settings).LintAsync(new string[] { }, new string[] { mainProjectTsconfig });
            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(HasPrettierErrors(result.Errors));
            Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
            Assert.HasCount(5, result.Errors);
            LintingError noUnnecessaryTypeAssertionError = result.Errors.First(le =>
                le.ErrorCode == "@typescript-eslint/no-unnecessary-type-assertion");
            Assert.IsNotNull(noUnnecessaryTypeAssertionError);
        }

        [TestMethod, TestCategory("Prettier")]
        public async Task StandardTsconfigFixErrors()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            try
            {
                // Fix the errors from the StandardTsconfig test
                File.Copy(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\a.ts"),
                    Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\aBup.ts"), true);
                MockSettings settings = new MockSettings { ShowPrettierErrors = true, UseTsConfig = true };
                string mainProjectTsconfig = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\tsconfig.json");
                LintingResult result = await new Linter(settings, fixErrors: true)
                    .LintAsync(new string[] { }, new string[] { mainProjectTsconfig });
                Assert.IsTrue(result.HasErrors);
                Assert.IsFalse(HasPrettierErrors(result.Errors));  // Have been fixed
                Assert.IsFalse(string.IsNullOrEmpty(result.Errors.First().FileName), "File name is empty");
                Assert.HasCount(1, result.Errors);
                bool errorExists = result.Errors.Any(le =>
                    le.ErrorCode == "@typescript-eslint/no-unnecessary-type-assertion");
                Assert.IsFalse(errorExists); // Has been fixed
                Assert.AreEqual("'bar' is assigned a value but never used.", result.Errors.First().Message);
                string actual = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\a.ts"));
                string expected = File.ReadAllText(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\aFixed.ts"));
                Assert.AreEqual(expected, actual);
            }
            finally
            {
                File.Copy(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\aBup.ts"),
                          Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\a.ts"), true);
                File.Delete(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"prettier\tsconfig\aBup.ts"));
            }
        }
    }
}
