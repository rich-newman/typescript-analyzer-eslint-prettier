using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TypeScriptAnalyzerEslintVsix;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    /// <summary>
    /// Tests how files are found for linting when the use tsconfig.json option is set to true
    /// Tests file discovery both from file paths and from selected items in Solution Explorer
    /// </summary>
    [TestClass]
    public class TsconfigLocationsTest
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
            string path = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\Tsconfig.sln");
            dte.Solution.Open(path);
            solution = dte.Solution;

            settings = new MockSettings() { UseTsConfig = true };
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
            dte?.Quit();
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
            TypeScriptAnalyzerEslintVsix.Package.Dte = null;
            MessageFilter.Revoke();
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task BasicEnvironment()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindForSingleItem()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
            string[] result = TsconfigLocations.FindParentTsconfigs(projectItemFullName);
            Assert.AreEqual(1, result.Length);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            Assert.AreEqual(expected, result[0]);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindForSingleItemSubfolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\test.ts");
            string[] result = TsconfigLocations.FindParentTsconfigs(projectItemFullName);
            Assert.AreEqual(1, result.Length);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            Assert.AreEqual(expected, result[0]);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindForSingleItemMultipleTsconfigs()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\file10.ts");
            string[] result = TsconfigLocations.FindParentTsconfigs(projectItemFullName);
            Assert.AreEqual(2, result.Length);
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.json");
            Assert.IsTrue(result.Contains(expected1));
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.app.json");
            Assert.IsTrue(result.Contains(expected2));
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindForSingleItemRoot()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Note folder c contains a tsconfig.json reference in VS that doesn't exist on disk: we don't want to try to lint this
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            string[] result = TsconfigLocations.FindParentTsconfigs(projectItemFullName);
            Assert.AreEqual(1, result.Length);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            Assert.AreEqual(expected, result[0]);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindForSingleItemNotsconfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Note there's a tsconfig.json in the folder but it's not in the project: logic changed so it's picked up
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file2.ts");
            string[] result = TsconfigLocations.FindParentTsconfigs(projectItemFullName);
            //Assert.IsNull(result);
            Assert.AreEqual(1, result.Length);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\tsconfig.json");
            Assert.AreEqual(expected, result[0]);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInProject()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfigTest.csproj");
            Project project = FindProject(projectFullName, solution);
            HashSet<string> results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            TsconfigLocations.FindTsconfigsInProject(project, results);
            string expectedConfig1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            string expectedConfig2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            string expectedConfig3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\tsconfig.json");
            string expectedConfig4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.json");
            string expectedConfig5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.app.json");
            Assert.AreEqual(5, results.Count);
            Assert.IsTrue(results.Contains(expectedConfig1));
            Assert.IsTrue(results.Contains(expectedConfig2));
            Assert.IsTrue(results.Contains(expectedConfig3));
            Assert.IsTrue(results.Contains(expectedConfig4));
            Assert.IsTrue(results.Contains(expectedConfig5));
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInProjectWithNodeModulesDisableIgnore()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            settings.EnableIgnore = false;
            string projectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfigTest.csproj");
            Project project = FindProject(projectFullName, solution);
            HashSet<string> results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            TsconfigLocations.FindTsconfigsInProject(project, results);
            string expectedConfig1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            string expectedConfig2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            string expectedConfig3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\tsconfig.json");
            string expectedConfig4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.json");
            string expectedConfig5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.app.json");
            // With EnableIgnore false the tsconfig in node_modules is included and will be linted
            string expectedConfig6 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\node_modules\tsconfig.json");
            Assert.AreEqual(6, results.Count);
            Assert.IsTrue(results.Contains(expectedConfig1));
            Assert.IsTrue(results.Contains(expectedConfig2));
            Assert.IsTrue(results.Contains(expectedConfig3));
            Assert.IsTrue(results.Contains(expectedConfig4));
            Assert.IsTrue(results.Contains(expectedConfig5));
            Assert.IsTrue(results.Contains(expectedConfig6));
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInProjectNotsconfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Folder b contains a tsconfig on disk that's not included in the VS project
            // Folder c contains a tsconfig in the VS project (tsconfigEmptyTest.csproj) that doesn't exist on disk
            // In both cases we don't lint with these
            string projectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\tsconfigEmptyTest.csproj");
            Project project = FindProject(projectFullName, solution);
            HashSet<string> results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            TsconfigLocations.FindTsconfigsInProject(project, results);
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSelectedItemsSingleFile()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Arrange
            string mainProjectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfigTest.csproj");
            Project mainProject = FindProject(mainProjectFullName, solution);
            string mainFile4FullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            ProjectItem file4 = FindProjectItemInProject(mainFile4FullName, mainProject);
            MockUIHierarchyItem mockFile4HierarchyItem = new MockUIHierarchyItem() { Object = file4 };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockFile4HierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            // Act
            string[] results = TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            // Assert
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            Assert.AreEqual(1, results.Length);
            Assert.IsTrue(results.Contains(expected));
            // We do create the map for selected individual files
            Assert.AreEqual(1, fileToProjectMap.Keys.Count);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[mainFile4FullName]);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSelectedItemsTsconfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string mainProjectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfigTest.csproj");
            Project mainProject = FindProject(mainProjectFullName, solution);
            string mainProjectTsconfigFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            ProjectItem tsconfig = FindProjectItemInProject(mainProjectTsconfigFullName, mainProject);
            MockUIHierarchyItem mockTsconfigHierarchyItem = new MockUIHierarchyItem() { Object = tsconfig };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockTsconfigHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            string expected = mainProjectTsconfigFullName;
            Assert.AreEqual(1, results.Length);
            Assert.IsTrue(results.Contains(expected));
            // If we lint having selected a tsconfig we don't search the project structure pre-lint, so can't construct the map
            Assert.AreEqual(0, fileToProjectMap.Keys.Count);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSelectedItemsNoTsconfig()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string emptyProjectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\tsconfigEmptyTest.csproj");
            Project emptyProject = FindProject(emptyProjectFullName, solution);
            string emptyFile2FullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file2.ts");
            ProjectItem file2 = FindProjectItemInProject(emptyFile2FullName, emptyProject);
            MockUIHierarchyItem mockFile2HierarchyItem = new MockUIHierarchyItem() { Object = file2 };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockFile2HierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            Assert.AreEqual(0, results.Length);
            Assert.AreEqual(0, fileToProjectMap.Keys.Count);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSelectedItemsSolution()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MockUIHierarchyItem mockSolutionHierarchyItem = new MockUIHierarchyItem() { Object = solution };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockSolutionHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            string expectedConfig1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            string expectedConfig2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            string expectedConfig3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\tsconfig.json");
            string expectedConfig4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.json");
            string expectedConfig5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.app.json");
            Assert.AreEqual(5, results.Length);
            Assert.IsTrue(results.Contains(expectedConfig1));
            Assert.IsTrue(results.Contains(expectedConfig2));
            Assert.IsTrue(results.Contains(expectedConfig3));
            Assert.IsTrue(results.Contains(expectedConfig4));
            Assert.IsTrue(results.Contains(expectedConfig5));
            Assert.AreEqual(0, fileToProjectMap.Keys.Count);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSelectedItemsProject()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string mainProjectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfigTest.csproj");
            Project mainProject = FindProject(mainProjectFullName, solution);

            MockUIHierarchyItem mockProjectHierarchyItem = new MockUIHierarchyItem() { Object = mainProject };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[] { mockProjectHierarchyItem };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\tsconfig.json");
            string expected4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.json");
            string expected5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.app.json");
            Assert.AreEqual(5, results.Length);
            Assert.IsTrue(results.Contains(expected1));
            Assert.IsTrue(results.Contains(expected2));
            Assert.IsTrue(results.Contains(expected3));
            Assert.IsTrue(results.Contains(expected4));
            Assert.IsTrue(results.Contains(expected5));
            Assert.AreEqual(0, fileToProjectMap.Keys.Count);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSelectedItemsMultipleFiles()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Includes two files with the same tsconfig.json and one with none
            string mainProjectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfigTest.csproj");
            Project mainProject = FindProject(mainProjectFullName, solution);
            string emptyProjectFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\tsconfigEmptyTest.csproj");
            Project emptyProject = FindProject(emptyProjectFullName, solution);

            string mainFile4FullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            ProjectItem file4 = FindProjectItemInProject(mainFile4FullName, mainProject);
            MockUIHierarchyItem mockFile4HierarchyItem = new MockUIHierarchyItem() { Object = file4 };

            string emptyFile2FullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\none\b\file2.ts");
            ProjectItem file2 = FindProjectItemInProject(emptyFile2FullName, emptyProject);
            MockUIHierarchyItem mockFile2HierarchyItem = new MockUIHierarchyItem() { Object = file2 };

            string mainFile1FullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
            ProjectItem file1 = FindProjectItemInProject(mainFile1FullName, mainProject);
            MockUIHierarchyItem mockFile1HierarchyItem = new MockUIHierarchyItem() { Object = file1 };
            UIHierarchyItem[] selectedItems = new UIHierarchyItem[]
                                              {
                                                  mockFile1HierarchyItem, mockFile2HierarchyItem, mockFile4HierarchyItem
                                              };
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            string[] results = TsconfigLocations.FindPathsFromSelectedItems(selectedItems, fileToProjectMap);

            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            Assert.AreEqual(1, results.Length);
            Assert.IsTrue(results.Contains(expected));
            // We create the fileToProjectMap for single selected files only, which is what we have here
            Assert.AreEqual(2, fileToProjectMap.Keys.Count);
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\file1.ts");
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected1]);
            Assert.AreEqual("tsconfigTest", fileToProjectMap[expected2]);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSolution()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            HashSet<string> results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            TsconfigLocations.FindTsconfigsInSolution(solution, results);
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\tsconfig.json");
            string expected4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.json");
            string expected5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.app.json");
            Assert.AreEqual(5, results.Count);
            Assert.IsTrue(results.Contains(expected1));
            Assert.IsTrue(results.Contains(expected2));
            Assert.IsTrue(results.Contains(expected3));
            Assert.IsTrue(results.Contains(expected4));
            Assert.IsTrue(results.Contains(expected5));
        }

        public static Project FindProject(string projectFullName, Solution solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (Project project in solution.Projects)
                if (project.FullName == projectFullName) return project;
            return null;
        }

        public static ProjectItem FindProjectItemInProject(string projectItemName, Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (ProjectItem projectItem in project.ProjectItems)
            {
                ProjectItem result = FindProjectItemInProjectItem(projectItemName, projectItem);
                if (result != null) return result;
            }
            return null;
        }

        private static ProjectItem FindProjectItemInProjectItem(string projectItemName, ProjectItem rootProjectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string fileName = rootProjectItem.GetFullPath();
            if (fileName == projectItemName) return rootProjectItem;
            if (rootProjectItem == null || rootProjectItem.ProjectItems == null) return null;
            foreach (ProjectItem subProjectItem in rootProjectItem.ProjectItems)
            {
                ProjectItem result = FindProjectItemInProjectItem(projectItemName, subProjectItem);
                if (result != null) return result;
            }
            return null;
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSingleFileInFolderView()
        {
            // If we lint an individual file in folder view we find the first tsconfig above it and put that in results, and put the 
            // file in the fileToProjectMap as a key.  There are no projects, so the values in the map are empty strings (currently).
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Arrange
            string path = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\c\file4.ts");
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            // Act
            string[] results = TsconfigLocations.FindPathsFromSelectedItemInFolderView(path, fileToProjectMap);

            // Assert
            // tsconfig in results, original file in fileToProjectMap
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            Assert.AreEqual(1, results.Length);
            Assert.IsTrue(results.Contains(expected));
            // We do create the map for selected individual files
            Assert.AreEqual(1, fileToProjectMap.Keys.Count);
            Assert.IsTrue(fileToProjectMap.Keys.Contains(path));
            Assert.AreEqual("", fileToProjectMap[path]);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInFolderInFolderView()
        {
            // If we lint a folder in folder view we find all tsconfig.json files at any level in it and put those in results
            // There are no individual files, so fileToProjectMap is empty
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Arrange
            string path = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple");
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            // Act
            string[] results = TsconfigLocations.FindPathsFromSelectedItemInFolderView(path, fileToProjectMap);

            // Assert
            string expected1 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            string expected2 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\a\tsconfig.json");
            string expected3 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\b\tsconfig.json");
            string expected4 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.json");
            string expected5 = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\d\tsconfig.app.json");
            Assert.AreEqual(5, results.Length);
            Assert.IsTrue(results.Contains(expected1));
            Assert.IsTrue(results.Contains(expected2));
            Assert.IsTrue(results.Contains(expected3));
            Assert.IsTrue(results.Contains(expected4));
            Assert.IsTrue(results.Contains(expected5));

            Assert.AreEqual(0, fileToProjectMap.Keys.Count);
        }

        [TestMethod, TestCategory("tsconfig Locations")]
        public async Task FindInSingleTsconfigFileInFolderView()
        {
            // If we lint a tsconfig.json in folder view then results should contain the tsconfig.json, fileToProject map again empty
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Arrange
            string path = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            Dictionary<string, string> fileToProjectMap = new Dictionary<string, string>();

            // Act
            string[] results = TsconfigLocations.FindPathsFromSelectedItemInFolderView(path, fileToProjectMap);

            // Assert
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"tsconfig\multiple\tsconfig.json");
            Assert.AreEqual(1, results.Length);
            Assert.IsTrue(results.Contains(expected));

            Assert.AreEqual(0, fileToProjectMap.Keys.Count);
        }
    }
}
