using Microsoft.VisualStudio.Sdk.TestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintTest
{
    [TestClass]
    public static class AssemblyMethods
    {
        internal static GlobalServiceProvider MockServiceProvider { get; private set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            _ = context;
            MockServiceProvider = new GlobalServiceProvider();
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            Linter.Server.Down();
            MockServiceProvider?.Dispose();
        }
    }
}
