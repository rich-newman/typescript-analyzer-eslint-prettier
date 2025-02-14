﻿# Default Configuration File

The default configuration file, .eslintrc.js, that the TypeScript Analyzer (ESLint) uses is in the TypeScript Analyzer GitHub repository, and a copy is below.

For more details on general configuration see the [Configuration documentation page](configuration.md).

## <a name="location" ></a>Location

When the TypeScript Analyzer is installed this file can be found in its own folder at c:\Users\\{userName\}\TypeScriptAnalyzerConfig\\.eslintrc.js.  

It's in its own folder to prevent other instances of ESLint using it in error.  This is a problem with the default Microsoft .eslintrc file that gets installed with Visual Studio in c:\Users\\{userName\}\.eslintrc.  The Analyzer is programmed to never look at the Microsoft file, but other linting may well find it and try to use it.

## Editing

The **default configuration file can be viewed and edited in Visual Studio** via a menu option: Tools/TypeScript Analyzer (ESLint)/Edit Default Config.

## Contents

Whilst this code looks quite scary this is mainly because it lists every rule that is enabled separately, as well as providing a link to the associated documentation.

These rules are the recommended rules for ESLint, plus the recommended rules for the TypeScript ESLint plugin, plus the single rule for the Prettier ESLint plugin.

There's a commented-out section for recommended TypeScript type-aware rules/rules that require type information.  These are commented out because we want the TypeScript Analyzer to work without additional configuration, and we need a tsconfig.json for these rules to function.  See the [documentation on Type Aware Rules](typeinformation.md) for more information.

Finally there's some code that allows Prettier to be disabled whilst retaining some formatting rules, see [the documentation on using Prettier for formatting](formatting.md) for more details.

## Structure

There are no rules that apply to **all** file types.  All rules are in overrides sections that list a set of file extensions that they apply to in a "files" entry.  This is because we need to be able to easily extend the configuration to deal with files other than TypeScript and JavaScript.  See the [documentation on using the linter with other plugins](plugins.md) for more information on this.

Instead there are 'overrides' sections for rules that apply to TypeScript files only, to JavaScript files only and to both TypeScript and JavaScript files.

## Plugins

The only ESLint plugins that are enabled by default, and are installed by the Analyzer, are @typescript-eslint and prettier.  These are enabled in the 'both TypeScript and JavaScript files' section of the configuration.

Other ESLint plugins can be installed and used, see the section on the same.

