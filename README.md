# TypeScript Analyzer (ESLint, Prettier)

**This extension will lint and format JavaScript and TypeScript files in Visual Studio.**  

It uses recommended rules for [ESLint](https://eslint.org/) for linting and [Prettier](https://prettier.io/) for formatting.  

For formatting, and where fixers are available for linting, it allows the code to be automatically fixed via context menu options.

The TypeScript Analyzer should also [work with almost any ESLint plugin](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/plugins.html), showing errors in the Visual Studio Error List and allowing fixing.

It's available for [Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=RichNewman.TypeScriptAnalyzerEslintPrettier), [Visual Studio 2019 and Visual Studio 2017](https://marketplace.visualstudio.com/items?itemName=RichNewman.TypeScriptAnalyzerEslintPrettier2019), all editions.

 The 'TypeScript Analyzer (ESLint, Prettier)' has the following features. A [walk through](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/walkthrough.html) showing these features being used, with pictures, is available.

- Errors are underlined in the code file with more detail if you hover, as usual, and shown in the Visual Studio Error List.
- The Analyzer lints as you type and lints on file open or save, but **can be instructed to lint an entire project or solution at once**, and show all errors in any files.  It can fix an entire solution at once.
- It **can fix and format on a save**.  This means that whenever you save a file Prettier will be invoked and will format it in a standard way, assuming Prettier is enabled.  Prettier is enabled by default, but fix on save is not.
- It **can run on a local build** in Visual Studio, and will prevent the build from starting if there are unfixed linting or formatting errors anywhere in the code being built.
- It **[can use type-aware TypeScript rules](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/typeinformation.html)**, provided a tsconfig.json file and appropriate configuration is provided.
- It **[works in Visual Studio folder view](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/folderview.html)**, including allowing entire folders to be linted/formatted at once.
- It **[is extensible to allow other ESLint plugins to be used](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/plugins.html)** in Visual Studio, with errors shown in the Visual Studio Error List and fixers available.  As an example, [instructions are available to use the Markdown plugin to lint and fix Markdown (.md) files](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/setupmarkdown.html).
- Instructions are also available for linting [React files](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/setupreact.html) and [Angular files](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/setupangular.html)
- [**Prettier can be disabled and the Analyzer can fall back to a simpler set of formatting rules using ESLint.**](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/formatting.html)  Alternatively formatting rules can be disabled altogether.
- Unlike other linters, **[logging is available](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/settings.html#logging)** in the Visual Studio Output window.  This is useful for diagnosing set-up and configuration issues.
- **[A default configuration file is provided](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/defaultconfig.html)** that explicitly enables all recommended rules separately, so you can see what's going on without having to find a list of rules on a website.  You can edit this for all your projects at once, or provide separate configuration for specific projects as necessary, as usual with ESLint. The Analyzer also respects .eslintignore files, as you'd expect.
- The Analyzer **is reasonably performant**, although clearly it is limited by ESLint's own performance capabilities.

**Note that in the latest versions of Visual Studio 2022 (after v17.4) linting errors from the TypeScript Analyzer are not appearing in Node/JavaScript/TypeScript Console Applications.  [We have an explanation, and a workaround for this available.](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/noteonvs2022templates.html)**

To install visit [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=RichNewman.TypeScriptAnalyzerEslintPrettier) or search for 'TypeScript Analyzer' in Extensions and Updates in Visual Studio.

[Documentation for the extension is available.](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/contents.html)