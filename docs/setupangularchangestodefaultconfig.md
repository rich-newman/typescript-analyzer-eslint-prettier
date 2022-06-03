# Changes Made to Default Configuration File for Angular

To create the [configuration file for Angular](setupangularconfig.md) the following changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc):

1. The section below was added as an element in the first overrides array, after the 'TypeScript-only Rules' entry.  This enables the linting rules for HTML files.
``` javascript
    {
      "files": ["*.html"],
      "parser": "@angular-eslint/template-parser",
      "plugins": ["@angular-eslint/template"],
      // These rules apply to both .html files AND .ts/.tsx files with inline templates, because of the 'extends' in the .ts override
      "rules": {
        // List of all angular-eslint plugin template rules: https://github.com/angular-eslint/angular-eslint/tree/master/packages/eslint-plugin-template/docs/rules
        // https://github.com/angular-eslint/angular-eslint/blob/65afe1c9d8437562803c90c1dc648ec4b9db3e72/packages/eslint-plugin-template/src/configs/recommended.json
        "@angular-eslint/template/banana-in-box": "error",    // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin-template/docs/rules/banana-in-box.md
        "@angular-eslint/template/eqeqeq": "error",           // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin-template/docs/rules/eqeqeq.md
        "@angular-eslint/template/no-negated-async": "error", // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin-template/docs/rules/no-negated-async.md
      }
    }
```
2. In the TypeScript-only rules entry plugins and extends entries were added as below, after the "files" property:
``` javascript
      "plugins": ["@angular-eslint",],
      // Make @angular-eslint use the rules in the .html section below to lint HTML inline templates in .ts files
      "extends": ["plugin:@angular-eslint/template/process-inline-templates"],
```
3. In the TypeScript-only rules entry the rules below were added:
``` javascript
        // List of all angular-eslint plugin rules: https://github.com/angular-eslint/angular-eslint/tree/master/packages/eslint-plugin/docs/rules
        // https://github.com/angular-eslint/angular-eslint/blob/f92b184c5b0b57328d0a323ac8c89f1b3017b8d4/packages/eslint-plugin/src/configs/recommended.json
        "@angular-eslint/component-class-suffix": "error",       // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/component-class-suffix.md
        "@angular-eslint/contextual-lifecycle": "error",         // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/contextual-lifecycle.md
        "@angular-eslint/directive-class-suffix": "error",       // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/directive-class-suffix.md
        "@angular-eslint/no-conflicting-lifecycle": "error",     // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-conflicting-lifecycle.md
        "@angular-eslint/no-empty-lifecycle-method": "error",    // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-empty-lifecycle-method.md
        "@angular-eslint/no-host-metadata-property": "error",    // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-host-metadata-property.md
        "@angular-eslint/no-input-rename": "error",              // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-input-rename.md
        "@angular-eslint/no-inputs-metadata-property": "error",  // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-inputs-metadata-property.md
        "@angular-eslint/no-output-native": "error",             // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-output-native.md
        "@angular-eslint/no-output-on-prefix": "error",          // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-output-on-prefix.md
        "@angular-eslint/no-output-rename": "error",             // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-output-rename.md
        "@angular-eslint/no-outputs-metadata-property": "error", // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/no-outputs-metadata-property.md
        "@angular-eslint/use-lifecycle-interface": "warn",       // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/use-lifecycle-interface.md
        "@angular-eslint/use-pipe-transform-interface": "error", // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/use-pipe-transform-interface.md
        "@angular-eslint/directive-selector": [
          "error",
          {
            "type": "attribute",
            "prefix": "app",
            "style": "camelCase"
          }
        ],                                                       // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/directive-selector.md
        "@angular-eslint/component-selector": [
          "error",
          {
            "type": "element",
            "prefix": "app",
            "style": "kebab-case"
          }
        ],                                                       // https://github.com/angular-eslint/angular-eslint/blob/master/packages/eslint-plugin/docs/rules/component-selector.md

```
4. In the same section, after the rules above, the [@angular-eslint extra recommended rules](https://github.com/angular-eslint/angular-eslint/blob/f92b184c5b0b57328d0a323ac8c89f1b3017b8d4/packages/eslint-plugin/src/configs/recommended--extra.json) for TypeScript files were added.  Where these rules were already in force in the file with a different configuration the original rule entry was commented out.
``` javascript
       "no-restricted-imports": [
          "error",
          {
            "paths": [
              {
                "name": "rxjs/Rx",
                "message": "Please import directly from 'rxjs' instead"
              }
            ]
          }
        ], // https://eslint.org/docs/rules/no-restricted-imports
        "@typescript-eslint/member-ordering": [
          "error",
          {
            "default": [
              "static-field",
              "instance-field",
              "static-method",
              "instance-method"
            ]
          }
        ], // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/member-ordering.md
        "no-restricted-syntax": [
          "error",
          {
            "selector": "CallExpression[callee.object.name=\"console\"][callee.property.name=/^(debug|info|time|timeEnd|trace)$/]",
            "message": "Unexpected property on console object was called"
          }
        ], // https://eslint.org/docs/rules/no-restricted-syntax
        "@typescript-eslint/no-inferrable-types": [
          "error",
          { "ignoreParameters": true }
        ], // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-inferrable-types.md
        "@typescript-eslint/no-non-null-assertion": "error",  // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/no-non-null-assertion.md
        "no-fallthrough": "error" // https://eslint.org/docs/rules/no-fallthrough
```
5. The configuration for the prettier/prettier rule was adjusted to fit with Angular's two-character indents, single quotes, no dangling commas and line endings.
``` javascript
        "prettier/prettier": ["warn", {
          "tabWidth": 2,
          "endOfLine": "auto",
          "printWidth": 80,
          "semi": true,
          "singleQuote": true,
          "quoteProps": "as-needed",
          "jsxSingleQuote": false,
          "trailingComma": "none",
          "bracketSpacing": true,
          "arrowParens": "always",
        }],
```
6.  The configuration if Prettier is disabled and ESLint used for formatting was also updated to reflect Angular settings described in 5 above:
``` javascript
    {
      "files": ["*.js", "*.jsx", "*.mjs", "*.cjs"],
      "rules": {
        "comma-dangle": ["warn", "never"],                 // https://eslint.org/docs/rules/comma-dangle
        "indent": ["warn", 2],                                        // https://eslint.org/docs/rules/indent
        "quotes": ["warn", "single",
          { "avoidEscape": true, "allowTemplateLiterals": true }],  // https://eslint.org/docs/rules/quotes
        "semi": "warn",                                               // https://eslint.org/docs/rules/semi
      },
    },
    {
      "files": ["*.ts", "*.tsx"],
      "rules": {
        "@typescript-eslint/comma-dangle": ["warn", "never"], // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/comma-dangle.md
        // The indent rule comes with a warning from its author but seems to work OK usually https://github.com/typescript-eslint/typescript-eslint/issues/1824
        "@typescript-eslint/indent": ["warn", 2],                        // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/indent.md
        "@typescript-eslint/member-delimiter-style": "warn",             // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/member-delimiter-style.md
        "@typescript-eslint/quotes": ["warn", "single",
          { "avoidEscape": true, "allowTemplateLiterals": true }],     // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/quotes.md
        "@typescript-eslint/semi": "warn",                               // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/semi.md
        "@typescript-eslint/type-annotation-spacing": "warn",            // https://github.com/typescript-eslint/typescript-eslint/blob/master/packages/eslint-plugin/docs/rules/type-annotation-spacing.md
      },
    },
```
The completed file should look like [the code on this link](setupangularconfig.md).