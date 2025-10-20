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
    /// <summary>
    /// Tests we construct the correct Solution Explorer items for linting when we are building
    /// </summary>
    /// <remarks>
    /// The rules are slightly different from regular file discovery: if a single item or items is
    /// selected in Solution Explorer we need to figure out which VS projects are being built
    /// </remarks>
    [TestClass]
    public class BuildSelectedItemsTest
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
            dte.Solution.Open(Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall/multiple/multiple.sln"));
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

        [TestMethod, TestCategory("Build Selected Items")]
        public async Task SelectedItemsWhenBuidingSolution()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            UIHierarchyItem[] results = BuildSelectedItems.Get(isBuildingSolution: true);
            Assert.HasCount(1, results);
            Solution solutionObject = results[0].Object as Solution;
            Assert.IsNotNull(solutionObject);
            Assert.EndsWith("\\src\\TypeScriptAnalyzerEslintTestShared\\artifacts\\localinstall\\multiple\\multiple.sln", solutionObject.FullName);
        }

        [TestMethod, TestCategory("Build Selected Items")]
        public async Task MapToProjectsSingleProjectItem()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Simulate fileA.ts being selected in Solution Explorer in multiple.sln.
            // Ensure a.csproj (which contains fileA.ts) is the item we calculate as building
            string fileAFullPath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\fileA.ts");
            Solution solution = TypeScriptAnalyzerEslintVsix.Package.Dte.ToolWindows.SolutionExplorer.UIHierarchyItems.Item(1).Object as Solution;
            ProjectItem projectItem = solution.FindProjectItem(fileAFullPath);
            MockUIHierarchyItem mockProjectItemHierarchyItem = new MockUIHierarchyItem { Object = projectItem };

            UIHierarchyItem[] results = BuildSelectedItems.MapToProjects(new UIHierarchyItem[] { mockProjectItemHierarchyItem }).ToArray();

            Assert.HasCount(1, results);
            Project projectObject = results[0].Object as Project;
            Assert.IsNotNull(projectObject);
            Assert.EndsWith("\\src\\TypeScriptAnalyzerEslintTestShared\\artifacts\\localinstall\\multiple\\a\\a.csproj", projectObject.FullName
);
        }

        [TestMethod, TestCategory("Build Selected Items")]
        public async Task MapToProjectsTwoProjectItemsInSameProject()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Simulate fileA.ts and fileAA.ts being selected in Solution Explorer in multiple.sln.
            string fileAFullPath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\fileA.ts");
            string fileAAFullPath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\fileAA.ts");
            Solution solution = TypeScriptAnalyzerEslintVsix.Package.Dte.ToolWindows.SolutionExplorer.UIHierarchyItems.Item(1).Object as Solution;
            ProjectItem projectItemA = solution.FindProjectItem(fileAFullPath);
            ProjectItem projectItemAA = solution.FindProjectItem(fileAAFullPath);
            MockUIHierarchyItem mockProjectItemHierarchyItemA = new MockUIHierarchyItem { Object = projectItemA };
            MockUIHierarchyItem mockProjectItemHierarchyItemAA = new MockUIHierarchyItem { Object = projectItemAA };

            UIHierarchyItem[] results = BuildSelectedItems.MapToProjects(
                new UIHierarchyItem[] { mockProjectItemHierarchyItemA, mockProjectItemHierarchyItemAA }).ToArray();

            Assert.HasCount(1, results);
            Project projectObject = results[0].Object as Project;
            Assert.IsNotNull(projectObject);
            Assert.EndsWith("\\src\\TypeScriptAnalyzerEslintTestShared\\artifacts\\localinstall\\multiple\\a\\a.csproj", projectObject.FullName
);
        }

        [TestMethod, TestCategory("Build Selected Items")]
        public async Task MapToProjectsTwoProjectItemsInDifferentProjects()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Simulate fileA.ts and fileB.ts being selected in Solution Explorer in multiple.sln.
            string fileAFullPath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\fileA.ts");
            string fileBFullPath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\b\fileB.ts");
            Solution solution = TypeScriptAnalyzerEslintVsix.Package.Dte.ToolWindows.SolutionExplorer.UIHierarchyItems.Item(1).Object as Solution;
            ProjectItem projectItemA = solution.FindProjectItem(fileAFullPath);
            ProjectItem projectItemB = solution.FindProjectItem(fileBFullPath);
            MockUIHierarchyItem mockProjectItemHierarchyItemA = new MockUIHierarchyItem { Object = projectItemA };
            MockUIHierarchyItem mockProjectItemHierarchyItemB = new MockUIHierarchyItem { Object = projectItemB };

            UIHierarchyItem[] results = BuildSelectedItems.MapToProjects(
                new UIHierarchyItem[] { mockProjectItemHierarchyItemA, mockProjectItemHierarchyItemB }).ToArray();

            Assert.HasCount(2, results);
            Project projectObject = results[0].Object as Project;
            Assert.IsNotNull(projectObject);
            Assert.EndsWith("\\src\\TypeScriptAnalyzerEslintTestShared\\artifacts\\localinstall\\multiple\\a\\a.csproj", projectObject.FullName
);
            Project projectObject2 = results[1].Object as Project;
            Assert.IsNotNull(projectObject2);
            Assert.EndsWith("\\src\\TypeScriptAnalyzerEslintTestShared\\artifacts\\localinstall\\multiple\\b\\b.csproj", projectObject2.FullName
);
        }


        [TestMethod, TestCategory("Build Selected Items")]
        public async Task MapToProjectsItemNotInProject()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // package.json in b isn't in a project or solution
            string fileFullPath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall/multiple/b/package.json");
            Assert.IsTrue(File.Exists(fileFullPath));
            Solution solution = TypeScriptAnalyzerEslintVsix.Package.Dte.ToolWindows.SolutionExplorer.UIHierarchyItems.Item(1).Object as Solution;
            ProjectItem projectItem = solution.FindProjectItem(fileFullPath);
            MockUIHierarchyItem mockProjectItemHierarchyItem = new MockUIHierarchyItem { Object = projectItem };

            UIHierarchyItem[] results = BuildSelectedItems.MapToProjects(new UIHierarchyItem[] { mockProjectItemHierarchyItem }).ToArray();

            Assert.IsEmpty(results);
        }
    }
}
