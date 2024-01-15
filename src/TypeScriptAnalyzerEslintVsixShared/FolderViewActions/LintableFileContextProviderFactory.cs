#if !VS2022x
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Workspace;
using Microsoft.VisualStudio.Workspace.Settings;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintVsix
{
    [ExportFileContextProvider(ProviderType, LintableFileContextType)]
    class LintableFileContextProviderFactory : IWorkspaceProviderFactory<IFileContextProvider>
    {
        // A 'FileContextProviderFactory.  One noun in a class name, Microsoft, or two if you're stretching it.
        private const string ProviderType = "7DA597D4-E125-43A6-8779-BF706028CCB2";
        public const string LintableFileContextType = "44098DB3-5D6A-4462-A160-DDF409D4A315";
        private static IWorkspace WorkspaceContext;

        public IFileContextProvider CreateProvider(IWorkspace workspaceContext)
        {
            //Package.Jtf.RunAsync(async () => 
            //    await Logger.LogAndWarnAsync($"In LintableFileContextProviderFactory.CreateProvider, workspace={workspaceContext.Location}", false)).Forget();
            WorkspaceContext = workspaceContext;
            return new LintableFileContextProvider();
        }

        // Nasty hack to force GetContextsForFileAsync to fire when we rightclick a Solution Explorer menu item after a Tools/Options
        // settings change.
        // Without this we can change the list of lintable file extensions, or even disable the Analyzer, in Tools/Options
        // and the extension won't necessarily notice the next time you rightclick an item.
        // There's almost certainly a better way of doing this, but I tried decompiling and couldn't find it in a sea of interfaces,
        // and the documentation is nonexistent.
        public static void RefreshContexts()
        {
            if (WorkspaceContext == null) return;
            Package.Jtf.RunAsync(() => RefreshContextsAsync()).Forget();
        }

        private static async Task RefreshContextsAsync()
        {
            try
            {
                using (IWorkspaceSettingsPersistance persistence = await WorkspaceContext.GetSettingsManager().GetPersistanceAsync(true))
                {
                    IWorkspaceSettingsSourceWriter writer = await persistence.GetWriter(SettingsTypes.Generic);
                    writer.SetProperty("Dummy", true);
                }
            }
            catch (Exception ex) { await Logger.LogAndWarnAsync(ex); }
        }

        private class LintableFileContextProvider : IFileContextProvider
        {

            public async Task<IReadOnlyCollection<FileContext>> GetContextsForFileAsync(string filePath,
                CancellationToken cancellationToken)
            {
                var fileContexts = new List<FileContext>();
                if (Package.Settings.ESLintEnable &&
                    (LintableFiles.IsLintableFile(filePath) || System.IO.Directory.Exists(filePath) ||
                    (Package.Settings.UseTsConfig && LintableFiles.IsLintableTsconfig(filePath))))
                {
                    fileContexts.Add(new FileContext(
                        new Guid(ProviderType),
                        new Guid(LintableFileContextType),
                        filePath,
                        Array.Empty<string>()));
                }
                return await Task.FromResult(fileContexts.ToArray());
            }
        }
    }
}
#endif