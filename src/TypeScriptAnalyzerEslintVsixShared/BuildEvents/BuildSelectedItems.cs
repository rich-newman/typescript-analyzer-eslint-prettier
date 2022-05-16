using EnvDTE;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAnalyzerEslintVsix
{
    /// <summary>
    /// Works out what we're building in terms of UIHierarchyItems in the Solution Explorer window, however we invoke the build
    /// </summary>
    public class BuildSelectedItems
    {
        public static UIHierarchyItem[] Get(bool isBuildingSolution)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return isBuildingSolution ?
            new UIHierarchyItem[] { GetUIHierarchySolutionItem() } :
            MapToProjects(Package.Dte.ToolWindows.SolutionExplorer.SelectedItems as UIHierarchyItem[]).ToArray();
        }

        private static UIHierarchyItem GetUIHierarchySolutionItem()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            UIHierarchyItems uiHierarchyItems = Package.Dte.ToolWindows.SolutionExplorer.UIHierarchyItems;
            foreach (UIHierarchyItem item in uiHierarchyItems)
            {
                if (item.Object is Solution) return item;
            }
            return null;
        }

        internal static IEnumerable<UIHierarchyItem> MapToProjects(UIHierarchyItem[] selectedItems)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (selectedItems == null) yield break;
            // Note that you can build a single project from the build menu, but your options are limited
            // to the project you have selected in Solution Explorer.  Here 'project you have selected' can
            // mean the project a selected item is in.  If you ctrl-click items in two projects the menu option
            // changes to 'Build Selection', meaning build both.  This logic replicates that.
            HashSet<string> seenPaths = new HashSet<string>();

            foreach (UIHierarchyItem selectedItem in selectedItems)
            {
                if (selectedItem.Object is ProjectItem projectItem &&
                    projectItem.ContainingProject?.GetRootFolder() is string projectItemRootFolder &&
                    !seenPaths.Contains(projectItemRootFolder))
                {
                    seenPaths.Add(projectItemRootFolder);
                    UIHierarchyItems uiHierarchyItems = Package.Dte.ToolWindows.SolutionExplorer.UIHierarchyItems;
                    UIHierarchyItem containingProjectHierarchyItem = GetHierarchyItemForProject(projectItemRootFolder, uiHierarchyItems);
                    if (containingProjectHierarchyItem != null) yield return containingProjectHierarchyItem;
                }
                else if (selectedItem.Object is Project project &&
                    project?.GetRootFolder() is string projectRootFolder &&
                    !seenPaths.Contains(projectRootFolder))
                {
                    seenPaths.Add(projectRootFolder);
                    yield return selectedItem;
                }
                else if (selectedItem.Object is Solution)
                    yield return selectedItem;
            }
        }

        private static UIHierarchyItem GetHierarchyItemForProject(string projectRootFolder, UIHierarchyItems uiHierarchyItems)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (UIHierarchyItem item in uiHierarchyItems)
            {
                if (item.Object is Project project && projectRootFolder == project.GetRootFolder()) return item;
                if (item.UIHierarchyItems != null)
                {
                    UIHierarchyItem uiHierarchyItem = GetHierarchyItemForProject(projectRootFolder, item.UIHierarchyItems);
                    if (uiHierarchyItem != null) return uiHierarchyItem;
                }
            }
            return null;
        }
    }
}
