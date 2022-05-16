using System.Collections.Generic;

namespace TypeScriptAnalyzerEslintLinter
{
    public interface ISettings
    {
        bool RunOnBuild { get; }
        bool ESLintEnable { get; }
        bool UseTsConfig { get; }
        void ResetSettings();
        bool ShowUnderlining { get; }
        bool EnableLogging { get; }
        bool LogFileNames { get; }
        bool LogFirstConfig { get; }
        bool LogTsConfig { get; }
        bool EnableLocal { get; }
        bool EnableIgnore { get; }
        int JvmMemory { get; }
        bool ShowPrettierErrors { get; }
        string TsconfigNamesPattern { get; }
        string LintableFileExtensions { get; }
        int LintInterval { get; }
        void Initialize();
    }
}
