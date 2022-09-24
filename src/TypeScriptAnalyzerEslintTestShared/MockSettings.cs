using System.Collections.Generic;
using System.IO;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintTest
{
    public class MockSettings : ISettings
    {
        private static MockSettings settings;

        public static MockSettings Instance
        {
            get
            {
                if (settings == null)
                    settings = new MockSettings();

                return settings;
            }
        }

        public static string CWD
        {
            get { return new FileInfo("../../../TypeScriptAnalyzerEslintTestShared/artifacts/").FullName; }
        }

        public bool ESLintEnable => true;
        public bool FixOnSave => false;
        public bool UseTsConfig { get; set; } = false;
        public bool RunOnBuild => false;
        public bool ShowUnderlining => true;
        public bool EnableLogging => false;
        public bool LogFileNames => false;
        public bool LogFirstConfig => false;
        public bool LogTsConfig => false;
        public bool EnableLocal { get; set; } = true;
        public bool EnableIgnore { get; set; } = true;
        public int JvmMemory { get; set; } = 0;
        public bool ShowPrettierErrors { get; set; } = false;
        public string TsconfigNamesPattern { get; set; } = TypeScriptAnalyzerEslintVsix.Settings.DefaultTsconfigNamesPattern;
        public string LintableFileExtensions { get; set; } = TypeScriptAnalyzerEslintVsix.Settings.DefaultLintableFileExtensions;
        public int LintInterval { get; set; } = 5000;
        public int SaveDelay => 300;
        public void Initialize() { }
        public void ResetSettings() { }
    }
}
