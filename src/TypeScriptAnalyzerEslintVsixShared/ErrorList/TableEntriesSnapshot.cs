using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableManager;
using TypeScriptAnalyzerEslintLinter;

namespace TypeScriptAnalyzerEslintVsix
{
    class TableEntriesSnapshot : TableEntriesSnapshotBase
    {
        private readonly List<LintingError> errors = new List<LintingError>();

        internal TableEntriesSnapshot(string filePath, string projectName, IEnumerable<LintingError> errors)
        {
            FilePath = filePath;
            ProjectName = projectName;
            this.errors.AddRange(errors);
        }

        public override int Count
        {
            get { return errors.Count; }
        }

        public string FilePath { get; }
        public string ProjectName { get; set; }
        public List<LintingError> Errors => errors;

        public override int VersionNumber { get; } = 1;

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            // NOT NECESSARILY CALLED ON UI THREAD
            content = null;

            if ((index >= 0) && (index < errors.Count))
            {
                if (columnName == StandardTableKeyNames.DocumentName)
                {
                    content = FilePath;
                }
                else if (columnName == StandardTableKeyNames.ErrorCategory)
                {
                    content = Vsix.Name;
                }
                else if (columnName == StandardTableKeyNames.Line)
                {
                    content = errors[index].LineNumber;
                }
                else if (columnName == StandardTableKeyNames.Column)
                {
                    content = errors[index].ColumnNumber;
                }
                else if (columnName == StandardTableKeyNames.Text)
                {
                    content = $"({errors[index].Provider.Name}) {errors[index].Message}";
                }
                else if (columnName == StandardTableKeyNames.ErrorSeverity)
                {
                    content = errors[index].ErrorType == LintingErrorType.Message ? __VSERRORCATEGORY.EC_MESSAGE :
                              errors[index].ErrorType == LintingErrorType.Warning ? __VSERRORCATEGORY.EC_WARNING : 
                                                                                     __VSERRORCATEGORY.EC_ERROR;
                }
                else if (columnName == StandardTableKeyNames.Priority)
                {
                    content = errors[index].ErrorType == LintingErrorType.Message ? vsTaskPriority.vsTaskPriorityLow :
                              errors[index].ErrorType == LintingErrorType.Warning ? vsTaskPriority.vsTaskPriorityMedium :
                                                                                     vsTaskPriority.vsTaskPriorityHigh;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = errors[index].IsBuildError ? ErrorSource.Build : ErrorSource.Other;
                }
                else if (columnName == StandardTableKeyNames.BuildTool)
                {
                    content = errors[index].Provider.Name;
                }
                else if (columnName == StandardTableKeyNames.ErrorCode)
                {
                    content = errors[index].ErrorCode;
                }
                else if (columnName == StandardTableKeyNames.ProjectName)
                {
                    content = ProjectName ?? "";
                }
                else if ((columnName == StandardTableKeyNames.ErrorCodeToolTip) || (columnName == StandardTableKeyNames.HelpLink))
                {
                    var error = errors[index];
                    string url;
                    if (!string.IsNullOrEmpty(error.HelpLink))
                    {
                        url = error.HelpLink;
                    }
                    else
                    {
                        url = string.Format("http://www.bing.com/search?q={0} {1}", errors[index].Provider.Name, errors[index].ErrorCode);
                    }

                    content = Uri.EscapeUriString(url);
                }
            }

            return content != null;
        }
    }
}
