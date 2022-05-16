using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TypeScriptAnalyzerEslintLinter
{
    public class Linter
    {
        public Linter(ISettings settings, bool fixErrors = false, Func<string, bool, Task> logger = null, string localInstallPath = "")
        {
            this.Settings = settings;
            FixErrors = fixErrors;
            this.logger = logger ?? ((s, e) => Task.CompletedTask);
            this.localInstallPath = localInstallPath;
        }

        public static NodeServer Server { get; } = new NodeServer();
        private readonly Func<string, bool, Task> logger;
        private readonly string localInstallPath;

        public readonly string Name = "eslint";
        public static readonly string ConfigFileName = ".eslintrc.js";

        private ISettings Settings { get; }
        private bool FixErrors { get; }

        private LintingResult Result { get; set; }

        public async Task<LintingResult> LintAsync(string[] files, string[] projectFiles,
            string text = null, bool isCalledFromBuild = false)
        {
            Result = new LintingResult(files);
            if (!Settings.ESLintEnable || !(files.Any() || projectFiles.Any())) return Result;
            ServerPostData postData = CreatePostData(files, projectFiles, text);
            string output = await Server.CallServerAsync(Name, postData, logger, Settings.JvmMemory);
            if (!string.IsNullOrEmpty(output)) await ParseErrorsAsync(output, isCalledFromBuild);
            return Result;
        }

        private ServerPostData CreatePostData(string[] files, string[] projectFiles, string text)
        {
            ServerPostData postData = new ServerPostData
            {
                ConfigFolder = GetBaseConfigFolder(),
                MsConfigFile = MsEslintConfigFile.MsFileName,
                Files = files,
                ProjectFiles = projectFiles,
                FixErrors = FixErrors,
                DirName = localInstallPath,
                EnableLogging = Settings.EnableLogging,
                LogFileNames = Settings.LogFileNames,
                LogFirstConfig = Settings.LogFirstConfig,
                LogTsConfig = Settings.LogTsConfig,
                EnableIgnore = Settings.EnableIgnore,
                IgnoreFile = GetIgnoreFile(files.Length > 0 ? files[0] : projectFiles[0])
            };
            if (text != null) postData.Text = text;
#if DEBUG
            postData.Debug = true;
#endif
            return postData;
        }

        private string GetBaseConfigFolder()
        {
            string configFolder = GetUserConfigFolder();
            return Directory.Exists(configFolder) ? configFolder : "";
        } 

        private string GetIgnoreFile(string filePath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filePath));
            while (directoryInfo != null)
            {
                string ignoreFile = Path.Combine(directoryInfo.FullName, ".eslintignore");
                if (File.Exists(ignoreFile)) return ignoreFile;
                directoryInfo = directoryInfo.Parent;
            }
            return "";
        }

        public static string GetUserConfigFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TypeScriptAnalyzerConfig");
        }

        private async Task ParseErrorsAsync(string output, bool isCalledFromBuild)
        {
            JObject obj;
            try
            {
                // Exceptions from the web server are first handled by the WebException catch block in NodeServer.cs/CallServerAsync
                // This deserializes the error object created on the server, and logs what it says
                // It RETURNS the string to display in the Error List rather than a return object, which is what output will be here
                // So we need to ensure it gets turned into an Error List error in this code: we do this rather clunkily by attempting
                // to parse it and catching the exception if that fails.
                obj = JObject.Parse(output);
            }
            catch (JsonReaderException)
            {
                LintingError le = new LintingError(Name, 0, 0, LintingErrorType.Error, Name)
                {
                    Message = output,
                    Provider = this
                };
                Result.Errors.Add(le);
                Result.HasVsErrors = true;
                return;
            }
            string logOutput = obj["log"]?.Value<string>() ?? "";
            if (logOutput != "")await  logger(logOutput, false);
            JArray array = obj["result"]?.Value<JArray>() ?? new JArray();
            await ParseEslintErrorsAsync(array, isCalledFromBuild);
        }

        private async Task ParseEslintErrorsAsync(JArray array, bool isCalledFromBuild)
        {
            bool hasVSErrors = false;
            foreach (JObject obj in array)
            {
                string fileName = obj["filePath"]?.Value<string>().Replace("/", "\\");
                if (string.IsNullOrEmpty(fileName)) continue;
                JArray messages = obj["messages"]?.Value<JArray>() ?? new JArray();
                foreach (JToken message in messages)
                {
                    try
                    {
                        string errorCode = GetPropertyValue<string>(message, "ruleId");
                        if (!Settings.ShowPrettierErrors && errorCode == "prettier/prettier") continue;
                        int lineNumber = GetPropertyValue(message, "line", valueIfNull: 1) - 1;  // eslint line numbers out by one
                        int columnNumber = GetPropertyValue(message, "column", valueIfNull: 1) - 1;
                        int messageSeverity = GetPropertyValue<int>(message, "severity");

                        // File ignored messages have severity 1, no error code, but are not VS warnings (which require action)
                        // Not sure if this is right: the only other way to test is on the string message though
                        if (messageSeverity == 1 && errorCode == null) messageSeverity = 0;
                        LintingErrorType lintingErrorType = (LintingErrorType)messageSeverity;
                        LintingError le = new LintingError(fileName, lineNumber, columnNumber, lintingErrorType, errorCode);
                        if (!Result.Errors.Contains(le))
                        {
                            le.Message = GetPropertyValue<string>(message, "message");
                            le.EndLineNumber = GetPropertyValue(message, "endLine", valueIfNull: 1) - 1;
                            le.EndColumnNumber = GetPropertyValue(message, "endColumn", valueIfNull: 1) - 1;

                            le.HelpLink = GetHelpLink(le.ErrorCode);
                            le.Provider = this;
                            le.IsBuildError = isCalledFromBuild;
                            Result.Errors.Add(le);
                        }
                        hasVSErrors = hasVSErrors || (lintingErrorType == LintingErrorType.Error);
                    }
                    catch (Exception e) {
                        string logMessage = $"A linting error failed to parse.\nThe linting error was:\n{message}\n" +
                            $"The exception thrown was:\n{e.Message}\nat\n{e.StackTrace}";
                        await logger(logMessage, true);
                    }
                }
            }
            Result.HasVsErrors = hasVSErrors;
        }

        // JSDoc rules don't always omit properties with no values for line/column numbers, they can include them with value null
        // JSON.NET parses these as properties with value a special JToken called JTokenType.Null
        // This method handles the issue - we want nulls treated as nulls
        private T GetPropertyValue<T>(JToken message, string propertyName, T valueIfNull = default)
        {
            JToken token = message[propertyName];
            return token == null || token.Type == JTokenType.Null ? valueIfNull : token.Value<T>();
        }

        // TODO move the error to rule documentation mappings to some form of editable file/screen (although not easy).
        private string GetHelpLink(string errorCode)
        {
            return errorCode == null ?
                "" :
            errorCode.StartsWith("react/") ?
                "https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/" +
                $"{RuleUrlName(errorCode)}" :
            errorCode.StartsWith("@typescript-eslint/") ?
                "https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/" +
                $"{RuleUrlName(errorCode)}" :
            errorCode.StartsWith("prettier/prettier") ?
                "https://prettier.io/docs/en/options.html" :
            errorCode.StartsWith("node/") ?
                "https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/" +
                $"{RuleUrlName(errorCode)}" :
            errorCode.StartsWith("import/") ?
                "https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/" +
                $"{RuleUrlName(errorCode)}" :
            errorCode.StartsWith("promise/") ?
                "https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/" +
                $"{RuleUrlName(errorCode)}" :
            errorCode.StartsWith("@angular-eslint/") ?
                "http://codelyzer.com/rules/" + $"{RuleUrlName(errorCode, "/")}" :
            errorCode.StartsWith("jsdoc/") ?
                "https://www.npmjs.com/package/eslint-plugin-jsdoc#" + $"{RuleUrlName(errorCode, "")}" :
            errorCode == "md/remark" ?
                "https://github.com/remarkjs/remark-lint#rules" :
                $"https://eslint.org/docs/rules/{errorCode}";
        }

        private string RuleUrlName(string errorCode, string extension = ".md")
        {
            int position = errorCode.IndexOf('/') + 1;
            return (position == errorCode.Length) ? "" : errorCode.Substring(position, errorCode.Length - position) + extension;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Linter lb))
                return false;
            else
                return Name.Equals(lb.Name);
        }

        public override int GetHashCode() => Name.GetHashCode();

        public override string ToString() => Name;
    }
}
