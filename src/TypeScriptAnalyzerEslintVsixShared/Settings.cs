using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintVsix
{
    public class Settings : DialogPage, ISettings
    {
        public Settings()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            ESLintEnable = true;
            FixOnSave = false;
            ShowUnderlining = true;
            RunOnBuild = false;
            EnableIgnore = true;
            EnableLocal = true;
            LintableFileExtensions = DefaultLintableFileExtensions;
            JvmMemory = 0;
            EnableLogging = false;
            LogFileNames = true;
            LogFirstConfig = false;
            LogTsConfig = false;
            ShowPrettierErrors = true;
            UseTsConfig = false;
            TsconfigNamesPattern = DefaultTsconfigNamesPattern;
            LintInterval = 3000;
        }

        public override void ResetSettings()
        {
            SetDefaults();
            base.ResetSettings();
            base.SaveSettingsToStorage();
        }

        [Category("Basic")]
        [DisplayName("Enable TypeScript Analyzer (ESLint)")]
        [Description("Enable TypeScript Analyzer to lint using ESLint")]
        [DefaultValue(true)]
        public bool ESLintEnable { get; set; }

        [Category("Basic")]
        [DisplayName("Fix and format on Save")]
        [Description("If True, runs a Fix when an individual file is saved.  This also formats with Prettier if enabled.")]
        [DefaultValue(false)]
        public bool FixOnSave { get; set; }

        [Category("Basic")]
        [DisplayName("TypeScript: lint using tsconfig.json")]
        [Description("If True, for TypeScript lints using contents of tsconfig files in the solution, rather than individual .ts files. " +
            "This allows rules that need type information to be used.")]
        [DefaultValue(false)]
        public bool UseTsConfig { get; set; }

        [Category("Basic")]
        [DisplayName("Run on build")]
        [Description("Runs the analyzer before a build.  The build will fail if there are any ESLint errors in the Error List.")]
        [DefaultValue(false)]
        public bool RunOnBuild { get; set; }

        [Category("Basic")]
        [DisplayName("Show Prettier errors in Error List")]
        [Description("If False, Prettier formatting errors are not shown in the Error List or underlined, but are fixable. To disable " +
            "Prettier entirely edit the config .eslintrc.js and set prettierEnabled=false.")]
        [DefaultValue(true)]
        public bool ShowPrettierErrors { get; set; }

        [Category("Basic")]
        [DisplayName("Show red/green underlining")]
        [Description("If True, shows red/green underlining in code files for errors/warnings, and gives details on a hover.")]
        [DefaultValue(true)]
        public bool ShowUnderlining { get; set; }

        [Category("ESLint Configuration")]
        [DisplayName("Enable ignore")]
        [Description("If False .eslintignore files or ignorePatterns in your configuration" +
            " are not respected.  This is equivalent to using --no-ignore from the command line with eslint.")]
        [DefaultValue(true)]
        public bool EnableIgnore { get; set; }

        [Category("ESLint Configuration")]
        [DisplayName("Enable local node_modules")]
        [Description("If True, searches for local node_modules to use before using the ones installed with the Analyzer. " +
            "If False, ALWAYS uses the node_modules installed with the Analyzer.")]
        [DefaultValue(true)]
        public bool EnableLocal { get; set; }

        public const string DefaultLintableFileExtensions = "js,jsx,ts,tsx,mjs,cjs";

        [Category("ESLint Configuration")]
        [DisplayName("File extensions to lint")]
        [Description("Comma-separated list of file extensions the linter will handle.")]
        [DefaultValue(DefaultLintableFileExtensions)]
        public string LintableFileExtensions { get; set; }

        [Category("ESLint Configuration")]
        [DisplayName("JavaScript VM memory (MB)")]
        [Description("If 0 uses the default memory that node allocates for the JavaScript VM that runs ESLint. If ESLint" +
            " is running out of memory and crashing set a higher value here, e.g. 4096.")]
        [DefaultValue(0)]
        public int JvmMemory { get; set; }

        public const string DefaultTsconfigNamesPattern = "tsconfig.json,tsconfig.*.json";

        [Category("Extended")]
        [DisplayName("Pattern to match tsconfig file names")]
        [Description("If 'TypeScript: lint using tsconfig.json' is enabled, any files that match this pattern will be treated as " +
            "tsconfigs and used for linting.")]
        [DefaultValue(DefaultTsconfigNamesPattern)]
        public string TsconfigNamesPattern { get; set; }

        [Category("Extended")]
        [DisplayName("Interval stopping typing to lint (ms)")]
        [Description("The time after the last keystroke that the Analyzer will run a lint. Reduce this value if errors don't " +
            "update quickly enough. Set to -1 to disable linting after typing.")]
        [DefaultValue(3000)]
        public int LintInterval { get; set; }

        [Category("Logging")]
        [DisplayName("Enable logging")]
        [Description("If True, prints a log of each lint run in the Visual Studio Output window " +
            "when 'TypeScript Analyzer (ESLint, Prettier)' is selected in the dropdown.")]
        [DefaultValue(false)]
        public bool EnableLogging { get; set; }

        [Category("Logging")]
        [DisplayName("Log file names")]
        [Description("If True and logging is enabled, prints a log entry for every single file name being linted on every linting run.")]
        [DefaultValue(true)]
        public bool LogFileNames { get; set; }

        [Category("Logging")]
        [DisplayName("Log first config")]
        [Description("If True and logging is enabled, uses calculateConfigForFile to calculate the config (.eslintrc.js) for " +
            "the first file being linted and logs the results")]
        [DefaultValue(false)]
        public bool LogFirstConfig { get; set; }

        [Category("Logging")]
        [DisplayName("Log tsconfig.json contents")]
        [Description("If True, logging is enabled and lint with tsconfig.json files is true, logs the contents of any " +
            "tsconfig.json used to lint files")]
        [DefaultValue(false)]
        public bool LogTsConfig { get; set; }

        // If we have a changed value stored in the registry we need to set that at startup: we do this immediately after
        // the object is instantiated in Initialize
        private bool lastESLintEnable;
        private string lastLintableFileExtensions;
        private int lastJvmMemory;

        public void Initialize()
        {
            lastESLintEnable = ESLintEnable;
            lastLintableFileExtensions = LintableFileExtensions;
            lastJvmMemory = JvmMemory;
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            SetTsConfigPattern(); // Has to be before base.OnApply because it can reset the value to the default
            base.OnApply(e);
            if (!ESLintEnable && ErrorListDataSource.Instance.HasErrors()) ErrorListDataSource.Instance.CleanAllErrors();
            if (ESLintEnable && !ShowPrettierErrors && ErrorListDataSource.Instance.HasPrettierErrors())
                ErrorListDataSource.Instance.CleanPrettierErrors();
            if(ESLintEnable != lastESLintEnable)
            {
                lastESLintEnable = ESLintEnable;
                if (ESLintEnable)
                    MsEslintConfigFile.Disable();
                else
                    MsEslintConfigFile.Enable();
            }
            // Clears down errors that are in files that aren't in the lintable extensions list: we only
            // want to do this if we've actually changed the list, as we can show other errors with tsconfig (edge case)
            if (LintableFileExtensions != lastLintableFileExtensions)
            {
                lastLintableFileExtensions = LintableFileExtensions;
                LintableFiles.SetLintableFileExtensions(LintableFileExtensions);
                if(ESLintEnable) ErrorListDataSource.Instance.CleanNonLintableFileErrors();
            }
            // If we change the memory allocated to the JavaScript VM we need to restart the server
            if(JvmMemory != lastJvmMemory)
            {
                lastJvmMemory = JvmMemory;
                Linter.Server.Down();
            }
            Package.TaggerProvider?.RefreshTags();
            LintableFileContextProviderFactory.RefreshContexts();
        }

        private void SetTsConfigPattern()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            bool success = LintableFiles.SetTsConfigPattern(TsconfigNamesPattern);
            if (!success)
            {
                Logger.LogAndWarn($"An invalid tsconfig pattern was provided: {TsconfigNamesPattern}");
                Logger.Log("The pattern was reset to the default pattern.");
                TsconfigNamesPattern = DefaultTsconfigNamesPattern;
                LintableFiles.SetTsConfigPattern(TsconfigNamesPattern);
            }
        }
    }
}
