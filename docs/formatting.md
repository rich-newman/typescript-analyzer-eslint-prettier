# Formatting Code with the TypeScript Analyzer

## Basic Prettier Operation

In the TypeScript Analyzer [Prettier](https://prettier.io/docs/en/index.html) is enabled by default for TypeScript and JavaScript files.  This means that:

- Prettier formatting issues are flagged as warnings in the Visual Studio editor and in the Error List.
- If you fix using the TypeScript Analyzer then your code is reformatted using Prettier to remove these warnings.

You can also fix and format whenever a file is saved, although this is not enabled by default.  To enable it set Tools/Menu/TypeScript Analyzer/Fix and format on Save to 'True'.

## Prettier is Opinionated

Prettier calls itself an ['opinionated code formatter'](https://prettier.io/).  There are only very limited configuration options, and even these seem to be [regarded as a mistake by the Prettier team](https://prettier.io/docs/en/option-philosophy.html).

It's possible you may find Prettier and the way it's implemented in the TypeScript Analyzer too opinionated for your needs.

As a result the TypeScript Analyzer exposes a number of ways of formatting in a less opinionated way.

## Options for Formatting Code Using the TypeScript Analyzer

1. Showing **Prettier errors and warnings in the code file and Error List can be disabled** through the TypeScript Analyzer's Settings page, whilst leaving formatting using Prettier still available if you fix.
2. The **configuration options that ARE available for Prettier can be set** through the default configuration file for all projects on a computer, or can be set in a local configuration file for a specific project.
3. **Prettier can be disabled through the configuration files so that standard ESLint formatting rules are used** instead of the Prettier rules.  These ESLint formatting rules will show formatting errors in the Visual Studio code window and Error List, and will allow fixing.  However, these rules are less extensive, opinionated and aggressive than Prettier.
4. Alternatively **all formatting rules can be disabled** through the configuration files, leaving the TypeScript Analyzer to run only linting rules.

To be clear, formatting rules are about code layout.  These are things like the placement of curly braces or how code should be indented.  Linting rules are about what the code does.  These are things like ensuring a triple equals (`===`) is used where appropriate rather than a double equals (`==`), or ensuring code blocks aren't empty.

More details on these formatting options are below.

### 1. Disabling Prettier Errors and Warnings, Formatting by Fixing Still Available

To do this go to Tools/Options/TypeScript Analyzer/ESLint and set 'Show Prettier Errors in Error List' to False.

### 2. Setting Prettier Configuration Options

To set Prettier configuration options open your [configuration file](configuration.md).  For the default configuration file you do this with Tools/TypeScript Analyzer (ESLint)/Edit Default Config.  

Search for 'prettier/prettier' in the file and you will find a section as below.  The options listed correspond to those that are discussed in the [Prettier documentation](https://prettier.io/docs/en/options.html).

``` javascript
                // prettier plugin recommended settings
                // We explicitly set prettier options to prettier defaults, apart from tabWidth and endOfLine which are set to not conflict with
                // the Visual Studio defaults.  If we use prettier's defaults for these two rules then it fights with (default) Visual Studio.
                // All options are described on https://prettier.io/docs/en/options.html
                "prettier/prettier": ["warn", {
                    "tabWidth": 4,
                    "endOfLine": "crlf",
                    "printWidth": 80,
                    "semi": true,
                    "singleQuote": false,
                    "quoteProps": "as-needed",
                    "jsxSingleQuote": false,
                    "trailingComma": "es5",
                    "bracketSpacing": true,
                    "arrowParens": "always",
                }],
```

### <a name="disableprettier"></a>3. Disabling Prettier, Enabling ESLint Formatting Rules

The [default configuration file](defaultconfig.md) contains some code that disables Prettier and enables some basic ESLint formatting rules such as enforcing semicolons at the end of lines.

To use this open your configuration file as above and **change the line `const prettierEnabled = true` at the top of the .file to `const prettierEnabled = false`**.  This merges  the rules specified in prettierDisabledConfig towards the bottom of the file into the main configuration, overwriting the existing rules where they conflict.  These rules are shown below.

``` javascript
    "overrides": [
        {
            "files": ["*.js", "*.jsx", "*.mjs", "*.cjs", "*.ts", "*.tsx"],
            "rules": {
                // Turn off Prettier's one rule
                "prettier/prettier": "off",
                // Enable appropriate ESLint rules, give them similar behavior to Prettier
                "arrow-parens": "warn",             // https://eslint.org/docs/rules/arrow-parens
                "eol-last": "warn",                 // https://eslint.org/docs/rules/eol-last
                "new-parens": "warn",               // https://eslint.org/docs/rules/new-parens
                "no-multiple-empty-lines": "warn",  // https://eslint.org/docs/rules/no-multiple-empty-lines
                "no-trailing-spaces": "warn",       // https://eslint.org/docs/rules/no-trailing-spaces
            },
        },
        {
            "files": ["*.js", "*.jsx", "*.mjs", "*.cjs"],
            "rules": {
                "comma-dangle": ["warn", "always-multiline"],                 // https://eslint.org/docs/rules/comma-dangle
                "indent": "warn",                                             // https://eslint.org/docs/rules/indent
                "quotes": ["warn", "double",
                    { "avoidEscape": true, "allowTemplateLiterals": true }],  // https://eslint.org/docs/rules/quotes
                "semi": "warn",                                               // https://eslint.org/docs/rules/semi
            },
        },
        {
            "files": ["*.ts", "*.tsx"],
            "rules": {
                "@typescript-eslint/comma-dangle": ["warn", "always-multiline"], // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/comma-dangle.md
                // The indent rule comes with a warning from its author but seems to work OK usually https://github.com/typescript-eslint/typescript-eslint/issues/1824
                "@typescript-eslint/indent": "warn",                             // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/indent.md
                "@typescript-eslint/member-delimiter-style": "warn",             // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/member-delimiter-style.md
                "@typescript-eslint/quotes": ["warn", "double",
                    { "avoidEscape": true, "allowTemplateLiterals": true }],     // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/quotes.md
                "@typescript-eslint/semi": "warn",                               // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/semi.md
                "@typescript-eslint/type-annotation-spacing": "warn",            // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/type-annotation-spacing.md
            },
        },
    ],
```

### <a name="disableformatting"><a/> 4. Disabling All Formatting Rules and Using Linting Rules Only

To disable all formatting rules open your configuration file as above.  Leave the line `const prettierEnabled = true` set to true, but disable the prettier/prettier rule.  To do this search for 'prettier/prettier' and change "warn" to "off", or just delete the rule entirely.