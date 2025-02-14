﻿using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TypeScriptAnalyzerEslintVsix;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    public class ProjectsTestBase
    {
        protected static EnvDTE80.DTE2 dte;
        protected static Solution solution;
        protected MockSettings settings;
        protected UIHierarchyItem[] selectedItems;
        protected MockErrorListDataSource mockErrorListDataSource;

        [TestInitialize]
        public void TestInitialize()
        {
            AssemblyMethods.MockServiceProvider.Reset();
            TypeScriptAnalyzerEslintVsix.Package.Jtf = ThreadHelper.JoinableTaskFactory;
            settings = new MockSettings() { UseTsConfig = false };
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

        protected void Initialize(string solutionPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            MessageFilter.Register();
            Type type = Type.GetTypeFromProgID(VisualStudioVersion.ProgID);
            object inst = Activator.CreateInstance(type, true);
            dte = (EnvDTE80.DTE2)inst;
            dte.Solution.Open(Path.GetFullPath(solutionPath));

            solution = dte.Solution;
            TypeScriptAnalyzerEslintVsix.Package.Dte = dte;
        }

        protected UIHierarchyItem[] Arrange(string solutionPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Initialize(solutionPath);
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = solution };
            selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };
            mockErrorListDataSource = new MockErrorListDataSource();
            ErrorListDataSource.InjectMockErrorListDataSource(mockErrorListDataSource);
            return selectedItems;
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
            dte?.Quit();
            TypeScriptAnalyzerEslintVsix.Package.Jtf = null;
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Dte = null;
            MessageFilter.Revoke();
        }
    }

    [TestClass]
    public class EmptyUnloadedProjectsTest : ProjectsTestBase
    {
        [TestMethod, TestCategory("Empty/Unloaded Projects")]
        public async Task LintEmptySolution()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                Arrange(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"empty\noprojects.sln"));

                bool hasVSErrors = await LintFilesCommandBase.LintSelectedItemsAsync(false, selectedItems);

                Assert.IsFalse(hasVSErrors);
                Assert.IsFalse(mockErrorListDataSource.HasErrors());
                Assert.AreEqual(0, mockErrorListDataSource.Snapshots.Count);
            }
            finally
            {
                ErrorListDataSource.InjectMockErrorListDataSource(null);
            }
        }

    }
}
