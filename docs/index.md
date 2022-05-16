## Pre-release documentation: This extension has not yet been released to Visual Studio Marketplace

# Overview

**This extension will lint and format JavaScript and TypeScript files in Visual Studio.**  

It uses recommended rules for [ESLint](https://eslint.org/) for linting and [Prettier](https://prettier.io/) for formatting.  

For formatting, and where fixers are available for linting, it allows the code to be automatically fixed via context menu options.

It's available for Visual Studio 2022, Visual Studio 2019 and Visual Studio 2017, all editions.

![Basic Fix](assets\images\basicfix.gif)

# Feature List

The TypeScript Analyzer has the features below.  A [walk through showing these features being used, with pictures, is available](walkthrough.md).

- Errors are underlined in the code file with [more detail if you hover](walkthrough.md#errorwithhover), as usual, and shown in the Visual Studio Error List:
- The TypeScript Analyzer lints as you type and lints on file open or save, but **can be instructed to lint an entire project or solution at once**, and show all errors in any files.  **It can fix an entire solution at once.**
- It **[can run on a local build](settings.md#runonbuild)** in Visual Studio, and will prevent the build from starting if there are unfixed linting or formatting errors anywhere in the code being built.
- It **[can use type-aware TypeScript rules](typeinformation.md)**, provided a tsconfig.json file and appropriate configuration is provided.
- It **[works in Visual Studio folder view](folderview.md)**, including allowing entire folders to be linted/formatted at once.
- It **[is extensible to allow other ESLint plugins to be used](plugins.md)** in Visual Studio, with errors shown in the Visual Studio Error List and fixers available.  As an example, instructions are available to use the Markdown plugin to lint and fix Markdown (.md) files.
- **[Prettier can be easily disabled](formatting.md)** and the Analyzer can fall back to a simpler set of formatting rules using ESLint.  Alternatively formatting rules can be disabled altogether.
- Unlike other linters, **[logging is available](settings.md#logging)** in the Visual Studio Output window.  This is useful for diagnosing set-up and configuration issues.
- **A [default configuration file](defaultconfig.md) is provided** that explicitly enables all recommended rules separately, so you can see what's going on without having to find a list of rules on a website.  You can edit this for all your projects at once, or provide separate configuration for specific projects as necessary, as usual with ESLint. The Analyzer also respects .eslintignore files, as you'd expect.
- The Analyzer **is reasonably performant**, although clearly it is limited by ESLint's own performance capabilities.

# Documentation

There is an abbreviated set of navigation links in the footer of this and every documentation page.  A full [table of contents for the documentation](contents.md) is also available.

# Source Code

The code for this extension is [available on GitHub](https://github.com/rich-newman/typescript-analyzer-eslint-prettier).