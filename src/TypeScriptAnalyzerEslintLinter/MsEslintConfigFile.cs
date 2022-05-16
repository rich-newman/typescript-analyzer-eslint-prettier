using System;
using System.IO;

namespace TypeScriptAnalyzerEslintLinter
{
    public static class MsEslintConfigFile
    {
        public static readonly string MsFileName = GetMsEslintConfigFile();
        private static readonly string backUpFileName = MsFileName + "tsabackup";

        public static void Disable()
        {
            if (File.Exists(MsFileName))
            {
                if (File.Exists(backUpFileName)) File.Delete(backUpFileName);
                File.Move(MsFileName, backUpFileName);
            }
        }

        public static void Enable()
        {
            if (File.Exists(backUpFileName) && !File.Exists(MsFileName))
                File.Move(backUpFileName, MsFileName);
        }

        private static string GetMsEslintConfigFile() =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".eslintrc");
    }
}