## <a name="defaulteslintrc"></a>Code of Default .eslintrc.js
``` javascript
// This file is the default configuration file for TypeScript Analyzer (ESLint)
// The default location in C# is something like c:\Users\{UserName}\TypeScriptAnalyzerConfig\.eslintrc.js

// If the prettierEnabled flag is set to false prettier is completely disabled, and the (less opinionated) set of ESLint formatting rules
// under prettierDisabledConfig below are enabled instead.  Delete these, or turn them off, if you want NO formatting rules in the linter.
const prettierEnabled = true;

const config = {
    "root": true,
    "ignorePatterns": [".eslintrc.js"],

    // We can lint almost any file extension with an appropriate plugin, so having rules that apply to them all makes little sense
    // Instead all rules are defined in overrides sections by file extension, even though we're not actually overriding anything
    "overrides": [
        {
            // This object has rules etc. that apply to BOTH JavaScript and TypeScript.
            // Rules that only apply to JavaScript are in a JavaScript object further below, and similarly for TypeScript.
            "files": ["*.js", "*.jsx", "*.mjs", "*.cjs", "*.ts", "*.tsx"],
            "parserOptions": {
                "ecmaVersion": 6,
                "sourceType": "module",
                "ecmaFeatures": {
                    "jsx": true, // Allows support of JSX, but use of React plugin is required to support React semantics
                },
            },
            // To disable a plugin commment it out here and also comment out its rules in the appropriate section below
            // Reverse that to enable one
            "plugins": [
                "@typescript-eslint",
                "prettier",
            ],
            "env": {
                "amd": true,
                "browser": true,
                "jquery": true,
                "node": true,
                "es6": true, // This enables ES6 global variables AND ES6 syntax
                "worker": true,
            },
            "rules": {
                // Below are all of the rules from eslint:recommended https://github.com/eslint/eslint/blob/master/conf/eslint-recommended.js,
                // apart from those that we want enabled for JavaScript only and not TypeScript, which are in the JavaScript object below
                // Options are off, warn, error (or 0, 1, 2 if you like meaningless numbers)
                // To add an option to a rule use [] e.g. "no-inner-declarations": ["error", "both"],
                // Full list of all rules: https://eslint.org/docs/rules/
                "for-direction": "error",                 // https://eslint.org/docs/rules/for-direction
                "no-async-promise-executor": "error",     // https://eslint.org/docs/rules/no-async-promise-executor
                "no-case-declarations": "error",          // https://eslint.org/docs/rules/no-case-declarations
                "no-class-assign": "error",               // https://eslint.org/docs/rules/no-class-assign
                "no-compare-neg-zero": "error",           // https://eslint.org/docs/rules/no-compare-neg-zero
                "no-cond-assign": "error",                // https://eslint.org/docs/rules/no-cond-assign
                "no-constant-condition": "error",         // https://eslint.org/docs/rules/no-constant-condition
                "no-control-regex": "error",              // https://eslint.org/docs/rules/no-control-regex
                "no-debugger": "error",                   // https://eslint.org/docs/rules/no-debugger
                "no-delete-var": "error",                 // https://eslint.org/docs/rules/no-delete-var
                "no-dupe-else-if": "error",               // https://eslint.org/docs/rules/no-dupe-else-if
                "no-duplicate-case": "error",             // https://eslint.org/docs/rules/no-duplicate-case
                "no-empty": "error",                      // https://eslint.org/docs/rules/no-empty
                "no-empty-character-class": "error",      // https://eslint.org/docs/rules/no-empty-character-class
                "no-empty-pattern": "error",              // https://eslint.org/docs/rules/no-empty-pattern
                "no-ex-assign": "error",                  // https://eslint.org/docs/rules/no-ex-assign
                "no-extra-boolean-cast": "error",         // https://eslint.org/docs/rules/no-extra-boolean-cast
                "no-fallthrough": "error",                // https://eslint.org/docs/rules/no-fallthrough
                "no-global-assign": "error",              // https://eslint.org/docs/rules/no-global-assign
                "no-inner-declarations": "error",         // https://eslint.org/docs/rules/no-inner-declarations
                "no-invalid-regexp": "error",             // https://eslint.org/docs/rules/no-invalid-regexp
                "no-irregular-whitespace": "error",       // https://eslint.org/docs/rules/no-irregular-whitespace
                "no-misleading-character-class": "error", // https://eslint.org/docs/rules/no-misleading-character-class
                "no-mixed-spaces-and-tabs": "error",      // https://eslint.org/docs/rules/no-mixed-spaces-and-tabs
                "no-nonoctal-decimal-escape": "error",    // https://eslint.org/docs/rules/no-nonoctal-decimal-escape
                "no-octal": "error",                      // https://eslint.org/docs/rules/no-octal
                "no-prototype-builtins": "error",         // https://eslint.org/docs/rules/no-prototype-builtins
                "no-regex-spaces": "error",               // https://eslint.org/docs/rules/no-regex-spaces
                "no-self-assign": "error",                // https://eslint.org/docs/rules/no-self-assign
                "no-shadow-restricted-names": "error",    // https://eslint.org/docs/rules/no-shadow-restricted-names
                "no-sparse-arrays": "error",              // https://eslint.org/docs/rules/no-sparse-arrays
                "no-unexpected-multiline": "error",       // https://eslint.org/docs/rules/no-unexpected-multiline
                "no-unsafe-finally": "error",             // https://eslint.org/docs/rules/no-unsafe-finally
                "no-unsafe-optional-chaining": "error",   // https://eslint.org/docs/rules/no-unsafe-optional-chaining
                "no-unused-labels": "error",              // https://eslint.org/docs/rules/no-unused-labels
                "no-useless-backreference": "error",      // https://eslint.org/docs/rules/no-useless-backreference
                "no-useless-catch": "error",              // https://eslint.org/docs/rules/no-useless-catch
                "no-useless-escape": "error",             // https://eslint.org/docs/rules/no-useless-escape
                "no-with": "error",                       // https://eslint.org/docs/rules/no-with
                "require-yield": "error",                 // https://eslint.org/docs/rules/require-yield
                "use-isnan": "error",                     // https://eslint.org/docs/rules/use-isnan

                // Other rules
                "default-param-last": "warn",             // https://eslint.org/docs/rules/default-param-last
                "eqeqeq": "warn",                         // https://eslint.org/docs/rules/eqeqeq

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
            }, // rules
        },
        {
            // JavaScript-only rules
            "files": ["*.js", "*.jsx", "*.mjs", "*.cjs"],
            "rules": {
                // Additional recommended rules, plus why they don't apply to TypeScript (ts)
                "constructor-super": "error",             // https://eslint.org/docs/rules/constructor-super ts(2335) & ts(2377)
                "getter-return": "error",                 // https://eslint.org/docs/rules/getter-return ts(2378)
                "no-const-assign": "error",               // https://eslint.org/docs/rules/no-const-assign ts(2588)
                "no-dupe-args": "error",                  // https://eslint.org/docs/rules/no-dupe-args ts(2300)
                "no-dupe-class-members": "error",         // https://eslint.org/docs/rules/no-dupe-class-members ts(2393) & ts(2300)
                "no-dupe-keys": "error",                  // https://eslint.org/docs/rules/no-dupe-keys ts(1117)
                "no-extra-semi": "error",                 // https://eslint.org/docs/rules/no-extra-semi ts rule used
                "no-func-assign": "error",                // https://eslint.org/docs/rules/no-func-assign ts(2539)
                "no-import-assign": "error",              // https://eslint.org/docs/rules/no-import-assign ts(2539) & ts(2540)
                "no-loss-of-precision": "error",          // https://eslint.org/docs/rules/no-loss-of-precision
                "no-new-symbol": "error",                 // https://eslint.org/docs/rules/no-new-symbol ts(2588)
                "no-obj-calls": "error",                  // https://eslint.org/docs/rules/no-obj-calls ts(2349)
                "no-redeclare": "error",                  // https://eslint.org/docs/rules/no-redeclare ts(2451)
                "no-setter-return": "error",              // https://eslint.org/docs/rules/no-setter-return ts(2408)
                "no-this-before-super": "error",          // https://eslint.org/docs/rules/no-this-before-super ts(2376)
                "no-undef": "error",                      // https://eslint.org/docs/rules/no-undef ts(2304)
                "no-unreachable": "error",                // https://eslint.org/docs/rules/no-unreachable ts(7027)
                "no-unsafe-negation": "error",            // https://eslint.org/docs/rules/no-unsafe-negation ts(2365) & ts(2360) & ts(2358)
                "no-unused-vars": "error",                // https://eslint.org/docs/rules/no-unused-vars ts rule used
                "valid-typeof": "error",                  // https://eslint.org/docs/rules/valid-typeof ts(2367)
            },  // rules
        },
        {
            // TypeScript-only rules
            "files": ["*.ts", "*.tsx"],
            "rules": {
                // For TypeScript files we follow typescript-eslint's recommendations re ESLint recommended rules: rules that are recommended to be
                // disabled are not applied to TypeScript (they are in the JavaScript only section above), additional recommended rules are enabled below
                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/src/configs/eslint-recommended.ts
                "no-var": "error", // ts transpiles let/const to var, so no need for vars any more
                "prefer-const": "error", // ts provides better types with const
                "prefer-rest-params": "error", // ts provides better types with rest args over arguments
                "prefer-spread": "error", // ts transpiles spread to apply, so no need for manual apply

                // TypeScript-specific recommended rules from the typescript-eslint plugin
                // https://github.com/typescript-eslint/typescript-eslint/tree/master/packages/eslint-plugin#supported-rules
                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/src/configs/recommended.ts
                "@typescript-eslint/adjacent-overload-signatures": "error",        // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/adjacent-overload-signatures.md
                "@typescript-eslint/ban-ts-comment": "error",                      // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/ban-ts-comment.md
                "@typescript-eslint/ban-types": "error",                           // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/ban-types.md
                "@typescript-eslint/no-array-constructor": "error",                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-array-constructor.md
                "@typescript-eslint/no-empty-function": "error",                   // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-empty-function.md
                "@typescript-eslint/no-empty-interface": "error",                  // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-empty-interface.md
                "@typescript-eslint/no-explicit-any": "warn",                      // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-explicit-any.md
                "@typescript-eslint/no-extra-non-null-assertion": "error",         // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-extra-non-null-assertion.md
                "@typescript-eslint/no-extra-semi": "error",                       // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-extra-semi.md
                "@typescript-eslint/no-inferrable-types": "error",                 // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-inferrable-types.md
                "@typescript-eslint/no-loss-of-precision": "error",                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-loss-of-precision.md
                "@typescript-eslint/no-misused-new": "error",                      // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-misused-new.md
                "@typescript-eslint/no-namespace": "error",                        // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-namespace.md
                "@typescript-eslint/no-non-null-asserted-optional-chain": "error", // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-non-null-asserted-optional-chain.md
                "@typescript-eslint/no-non-null-assertion": "warn",                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-non-null-assertion.md
                "@typescript-eslint/no-this-alias": "error",                       // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-this-alias.md
                "@typescript-eslint/no-unnecessary-type-constraint": "error",      // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-unnecessary-type-constraint.md
                "@typescript-eslint/no-unused-vars": "warn",                       // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-unused-vars.md
                "@typescript-eslint/no-var-requires": "error",                     // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-var-requires.md
                "@typescript-eslint/prefer-as-const": "error",                     // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/prefer-as-const.md
                "@typescript-eslint/prefer-namespace-keyword": "error",            // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/prefer-namespace-keyword.md
                "@typescript-eslint/triple-slash-reference": "error",              // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/triple-slash-reference.md

                // TypeScript-specific recommended rules from the typescript-eslint plugin that need type information: for these
                // to work we need a tsconfig.json that includes the files we want linted, and to turn it on by setting
                // Tools/Options/TypeScript/Lint with tsconfig.json files to true.
                // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/src/configs/recommended-requiring-type-checking.ts
                //"@typescript-eslint/await-thenable": "error",                 // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/await-thenable.md
                //"@typescript-eslint/no-floating-promises": "error",           // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-floating-promises.md
                //"@typescript-eslint/no-for-in-array": "error",                // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-for-in-array.md
                //"@typescript-eslint/no-implied-eval'": "error",               // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-implied-eval.md
                //"@typescript-eslint/no-misused-promises": "error",            // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-misused-promises.md
                //"@typescript-eslint/no-unnecessary-type-assertion": "error",  // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-unnecessary-type-assertion.md
                //"@typescript-eslint/no-unsafe-argument": "error",             // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-unsafe-argument.md
                //"@typescript-eslint/no-unsafe-assignment": "error",           // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-unsafe-assignment.md
                //"@typescript-eslint/no-unsafe-call": "error",                 // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-unsafe-call.md
                //"@typescript-eslint/no-unsafe-member-access": "error",        // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-unsafe-member-access.md
                //"@typescript-eslint/no-unsafe-return": "error",               // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/no-unsafe-return.md
                //"@typescript-eslint/require-await": "error",                  // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/require-await.md
                //"@typescript-eslint/restrict-plus-operands": "error",         // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/restrict-plus-operands.md
                //"@typescript-eslint/restrict-template-expressions": "error",  // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/restrict-template-expressions.md
                //"@typescript-eslint/unbound-method": "error",                 // https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/unbound-method.md
            },  // rules
        },
    ],
};

const prettierDisabledConfig = {
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
};

if (!prettierEnabled) {
    config.overrides[0].plugins.splice(1, 1); // Remove element at index 1 (="prettier")
    config.overrides[0].rules = Object.assign(config.overrides[0].rules, prettierDisabledConfig.overrides[0].rules);
    config.overrides[1].rules = Object.assign(config.overrides[1].rules, prettierDisabledConfig.overrides[1].rules);
    config.overrides[2].rules = Object.assign(config.overrides[2].rules, prettierDisabledConfig.overrides[2].rules);
}

module.exports = config;
```