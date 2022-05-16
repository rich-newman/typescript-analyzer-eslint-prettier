using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintVsix
{
    /// <summary>
    /// Wrapper for a LintingError that provides additional information that 
    /// allows underlining ('tagging') in the code window
    /// </summary>
    public class LintingErrorTag : IErrorTag
    {
        public string ErrorType { get; }
        public object ToolTipContent { get; }

        internal LintingErrorTag(LintingError lintingError)
        {
            ErrorType = lintingError.ErrorType == LintingErrorType.Message ? "hinted suggestion" :
                        lintingError.ErrorType == LintingErrorType.Warning ? PredefinedErrorTypeNames.Warning :
                                                                             PredefinedErrorTypeNames.SyntaxError ;
            ToolTipContent = $"({lintingError.Provider.Name}) {lintingError.Message} ({lintingError.ErrorCode})";
        }
    }
}
