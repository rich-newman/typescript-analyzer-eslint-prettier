using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintVsix
{
    class Tagger : ITagger<LintingErrorTag> //, IDisposable
    {
        private ITextSnapshot currentTextSnapshot;

        // FilePath can change whilst the tagger is in use if we rename an open file, so don't key on it
        // document, buffer, and textView are all always the same object for a given tagger because we create a new tagger
        // if the view changes.
        internal string FilePath
        {
            get
            {
                if (currentTextSnapshot.TextBuffer.Properties.TryGetProperty("lint_filename_eslint", out string fileName)
                    && fileName != null) return fileName;
#if DEBUG
                throw new Exception("Tagger created but can't find file name");
                //return "";
#else
                return "";
#endif

            }
        }


        internal Tagger(ITextBuffer buffer)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            currentTextSnapshot = buffer.CurrentSnapshot;
            Debug.WriteLine($"Creating Tagger for {FilePath}, thread={Thread.CurrentThread.ManagedThreadId}");
            //this.TagsChanged += OnTagsChanged;
        }

        //private void OnTagsChanged(object sender, SnapshotSpanEventArgs e)
        //{
        //    Debug.WriteLine($"OnTagsChanged: text {e.Span.GetText()}, file={FilePath}, thread={Thread.CurrentThread.ManagedThreadId}");
        //}

        private bool isFixing = false;
        public void RefreshTags(bool isFixing)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            this.isFixing = isFixing;  // If we fix we need to calculate tags on the next snapshot
            tagSpans = null;
            Debug.WriteLine($"In RefreshTags calling TagsChanged, file={FilePath}, "
                + $"thread={Thread.CurrentThread.ManagedThreadId}");
            TagsChanged?.Invoke(this,
                new SnapshotSpanEventArgs(new SnapshotSpan(currentTextSnapshot, 0, currentTextSnapshot.Length)));
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<LintingErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (Package.Settings == null || !Package.Settings.ShowUnderlining
                || !Package.Settings.ESLintEnable) yield break;
            UpdateTagSpans(spans);
            foreach (ITagSpan<LintingErrorTag> tagSpan in tagSpans)
            {
                if (spans.IntersectsWith(tagSpan.Span))
                    yield return tagSpan;
            }
        }

        private void UpdateTagSpans(NormalizedSnapshotSpanCollection spans)
        {
            try
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
#if DEBUG
                Debug.WriteLine($"GetTags: File={FilePath}, New TextSnapshot version={spans[0].Snapshot.Version}, " +
                    $"old TextSnapshot version={currentTextSnapshot.Version}, thread={Thread.CurrentThread.ManagedThreadId}");
                if (!IsSpansValid(spans)) throw new Exception("Invalid spans in GetTags");
                if (IsTextBufferChanged(spans)) throw new Exception("Text buffer changed in Tagger");
#endif
                bool isTextSnapshotChanged = IsTextSnapshotChanged(spans);
                if (isTextSnapshotChanged) currentTextSnapshot = spans[0].Snapshot;
                if (isFixing && isTextSnapshotChanged) // Force recalc when fixing and the text snapshot changes
                {
                    isFixing = false;
                    CalculateTagSpans();
                }
                else if (tagSpans == null)
                    CalculateTagSpans();
                else if (isTextSnapshotChanged)
                    UpdateTagSpansForNewTextSnapshot();
            }
#if DEBUG
            catch (Exception ex)
#else
            catch (Exception)
#endif
            {
                // When we're fixing we force calls to CalculateTagSpans, and under certain circumstances it can run with the new
                // linting errors but the old text snapshot, which may throw errors.  GetTags is then called again with the
                // new snapshot and correctly updates the tags.  This works, but it also is horrible.  However making
                // the tagger reliably update on a fix was a mission, so a small price to pay.
                // So we can get errors here if we're fixing that aren't really a problem (isFixing will be true) 
#if DEBUG
                Logger.LogAndWarn(ex);
#endif
            }
        }

