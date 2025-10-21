using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TypeScriptAnalyzerEslintVsix;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintTest
{
    [TestClass]
    public class LocalNodeModulesLocationsTest
    {
        private static MockSettings settings = null;

        public TestContext TestContext { get; set; } = null;

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

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task InProjectFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\b\b.csproj");
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\b");
            Assert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task InParentFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\a.csproj");
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple");
            Assert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task InSolutionFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\multiple.sln");
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple");
            Assert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task InNoLocalFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            try
            {
                // If we install ESLint at the solution root for linting the Analyzer solution it becomes a valid local install for
                // the test.  So we temporarily remove package.json at the root to stop it being found.
                BackupPackageJson();
                string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\none");
                string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
                Assert.AreEqual("", result);
            }
            finally { RestorePackageJson(); }
        }

        private void BackupPackageJson()
        {
            string packageJsonPath = Path.Combine(VisualStudioVersion.GetSolutionFolder(), "package.json");
            string packageJsonBakPath = Path.Combine(VisualStudioVersion.GetSolutionFolder(), "packagebak.json");
            if (File.Exists(packageJsonBakPath)) File.Delete(packageJsonBakPath);
            if (File.Exists(packageJsonPath)) File.Move(packageJsonPath, packageJsonBakPath);
        }

        private void RestorePackageJson()
        {
            string packageJsonPath = Path.Combine(VisualStudioVersion.GetSolutionFolder(), "package.json");
            string packageJsonBakPath = Path.Combine(VisualStudioVersion.GetSolutionFolder(), "packagebak.json");
            if (!File.Exists(packageJsonPath) && File.Exists(packageJsonBakPath)) File.Move(packageJsonBakPath, packageJsonPath);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task LocalNodeModulesDisabledInProjectFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\b\b.csproj");
            settings.EnableLocalNodeModules = false;
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            Assert.AreEqual("", result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task LocalNodeModulesDisabledInParentFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TestContext.CancellationToken);
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\a.csproj");
            settings.EnableLocalNodeModules = false;
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            Assert.AreEqual("", result);
        }
    }
}
