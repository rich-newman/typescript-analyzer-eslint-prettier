using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;
using Task = System.Threading.Tasks.Task;

namespace TypeScriptAnalyzerEslintVsix
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class FileListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                IWpfTextView wpfTextView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
                if (TextDocumentFactoryService.TryGetTextDocument(wpfTextView.TextDataModel.DocumentBuffer, out ITextDocument textDocument))
                {
                    if (Package.Settings == null)
                        Package.UnhandledStartUpFiles.Add(new Tuple<IWpfTextView, ITextDocument>(wpfTextView, textDocument));
                    else
                    {
                        Package.Jtf.RunAsync(() => OnFileOpenedAsync(wpfTextView, textDocument)).Forget();
                    }
                }
            }
            catch (Exception ex) { Logger.LogAndWarn(ex); }
        }

        public static async Task OnFileOpenedAsync(IWpfTextView wpfTextView, ITextDocument textDocument)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            try
            {
                // Called on UI thread
                if (wpfTextView == null || textDocument == null) return; // || !IsInSolution(textDocument.FilePath)) return;
                if (!LintableFiles.IsRootedAndExists(textDocument.FilePath)) return;  // Is the filepath valid and does the file exist
                AddTextViewToBufferList(wpfTextView);
                wpfTextView.Properties.AddProperty("lint_document_eslint", textDocument);
                wpfTextView.Closed += TextviewClosed;
                // It's possible to open a second textview on the same underlying file/buffer - we exit if this is happening
                if (wpfTextView.TextBuffer.Properties.TryGetProperty("lint_filename_eslint", out string fileName) && fileName != null) return;
                if (!wpfTextView.TextBuffer.Properties.ContainsProperty("timer"))
                    wpfTextView.TextBuffer.Properties.AddProperty("timer", 
                        new Timer(textBufferChangedTimerCallback, wpfTextView.TextBuffer, Timeout.Infinite, Timeout.Infinite));
                wpfTextView.TextBuffer.Properties.AddProperty("lint_filename_eslint", textDocument.FilePath);
                textDocument.FileActionOccurred += fileActionOccurredEventHandler; // Hook the event whether lintable or not: it may become lintable
                wpfTextView.TextBuffer.Changed += textBufferChangedEventHandler;
                if (!LintableFiles.IsLintableFile(textDocument.FilePath)) return;
                // Don't run linter again if error list already contains errors for the file
                if (!ErrorListDataSource.Instance.HasErrors(textDocument.FilePath))
                {
                    //Logger.Log("File opened, linting: " + textDocument.FilePath);
                    await LinterService.LintTextAsync(wpfTextView.TextSnapshot.GetText(), textDocument.FilePath);
                }
            }
            catch (Exception ex) { await Logger.LogAndWarnAsync(ex); }
        }

        // This is clunky.  Is there a better way of tracking text views for a buffer?
        private static void AddTextViewToBufferList(IWpfTextView wpfTextView)
        {
            if (wpfTextView.TextBuffer.Properties.TryGetProperty("textview_list_eslint", out List<IWpfTextView> textViewList)
                && textViewList != null)
                textViewList.Add(wpfTextView);
            else
                wpfTextView.TextBuffer.Properties.AddProperty("textview_list_eslint", new List<IWpfTextView> { wpfTextView });
        }

        // Returns true if list is empty after the removal
        private static bool RemoveTextViewFromBufferList(IWpfTextView wpfTextView)
        {
            List<IWpfTextView> textViewList = (List<IWpfTextView>)wpfTextView.TextBuffer.Properties["textview_list_eslint"];
            textViewList.Remove(wpfTextView);
            return textViewList.Count == 0;
        }

        private static void TextviewClosed(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                IWpfTextView wpfTextView = (IWpfTextView)sender;
                wpfTextView.Closed -= TextviewClosed;
                bool bufferClosing = RemoveTextViewFromBufferList(wpfTextView);
                if (!bufferClosing) return;
                DisposeTimer(wpfTextView.TextBuffer);
                wpfTextView.TextBuffer.Changed -= textBufferChangedEventHandler;
                if (wpfTextView.Properties.TryGetProperty("lint_document_eslint", out ITextDocument textDocument))
                    textDocument.FileActionOccurred -= fileActionOccurredEventHandler;
                //filePathToBufferMap.Remove(textDocument.FilePath);
                if (wpfTextView.TextBuffer.Properties.TryGetProperty("lint_filename_eslint", out string fileName))
                {
                    ErrorListDataSource.Instance.CleanErrors(new[] { fileName });
                    // If we make a change in a file and then close it before we save, then we get a dialog asking if we want to save.
                    // If we click 'yes' then OnFileActionOccurred fires because we've saved and then TextviewClosed fires because
                    // we closed.
                    // OnFileActionOccurred calls the linter (await LintFileAsync), which calls the linter web service.  This is a
                    // genuine async call and it frees the UI thread and puts a continuation on the message loop.
                    // Because we call OnFileActionOccurred async the free UI thread allows TextviewClosed to be called on the UI thread
                    // while the linter is running.  TextviewClosed will clear the errors down, but then the continuation of
                    // OnFileActionOccurred will put them back when the linting run completes unless we stop it.
                    // If you're still with me, this means we need a mechanism to stop us showing errors for files that were closed
                    // since our linting run started (and no new linting run has been started).
                    // This is FilesClosedWhileLinting in the ErrorListDataSource.
                    ErrorListDataSource.FilesClosedWhileLinting.Add(fileName);
                }
            }
            catch (Exception ex) {
                ThreadHelper.ThrowIfNotOnUIThread();
                Logger.LogAndWarn(ex); 
            }
        }

        private static readonly EventHandler<TextContentChangedEventArgs> textBufferChangedEventHandler =
            (s, e) => Package.Jtf.RunAsync(() => OnTextBufferChangedAsync(s, e));

        private static async Task OnTextBufferChangedAsync(object sender, TextContentChangedEventArgs e)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            try
            {
                if (!Package.Settings.ESLintEnable || Package.Settings.LintInterval == -1) return;
                ITextBuffer textBuffer = (ITextBuffer)sender;
                // TODO we don't really want to be doing this check on every keystroke in every window unless we're actually linting
                // Note that if we take it out at the moment we try to lint nonlintable files since we set up a timer for everything
                // in case it's added to the lintable file extensions list
                if (textBuffer.Properties.TryGetProperty("lint_filename_eslint", out string fileName) 
                    && !LintableFiles.IsLintableFile(fileName)) return;
                if (textBuffer.Properties.TryGetProperty("timer", out Timer timer))
                    // Every time we change the text buffer reset the timer to lint after another LintInterval milliseconds have passed
                    // So we only lint if we stop typing for LintInterval milliseconds
                    timer.Change(Package.Settings.LintInterval, Timeout.Infinite);
            }
            catch (Exception ex) { await Logger.LogAndWarnAsync(ex); }
        }

        private static void DisposeTimer(ITextBuffer textBuffer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (textBuffer != null && textBuffer.Properties.TryGetProperty("timer", out Timer timer))
                timer.Dispose();
        }

        private static void CancelTimer(ITextBuffer textBuffer)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (textBuffer != null && textBuffer.Properties.TryGetProperty("timer", out Timer timer))
                timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private static readonly TimerCallback textBufferChangedTimerCallback =
            s => Package.Jtf.RunAsync(() => OnTextBufferChangedTimerCallbackAsync(s));

        private static async Task OnTextBufferChangedTimerCallbackAsync(object state)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            try
            {
                if (!Package.Settings.ESLintEnable) return;
                ITextBuffer textBuffer = (ITextBuffer)state;
                if (textBuffer.Properties.TryGetProperty("lint_filename_eslint", out string fileName))
                {
                    System.Diagnostics.Debug.WriteLine($"Running lint on timer for file {fileName}");
                    await LinterService.LintTextAsync(textBuffer.CurrentSnapshot.GetText(), fileName);
                }
            }
            catch (Exception ex) { await Logger.LogAndWarnAsync(ex); }
        }

        private static readonly EventHandler<TextDocumentFileActionEventArgs> fileActionOccurredEventHandler =
            (s, e) => Package.Jtf.RunAsync(() => OnFileActionOccurredAsync(s, e));

        private static async Task OnFileActionOccurredAsync(object sender, TextDocumentFileActionEventArgs e)
        {
            await Package.Jtf.SwitchToMainThreadAsync();
            try
            {
                if (EventLintingSuspended) return;
                ITextBuffer textBuffer = (sender as ITextDocument)?.TextBuffer;
                // Called from UI thread: file save etc
                // Ignore settings with tsconfigs enabled are used for folders we don't want to look for tsconfigs in, not for individual
                // files we may or may not want to lint
                if ((e.FileActionType == FileActionTypes.ContentSavedToDisk || e.FileActionType == FileActionTypes.DocumentRenamed) &&
                    LintableFiles.IsLintableFile(e.FilePath))
                {
                    //Logger.Log("File action, linting: " + e.FilePath + ", action: " + e.FileActionType);
                    CancelTimer(textBuffer);
                    bool fix = (e.FileActionType == FileActionTypes.ContentSavedToDisk) 
                               && Package.Settings.FixOnSave && !FixOnSaveSuspended;
                    if (fix) await Task.Delay(Settings.SaveDelay);  
                    await LinterService.LintTextAsync(textBuffer.CurrentSnapshot.GetText(), e.FilePath, fixErrors: fix);
                }
                // Not a lintable file, has been renamed or saved, may have existing errors (was lintable before a config change)
                else if (e.FileActionType == FileActionTypes.ContentSavedToDisk || e.FileActionType == FileActionTypes.DocumentRenamed)
                {
                    CancelTimer(textBuffer);
                    ErrorListDataSource.Instance.CleanErrors(new[] { e.FilePath });
                    Package.TaggerProvider?.RefreshTags();
                }
                if (e.FileActionType == FileActionTypes.DocumentRenamed)
                {
                    CleanErrorsForOldFileName(sender, e);
                }
            }
            catch (Exception ex) { await Logger.LogAndWarnAsync(ex); }
        }

        private static void CleanErrorsForOldFileName(object sender, TextDocumentFileActionEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ITextBuffer textBuffer = (sender as ITextDocument)?.TextBuffer;
            if (textBuffer != null && textBuffer.Properties.TryGetProperty("lint_filename_eslint", out string oldFileName))
            {
                ErrorListDataSource.Instance.CleanErrors(new[] { oldFileName });
                textBuffer.Properties["lint_filename_eslint"] = e.FilePath;
            }
        }

        public static bool EventLintingSuspended = false; // Must be accessed from UI thread
        // We explicitly prevent fixing on a save during a build. This prevents a fix if the build itself
        // saves a file, and if our user saves, both of which are confusing re what gets built.
        public static bool FixOnSaveSuspended = false; // Must be accessed from UI thread 
    }
}