using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace TypeScriptAnalyzerEslintVsix
{
    public static class LocalNodeModulesLocations
    {
        public static string FindLocalInstallFromPath(string fullPath)
        {
            return Package.Settings.EnableLocal ?
                FindLocalInstallFromDirectoryName(Path.GetDirectoryName(fullPath)) : "";
        }

        private static string FindLocalInstallFromDirectoryName(string directoryName)
        {
            if (IsLocalInstall(directoryName)) return directoryName;
            DirectoryInfo parentDirectoryInfo = Directory.GetParent(directoryName);
            if (parentDirectoryInfo == null) return "";
            return FindLocalInstallFromDirectoryName(parentDirectoryInfo.FullName);
        }

        private static bool IsLocalInstall(string directory) => 
            File.Exists(Path.Combine(directory, "package.json")) && Directory.Exists(Path.Combine(directory, "node_modules", "eslint"));
    }
}