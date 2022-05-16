using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using Microsoft.VisualStudio.Shell;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintVsix
{
    internal sealed class EditConfigFilesCommand
    {
        private readonly Microsoft.VisualStudio.Shell.Package package;

        private EditConfigFilesCommand(Microsoft.VisualStudio.Shell.Package package)
        {
            this.package = package;

            List<CommandID> list = new List<CommandID>
            {
                new CommandID(PackageGuids.ToolsMenuGuid, PackageIds.EditESLint)
            };

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            Microsoft.Assumes.Present(commandService);

            foreach (var id in list)
            {
                var menuItem = new MenuCommand(EditConfig, id);
                commandService.AddCommand(menuItem);
            }
        }

        public static EditConfigFilesCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        public static void Initialize(Microsoft.VisualStudio.Shell.Package package)
        {
            Instance = new EditConfigFilesCommand(package);
        }

        private void EditConfig(object sender, EventArgs e)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var button = (MenuCommand)sender;
                string folder = Linter.GetUserConfigFolder();
                string fileName = GetFileName(button.CommandID.ID);
                string configFile = Path.Combine(folder, fileName);
                if (!string.IsNullOrEmpty(configFile) && File.Exists(configFile))
                    Package.Dte.ExecuteCommand("File.OpenFile", "\"" + configFile + "\"");
                else
                    Package.Dte.StatusBar.Text = $"Configuration file not found: {configFile}";
            }
            catch (Exception ex) { Logger.LogAndWarn(ex); }
        }

        private string GetFileName(int commandId)
        {
            switch (commandId)
            {
                case PackageIds.EditESLint:
                    return Linter.ConfigFileName;
            }
            return null;
        }
    }
}
