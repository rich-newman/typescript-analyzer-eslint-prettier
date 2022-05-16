# Configuration

## Default Configuration

The TypeScript Analyzer will run after installation with no additional configuration necessary to get it working.

Of course [ESLint has quite complex configuration options](https://eslint.org/docs/user-guide/configuring/).  One of the aims of the TypeScript Analyzer is to allow you to lint and format without necessarily having to understand all that.

To achieve this the TypeScript Analyzer uses a [default configuration file](defaultconfig.md) (.eslintrc.js) in its own folder (c:\Users\\{username\}\TypeScriptAnalyzerConfig).  This configuration file is used for all Visual Studio linting and formatting by the TypeScript Analyzer unless you override it by providing your own file locally for a specific solution or project or even folder.

The **default configuration file can be viewed and edited in Visual Studio** via a menu option: Tools/TypeScript Analyzer (ESLint)/Edit Default Config.

## Enabling and Disabling Rules

In the default configuration file all rules are listed separately, with a link to the associated documentation.  Here you can disable a rule you don't like.  You can also change a rule from generating errors so that it only generates warnings.

For example, let's say you don't like the [no-debugger rule](https://eslint.org/docs/rules/no-debugger), which by default generates errors if you put a `debugger` statement in your code.  To change this:

- Bring up the default configuration file in the Visual Studio editor using Tools/TypeScript Analyzer (ESLint)/Edit Default Config.
- Search for 'no-debugger'.  By default the entry in the file looks as below:
```js
"no-debugger": "error",                   // https://eslint.org/docs/rules/no-debugger
```
- Change `"error"` to either `"warn"` or `"off"` depending on how you want the rule to work in the future.

If a **rule has possible options** then the ESLint syntax is to replace "error"/"warn"/"off" with an array whose first element is the old "error"/"warn"/"off" setting, and subsequent elements are the options.  This is just how ESLint expects configuration files to work.  For example, the [eqeqeq rule](https://eslint.org/docs/rules/eqeqeq) has a ['smart' option](https://eslint.org/docs/rules/eqeqeq#smart), and we'd set that with code `"eqeqeq": ["error", "smart"]`.

## Enabling and Disabling Prettier 

See the documentation on Prettier for how to [disable Prettier and fall back to simpler formatting rules](formatting.md#disableprettier), or to [disable formatting altogether](formatting.md#disableformatting).

## Adding a Rule

You may want to add a rule that isn't in the set of recommended rules.

For example the TypeScript ESLint plugin has an ['array-type' rule](https://github.com/typescript-eslint/typescript-eslint/blob/HEAD/packages/eslint-plugin/docs/rules/array-type.md).  This controls what syntax you can use for declaring array types.  This rule applies to TypeScript type annotations, which means that it won't work in JavaScript.  So we want to set it up to apply to TypeScript files only.

To add this rule:

- Bring up the default configuration file in the Visual Studio editor using Tools/TypeScript Analyzer (ESLint)/Edit Default Config.
- Find the 'TypeScript-only rules' section.  This is the one that contains the line `"files": ["*.ts", "*.tsx"],`. In the list of rules in that section, after the @typescript-eslint/triple-slash-reference rule, add the code below

`"@typescript-eslint/array-type": "warn",`

- In a TypeScript file in your project add the code `const x: Array<string> = ['a', 'b'];`, which the documentation says is [incorrect code](https://github.com/typescript-eslint/typescript-eslint/blob/HEAD/packages/eslint-plugin/docs/rules/array-type.md#-incorrect) under the rule.  You should see 'Array<string>' underlined in green caused by the new rule, and more details if you hover over it.

## Restoring the Default Configuration File

You can restore the default configuration file to its original contents, as well as resetting any changes to the Tools/Options/TypeScript Analyzer/ESLint settings, with the menu option Tools/TypeScript Analyzer (ESLint)/Reset TypeScript Analyzer (ESLint) Settings...

## Local Configuration

You can override the default configuration file by providing a configuration file locally in any of the formats that ESLint supports.

[Information on providing your own local configuration is available in a separate article.](localconfiguration.md)

## Ignoring Files

You can configure the TypeScript Analyzer to ignore files in the same way that ESLint does.  [More documentation on this is also available.](ignoringfiles.md)