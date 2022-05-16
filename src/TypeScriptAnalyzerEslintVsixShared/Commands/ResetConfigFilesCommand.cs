using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintVsix
{
    internal sealed class ResetConfigFilesCommand
    {
        private readonly Microsoft.VisualStudio.Shell.Package package;

        private ResetConfigFilesCommand(Microsoft.VisualStudio.Shell.Package package)
        {
            this.package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            Microsoft.Assumes.Present(commandService);

            var menuCommandID = new CommandID(PackageGuids.ToolsMenuGuid, PackageIds.ResetConfigFiles);
            var menuItem = new OleMenuCommand((s,e) => 
            {
                (this.package as AsyncPackage).JoinableTaskFactory.RunAsync(() => ResetConfigurationFilesAsync(s, e)).Forget();
            }, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static ResetConfigFilesCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        public static void Initialize(Microsoft.VisualStudio.Shell.Package package)
        {
            Instance = new ResetConfigFilesCommand(package);
        }

        private async System.Threading.Tasks.Task ResetConfigurationFilesAsync(object sender, EventArgs e)
        {
            await (package as AsyncPackage).JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                string msg = "This will reset the configuration for the TypeScript Analyzer (ESLint) to its defaults.\n\n" +
                    "This means restoring the default options in Tools/Options/TypeScript Analyzer and " +
                    $"restoring .eslintrc.js in {Linter.GetUserConfigFolder()}.\n\n" +
                    "Do you wish to continue?";
                var result = MessageBox.Show(msg, Vsix.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    await LinterService.CopyResourceFilesToUserProfileAsync(true);
                    Package.Settings.ResetSettings();
                    Package.Dte.StatusBar.Text = "TypeScript Analyzer (ESLint) configuration has been reset";
                }
            }
            catch (Exception ex)
            {
                await Logger.LogAndWarnAsync(ex);
            }

        }
    }
}
