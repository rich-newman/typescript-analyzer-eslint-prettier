using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAnalyzerEslintVsix
{
    public static class LintFileLocations
    {
        private static void FindInSolution(Solution solution, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (solution.Projects == null) return;
            foreach (Project project in solution.Projects)
                FindInProject(project, fileToProjectMap);
        }

        private static void FindInProject(Project project, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (project.ProjectItems == null) return;
            foreach (ProjectItem projectItem in project.ProjectItems)
                FindInProjectItem(projectItem, fileToProjectMap);
        }

        private static void FindInProjectItem(ProjectItem projectItem, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (IsIgnoredNodeModulesFolder(projectItem)) return;
            string itemPath = projectItem.GetFullPath();
            // We return the keys to the fileToProjectMap to be individually linted: TypeScript files are handled by TsconfigLocations
            // if we're using tsconfigs, and shouldn't be added here
            if (LintableFiles.IsLintableFile(itemPath) && !fileToProjectMap.ContainsKey(itemPath) 
                && !IsTypeScriptFileAndUsingTsconfig(itemPath))
                fileToProjectMap.Add(itemPath, projectItem.ContainingProject.Name);
            if (projectItem.ProjectItems == null) return;
            foreach (ProjectItem subProjectItem in projectItem.ProjectItems)
                FindInProjectItem(subProjectItem, fileToProjectMap);
        }

        public static bool IsIgnoredNodeModulesFolder(ProjectItem projectItem)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return Package.Settings.EnableIgnore && IsPhysicalFolder(projectItem)
                && projectItem.Name == "node_modules";
        }

        public static bool IsPhysicalFolder(ProjectItem projectItem)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return string.Equals(projectItem.Kind, Constants.vsProjectItemKindPhysicalFolder, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTypeScriptFileAndUsingTsconfig(string itemPath) =>
            Package.Settings.UseTsConfig && LintableFiles.IsTypeScriptFileExtension(itemPath);

        public static string[] FindPathsFromSelectedItems(UIHierarchyItem[] items, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (UIHierarchyItem selItem in items)
            {
                if (!LintableFiles.IsLintable(selItem)) continue;
                if (selItem.Object is Solution solution)
                    FindInSolution(solution, fileToProjectMap);
                else if (selItem.Object is Project project)
                    FindInProject(project, fileToProjectMap);
                else if (selItem.Object is ProjectItem item)
                    FindInProjectItem(item, fileToProjectMap);
            }
            return fileToProjectMap.Keys.ToArray();
        }

        public static string[] FindPathsFromSelectedItemInFolderView(string path, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            FindPathsFromSelectedItemInFolderViewInternal(path, fileToProjectMap);
            return fileToProjectMap.Keys.ToArray();
        }

        private static void FindPathsFromSelectedItemInFolderViewInternal(string path, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (IsIgnoredNodeModulesFolder(path)) return;
            if (LintableFiles.IsLintableFile(path) && !fileToProjectMap.ContainsKey(path) && !IsTypeScriptFileAndUsingTsconfig(path))
                fileToProjectMap.Add(path, "");
            if (!System.IO.Directory.Exists(path)) return;
            foreach (string filePath in System.IO.Directory.GetFiles(path))
                FindPathsFromSelectedItemInFolderViewInternal(filePath, fileToProjectMap);
            foreach (string directoryPath in System.IO.Directory.GetDirectories(path))
                FindPathsFromSelectedItemInFolderViewInternal(directoryPath, fileToProjectMap);
        }

        public static bool IsIgnoredNodeModulesFolder(string path)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return Package.Settings.EnableIgnore && System.IO.Directory.Exists(path)
                && new System.IO.DirectoryInfo(path).Name == "node_modules";
        }
    }
}