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
    // @typescript-eslint/parser is specified by the Analyzer unless you change the 'Set parser from config' setting to True in 
    // Tools/ Options/TypeScript Analyzer, in which case you need a parser setting here
    // "parser": "espree", // JavaScript parser in eslint
    "parser": "@angular-eslint/template-parser", // angular-eslint parser
    // To disable a plugin commment it out here and also comment out its rules in the appropriate section below
    "plugins": [
        "@typescript-eslint",
        "react",
        "prettier",
        "import",
        "node",
        "promise",
        "@angular-eslint",
        "@angular-eslint/template"
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
        "@angular-eslint/component-class-suffix": "error",
        "@angular-eslint/contextual-lifecycle": "error",
        "@angular-eslint/directive-class-suffix": "error",
        "@angular-eslint/no-conflicting-lifecycle": "error",
        "@angular-eslint/no-empty-lifecycle-method": "error",
        "@angular-eslint/no-host-metadata-property": "error",
        "@angular-eslint/no-input-rename": "error",
        "@angular-eslint/no-inputs-metadata-property": "error",
        "@angular-eslint/no-output-native": "error",
        "@angular-eslint/no-output-on-prefix": "error",
        "@angular-eslint/no-output-rename": "error",
        "@angular-eslint/no-outputs-metadata-property": "error",
        "@angular-eslint/use-lifecycle-interface": "warn",
        "@angular-eslint/use-pipe-transform-interface": "error",
        "@angular-eslint/directive-selector": [
            "error",
            {
                "type": "attribute",
                "prefix": "app",
                "style": "camelCase"
            }
        ],
        "@angular-eslint/component-selector": [
            "error",
            {
                "type": "element",
                "prefix": "app",
                "style": "kebab-case"
            }
        ],
        "@angular-eslint/template/banana-in-box": "error",
        "@angular-eslint/template/eqeqeq": "error",
        "@angular-eslint/template/no-negated-async": "error",
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

        // react plugin recommended rules https://github.com/yannickcr/eslint-plugin-react#recommended
        "react/display-name": "warn",             // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/display-name.md
        "react/no-children-prop": "warn",         // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-children-prop.md
        "react/no-danger-with-children": "warn",  // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-danger-with-children.md
        "react/no-deprecated": "warn",            // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-deprecated.md
        "react/no-direct-mutation-state": "warn", // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-direct-mutation-state.md
        "react/no-find-dom-node": "warn",         // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-find-dom-node.md
        "react/no-is-mounted": "warn",            // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-is-mounted.md
        "react/no-render-return-value": "warn",   // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-render-return-value.md
        "react/no-string-refs": "warn",           // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-string-refs.md
        "react/no-unescaped-entities": "warn",    // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-unescaped-entities.md
        "react/no-unknown-property": "warn",      // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-unknown-property.md
        "react/prop-types": "warn",               // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/prop-types.md
        "react/react-in-jsx-scope": "warn",       // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/react-in-jsx-scope.md
        "react/require-render-return": "warn",    // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/require-render-return.md
        "react/jsx-key": "warn",                  // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/jsx-key.md
        "react/jsx-no-comment-textnodes": "warn", // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/jsx-no-comment-textnodes.md
        "react/jsx-no-duplicate-props": "warn",   // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/jsx-no-duplicate-props.md
        "react/jsx-no-target-blank": "warn",      // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/jsx-no-target-blank.md
        "react/jsx-no-undef": "warn",             // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/jsx-no-undef.md
        "react/jsx-uses-react": "warn",           // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/jsx-uses-react.md
        "react/jsx-uses-vars": "warn",            // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/jsx-uses-vars.md

        //// Some additional React rules
        //"react/no-danger": "warn",                // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-danger.md
        //"react/no-did-mount-set-state": "warn",   // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-did-mount-set-state.md
        //"react/no-did-update-set-state": "warn",  // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-did-update-set-state.md

        //// node plugin recommended rules
        //"node/no-deprecated-api": "error",                     // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-deprecated-api.md
        //"node/no-exports-assign": "error",                     // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-exports-assign.md
        //"node/no-extraneous-import": "error",                  // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-extraneous-import.md
        //"node/no-extraneous-require": "error",                 // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-extraneous-require.md
        //"node/no-missing-import": "error",                     // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-missing-import.md
        //"node/no-missing-require": "error",                    // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-missing-require.md
        //"node/no-process-exit": "error",                       // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-process-exit.md
        //"node/no-unpublished-bin": "error",                    // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unpublished-bin.md
        //"node/no-unpublished-import": "error",                 // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unpublished-import.md
        //"node/no-unpublished-require": "error",                // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unpublished-require.md
        //"node/no-unsupported-features/es-builtins": "error",   // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unsupported-features/es-builtins.md
        //"node/no-unsupported-features/es-syntax": "error",     // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unsupported-features/es-syntax.md
        //"node/no-unsupported-features/node-builtins": "error", // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unsupported-features/node-builtins.md
        //"node/process-exit-as-throw": "error",                 // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/process-exit-as-throw.md
        //"node/shebang": "error",                               // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/shebang.md

        //// import plugin recommended rules
        //'import/no-unresolved': 'error',             // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/no-unresolved.md
        //'import/named': 'error',                     // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/named.md
        //'import/namespace': 'error',                 // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/namespace.md
        //'import/default': 'error',                   // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/default.md
        //'import/export': 'error',                    // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/export.md
        //'import/no-named-as-default': 'warn',        // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/no-named-as-default.md
        //'import/no-named-as-default-member': 'warn', // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/no-named-as-default-member.md
        //'import/no-duplicates': 'warn',              // https://github.com/import-js/eslint-plugin-import/blob/master/docs/rules/no-duplicates.md

        // promise plugin recommended rules
        'promise/always-return': 'error',         // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/always-return.md
        'promise/no-return-wrap': 'error',        // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/no-return-wrap.md
        'promise/param-names': 'error',           // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/param-names.md
        'promise/catch-or-return': 'error',       // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/catch-or-return.md
        'promise/no-native': 'off',               // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/no-native.md
        'promise/no-nesting': 'warn',             // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/no-nesting.md
        'promise/no-promise-in-callback': 'warn', // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/no-promise-in-callback.md
        'promise/no-callback-in-promise': 'warn', // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/no-callback-in-promise.md
        'promise/avoid-new': 'off',               // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/avoid-new.md
        'promise/no-new-statics': 'error',        // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/no-new-statics.md
        'promise/no-return-in-finally': 'warn',   // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/no-return-in-finally.md
        'promise/valid-params': 'warn',           // https://github.com/xjamundx/eslint-plugin-promise/blob/development/docs/rules/valid-params.md

        // prettier plugin recommended settings
        // We explicitly set prettier options to prettier defaults, apart from tabWidth and endOfLine which are set to not conflict with
        // the Visual Studio defaults.  If we use prettier's defaults for these two rules then it fights with Visual Studio.
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

                // TypeScript compilation already ensures that named imports exist in the referenced module
                'import/named': 'off',
            }
        }
    ]
};