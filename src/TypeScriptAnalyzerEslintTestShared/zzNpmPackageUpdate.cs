#pragma warning disable IDE0079
#pragma warning disable MSTEST0016
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TypeScriptAnalyzerEslintTest
{
    /// <summary>
    /// Goes to npm and updates all packages listed in docs/_config.yml under packageversions to the latest in that file, which
    /// is used for the docs only, and in the two package.json files in the solution.  The package.json in 
    /// TypeScriptAnalyzerEslintLinter/Node is the one used to construct the Node folder used in a release.  The
    /// package.json in Solution Items is used to allow linting of Markdown in the project (using the Analyzer to check itself).
    /// For @types/node we don't necessarily want the latest version, so an override variable is provided below.
    /// </summary>
    /// <remarks>This method is very slow, as it makes individual calls to npm using a Diagnostics.Process for every package.  It's
    /// not as slow as doing this manually though.</remarks>
    [TestClass]
    public class ZzNpmPackageUpdate
    {
        private readonly string typesNodeOverride = "20.11.0";
        private readonly string configLineTemplate = "        \"[0]\": \"[1]\"";
        private readonly string packageJsonLineTemplate = "    \"[0]\": \"[1]\",";

        // [TestMethod, TestCategory("zzRelease Support Only")]
        public void UpdateNpmVersionsInConfigFiles()
        {
            string executionPath = TypeScriptAnalyzerEslintLinter.NodeServer.ExecutionPath;
#if VS2022
            string docsDirectory = executionPath.Replace("src\\TypeScriptAnalyzerEslintTest64\\bin\\x64\\Debug\\Node", "docs");
#else
            string docsDirectory = executionPath.Replace("src\\TypeScriptAnalyzerEslintTest\\bin\\Debug\\Node", "docs");
#endif
            string configFile = Path.Combine(docsDirectory, "_config.yml");
            string[] configLines = File.ReadAllLines(configFile);
            bool foundPackages = false;
            Dictionary<string, string> packages = new Dictionary<string, string>();
            for (int index = 0; index < configLines.Length; index++)
            {
                string line = configLines[index];
                if (foundPackages && !string.IsNullOrWhiteSpace(line) && !line.Contains("|-"))
                {
                    string[] package = line.Replace("\"", "").Replace(" ", "").Split(':');
                    string latestVersion = OverrideExists(package[0]) ? typesNodeOverride : 
                        GetLatestNpmVersion(package[0]).Replace("\n", "");
                    if (latestVersion != package[1])
                    {
                        string newLine = configLineTemplate.Replace("[0]", package[0]).Replace("[1]", latestVersion);
                        configLines[index] = newLine;
                        packages.Add(package[0], latestVersion);
                    }
                }
                if (line.Contains("packageversions:")) foundPackages = true;
            }
            File.WriteAllLines(configFile, configLines);

            string dogfoodPackageJsonFile = docsDirectory.Replace("\\docs", "\\package.json");
            UpdatePackageJsonFile(dogfoodPackageJsonFile, packages);
            string nodeServerPackageJsonFile = 
                docsDirectory.Replace("\\docs", "\\src\\TypeScriptAnalyzerEslintLinter\\Node\\package.json");
            UpdatePackageJsonFile(nodeServerPackageJsonFile, packages);
        }

        private bool OverrideExists(string packageName) => packageName == "@types/node" && typesNodeOverride != "";

        private void UpdatePackageJsonFile(string file, Dictionary<string, string> packages)
        {
            string[] lines = File.ReadAllLines(file);
            bool foundPackages = false;
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index];
                if (foundPackages && line.Contains("}")) 
                { 
                    foundPackages = false;
                    RemoveFinalCommaFromLine(index - 1, lines);
                }
                if (foundPackages && !string.IsNullOrWhiteSpace(line))
                {
                    string[] package = line.Replace("\"", "").Replace(" ", "").Split(':');
                    if (package.Length == 2 && packages.ContainsKey(package[0]))
                    {
                        string newLine = packageJsonLineTemplate.Replace("[0]", package[0]).Replace("[1]", packages[package[0]]);
                        lines[index] = newLine;
                    }
                }
                if (line.Contains("dependencies") || line.Contains("devDependencies")) foundPackages = true;
            }
            File.WriteAllLines(file, lines);
        }

        private void RemoveFinalCommaFromLine(int index, string[] lines)
        {
            if (index < 0 || index >= lines.Length) return;
            string line = lines[index];
            if(line.EndsWith(",")) 
                lines[index] = line.Remove(line.Length - 1);
        }

        private string GetLatestNpmVersion(string packageName)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C npm view {packageName} version",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}
