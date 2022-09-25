using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintVsix
{
    internal class LintActiveFileCommandBase
    {
        protected AsyncPackage package;
        public static LintActiveFileCommandBase Instance { get; protected set; }
        protected IServiceProvider ServiceProvider => package;

        internal static async System.Threading.Tasks.Task LintActiveFileAsync(bool fixErrors)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            try
            {
                FileListener.EventLintingSuspended = true;
                IWpfTextView wpfTextView = GetWpfView();
                string fileName = GetPath(wpfTextView);
                string text = wpfTextView.TextBuffer.CurrentSnapshot.GetText();
                if (fixErrors)
                {
                    // The fix updates the file on disk, and if we have unsaved changes VS will then ask, confusingly, if we want
                    // to reload from disk while showing the unchanged file.  So save the changes from the IDE before we start to avoid
                    // this. VS will reload automatically.
                    if (System.IO.File.ReadAllText(fileName) != text)
                    {
                        Package.Dte.ExecuteCommand("File.SaveSelectedItems");
                        await Task.Delay(Settings.SaveDelay);  
                    }
                }
                await LinterService.LintTextAsync(text, fileName, fixErrors);
            }
            catch (Exception ex) 
            { 
                await Logger.LogAndWarnAsync(ex); 
                TypeScriptAnalyzerEslintLinter.Linter.Server.Down(); 
            }
            finally
            {
                FileListener.EventLintingSuspended = false;
            }
        }

        private static IWpfTextView GetWpfView()
        {
            IVsTextManager textManager = (IVsTextManager)Instance.ServiceProvider.GetService(typeof(SVsTextManager));
            Microsoft.Assumes.Present(textManager);
            IComponentModel componentModel = (IComponentModel)Instance.ServiceProvider.GetService(typeof(SComponentModel));
            Microsoft.Assumes.Present(componentModel);
            IVsEditorAdaptersFactoryService editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            textManager.GetActiveView(1, null, out IVsTextView textViewCurrent);
            return editor.GetWpfTextView(textViewCurrent);
        }

        private static string GetPath(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            textView.TextBuffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer bufferAdapter);
            if (!(bufferAdapter is IPersistFileFormat persistFileFormat)) return null;
            persistFileFormat.GetCurFile(out string filePath, out _);
            return filePath;
        }

        private static string GetActiveCodeFilePath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IWpfTextView wpfTextView = GetWpfView();
            string fileName = GetPath(wpfTextView);
            return fileName;
        }

        protected void BeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                ((OleMenuCommand)sender).Visible = Package.Settings.ESLintEnable
                    && LintableFiles.IsLintableFile(GetActiveCodeFilePath());
            }
            catch (Exception ex)
            {
                Logger.LogAndWarn(ex);
            }
        }
    }
}
