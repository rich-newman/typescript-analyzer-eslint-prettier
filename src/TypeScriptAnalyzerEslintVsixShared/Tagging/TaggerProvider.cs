using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace TypeScriptAnalyzerEslintVsix
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [ContentType("projection")]
    [TagType(typeof(IErrorTag))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [TextViewRole(PredefinedTextViewRoles.Analyzable)]
    public class TaggerProvider : IViewTaggerProvider
    {
        public TaggerProvider()
        {
            Package.TaggerProvider = this;
        }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (typeof(IErrorTag) != typeof(T) || textView.TextBuffer != buffer) return null;
            // We create a tagger for all text files, even non-lintable, in case they become lintable
            if (!taggerCache.ContainsKey(textView))
            {
                taggerCache.Add(textView, new Tagger(buffer));
                textView.Closed += (s, e) => taggerCache.Remove(textView);
            }
            return taggerCache[textView] as ITagger<T>;
        }

        // We key on ITextView (rather than filenames) because of renames with open files, when the text view remains
        // the same but the file name changes
        private readonly Dictionary<ITextView, Tagger> taggerCache = new Dictionary<ITextView, Tagger>();

        public void RefreshTags(bool isFixing = false)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            foreach (KeyValuePair<ITextView, Tagger> tagger in taggerCache)
                tagger.Value.RefreshTags(isFixing);
        }

        //[Conditional("DEBUG")]
        //private void DebugDumpTaggers()
        //{
        //    Debug.WriteLine("CURRENT TAGGERS:");
        //    foreach (KeyValuePair<ITextView, Tagger> tagger in taggerCache)
        //    {
        //        Debug.WriteLine(tagger.Value.FilePath);
        //    }
        //}

    }
}
