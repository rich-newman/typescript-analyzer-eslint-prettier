using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TypeScriptAnalyzerEslintTest
{
    [TestClass]
    public class ZzGenerateCsprojEntries
    {
        private string result;
        private int pathLocation;
        [TestMethod, TestCategory("zzRelease Support Only")]
        public void GenerateNodeFileEntriesInCsproj()
        {
            // Results are in typescript-analyzer\src\Shared\Node\temp.txt
            // and need to be used to update TypeScriptAnalyzerEslintVsix.csproj and TypeScriptAnalyzerEslintVsix64.csproj
            result = "";
            string executionPath = TypeScriptAnalyzerEslintLinter.NodeServer.ExecutionPath;
#if VS2022
            string sharedDirectory = executionPath.Replace("TypeScriptAnalyzerEslintTest64\\bin\\x64\\Debug", "Shared");
#else
            string sharedDirectory = executionPath.Replace("TypeScriptAnalyzerEslintTest\\bin\\Debug", "Shared");
#endif
            Assert.IsTrue(Directory.Exists(sharedDirectory), $"Source folder for node files ({sharedDirectory}) doesn't exist");
            pathLocation = sharedDirectory.LastIndexOf($"\\{TypeScriptAnalyzerEslintLinter.Constants.NODE_FOLDER_NAME}") + 1;
            ProcessDirectory(sharedDirectory);
            File.WriteAllText(sharedDirectory + "\\temp.txt", result);
        }

        private void ProcessDirectory(string targetDirectory)
        {
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        private readonly string template = @"
    <Content Include=""TSAREPLACE2"">
      <Link>TSAREPLACE1</Link>
      <IncludeInVSIX>true</IncludeInVSIX>
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>";

        //private readonly string template = @"    <Content Include=""REPLACE"">
        //      <IncludeInVSIX>true</IncludeInVSIX>
        //      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        //    </Content>
        //";
        private void ProcessFile(string path)
        {
            string pathFromNodeFolder = path.Substring(pathLocation, path.Length - pathLocation);
            string entry = template.Replace("TSAREPLACE1", pathFromNodeFolder).Replace("TSAREPLACE2", @"..\Shared\" + pathFromNodeFolder);
            result += entry;
        }
    }
}
