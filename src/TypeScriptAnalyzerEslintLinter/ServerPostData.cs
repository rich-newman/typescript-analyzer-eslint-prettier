using System.Collections.Generic;
using Newtonsoft.Json;

namespace TypeScriptAnalyzerEslintLinter
{
    public class ServerPostData
    {
        [JsonProperty("configfolder")]
        public string ConfigFolder { get; set; }

        [JsonProperty("msconfigfile")]
        public string MsConfigFile { get; set; }

        [JsonProperty("files")]
        public IEnumerable<string> Files { get; set; }

        [JsonProperty("projectfiles")]
        public IEnumerable<string> ProjectFiles { get; set; }

        [JsonProperty("fixerrors")]
        public bool FixErrors { get; set; }

        [JsonProperty("dirname")]
        public string DirName { get; set; }

        [JsonProperty("enableignore")]
        public bool EnableIgnore { get; set; }

        [JsonProperty("ignorefile")]
        public string IgnoreFile { get; set; }

        [JsonProperty("debug")]
        public bool Debug { get; set; }

        [JsonProperty("enablelogging")]
        public bool EnableLogging { get; set; }

        [JsonProperty("logfilenames")]
        public bool LogFileNames { get; set; }

        [JsonProperty("logfirstconfig")]
        public bool LogFirstConfig { get; set; }

        [JsonProperty("logtsconfig")]
        public bool LogTsConfig { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

    }
}
