using EnvDTE;
using System;

namespace TypeScriptAnalyzerEslintTest
{
    public class MockUIHierarchyItem : UIHierarchyItem
    {
        public void Select(vsUISelectionType How)
        {
            throw new NotImplementedException();
        }

        public DTE DTE => throw new NotImplementedException();

        public UIHierarchyItems Collection => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public UIHierarchyItems UIHierarchyItems => throw new NotImplementedException();

        public object Object { get; set; }

        public bool IsSelected => throw new NotImplementedException();
    }
}
