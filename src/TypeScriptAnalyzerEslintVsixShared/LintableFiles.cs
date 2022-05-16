using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace TypeScriptAnalyzerEslintVsix
{
    /// <summary>
    /// Methods to test whether various items can be linted
    /// </summary>
    /// <remarks>
    /// I don't think 'lintable' is a valid adjective.  
    /// But then I don't think 'lint' is a valid verb either. http://www.dictionary.com/browse/lint
    /// </remarks>
    public static class LintableFiles
    {
        public static bool AreAllSelectedItemsLintable()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            UIHierarchyItem[] selectedItems = Package.Dte.ToolWindows.SolutionExplorer.SelectedItems as UIHierarchyItem[];
            // In this test we check whether what's clicked on is lintable, meaning it's an item that might contain
            // files that can be linted.  We don't actually check whether there ARE any files that can be linted associated
            // with the item.  For example, if you rightclick a solution file we check it's a solution file, but we don't 
            // check for valid .ts or .tsx files in the solution. This applies to the ignore options as well (ignore patterns, 
            // ignore nested).
            foreach (UIHierarchyItem selectedItem in selectedItems)
            {
                if (!IsLintable(selectedItem)) return false;
            }
            return true;
        }

        public static bool IsLintable(UIHierarchyItem selectedItem)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return selectedItem.Object is Solution ||
                   selectedItem.Object is Project ||
                  (selectedItem.Object is ProjectItem item &&
                        item.GetFullPath() is string projectItemPath &&
                        IsLintableProjectItem(projectItemPath));
        }


        private static bool IsLintableProjectItem(string projectItemPath)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return Directory.Exists(projectItemPath) ||
                   (!Package.Settings.UseTsConfig && IsLintableFile(projectItemPath)) ||
                   (Package.Settings.UseTsConfig &&
                       (IsLintableTsconfig(projectItemPath) || IsLintableFile(projectItemPath)));
        }


        //public static bool IsLintableFileInSolution(string filePath)
        //{
        //    Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
        //    return IsLintableFile(filePath) && IsFileInSolution(filePath);
        //}

        // Check if filename is absolute because when debugging, script files are sometimes dynamically created.
        public static bool IsLintableFile(string filePath) => IsLintableFileExtension(filePath) && IsRootedAndExists(filePath);
        public static bool IsLintableTypeScriptFile(string filePath) => IsLintableFile(filePath) && IsTypeScriptFileExtension(filePath);
        public static bool IsRootedAndExists(string filePath) => Path.IsPathRooted(filePath) && File.Exists(filePath);
        public static bool IsLintableFileExtension(string fileName) => LintableFileExtensions.Contains(Path.GetExtension(fileName));
        private static readonly HashSet<string> TypeScriptFileExtensions 
            = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".ts", ".tsx" };
        public static bool IsTypeScriptFileExtension(string fileName) => TypeScriptFileExtensions.Contains(Path.GetExtension(fileName));
        public static bool IsLintableTsconfig(string filePath) => IsRootedAndExists(filePath) && IsValidTsConfigFileName(filePath);
        private static bool IsValidTsConfigFileName(string filePath) => 
            TsConfigRegex.IsMatch(Path.GetFileName(filePath));

        public static HashSet<string> LintableFileExtensions { get; set; }
            = CreateLintableFileExtensions(Package.Settings.LintableFileExtensions);

        private static HashSet<string> CreateLintableFileExtensions(string input)
        {
            var extensions = input.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            HashSet<string> result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string extension in extensions)
                result.Add((extension.StartsWith(".") ? "" : ".") + extension.Trim());
            return result;
        }

        public static void SetLintableFileExtensions(string input) => LintableFileExtensions = CreateLintableFileExtensions(input);

        public static Regex TsConfigRegex { get; set; } = CreateTsConfigRegex(Package.Settings.TsconfigNamesPattern);

        private static Regex CreateTsConfigRegex(string pattern)
        {
            bool mult = pattern.Contains(",");
            string regexString = "^" + (mult ? "(" : "") +
                Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".").Replace(",", "|")
                + (mult ? ")" : "") + "$";
            return new Regex(regexString, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public static bool SetTsConfigPattern(string pattern)
        {
            // pattern is user-entered. I'm not sure we can get an invalid RegEx here, but we guard against it anyway.
            try
            {
                TsConfigRegex = CreateTsConfigRegex(pattern);
                return true;
            }
            catch (System.Exception) { return false; }
        }

        //private static bool IsFileInSolution(string filePath)
        //{
        //    Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
        //    return Package.Dte.Solution.FindProjectItem(filePath)?.GetFullPath() is string;
        //}
    }
}
