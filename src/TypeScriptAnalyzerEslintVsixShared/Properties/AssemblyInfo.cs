using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("TypeScriptAnalyzerEslintVsix")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Rich Newman")]
[assembly: AssemblyProduct("TypeScript Analyzer (ESLint)")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion(TypeScriptAnalyzerEslintLinter.Constants.VERSION)]
[assembly: AssemblyFileVersion(TypeScriptAnalyzerEslintLinter.Constants.VERSION)]

#if VS2022
[assembly: InternalsVisibleTo("TypeScriptAnalyzerEslintTest64")]
#else
[assembly: InternalsVisibleTo("TypeScriptAnalyzerEslintTest")]
#endif
