# Configuration File (.eslintrc.js) for Vue Plugin, for Use with TypeScript and Vue 3

``` javascript
// If the prettierEnabled flag is set to false prettier is completely disabled, and the (less opinionated) set of ESLint formatting rules
// under prettierDisabledConfig below are enabled instead.  Delete these, or turn them off, if you want NO formatting rules in the linter.
const prettierEnabled = true;

const config = {
    "root": true,
    // To use ignorePatterns for local files, copy .eslintrc.js into your project: the paths won't be discoverable with the default config
    "ignorePatterns": [".eslintrc.js", "*.d.ts"],

    // We can lint almost any file extension with an appropriate plugin, so having rules that apply to them all makes little sense
    // Instead all rules are defined in overrides sections by file extension, even though we're not actually overriding anything
    "overrides": [
        {
            // This object has rules etc. that apply to BOTH JavaScript and TypeScript, as well as .vue files.
            // Rules that only apply to JavaScript are in a JavaScript object further below, and similarly for TypeScript and .vue.
            "files": ["*.js", "*.jsx", "*.mjs", "*.cjs", "*.ts", "*.tsx", "*.vue"],
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
                "prettier-vue",
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

                // All options are described on https://prettier.io/docs/en/options.html
                "prettier-vue/prettier": [
                    "warn",
                    {
                        "tabWidth": 4,
                        "endOfLine": "crlf",
                        "printWidth": 80,
                        "semi": true,
                        "singleQuote": true,
                        "quoteProps": "as-needed",
                        "jsxSingleQuote": false,
                        "trailingComma": "es5",
                        "bracketSpacing": true,
                        "arrowParens": "always"
                    }
                ],
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
            "files": ["*.ts", "*.tsx", "*.vue"],
            // Comment in with appropriate path for type-aware rules, including in TypeScript scripts in .vue files
            //"parserOptions": {
            //    "tsconfigRootDir": "C:/Dotnet/vueproject3/vueproject3",
            //    "project": ["./tsconfig.json"],
            //},
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
        {
            "files": ["*.vue"],
            "settings": {
                "prettier-vue": {
                    "SFCBlocks": {
                        "template": false,
                        "script": true,
                        "style":false
                    }
                }
            },
            "plugins": ["vue"],
            "parser": "vue-eslint-parser",
            "parserOptions": {
                // Parser for script tags only: templates are parsed by the Vue parser set above
                "parser": "@typescript-eslint/parser",
                "extraFileExtensions": ['.vue'],
            },
            "rules": {
                // base
                "vue/comment-directive": "error",                        // https://eslint.vuejs.org/rules/comment-directive.html
                "vue/jsx-uses-vars": "error",                            // https://eslint.vuejs.org/rules/jsx-uses-vars.html
                // essential
                "vue/multi-word-component-names": "error",               // https://eslint.vuejs.org/rules/multi-word-component-names.html
                "vue/no-arrow-functions-in-watch": "error",              // https://eslint.vuejs.org/rules/no-arrow-functions-in-watch.html
                "vue/no-async-in-computed-properties": "error",          // https://eslint.vuejs.org/rules/no-async-in-computed-properties.html
                "vue/no-child-content": "error",                         // https://eslint.vuejs.org/rules/no-child-content.html
                "vue/no-computed-properties-in-data": "error",           // https://eslint.vuejs.org/rules/no-computed-properties-in-data.html
                "vue/no-custom-modifiers-on-v-model": "error",           // https://eslint.vuejs.org/rules/no-custom-modifiers-on-v-model.html
                "vue/no-dupe-keys": "error",                             // https://eslint.vuejs.org/rules/no-dupe-keys.html
                "vue/no-dupe-v-else-if": "error",                        // https://eslint.vuejs.org/rules/no-dupe-v-else-if.html
                "vue/no-duplicate-attributes": "error",                  // https://eslint.vuejs.org/rules/no-duplicate-attributes.html
                "vue/no-export-in-script-setup": "error",                // https://eslint.vuejs.org/rules/no-export-in-script-setup.html
                "vue/no-multiple-template-root": "error",                // https://eslint.vuejs.org/rules/no-multiple-template-root.html
                "vue/no-mutating-props": "error",                        // https://eslint.vuejs.org/rules/no-mutating-props.html
                "vue/no-parsing-error": "error",                         // https://eslint.vuejs.org/rules/no-parsing-error.html
                "vue/no-ref-as-operand": "error",                        // https://eslint.vuejs.org/rules/no-ref-as-operand.html
                "vue/no-reserved-component-names": "error",              // https://eslint.vuejs.org/rules/no-reserved-component-names.html
                "vue/no-reserved-keys": "error",                         // https://eslint.vuejs.org/rules/no-reserved-keys.html
                "vue/no-reserved-props": ["error", { vueVersion: 2 }],    // https://eslint.vuejs.org/rules/no-reserved-props.html
                "vue/no-setup-props-destructure": "error",               // https://eslint.vuejs.org/rules/no-setup-props-destructure.html
                "vue/no-shared-component-data": "error",                 // https://eslint.vuejs.org/rules/no-shared-component-data.html
                "vue/no-side-effects-in-computed-properties": "error",   // https://eslint.vuejs.org/rules/no-side-effects-in-computed-properties.html
                "vue/no-template-key": "error",                          // https://eslint.vuejs.org/rules/no-template-key.html
                "vue/no-textarea-mustache": "error",                     // https://eslint.vuejs.org/rules/no-textarea-mustache.html
                "vue/no-unused-components": "error",                     // https://eslint.vuejs.org/rules/no-unused-components.html
                "vue/no-unused-vars": "error",                           // https://eslint.vuejs.org/rules/no-unused-vars.html
                "vue/no-use-computed-property-like-method": "error",     // https://eslint.vuejs.org/rules/no-use-computed-property-like-method.html
                "vue/no-use-v-if-with-v-for": "error",                   // https://eslint.vuejs.org/rules/no-use-v-if-with-v-for.html
                "vue/no-useless-template-attributes": "error",           // https://eslint.vuejs.org/rules/no-useless-template-attributes.html
                "vue/no-v-for-template-key": "error",                    // https://eslint.vuejs.org/rules/no-v-for-template-key.html
                "vue/no-v-model-argument": "error",                      // https://eslint.vuejs.org/rules/no-v-model-argument.html
                "vue/no-v-text-v-html-on-component": "error",            // https://eslint.vuejs.org/rules/no-v-text-v-html-on-component.html
                "vue/require-component-is": "error",                     // https://eslint.vuejs.org/rules/require-component-is.html
                "vue/require-prop-type-constructor": "error",            // https://eslint.vuejs.org/rules/require-prop-type-constructor.html
                "vue/require-render-return": "error",                    // https://eslint.vuejs.org/rules/require-render-return.html
                "vue/require-v-for-key": "error",                        // https://eslint.vuejs.org/rules/require-v-for-key.html
                "vue/require-valid-default-prop": "error",               // https://eslint.vuejs.org/rules/require-valid-default-prop.html
                "vue/return-in-computed-property": "error",              // https://eslint.vuejs.org/rules/return-in-computed-property.html
                "vue/return-in-emits-validator": "error",                // https://eslint.vuejs.org/rules/return-in-emits-validator.html
                "vue/use-v-on-exact": "error",                           // https://eslint.vuejs.org/rules/use-v-on-exact.html
                "vue/valid-attribute-name": "error",                     // https://eslint.vuejs.org/rules/valid-attribute-name.html
                "vue/valid-define-emits": "error",                       // https://eslint.vuejs.org/rules/valid-define-emits.html
                "vue/valid-define-props": "error",                       // https://eslint.vuejs.org/rules/valid-define-props.html
                "vue/valid-model-definition": "error",                   // https://eslint.vuejs.org/rules/valid-model-definition.html
                "vue/valid-next-tick": "error",                          // https://eslint.vuejs.org/rules/valid-next-tick.html
                "vue/valid-template-root": "error",                      // https://eslint.vuejs.org/rules/valid-template-root.html
                "vue/valid-v-bind-sync": "error",                        // https://eslint.vuejs.org/rules/valid-v-bind-sync.html
                "vue/valid-v-bind": "error",                             // https://eslint.vuejs.org/rules/valid-v-bind.html
                "vue/valid-v-cloak": "error",                            // https://eslint.vuejs.org/rules/valid-v-cloak.html
                "vue/valid-v-else-if": "error",                          // https://eslint.vuejs.org/rules/valid-v-else-if.html
                "vue/valid-v-else": "error",                             // https://eslint.vuejs.org/rules/valid-v-else.html
                "vue/valid-v-for": "error",                              // https://eslint.vuejs.org/rules/valid-v-for.html
                "vue/valid-v-html": "error",                             // https://eslint.vuejs.org/rules/valid-v-html.html
                "vue/valid-v-if": "error",                               // https://eslint.vuejs.org/rules/valid-v-if.html
                "vue/valid-v-model": "error",                            // https://eslint.vuejs.org/rules/valid-v-model.html
                "vue/valid-v-on": "error",                               // https://eslint.vuejs.org/rules/valid-v-on.html
                "vue/valid-v-once": "error",                             // https://eslint.vuejs.org/rules/valid-v-once.html
                "vue/valid-v-pre": "error",                              // https://eslint.vuejs.org/rules/valid-v-pre.html
                "vue/valid-v-show": "error",                             // https://eslint.vuejs.org/rules/valid-v-show.html
                "vue/valid-v-slot": "error",                             // https://eslint.vuejs.org/rules/valid-v-slot.html
                "vue/valid-v-text": "error",                             // https://eslint.vuejs.org/rules/valid-v-text.html
                // strongly recommended
                "vue/attribute-hyphenation": "warn",                     // https://eslint.vuejs.org/rules/attribute-hyphenation.html
                "vue/component-definition-name-casing": "warn",          // https://eslint.vuejs.org/rules/component-definition-name-casing.html
                "vue/first-attribute-linebreak": "warn",                 // https://eslint.vuejs.org/rules/first-attribute-linebreak.html
                "vue/html-closing-bracket-newline": "warn",              // https://eslint.vuejs.org/rules/html-closing-bracket-newline.html
                "vue/html-closing-bracket-spacing": "warn",              // https://eslint.vuejs.org/rules/html-closing-bracket-spacing.html
                "vue/html-end-tags": "warn",                             // https://eslint.vuejs.org/rules/html-end-tags.html
                "vue/html-indent": ["warn", 4],                          // https://eslint.vuejs.org/rules/html-indent.html
                "vue/html-quotes": "warn",                               // https://eslint.vuejs.org/rules/html-quotes.html
                "vue/html-self-closing": "warn",                         // https://eslint.vuejs.org/rules/html-self-closing.html
                "vue/max-attributes-per-line": ["warn",
                    { "singleline": { "max": 2 } }],                     // https://eslint.vuejs.org/rules/max-attributes-per-line.html
                "vue/multiline-html-element-content-newline": "warn",    // https://eslint.vuejs.org/rules/multiline-html-element-content-newline.html
                "vue/mustache-interpolation-spacing": "warn",            // https://eslint.vuejs.org/rules/mustache-interpolation-spacing.html
                "vue/no-multi-spaces": "warn",                           // https://eslint.vuejs.org/rules/no-multi-spaces.html
                "vue/no-spaces-around-equal-signs-in-attribute": "warn", // https://eslint.vuejs.org/rules/no-spaces-around-equal-signs-in-attribute.html
                "vue/no-template-shadow": "warn",                        // https://eslint.vuejs.org/rules/no-template-shadow.html
                "vue/one-component-per-file": "warn",                    // https://eslint.vuejs.org/rules/one-component-per-file.html
                "vue/prop-name-casing": "warn",                          // https://eslint.vuejs.org/rules/prop-name-casing.html
                "vue/require-default-prop": "warn",                      // https://eslint.vuejs.org/rules/require-default-prop.html
                "vue/require-prop-types": "warn",                        // https://eslint.vuejs.org/rules/require-prop-types.html
                "vue/singleline-html-element-content-newline": "warn",   // https://eslint.vuejs.org/rules/singleline-html-element-content-newline.html
                "vue/v-bind-style": "warn",                              // https://eslint.vuejs.org/rules/v-bind-style.html
                "vue/v-on-style": "warn",                                // https://eslint.vuejs.org/rules/v-on-style.html
                "vue/v-slot-style": "warn",                              // https://eslint.vuejs.org/rules/v-slot-style.html
                // recommended
                "vue/attributes-order": "warn",                          // https://eslint.vuejs.org/rules/attributes-order.html
                "vue/component-tags-order": "warn",                      // https://eslint.vuejs.org/rules/component-tags-order.html
                "vue/no-lone-template": "warn",                          // https://eslint.vuejs.org/rules/no-lone-template.html
                "vue/no-multiple-slot-args": "warn",                     // https://eslint.vuejs.org/rules/no-multiple-slot-args.html
                "vue/no-v-html": "warn",                                 // https://eslint.vuejs.org/rules/no-v-html.html
                "vue/order-in-components": "warn",                       // https://eslint.vuejs.org/rules/order-in-components.html
                "vue/this-in-template": "warn"                           // https://eslint.vuejs.org/rules/this-in-template.html
            }
        },
    ],
};

const prettierDisabledConfig = {
    "overrides": [
        {
            "files": ["*.js", "*.jsx", "*.mjs", "*.cjs", "*.ts", "*.tsx", ".vue"],
            "rules": {
                // Turn off Prettier's one rule
                "prettier-vue/prettier": "off",
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
                "indent": ["warn", 2],                                             // https://eslint.org/docs/rules/indent
                "quotes": ["warn", "single",
                    { "avoidEscape": true, "allowTemplateLiterals": true }],  // https://eslint.org/docs/rules/quotes
                "semi": "warn",                                               // https://eslint.org/docs/rules/semi
            },
        },
        {
            "files": ["*.ts", "*.tsx", ".vue"],
            "rules": {
                "@typescript-eslint/comma-dangle": ["warn", "always-multiline"], // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/comma-dangle.md
                // The indent rule comes with a warning from its author but seems to work OK usually https://github.com/typescript-eslint/typescript-eslint/issues/1824
                "@typescript-eslint/indent": ["warn", 2],                            // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/indent.md
                "@typescript-eslint/member-delimiter-style": "warn",             // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/member-delimiter-style.md
                "@typescript-eslint/quotes": ["warn", "single",
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