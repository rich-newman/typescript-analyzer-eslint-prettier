using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintVsix;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    /// <summary>
    /// Tests how files are found for linting from various selected items in Solution Explorer
    /// This is the normal case of linting all files in a VS project: use tsconfig.json option is set to false
    /// </summary>
    [TestClass]
    public class LintFileLocationsTest
    {
        private static EnvDTE80.DTE2 dte = null;
        private static EnvDTE.Solution solution = null;
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

            settings = new MockSettings() { UseTsConfig = false };
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
            settings.EnableIgnore = true;
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

        [TestMethod, TestCategory("Lint File Locations")]
        public async Task GetLintFilesForSolution()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = solution };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();
            string[] results = LintFileLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            Assert.AreEqual(11, results.Length);

            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react-dom.d.ts");
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react.d.ts");
            string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\test.ts");
            string expected4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
            string expected5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            string expected6 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\file3.ts");
            string expected7 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file5.ts");
            string expected8 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file6.tsx");
            string expected9 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\file9.ts"); // Linked file in Tsconfig.sln\tsconfigTest.csproj
            string expected10 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\file7.ts"); // Nested file under HtmlPage1.html
            string expected11 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\file10.ts");

            Assert.IsTrue(results.Contains(expected1));
            Assert.IsTrue(results.Contains(expected2));
            Assert.IsTrue(results.Contains(expected3));
            Assert.IsTrue(results.Contains(expected4));
            Assert.IsTrue(results.Contains(expected5));
            Assert.IsTrue(results.Contains(expected6));
            Assert.IsTrue(results.Contains(expected7));
            Assert.IsTrue(results.Contains(expected8));
            Assert.IsTrue(results.Contains(expected9));
            Assert.IsTrue(results.Contains(expected10)); // file7 nested, but included
            Assert.IsTrue(results.Contains(expected11)); // file7 nested, but included

            Assert.AreEqual(11, fileToProjectMap.Keys.Count);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected1]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected2]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected3]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected4]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected5]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected6]);
            Assert.AreEqual("tsconfigEmptyTest", fileToProjectMap[expected7]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected8]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected9]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected10]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected11]);

        }

        [TestMethod, TestCategory("Lint File Locations")]
        public async Task GetLintFilesForProject()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\tsconfigEmptyTest.csproj");
            Project project = TsconfigLocationsTest.FindProject(projectFullName, solution);
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = project };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = LintFileLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            Assert.AreEqual(1, results.Length);
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file5.ts");
            Assert.IsTrue(results.Contains(expected1));
            Assert.AreEqual(1, fileToProjectMap.Keys.Count);
            Assert.AreEqual(fileToProjectMap[expected1], "tsconfigEmptyTest");
        }

        [TestMethod, TestCategory("Lint File Locations")]
        public async Task GetLintFilesForSingleItem()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
            string projectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfigTest.csproj");
            Project project = TsconfigLocationsTest.FindProject(projectFullName, solution);
            ProjectItem projectItem = TsconfigLocationsTest.FindProjectItemInProject(projectItemFullName, project);

            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = projectItem };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = LintFileLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            Assert.AreEqual(1, results.Length);
            Assert.AreEqual(projectItemFullName, results[0]);
            Assert.AreEqual(1, fileToProjectMap.Keys.Count);
            Assert.AreEqual(fileToProjectMap[projectItemFullName], "tsconfigTest");
        }

        // ESLint ignores the node_modules folder completely (unless you pass no-ignore), so we don't want our code to find all those files
        // in that case
        [TestMethod, TestCategory("Lint File Locations")]
        public async Task GetLintFilesForProjectWithNodeModules()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(),
                @"tsconfig\multiple\tsconfigTest.csproj");
            Project project = TsconfigLocationsTest.FindProject(projectFullName, solution);
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = project };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = LintFileLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            Assert.AreEqual(10, results.Length);
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            Assert.IsTrue(results.Contains(expected1));
            Assert.AreEqual(10, fileToProjectMap.Keys.Count);
            Assert.AreEqual(fileToProjectMap[expected1], "tsconfigTest");
            string notExpected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(),
                @"tsconfig\multiple\node_modules\d\filenm5.ts");
            Assert.IsTrue(File.Exists(notExpected1));
            Assert.IsFalse(results.Contains(notExpected1));
            string notExpected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(),
                @"tsconfig\multiple\node_modules\filenm4.ts");
            Assert.IsTrue(File.Exists(notExpected2));
            Assert.IsFalse(results.Contains(notExpected2));
        }

        // When we disable the default ignores, with Tools/Options/Disable Ignore = true, we CAN lint in node_modules
        [TestMethod, TestCategory("Lint File Locations")]
        public async Task GetLintFilesForProjectWithNodeModulesDisableIgnore()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(),
                @"tsconfig\multiple\tsconfigTest.csproj");
            Project project = TsconfigLocationsTest.FindProject(projectFullName, solution);
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = project };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            settings.EnableIgnore = false;

            string[] results = LintFileLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            Assert.AreEqual(12, results.Length);
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            Assert.IsTrue(results.Contains(expected1));
            Assert.AreEqual(12, fileToProjectMap.Keys.Count);
            Assert.AreEqual(fileToProjectMap[expected1], "tsconfigTest");
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(),
                @"tsconfig\multiple\node_modules\d\filenm5.ts");
            Assert.IsTrue(results.Contains(expected2));
            string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(),
                @"tsconfig\multiple\node_modules\filenm4.ts");
            Assert.IsTrue(results.Contains(expected3));
        }

        // The tests below here don't actually need the loaded solution in class initialize
        [TestMethod, TestCategory("Lint File Locations")]
        public async Task GetLintFilesForSingleFileInFolderView()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string path = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = LintFileLocations.FindPathsFromSelectedItemInFolderView(path, fileToProjectMap);

            Assert.AreEqual(1, results.Length);
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            Assert.IsTrue(results.Contains(expected1));
            Assert.AreEqual(1, fileToProjectMap.Keys.Count);
            Assert.AreEqual(fileToProjectMap[expected1], "");
        }

        [TestMethod, TestCategory("Lint File Locations")]
        public async Task GetLintFilesForFolderInFolderView()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string path = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple");
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = LintFileLocations.FindPathsFromSelectedItemInFolderView(path, fileToProjectMap);

            Assert.AreEqual(10, results.Length);
            Assert.AreEqual(10, fileToProjectMap.Keys.Count);

            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react-dom.d.ts");
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\react.d.ts");
            string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\test.ts");
            string expected4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
            string expected5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            string expected6 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\file3.ts");
            // file5.ts was in solution being tested in GetLintFilesForSolution above, but not in tsconfg\multiple folder
            //string expected7 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file5.ts"); 
            string expected8 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file6.tsx");
            //string expected9 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\file9.ts");
            string expected10 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\file7.ts"); // Nested file under HtmlPage1.html
            string expected11 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\file10.ts");
            string expected12 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\file2.ts"); // Not in project, but is in folder

            Assert.IsTrue(results.Contains(expected1));
            Assert.IsTrue(results.Contains(expected2));
            Assert.IsTrue(results.Contains(expected3));
            Assert.IsTrue(results.Contains(expected4));
            Assert.IsTrue(results.Contains(expected5));
            Assert.IsTrue(results.Contains(expected6));
            //Assert.IsTrue(results.Contains(expected7));
            Assert.IsTrue(results.Contains(expected8));
            //Assert.IsTrue(results.Contains(expected9));
            Assert.IsTrue(results.Contains(expected10)); 
            Assert.IsTrue(results.Contains(expected11));
            Assert.IsTrue(results.Contains(expected12));

            Assert.AreEqual(fileToProjectMap[expected1], "");
        }
    }
}
