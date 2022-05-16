/*
 * Code used to test whether the various hooks set up for text buffers and other other objects were causing them to memory leak.
 * When fully set up it adds two menu options to the Solution Explorer context menu. They are only there because it's easy.
 * 
 * 'Set Weak Reference' sets a weak reference to the underlying text buffer of the currently active document.
 * 'Display Weak Reference' does a garbage collection and then shows whether that reference is still alive.
 * 
 * Clearly if you close the document and do Display Weak Reference then eventually you expect the weak reference to die, which it does.
 * It doesn't do it immediately, it needs a few calls, and if I'm honest I'm not sure why.  You see the same behavior in an empty VSIX
 * project.  Also in an empty project sometimes the reference won't die until the project is closed, so it's you, MS, not me.
 * If you set up a strong reference to a text buffer in, say, the FileListener, then this code will correctly show that 
 * the weak reference to the text buffer won't die. I think it's best to test with a Release build without a debugger attached.
 * 
 * To enable this code:
 * - set a GCTESTING conditional compilation symbol on the build tab of TypeScriptAnalyzerEslintVsix or TypeScriptAnalyzerEslintVsix64 
 *   depending on your version of Visual Studio.
 * - In VsCommandTable.vsct XML add the code below to the <Buttons>, <!-- Context menu --> section, 
 *   after the button for the CleanErrorsCommand:
 *   
 * 		<Button guid="SolutionExplorerGuid" id="SetWeakReferenceCommand" priority="0x0250" type="Button">
			<Parent guid="SolutionExplorerGuid" id="SolutionExplorerContextMenuGroup" />
			<CommandFlag>DynamicVisibility</CommandFlag>
			<CommandFlag>DefaultInvisible</CommandFlag>
			<Strings>
				<ButtonText>Set Weak Reference</ButtonText>
			</Strings>
		</Button>
		<Button guid="SolutionExplorerGuid" id="DisplayWeakReferenceCommand" priority="0x0300" type="Button">
			<Parent guid="SolutionExplorerGuid" id="SolutionExplorerContextMenuGroup" />
			<Icon guid="ImageCatalogGuid" id="ClearWindowContent" />
			<CommandFlag>DynamicVisibility</CommandFlag>
			<CommandFlag>DefaultInvisible</CommandFlag>
			<Strings>
				<ButtonText>Display Weak Reference</ButtonText>
			</Strings>
		</Button>
 * 
 * - In VsCommandTable.vsct XML add the code below to the <Symbols>, <!-- Context menu --> section, 
 *   after the IDSymbol for the CleanErrorsCommand:
 *   
 *   	<IDSymbol name="SetWeakReferenceCommand" value="0x0250" />
		<IDSymbol name="DisplayWeakReferenceCommand" value="0x0300" />
 *
 * - If you have Mads' VSIX Synchronizer extension installed it will regenerate the VsCommandTable.cs from this code.  If not you need to
 *   add the code below to the PackageIds class:
 *   
 *      public const int SetWeakReferenceCommand = 0x0250;
        public const int DisplayWeakReferenceCommand = 0x0300;
 * 
 * - The output, showing which buffer has a weak reference if you click Set Weak Reference, and the status if you click Display Weak
 *   reference, is in the regular Visual Studio Output window on the TypeScript Analyzer dropdown tab.  It's possible to set the weak
 *   reference to the actual Output window if it's active when you click the menu option, in which case it will display 'WEAKREFERENCE
 *   set to Output: ...
 */

#if GCTESTING
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;

namespace TypeScriptAnalyzerEslintVsix
{
    internal sealed class SetWeakReferenceCommand
    {
        internal static WeakReference weakReference;

        private readonly Package package;
        private SetWeakReferenceCommand(Package package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            Microsoft.Assumes.Present(commandService);
            var menuCommandID = new CommandID(PackageGuids.SolutionExplorerGuid, PackageIds.SetWeakReferenceCommand);
            var menuItem = new OleMenuCommand(SetWeakReference, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static SetWeakReferenceCommand Instance { get; private set; }
        private IServiceProvider ServiceProvider => package;
        public static void Initialize(Package package) { Instance = new SetWeakReferenceCommand(package); }

        private void SetWeakReference(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IWpfTextView wpfTextView = GetWpfView();
            weakReference = new WeakReference(wpfTextView.TextBuffer);
            string path = GetPath(wpfTextView);
            string message = path != null ? $"file {path}" : wpfTextView.TextBuffer.ToString();
            Logger.LogAndWarn($"WEAKREFERENCE set to {message}", false);
        }

        private IWpfTextView GetWpfView()
        {
            IVsTextManager textManager = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
            Microsoft.Assumes.Present(textManager);
            IComponentModel componentModel = (IComponentModel)this.ServiceProvider.GetService(typeof(SComponentModel));
            Microsoft.Assumes.Present(componentModel);
            IVsEditorAdaptersFactoryService editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            textManager.GetActiveView(1, null, out IVsTextView textViewCurrent);
            return editor.GetWpfTextView(textViewCurrent);
        }

        public static string GetPath(IWpfTextView textView)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            textView.TextBuffer.Properties.TryGetProperty(typeof(IVsTextBuffer), out IVsTextBuffer bufferAdapter);
            if (!(bufferAdapter is IPersistFileFormat persistFileFormat)) return null;
            persistFileFormat.GetCurFile(out string filePath, out _);
            return filePath;
        }

    }

    internal class DisplayWeakReferenceCommand
    {
        private readonly Package package;
        private DisplayWeakReferenceCommand(Package package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            Microsoft.Assumes.Present(commandService);
            var menuCommandID = new CommandID(PackageGuids.SolutionExplorerGuid, PackageIds.DisplayWeakReferenceCommand);
            var menuItem = new OleMenuCommand(DisplayWeakReference, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static DisplayWeakReferenceCommand Instance { get; private set; }
        private IServiceProvider ServiceProvider => package;
        public static void Initialize(Package package) { Instance = new DisplayWeakReferenceCommand(package); }

        private void DisplayWeakReference(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            string message = SetWeakReferenceCommand.weakReference == null ? "null" :
                SetWeakReferenceCommand.weakReference.IsAlive ? "alive" : "dead";
            Logger.LogAndWarn($"WEAKREFERENCE is {message}", false);
        }
    }
}
#endif