namespace TypeScriptAnalyzerEslintLinter
{
    public enum LintingErrorType
    {
        // Make the values explicit because we use them to convert from ESLint message severity
        Message = 0,
        Warning = 1,
        Error = 2
    }

    public class LintingError
    {
        public LintingError(string fileName, int lineNumber, int columnNumber, LintingErrorType errorType, string errorCode, string message)
        {
            FileName = fileName;
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
            ErrorType = errorType;
            ErrorCode = errorCode;
            Message = message;
        }

        public Linter Provider { get; set; }
        public string FileName { get; }
        public string Message { get; set; }
        public int LineNumber { get; }
        public int ColumnNumber { get; }
        public int? EndLineNumber { get; set; }
        public int? EndColumnNumber { get; set; }
        public LintingErrorType ErrorType { get; } = LintingErrorType.Error;
        //public bool IsError { get; } = true;
        public string ErrorCode { get; }
        public string HelpLink { get; set; }
        public bool IsBuildError { get; set; }

        public override string ToString()
        {
            return Message;
        }

        protected bool Equals(LintingError other)
        {
            return string.Equals(FileName, other.FileName) && LineNumber == other.LineNumber && ColumnNumber == other.ColumnNumber &&
                ErrorType == other.ErrorType && string.Equals(ErrorCode, other.ErrorCode) && string.Equals(Message, other.Message);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LintingError)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FileName != null ? FileName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LineNumber;
                hashCode = (hashCode * 397) ^ ColumnNumber;
                hashCode = (hashCode * 397) ^ ErrorType.GetHashCode();
                hashCode = (hashCode * 397) ^ (ErrorCode != null ? ErrorCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
