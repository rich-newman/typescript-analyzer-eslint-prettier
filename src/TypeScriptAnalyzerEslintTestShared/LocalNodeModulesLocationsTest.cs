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
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\b\b.csproj");
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\b");
            Assert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task InParentFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\a.csproj");
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple");
            Assert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task InSolutionFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\multiple.sln");
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            string expected = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple");
            Assert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task InNoLocalFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\none");
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            string expected = "";
            Assert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task LocalNodeModulesDisabledInProjectFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\b\b.csproj");
            settings.EnableLocal = false;
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            Assert.AreEqual("", result);
        }

        [TestMethod, TestCategory("Local Node Module Locations")]
        public async Task LocalNodeModulesDisabledInParentFolder()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            string projectItemFullName = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\a.csproj");
            settings.EnableLocal = false;
            string result = LocalNodeModulesLocations.FindLocalInstallFromPath(projectItemFullName);
            Assert.AreEqual("", result);
        }
    }
}
