# Visual Studio 2022 'Node.js Console Applications' And 'JavaScript/TypeScript Console Applications' Not Showing Linting Errors

## Background

New projects created with the Visual Studio 2022 project types 'JavaScript Console Application', 'TypeScript Console Application or 'Node.js Console Application' **do not show linting errors**, or prettier errors, even if the TypeScript Analyzer is correctly installed and working.

This behavior started in Visual Studio 2002 v17.4 and later.  Earlier versions, including all versions of Visual Studio 2017 and 2019, would show linting errors in such projects.

The reason is that projects created with these templates include a local install of the ESLint npm package, and configuration for the package.  The TypeScript Analyzer will use a [local install](installs.md) and [configuration](localconfiguration.md) if it finds them.  However, **by default the configuration in these Visual Studio projects types includes no linting rules.**  This means the TypeScript Analyzer has no linting rules to apply, and cannot generate linting errors.

Note that the situation is slightly worse in a 'JavaScript Console Application' project, as this installs ESLint locally and no other packages, and has no local configuration.  The TypeScript Analyzer tries to use the local 'install' of ESLint with its default .eslintrc.js configuration file, which expects Prettier and TypeScript packages to be installed alongside ESLint.  As a result we get errors.  The workaround below still works in this case however.

## Workaround

To have the TypeScript Analyzer actually do some useful linting in these projects clearly you can alter the local configuration.  The configuration that overrides the default .eslintrc.js file is in package.json under heading eslintConfig.  You can add rules here as a workaround.

However, as a simpler workaround we have made it possible to *force* the TypeScript Analyzer to use its own install of npm packages, AND its own configuration file, .eslintrc.js, even if there are local versions.

**To do this, go to Tools/Options/TypeScript Analyzer/ESLint and:**

**- Set 'Enable local config (.eslintrc.js)' to False**

**- Set 'Enable local node_modules' to False**

You can test this has worked by running the TypeScript Analyzer in the default app.js/app.ts file in the project, which will have the line `console.log('Hello world');` in it.  To do this rightclick in the code window 'Run TypeScript Analyzer (ESLint) on Code File'.  This should give a a Prettier error in the Error List to update 'Hello world'); to "Hello world");CRLF.  'Hello world' should be underlined in green in the code editor.