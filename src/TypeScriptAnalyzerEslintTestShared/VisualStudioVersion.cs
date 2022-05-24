using System.IO;

namespace TypeScriptAnalyzerEslintTest
{
    public static class VisualStudioVersion
    {
        // ProgID is the version of Visual Studio that is instantiated in the tests to load test solutions into
        // 15.0 is VS2017, 16.0 is VS2019, and 17.0 is VS2022
        // The tests should pass in any other version of Visual Studio than one you're running, assuming it's installed
#if VS2022
        public const string ProgID = "VisualStudio.DTE.17.0";
#else
        public const string ProgID = "VisualStudio.DTE.16.0";
#endif
        // If we've loaded the solution in VS2022 the path from the executable to the artifacts is different from VS2019
        public static string GetArtifactsFolder()
        {
#if VS2022
            return Path.GetFullPath(@"..\..\..\..\TypeScriptAnalyzerEslintTestShared\artifacts");
#else
            return Path.GetFullPath(@"..\..\..\TypeScriptAnalyzerEslintTestShared\artifacts");
#endif
        }

        public static string GetSolutionFolder()
        {
#if VS2022
            return Path.GetFullPath(@"..\..\..\..\..");
#else
            return Path.GetFullPath(@"..\..\..\..");
#endif
        }
    }
}