#if DEBUG
        private bool IsSpansValid(NormalizedSnapshotSpanCollection spans) =>
            (spans?.Count ?? 0) > 0 && spans[0].Snapshot?.TextBuffer != null;

        private bool IsTextBufferChanged(NormalizedSnapshotSpanCollection spans) =>
            spans[0].Snapshot.TextBuffer != currentTextSnapshot.TextBuffer;
#endif

        private bool IsTextSnapshotChanged(NormalizedSnapshotSpanCollection spans) =>
            spans[0].Snapshot != currentTextSnapshot;

        private List<ITagSpan<LintingErrorTag>> tagSpans = null;
        private void CalculateTagSpans()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Debug.WriteLine($"CalculateTagSpans, file={FilePath}, thread={Thread.CurrentThread.ManagedThreadId}");
            tagSpans = new List<ITagSpan<LintingErrorTag>>();
            if (!ErrorListDataSource.Snapshots.ContainsKey(FilePath)) return;
            List<LintingError> errors = ErrorListDataSource.Snapshots[FilePath].Errors;  // Immutable snapshot
            if (errors == null || errors.Count == 0) return;
            foreach (LintingError lintingError in errors)
            {
                LintingErrorTag lintingErrorTag = new LintingErrorTag(lintingError);
                SnapshotPoint startSnapshotPoint = CalculateSnapshotPoint(lintingError.LineNumber, lintingError.ColumnNumber);
                SnapshotPoint endSnapshotPoint = IsEndProvided(lintingError) ?
                    CalculateSnapshotPoint(lintingError.EndLineNumber.Value, lintingError.EndColumnNumber.Value) :
                    startSnapshotPoint;  // snapshot [1, 1) does include character at 1
                if (endSnapshotPoint < startSnapshotPoint) endSnapshotPoint = startSnapshotPoint;  // I don't trust ESLint
                SnapshotSpan snapshotSpan = new SnapshotSpan(startSnapshotPoint, endSnapshotPoint);
                ITagSpan<LintingErrorTag> tagSpan = new TagSpan<LintingErrorTag>(snapshotSpan, lintingErrorTag);
                tagSpans.Add(tagSpan);
            }
        }

        private SnapshotPoint CalculateSnapshotPoint(int lineNumber, int columnNumber) =>
            currentTextSnapshot.GetLineFromLineNumber(lineNumber).Start.Add(columnNumber);

        private bool IsEndProvided(LintingError lintingError) =>
            lintingError.EndColumnNumber != null && lintingError.EndLineNumber != null // Most parsers use null to indicate no end
            && !(lintingError.EndColumnNumber == 0 && lintingError.EndLineNumber == 0) // espree parser uses 0,0 to indicate no end
            && !(lintingError.EndColumnNumber == -1); // Markdown parser uses -1,0 to indicate no end
        // This mess is almost certainly a result of the anti-null brigade in action, but null is right here: it means no value

        public void UpdateTagSpansForNewTextSnapshot()
        {
            Debug.WriteLine($"UpdateTagSpansForNewTextSnapshot, file={FilePath}: " +
                $"New TextSnapshot version={currentTextSnapshot.Version}, thread={Thread.CurrentThread.ManagedThreadId}");
            List<ITagSpan<LintingErrorTag>> newTagSpans = new List<ITagSpan<LintingErrorTag>>();
            foreach (ITagSpan<LintingErrorTag> tagSpan in tagSpans)
            {
                SnapshotSpan newSpan = tagSpan.Span.TranslateTo(currentTextSnapshot, SpanTrackingMode.EdgeExclusive);
                if (newSpan != null) newTagSpans.Add(new TagSpan<LintingErrorTag>(newSpan, tagSpan.Tag));
            }
            tagSpans = newTagSpans;
        }
    }
}
