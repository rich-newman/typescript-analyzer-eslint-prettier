## Pre-release: This extension has not yet been released to Visual Studio Marketplace

# TypeScript Analyzer (ESLint, Prettier)

**This extension will lint and format JavaScript and TypeScript files in Visual Studio.**  

It uses recommended rules for [ESLint](https://eslint.org/) for linting and [Prettier](https://prettier.io/) for formatting.  

For formatting, and where fixers are available for linting, it allows the code to be automatically fixed via context menu options.

It's available for Visual Studio 2022, Visual Studio 2019 and Visual Studio 2017, all editions.

It has the following features:

- Errors are underlined in the code file with more detail if you hover, as usual, and shown in the Visual Studio Error List.
- The Analyzer lints as you type and lints on file open or save, but **can be instructed to lint an entire project or solution at once**, and show all errors in any files.  If can fix an entire solution at once.
- It **can run on a local build** in Visual Studio, and to prevent the build from starting if there are unfixed linting or formatting errors anywhere in the code being built.
- It **can use type-aware TypeScript rules**, provided a tsconfig.json file and appropriate configuration is provided.
- It **works in Visual Studio folder view**, including allowing entire folders to be linted/formatted at once.
- It **is extensible to allow other ESLint plugins to be used** in Visual Studio, with errors shown in the Visual Studio Error List and fixers available.  As an example, instructions are available to use the Markdown plugin to lint and fix Markdown (.md) files.
- Instructions are also available for linting React and Vue files.
- **Prettier can be disabled and the Analyzer can fall back to a simpler set of formatting rules using ESLint**.  Alternatively formatting rules can be disabled altogether.
- Unlike other linters, **logging is available** in the Visual Studio Output window.  This is useful for diagnosing set-up and configuration issues.
- **A default configuration file is provided** that explicitly enables all recommended rules separately, so you can see what's going on without having to find a list of rules on a website.  You can edit this for all your projects at once, or provide separate configuration for specific projects as necessary, as usual with ESLint. The Analyzer also respects .eslintignore files, as you'd expect.
- The Analyzer **is reasonably performant**, although clearly it is limited by ESLint's own performance capabilities.  In particular it's fairly easy to run out of memory if the memory is set to the default and you're trying to lint a large project. To mitigate this the memory used by the JavaScript Virtual Machine is configurable.  

To install visit Visual Studio Marketplace or search for 'TypeScript Analyzer' in Extensions and Updates in Visual Studio.

[Documentation for the extension is available.](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/)


