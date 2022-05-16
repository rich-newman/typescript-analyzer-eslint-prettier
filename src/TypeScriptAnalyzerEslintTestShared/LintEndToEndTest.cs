using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintVsix;
using TypeScriptAnalyzerEslintLinter;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    /// <summary>
    /// Tests the whole process of linting end-to-end from selected items in Solution Explorer
    /// to the snapshots parsed into the error list, both with and without use tsconfig.json set
    /// </summary>
    /// <remarks>
    /// This was complicated before the JoinableTaskFactory came along, now it's mindboggling. I wrote it, and I think it's mindboggling.
    /// </remarks>
    [TestClass]
    public class LintEndToEndTest
    {
        private static EnvDTE80.DTE2 dte = null;
        private static Solution solution = null;
        private static MockSettings settings = null;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _ = testContext; // https://github.com/dotnet/roslyn/issues/35063#issuecomment-484616262
            ThreadHelper.JoinableTaskFactory.Run(async () => { await ClassInitializeAsync(); });
        }

        public static async Task ClassInitializeAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MessageFilter.Register();
            Type type = System.Type.GetTypeFromProgID(VisualStudioVersion.ProgID);
            object inst = System.Activator.CreateInstance(type, true);
            dte = (EnvDTE80.DTE2)inst;
            dte.Solution.Open(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\Tsconfig.sln"));
            solution = dte.Solution;
            settings = new MockSettings();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            AssemblyMethods.MockServiceProvider.Reset();
            TypeScriptAnalyzerEslintVsix.Package.Jtf = ThreadHelper.JoinableTaskFactory;
            TypeScriptAnalyzerEslintVsix.Package.Settings = settings;
            TypeScriptAnalyzerEslintVsix.Package.Dte = dte;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TypeScriptAnalyzerEslintVsix.Package.Jtf = null;
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Dte = null;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ThreadHelper.JoinableTaskFactory.Run(async () => { await ClassCleanupAsync(); });
        }

        public static async Task ClassCleanupAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (solution != null) { solution.Close(); solution = null; }
            if (dte != null) dte.Quit();
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Dte = null;
            MessageFilter.Revoke();
        }

        [TestMethod, TestCategory("Lint End to End")]
        public async Task LintSolution()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = solution };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };

            MockErrorListDataSource mockErrorListDataSource = new MockErrorListDataSource();
            ErrorListDataSource.InjectMockErrorListDataSource(mockErrorListDataSource);

            settings.UseTsConfig = false;

            try
            {
                bool hasVSErrors = await LintFilesCommandBase.LintSelectedItemsAsync(false, selectedItems);

                Assert.IsTrue(hasVSErrors); // By default ESLint errors are also VS errors now
                Assert.IsTrue(mockErrorListDataSource.HasErrors());
                Assert.AreEqual(10, mockErrorListDataSource.Snapshots.Count);

                // See LintFileLocationsTest.GetLintFilesForSolutionIncludeNested
                //string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react-dom.d.ts");
                string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react-dom.d.ts");

                string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react.d.ts");
                string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\test.ts");
                string expected4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
                string expected5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
                string expected6 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\file3.ts");
                string expected7 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file5.ts");
                string expected8 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file6.tsx");
                string expected9 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\file7.ts");
                string expected10 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\file9.ts"); // Linked file

                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected1));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected2));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected3));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected4));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected5));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected6));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected7));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected8));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected9));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected10));

                // May be too painful when we upgrade
                Assert.AreEqual(22, mockErrorListDataSource.Snapshots[expected1].Count());
                Assert.AreEqual(341, mockErrorListDataSource.Snapshots[expected2].Count());
                Assert.AreEqual(3, mockErrorListDataSource.Snapshots[expected3].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected4].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected5].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected6].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected7].Count());
                Assert.AreEqual(3, mockErrorListDataSource.Snapshots[expected8].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected9].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected10].Count());

                // Pick an error and check we are generating all details - expected4 is file1.ts
                LintingError lintingError = mockErrorListDataSource.Snapshots[expected4].First(le => le.ErrorCode.EndsWith("no-empty-function"));
                Assert.AreEqual(expected4, lintingError.FileName);
                Assert.AreEqual("Unexpected empty function 'file1'.", lintingError.Message);
                Assert.AreEqual(2, lintingError.LineNumber);
                Assert.AreEqual(17, lintingError.ColumnNumber);
                Assert.AreEqual(2, lintingError.EndLineNumber);
                Assert.AreEqual(20, lintingError.EndColumnNumber);
                Assert.AreEqual("https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-empty-function.md", lintingError.HelpLink);
                Assert.IsTrue(lintingError.ErrorType == LintingErrorType.Error);
                Assert.IsFalse(lintingError.IsBuildError);
                Assert.AreEqual("eslint", lintingError.Provider.Name);
            }
            finally
            {
                ErrorListDataSource.InjectMockErrorListDataSource(null);
                settings.UseTsConfig = false;
            }

        }

        [TestMethod, TestCategory("Lint End to End")]
        public async Task LintSolutionUseTsconfigs()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = solution };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };

            MockErrorListDataSource mockErrorListDataSource = new MockErrorListDataSource();
            ErrorListDataSource.InjectMockErrorListDataSource(mockErrorListDataSource);

            settings.UseTsConfig = true;

            try
            {
                bool hasVSErrors = await LintFilesCommandBase.LintSelectedItemsAsync(false, selectedItems);

                Assert.IsTrue(hasVSErrors);
                Assert.IsTrue(mockErrorListDataSource.HasErrors());
                Assert.AreEqual(9, mockErrorListDataSource.Snapshots.Count);

                // Note file5 is referenced by a tsconfig that isn't in the project, so doesn't get included
                string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
                string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\file2.ts");
                string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\file3.ts");
                string expected4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
                //string expected5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file5.ts");
                string expected6 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file6.tsx");
                string expected7 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\file7.ts");
                string expected8 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\test.ts");
                string expected9 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react-dom.d.ts");
                string expected10 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react.d.ts");


                // TODO .d.ts files were excluded in TSLint by TSLint itself, but here get included without an ignore pattern
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected1));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected2));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected3));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected4));
                //Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected5));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected6));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected7));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected8));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected9));
                Assert.IsTrue(mockErrorListDataSource.Snapshots.Keys.Contains(expected10));

                // Similar to TslintWithTsconfigTest.LintAll, again this level of detail may be too much
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected1].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected2].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected3].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected4].Count());
                //Assert.AreEqual(4, mockErrorListDataSource.Snapshots[expected5].Count());
                Assert.AreEqual(3, mockErrorListDataSource.Snapshots[expected6].Count());
                Assert.AreEqual(2, mockErrorListDataSource.Snapshots[expected7].Count());
                Assert.AreEqual(3, mockErrorListDataSource.Snapshots[expected8].Count());
                Assert.AreEqual(22, mockErrorListDataSource.Snapshots[expected9].Count());
                Assert.AreEqual(341, mockErrorListDataSource.Snapshots[expected10].Count());
            }
            finally
            {
                ErrorListDataSource.InjectMockErrorListDataSource(null);
                settings.UseTsConfig = false;
            }

        }
    }
}
