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
    public class LintableFileTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
            TypeScriptAnalyzerEslintVsix.Package.Settings = new MockSettings();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TypeScriptAnalyzerEslintVsix.Package.Settings = null;
        }

        [TestMethod, TestCategory("Lintable File")]
        public void NullFileNameIsNotLintable()
        {
            bool result = LintableFiles.IsLintableFile(null);
            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory("Lintable File")]
        public void EmptyFileNameIsNotLintable()
        {
            bool result = LintableFiles.IsLintableFile("");
            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory("Lintable File")]
        public void NonExistentFileIsNotLintable()
        {
            bool result = LintableFiles.IsLintableFile("c:\\DoesntExist.xyz");
            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory("Lintable File")]
        public void FileWithNonLintableFileExtensionIsNotLintable()
        {
            // MockSettings gets the default list of lintable file extensions in Settings.DefaultLintableFileExtensions
            // This doesn't include .sln files
            string testFilePath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\multiple.sln");
            bool result = LintableFiles.IsLintableFile(testFilePath);
            Assert.IsTrue(File.Exists(testFilePath));
            Assert.IsFalse(result);
        }

        [TestMethod, TestCategory("Lintable File")]
        public void FileWithLintableFileExtensionIsLintable()
        {
            // MockSettings gets the default list of lintable file extensions in Settings.DefaultLintableFileExtensions
            // This includes .ts files
            string testFilePath = Path.Combine(VisualStudioVersion.GetArtifactsFolder(), @"localinstall\multiple\a\fileA.ts");
            bool result = LintableFiles.IsLintableFile(testFilePath);
            Assert.IsTrue(File.Exists(testFilePath));
            Assert.IsTrue(result);
        }
    }
}
