using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintVsix;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{

    [TestClass]
    public class EslintConfigWithTypeInfoTest
    {
        private static EnvDTE80.DTE2 dte = null;
        private static Solution solution = null;
        private static MockSettings settings = null;

        public TestContext TestContext { get; set; } = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () => { await ClassInitializeAsync(testContext); });
        }

        public static async Task ClassInitializeAsync(TestContext testContext)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(testContext.CancellationToken);
            MessageFilter.Register();
            Type type = Type.GetTypeFromProgID(VisualStudioVersion.ProgID);
            object inst = Activator.CreateInstance(type, true);
            dte = (EnvDTE80.DTE2)inst;
            string path = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"bad-eslint-config-typeinfo\bad-eslint-config.sln");
            dte.Solution.Open(path);
            solution = dte.Solution;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            AssemblyMethods.MockServiceProvider.Reset();
            TypeScriptAnalyzerEslintVsix.Package.Jtf = ThreadHelper.JoinableTaskFactory;
            settings = new MockSettings();
            TypeScriptAnalyzerEslintVsix.Package.Settings = settings;
            TypeScriptAnalyzerEslintVsix.Package.Dte = dte;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TypeScriptAnalyzerEslintVsix.Package.Jtf = null;
            settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Dte = null;
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
            if (solution != null) { solution.Close(); solution = null; }
            dte?.Quit();
            TypeScriptAnalyzerEslintVsix.Package.Jtf = null;
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Dte = null;
            MessageFilter.Revoke();
        }

        [TestMethod, TestCategory("Lint End to End")]
        public async Task LintWithConfigThatNeedsTypeInfoAndNoneProvided()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = solution };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };

            MockErrorListDataSource mockErrorListDataSource = new MockErrorListDataSource();
            ErrorListDataSource.InjectMockErrorListDataSource(mockErrorListDataSource);

            settings.UseTsConfig = false;

            try
            {
                // Test with a .eslintrc.js config file that has a rule that needs type info, but use tsconfig not enabled
                // Test we get an error message that says how to fix it
                bool hasVSErrors = await LintFilesCommandBase.LintSelectedItemsAsync(false, selectedItems);

                Assert.IsTrue(hasVSErrors);
                Assert.IsTrue(mockErrorListDataSource.HasErrors());
                Assert.HasCount(1, mockErrorListDataSource.Snapshots);

                CollectionAssert.AreEquivalent(new string[] { "eslint" }, mockErrorListDataSource.Snapshots.Keys.ToArray());
                var actualMsg = mockErrorListDataSource.Snapshots["eslint"].Select(e => e.Message).First();
                Assert.StartsWith("ESLint webserver error caused by a rule included in config that " +
                    "needs type information", actualMsg);
            }
            finally
            {
                ErrorListDataSource.InjectMockErrorListDataSource(null);
                settings.UseTsConfig = false;
            }

        }

        [TestMethod, TestCategory("Lint End to End")]
        public async Task LintWithProvidedTypeInfo()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = solution };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };

            MockErrorListDataSource mockErrorListDataSource = new MockErrorListDataSource();
            ErrorListDataSource.InjectMockErrorListDataSource(mockErrorListDataSource);

            settings.UseTsConfig = true;

            try
            {
                // Same test as above with UseTsConfig set to true: we should get linting errors, not a web server error
                bool hasVSErrors = await LintFilesCommandBase.LintSelectedItemsAsync(false, selectedItems);

                Assert.IsTrue(hasVSErrors);
                Assert.IsTrue(mockErrorListDataSource.HasErrors());
                Assert.HasCount(1, mockErrorListDataSource.Snapshots);

                string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"bad-eslint-config-typeinfo\badtest.ts");
                Assert.AreEqual(expected, mockErrorListDataSource.Snapshots.Keys.First());
                Assert.AreEqual(3, mockErrorListDataSource.Snapshots[expected].Count());
            }
            finally
            {
                ErrorListDataSource.InjectMockErrorListDataSource(null);
                settings.UseTsConfig = false;
            }

        }

    }
}
