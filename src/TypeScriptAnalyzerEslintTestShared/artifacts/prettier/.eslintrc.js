// Default configuration file for TypeScript Analyzer (ESLint)
// Default location is Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TypeScriptAnalyzerConfig")
// which usually resolves to c:\Users\{UserName}\TypeScriptAnalyzerConfig\.eslintrc.js
module.exports = {
    "root": true,
    "parserOptions": {
        "ecmaVersion": 6,
        "sourceType": "module",
        "ecmaFeatures": {
            "jsx": true // Allows support of JSX, but use of React plugin is required to support React semantics
        }
    },
    "parser": "../../../Shared/Node/node_modules/@typescript-eslint/parser", // TypeScript parser used for both .js and .ts files
    "plugins": [
        "@typescript-eslint",
        "prettier"
    ],
    "env": {
        "amd": true,
        "browser": true,
        "jquery": true,
        "node": true,
        "es6": true, // This enables ES6 global variables AND ES6 syntax
        "worker": true
    },
    "rules": {
        // Below are all of the rules from eslint:recommended https://github.com/eslint/eslint/blob/master/conf/eslint-recommended.js
        // Options are off, warn, error (or 0, 1, 2 if you like meaningless numbers)
        // To add an option to a rule use [] e.g. "no-inner-declarations": ["error", "both"],
        // Full list of all rules: https://eslint.org/docs/rules/
        "constructor-super": "error",             // https://eslint.org/docs/rules/constructor-super
        "for-direction": "error",                 // https://eslint.org/docs/rules/for-direction
        "getter-return": "error",                 // https://eslint.org/docs/rules/getter-return
        "no-async-promise-executor": "error",     // https://eslint.org/docs/rules/no-async-promise-executor
        "no-case-declarations": "error",          // https://eslint.org/docs/rules/no-case-declarations
        "no-class-assign": "error",               // https://eslint.org/docs/rules/no-class-assign
        "no-compare-neg-zero": "error",           // https://eslint.org/docs/rules/no-compare-neg-zero
        "no-cond-assign": "error",                // https://eslint.org/docs/rules/no-cond-assign
        "no-const-assign": "error",               // https://eslint.org/docs/rules/no-const-assign
        "no-constant-condition": "error",         // https://eslint.org/docs/rules/no-constant-condition
        "no-control-regex": "error",              // https://eslint.org/docs/rules/no-control-regex
        "no-debugger": "error",                   // https://eslint.org/docs/rules/no-debugger
        "no-delete-var": "error",                 // https://eslint.org/docs/rules/no-delete-var
        "no-dupe-args": "error",                  // https://eslint.org/docs/rules/no-dupe-args
        "no-dupe-class-members": "error",         // https://eslint.org/docs/rules/no-dupe-class-members
        "no-dupe-else-if": "error",               // https://eslint.org/docs/rules/no-dupe-else-if
        "no-dupe-keys": "error",                  // https://eslint.org/docs/rules/no-dupe-keys
        "no-duplicate-case": "error",             // https://eslint.org/docs/rules/no-duplicate-case
        "no-empty": "error",                      // https://eslint.org/docs/rules/no-empty
        "no-empty-character-class": "error",      // https://eslint.org/docs/rules/no-empty-character-class
        "no-empty-pattern": "error",              // https://eslint.org/docs/rules/no-empty-pattern
        "no-ex-assign": "error",                  // https://eslint.org/docs/rules/no-ex-assign
        "no-extra-boolean-cast": "error",         // https://eslint.org/docs/rules/no-extra-boolean-cast
        "no-extra-semi": "error",                 // https://eslint.org/docs/rules/no-extra-semi
        "no-fallthrough": "error",                // https://eslint.org/docs/rules/no-fallthrough
        "no-func-assign": "error",                // https://eslint.org/docs/rules/no-func-assign
        "no-global-assign": "error",              // https://eslint.org/docs/rules/no-global-assign
        "no-import-assign": "error",              // https://eslint.org/docs/rules/no-import-assign
        "no-inner-declarations": "error",         // https://eslint.org/docs/rules/no-inner-declarations
        "no-invalid-regexp": "error",             // https://eslint.org/docs/rules/no-invalid-regexp
        "no-irregular-whitespace": "error",       // https://eslint.org/docs/rules/no-irregular-whitespace
        "no-misleading-character-class": "error", // https://eslint.org/docs/rules/no-misleading-character-class
        "no-mixed-spaces-and-tabs": "error",      // https://eslint.org/docs/rules/no-mixed-spaces-and-tabs
        "no-new-symbol": "error",                 // https://eslint.org/docs/rules/no-new-symbol
        "no-obj-calls": "error",                  // https://eslint.org/docs/rules/no-obj-calls
        "no-octal": "error",                      // https://eslint.org/docs/rules/no-octal
        "no-prototype-builtins": "error",         // https://eslint.org/docs/rules/no-prototype-builtins
        "no-redeclare": "error",                  // https://eslint.org/docs/rules/no-redeclare
        "no-regex-spaces": "error",               // https://eslint.org/docs/rules/no-regex-spaces
        "no-self-assign": "error",                // https://eslint.org/docs/rules/no-self-assign
        "no-setter-return": "error",              // https://eslint.org/docs/rules/no-setter-return
        "no-shadow-restricted-names": "error",    // https://eslint.org/docs/rules/no-shadow-restricted-names
        "no-sparse-arrays": "error",              // https://eslint.org/docs/rules/no-sparse-arrays
        "no-this-before-super": "error",          // https://eslint.org/docs/rules/no-this-before-super
        "no-undef": "error",                      // https://eslint.org/docs/rules/no-undef
        "no-unexpected-multiline": "error",       // https://eslint.org/docs/rules/no-unexpected-multiline
        "no-unreachable": "error",                // https://eslint.org/docs/rules/no-unreachable
        "no-unsafe-finally": "error",             // https://eslint.org/docs/rules/no-unsafe-finally
        "no-unsafe-negation": "error",            // https://eslint.org/docs/rules/no-unsafe-negation
        "no-unused-labels": "error",              // https://eslint.org/docs/rules/no-unused-labels
        "no-unused-vars": "error",                // https://eslint.org/docs/rules/no-unused-vars
        "no-useless-catch": "error",              // https://eslint.org/docs/rules/no-useless-catch
        "no-useless-escape": "error",             // https://eslint.org/docs/rules/no-useless-escape
        "no-with": "error",                       // https://eslint.org/docs/rules/no-with
        "require-yield": "error",                 // https://eslint.org/docs/rules/require-yield
        "use-isnan": "error",                     // https://eslint.org/docs/rules/use-isnan
        "valid-typeof": "error",                  // https://eslint.org/docs/rules/valid-typeof

        // Other rules
        "default-param-last": "warn",             // https://eslint.org/docs/rules/default-param-last
        "eqeqeq": "warn",                         // https://eslint.org/docs/rules/eqeqeq

        // We explicitly set prettier options to prettier defaults, apart from tabWidth and endOfLine which are set to not conflict with 
        // the Visual Studio defaults.  With prettier's defaults we get Visual Studio into a fight about opinions with prettier.
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
            "jsxBracketSameLine": false,
            "arrowParens": "always"
        }]
    },

    "overrides": [
        {
            "files": ["*.ts", "*.tsx"],
            "rules": {
                // For TypeScript files disable/enable ESLint rules that are covered by TypeScript rules or by the TypeScript compiler
                // These overrides are from typescript-eslint
                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/src/configs/eslint-recommended.ts
                "constructor-super": "off", // ts(2335) & ts(2377)
                "getter-return": "off", // ts(2378)
                "no-const-assign": "off", // ts(2588)
                "no-dupe-args": "off", // ts(2300)
                "no-dupe-class-members": "off", // ts(2393) & ts(2300)
                "no-dupe-keys": "off", // ts(1117)
                "no-func-assign": "off", // ts(2539)
                "no-import-assign": "off", // ts(2539) & ts(2540)
                "no-new-symbol": "off", // ts(2588)
                "no-obj-calls": "off", // ts(2349)
                "no-redeclare": "off", // ts(2451)
                "no-setter-return": "off", // ts(2408)
                "no-this-before-super": "off", // ts(2376)
                "no-undef": "off", // ts(2304)
                "no-unreachable": "off", // ts(7027)
                "no-unsafe-negation": "off", // ts(2365) & ts(2360) & ts(2358)
                "no-var": "error", // ts transpiles let/const to var, so no need for vars any more
                "prefer-const": "error", // ts provides better types with const
                "prefer-rest-params": "error", // ts provides better types with rest args over arguments
                "prefer-spread": "error", // ts transpiles spread to apply, so no need for manual apply
                "valid-typeof": "off", // ts(2367)

                // TypeScript-specific recommended rules from the typescript-eslint plugin
                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/src/configs/recommended.ts
                "@typescript-eslint/adjacent-overload-signatures": "error",        // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/adjacent-overload-signatures.md
                "@typescript-eslint/ban-ts-comment": "error",                      // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/ban-ts-comment.md
                "@typescript-eslint/ban-types": "error",                           // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/ban-types.md
                "@typescript-eslint/explicit-module-boundary-types": "warn",       // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/explicit-module-boundary-types.md
                "no-array-constructor": "off",
                "@typescript-eslint/no-array-constructor": "error",                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-array-constructor.md
                "no-empty-function": "off",
                "@typescript-eslint/no-empty-function": "error",                   // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-empty-function.md
                "@typescript-eslint/no-empty-interface": "error",                  // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-empty-interface.md
                "@typescript-eslint/no-explicit-any": "warn",                      // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-explicit-any.md
                "@typescript-eslint/no-extra-non-null-assertion": "error",         // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-extra-non-null-assertion.md
                "no-extra-semi": "off",
                "@typescript-eslint/no-extra-semi": "error",                       // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-extra-semi.md
                "@typescript-eslint/no-inferrable-types": "error",                 // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-inferrable-types.md
                "@typescript-eslint/no-misused-new": "error",                      // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-misused-new.md
                "@typescript-eslint/no-namespace": "error",                        // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-namespace.md
                "@typescript-eslint/no-non-null-asserted-optional-chain": "error", // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-non-null-asserted-optional-chain.md
                "@typescript-eslint/no-non-null-assertion": "warn",                // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-non-null-assertion.md
                "@typescript-eslint/no-this-alias": "error",                       // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-this-alias.md
                "no-unused-vars": "off",
                "@typescript-eslint/no-unused-vars": "warn",                       // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-unused-vars.md
                "@typescript-eslint/no-var-requires": "error",                     // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-var-requires.md
                "@typescript-eslint/prefer-as-const": "error",                     // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/prefer-as-const.md
                "@typescript-eslint/prefer-namespace-keyword": "error",            // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/prefer-namespace-keyword.md
                "@typescript-eslint/triple-slash-reference": "error",              // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/triple-slash-reference.md
                //"@typescript-eslint/no-unnecessary-type-assertion": "error",     // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-unnecessary-type-assertion.md
            }
        }
    ]
};