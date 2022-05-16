using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;

namespace TypeScriptAnalyzerEslintVsix
{
    /// <summary>
    /// Sundry extension and static methods for Visual Studio projects and project items
    /// </summary>
    public static class ProjectExtensions
    {
        public static string GetRootFolder(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(project.FullName))
                return null;

            string fullPath;

            try
            {
                fullPath = project.Properties.Item("FullPath").Value as string;
            }
            catch (ArgumentException)
            {
                try
                {
                    // MFC projects don't have FullPath, and there seems to be no way to query existence
                    fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                }
                catch (ArgumentException)
                {
                    // Installer projects have a ProjectPath.
                    fullPath = project.Properties.Item("ProjectPath").Value as string;
                }
            }

            if (string.IsNullOrEmpty(fullPath))
                return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : null;

            if (Directory.Exists(fullPath))
                return fullPath;

            if (File.Exists(fullPath))
                return Path.GetDirectoryName(fullPath);

            return null;
        }

        public static async System.Threading.Tasks.Task<string> GetFullPathAsync(this ProjectItem projectItem)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            return GetFullPath(projectItem);
        }

        public static string GetFullPath(this ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string objectTypeName = projectItem.Object?.GetType().FullName;
            if (objectTypeName == "Microsoft.NodejsTools.Project.NodeModulesNode") return null;
            try
            {
                return projectItem.Properties?.Item("FullPath")?.Value?.ToString();
            }
            // If FullPath doesn't exist then .Item throws.  Which is a damn shame because we wouldn't need this method.
            catch (ArgumentException)
            {
                Logger.Log("GetFullPath throws for " + objectTypeName);
                return null;
            }
        }

        public static string GetProjectNameFromFilePath(string filePath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ProjectItem projectItem = Package.Dte.Solution.FindProjectItem(filePath);
            return projectItem?.ContainingProject?.Name != null ? projectItem.ContainingProject.Name : "";
        }

        // Used to get rid of the compiler's way too enthusiastic warnings re RunAsync
        public static void Forget(this Microsoft.VisualStudio.Threading.JoinableTask _) { }
    }
}
