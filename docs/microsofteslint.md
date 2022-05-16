# Incompatibility with Microsoft ESLint Implementation in Visual Studio

## Summary

There already is an ESLint implementation in Visual Studio, provided by Microsoft.  This has some [significant differences to the TypeScript Analyzer implementation.](microsofteslintdifferences.md)

For the Microsoft ESLint implementation to work it has to be explicitly enabled.

**The TypeScript Analyzer is incompatible with Microsoft's ESLint implementation.**  The exact reasons for this are given below.

If you have both the TypeScript Analyzer and Microsoft's ESLint implementation enabled then the Analyzer should work, but the Microsoft ESLint implementation may not work, or may give errors.  **It is best to disable one or other linter.**

Errors in the Error List are tagged `(eslint)` if they come from the TypeScript Analyzer, and `(ESLint)` if they come from Microsoft's implementation.

## Disabling the Linters

**You can disable Microsoft's ESLint implementation** by going to Tools/Options/Text Editor/'JavaScript/TypeScript'/Linting/General and clearing the Enable ESLint checkbox.

**You can disable the TypeScript Analyzer** by going to Tools/Options/TypeScriptAnalyzer/ESLint and setting Enable TypeScript Analyzer (ESLint) to False.  If you do this then the global .eslintrc file that the Microsoft linting uses is restored, and it should start working (see below for more details on this).

## Why the Linters are Incompatible

The specific reason we can't run both linters successfully simultaneously is that Microsoft's ESLint implementation creates an unfriendly .eslintrc configuration file in c:\Users\{username}.  

The TypeScript Analyzer will attempt to use this configuration file if the file being linted is in a subfolder and there's no local configuration file.  The default paths for new Visual Studio projects are subfolders of this path, of course.  

Since by default the .eslintrc file references plugins that are not shipped with the TypeScript Analyzer any lint will fail if it tries to use this configuration file.  

This means **the TypeScript Analyzer would fail in any new project if it didn't deal with the problem.**

## How the TypeScript Analyzer Handles This

For this reason the TypeScript Analyzer **disables the Microsoft ESLint implementation** if it has been enabled.  It does this naively by renaming c:\Users\{username}\.eslintrc to c:\Users\{username}\.eslintrctsabackup.  It checks if it needs to do this at the start of every linting run.

**If you disable the TypeScript Analyzer then the Microsoft ESLint implementation will be re-enabled** by renaming the file back to its original name.  You disable the TypeScript Analyzer by setting Tools/Options/TypeScript Analyzer/ESLint/'Enable TypeScript Analyzer (ESLint)' to False.