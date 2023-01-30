# Differences Between the TypeScript Analyzer (ESLint) and Microsoft Implementation of ESLint

 The TypeScript Analyzer extension has some different features to the Microsoft implementation of ESLint that exists in Visual Studio:

- TypeScript Analyzer allows you to fix errors for an entire project or solution.  In the latest versions of Visual Studio 2022 the Microsoft Implementation allows fixing in the current code file only.  Earlier versions do not support fixing at all.
- TypeScript Analyzer uses Prettier for formatting, or can allow simple formatting using ESLint itself.
- TypeScript Analyzer can fix on a save, including doing Prettier formatting whenever a file is saved.
- TypeScript Analyzer can be used to prevent Visual Studio from building if you have linting or formatting errors.
- TypeScript Analyzer will allow you to lint files and code that are not TypeScript or JavaScript with an appropriate plugin, show the errors in the Error List, and allow fixing if that's possible.  For example, linting of Markdown in Markdown files in your solution is possible with the eslint-plugin-md plugin.  This requires a little set up, of course.
- The Microsoft implementation has React and node plugins installed and enables some ESLint rules for these by default.  To do this with the TypeScript Analyzer requires [some set up and configuration](setupreact.md).