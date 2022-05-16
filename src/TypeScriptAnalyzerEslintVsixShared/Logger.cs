using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading.Tasks;

namespace TypeScriptAnalyzerEslintVsix
{
    public static class Logger
    {
        private static IVsOutputWindowPane pane;
        private static Package provider;
        private static string name;

        public static void Initialize(Package provider, string name)
        {
            Logger.provider = provider;
            Logger.name = name;
        }

        public static void Log(string message)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (string.IsNullOrEmpty(message)) return;
            try
            {
                if (EnsurePane())
                    pane.OutputStringThreadSafe(message + Environment.NewLine);
            }
            catch { }
        }

        public static async Task LogAsync(Exception ex)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            Log(ex);
        }

        public static void Log(Exception ex)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (ex != null) Log(ex.ToString());
            }
            catch { }
        }

        public static async Task LogAndWarnAsync(Exception ex)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            LogAndWarn(ex);
        }

        public static void LogAndWarn(Exception ex)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (ex != null) { 
                LogAndWarn(ex.Message + ex.StackTrace);
            }
        }

        public static async Task LogAndWarnAsync(string message, bool showWarning = true)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            LogAndWarn(message, showWarning);
        }

        public static void LogAndWarn(string message, bool showWarning = true)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                Log(message);
                if (showWarning)
                    Package.Dte.StatusBar.Text = 
                        "A TypeScript Analyzer (ESLint) error occurred. See Output window for more details.";
            }
            catch { }
        }

        private static bool EnsurePane()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            // during unit tests, provider is not set. Do not try to get pane then.
            if (pane == null && provider != null)
            {
                Guid guid = Guid.NewGuid();
                IVsOutputWindow output = provider.GetIVsOutputWindow();
                output.CreatePane(ref guid, name, 1, 1);
                output.GetPane(ref guid, out pane);
            }

            return pane != null;
        }
    }
}