using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TypeScriptAnalyzerEslintVsix
{
    // Currently finding tsconfigs can get confusing, because there are broadly two scenarios:
    // 1. We open/save/request a lint on a single .ts file
    // 2. We request a lint on a project or solution from Solution Explorer
    // In case 1 we have to try to find an associated tsconfig.json to use for type rules: we search the folder and parent folders.
    // We lint with the first tsconfig.json we find and filter the results to the individual file requested.
    // In case 2 we find all tsconfig.jsons in the project or solution, lint with them, and show all results.
    // Case 1 means there's no real link between VS projects and tsconfig.json projects, except we do insist any tsconfigs we want to
    // lint with are included in a project somewhere in the solution.
    // Other possibilities include requesting a lint with a tsconfig.json, where we do just that and show all results, and requesting a
    // lint on a folder in Solution Explorer.  Here again we should find all tsconfigs in the folder or below and lint.
    // Edit there's a case 3 which we haven't dealt with well: we request a lint on a folder in Solution Explorer
    public static class TsconfigLocations
    {
        // Given a path to a file finds any lintable tsconfig.json in the folder of the path, or any parent folder
        // 'lintable' means 'exists and is in a VS project in this solution'
        public static string[] FindParentTsconfigs(string projectItemFullPath)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            DirectoryInfo folder = Directory.GetParent(projectItemFullPath);
            List<string> results = new List<string>();
            while (folder != null)
            {
                foreach (FileInfo fileInfo in folder.EnumerateFiles())
                {
                    if (LintableFiles.IsLintableTsconfig(fileInfo.FullName))
                        results.Add(fileInfo.FullName);
                }
                if (results.Count > 0) return results.ToArray();
                folder = folder.Parent;
            }
            return null;
        }

        public static string[] FindPathsFromSelectedItems(UIHierarchyItem[] items, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // Belt and braces for clarity of intent only: we check in the calling code in any case
            if (!Package.Settings.UseTsConfig) return new string[] { };
            HashSet<string> tsconfigFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (UIHierarchyItem selItem in items)
            {
                if (!LintableFiles.IsLintable(selItem)) continue;
                if (selItem.Object is Solution solution)
                    FindTsconfigsInSolution(solution, tsconfigFiles);
                else if (selItem.Object is Project project)
                    FindTsconfigsInProject(project, tsconfigFiles);
                else if (selItem.Object is ProjectItem item && item.GetFullPath() is string projectItemPath)
                    FindTsconfigsFromSelectedProjectItem(projectItemPath, item, tsconfigFiles, fileToProjectMap);
            }
            return tsconfigFiles.ToArray();
        }

        private static void FindTsconfigsFromSelectedProjectItem(string projectItemPath, ProjectItem item, HashSet<string> result,
                                                         Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (LintableFiles.IsLintableTypeScriptFile(projectItemPath))
            {
                if(!fileToProjectMap.ContainsKey(projectItemPath)) fileToProjectMap.Add(projectItemPath, item.ContainingProject.Name);
                string[] tsconfigs = FindParentTsconfigs(projectItemPath);
                if (tsconfigs != null) result.UnionWith(tsconfigs);
            }
            else if (LintFileLocations.IsPhysicalFolder(item) || LintableFiles.IsLintableTsconfig(projectItemPath))
            {
                FindTsConfigsInProjectItem(item, result);
            }
        }

        internal static void FindTsconfigsInSolution(Solution solution, HashSet<string> result)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (solution.Projects == null) return;
            foreach (Project project in solution.Projects)
                FindTsconfigsInProject(project, result);
        }

        internal static void FindTsconfigsInProject(Project project, HashSet<string> result)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (project.ProjectItems == null) return;
            foreach (ProjectItem projectItem in project.ProjectItems)
                FindTsConfigsInProjectItem(projectItem, result);
        }

        private static void FindTsConfigsInProjectItem(ProjectItem projectItem, HashSet<string> result)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (LintFileLocations.IsIgnoredNodeModulesFolder(projectItem)) return;
            string itemPath = projectItem.GetFullPath();
            if (LintableFiles.IsLintableTsconfig(itemPath) && !result.Contains(itemPath)) result.Add(itemPath);
            // A project item can be a folder or a nested file, so we may need to continue searching down the tree
            if (projectItem.ProjectItems == null) return;
            foreach (ProjectItem subProjectItem in projectItem.ProjectItems)
                FindTsConfigsInProjectItem(subProjectItem, result);
        }


        public static string[] FindPathsFromSelectedItemInFolderView(string path, Dictionary<string, string> fileToProjectMap)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (!Package.Settings.UseTsConfig) return new string[] { };
            HashSet<string> results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (LintableFiles.IsLintableFile(path))
            {
                if (!fileToProjectMap.ContainsKey(path)) fileToProjectMap.Add(path, "");
                string[] tsconfigs = FindParentTsconfigs(path);
                if (tsconfigs != null) results.UnionWith(tsconfigs);
            }
            else if (System.IO.Directory.Exists(path) || LintableFiles.IsLintableTsconfig(path))
            {
                FindTsConfigsInPath(path, results);
            }
            return results.ToArray();
        }


        private static void FindTsConfigsInPath(string path, HashSet<string> result)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (LintFileLocations.IsIgnoredNodeModulesFolder(path)) return;
            if (LintableFiles.IsLintableTsconfig(path) && !result.Contains(path)) result.Add(path);
            if (!System.IO.Directory.Exists(path)) return;
            foreach (string filePath in System.IO.Directory.GetFiles(path))
                FindTsConfigsInPath(filePath, result);
            foreach (string directoryPath in System.IO.Directory.GetDirectories(path))
                FindTsConfigsInPath(directoryPath, result);
        }
    }
}
