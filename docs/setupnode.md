# Set Up for Node Plugin

## Overview

ESLint has a [plugin to lint and fix Node.js code in JavaScript and TypeScript files](https://github.com/mysticatea/eslint-plugin-node#readme).  

We can set up the TypeScript Analyzer to use this plugin on JavaScript and TypeScript files in Visual Studio projects.  Errors will be shown in the Visual Studio Error List and underlined in the code window.   This means setting up a [local install](creatinglocalinstall.md) with the plugin, and a [configuration file](configuration.md) with appropriate rules enabled.

Detailed instructions on how to do this in a Node.js Console application are below, separately for [TypeScript](setupnode.md#typescript) and [JavaScript](setupnode.md#javascript) projects.

## <a name="typescript"></a>Instructions for TypeScript

1. Create a new Blank Node.js Console Application (TypeScript).
2. Doubleclick package.json in Solution Explorer to edit it.  Replace the existing devDependencies section with the code below and save.  These are the dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the new plugin, eslint-plugin-node:
``` json
"devDependencies": {
    "@types/node": "18.11.18",
    "@typescript-eslint/eslint-plugin": "5.48.1",
    "@typescript-eslint/parser": "5.48.1",
    "eslint": "8.31.0",
    "eslint-plugin-prettier": "4.2.1",
    "prettier": "2.8.2",
    "typescript": "4.9.4",
    "eslint-plugin-node": "11.1.0"
}
```
3. If the package.json contains an eslintConfig section you can optionally remove this entire section.  If you create a local configuration file as described below it will override this in any case, but it can be distracting to have unused configuration in your project.  You will only have a eslintConfig section if you are using Visual Studio 2022 v17.4 or later.
4. Install the npm packages by rightclicking 'npm' in Solution Explorer and running 'Install npm Packages' in VS2019 or VS2022, or 'Install Missing npm Packages' in VS2017.
5. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project.  To do this rightclick the project name, Add/New File..., enter .eslintrc.js, and click OK.
6. Copy the [file contents on this link](setupnodeconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the Node.js rules in the plugin.  The actual changes made are [detailed at the end of this article](setupnode.md#changesmadetodefaultconfig).
7. Test the rules work.  Open app.ts, and add the code below.  This is taken from the [plugin docs](https://github.com/mysticatea/eslint-plugin-node) for the [no-exports-assign rule](https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-exports-assign.md):
``` javascript
  exports = {};
```
You should get a node/no-exports-assign error "Unexpected assignment to 'exports' variable. Use 'module.exports' instead." in the Error List.  This will also appear if you hover over the line above in the code window.  Note that as usual clicking the link in the Code column in the Error List should take you to the help page for this error. 

## <a name="javascript"></a>Instructions for JavaScript

1. Create a new Blank Node.js Console Application (JavaScript).
2. Edit package.json.  You need to create a new devDependencies entry at the same level as "author".  So just replace the `"author": {"name": ""},` entry with what's below and save.  These are the dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the new plugin, eslint-plugin-node:
``` json
"author": {
    "name": ""
},
"devDependencies": {
    "@types/node": "18.11.18",
    "@typescript-eslint/eslint-plugin": "5.48.1",
    "@typescript-eslint/parser": "5.48.1",
    "eslint": "8.31.0",
    "eslint-plugin-prettier": "4.2.1",
    "prettier": "2.8.2",
    "typescript": "4.9.4",
    "eslint-plugin-node": "11.1.0"
}
```
3. Install these npm packages by rightclicking 'npm' in Solution Explorer and running 'Install npm Packages' in VS2019 or VS2022, or 'Install Missing npm Packages' in VS2017.
4. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project.  To do this rightclick the project name, Add/New File..., enter .eslintrc.js, and click OK.
5. Copy the [file contents on this link](setupnodeconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the Node.js rules in the plugin.  The actual changes made are [detailed at the end of this article](setupnode.md#changesmadetodefaultconfig).
6. Test the rules work.  In app.js paste in the code below.  These are taken from the [plugin docs](https://github.com/mysticatea/eslint-plugin-node) for the [no-exports-assign rule](https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-exports-assign.md) and the [no-missing-import](https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-missing-import.md) rule:
``` javascript
import typoFile from "./typo-file";
exports = {};
```
You should get a node/no-exports-assign error "Unexpected assignment to 'exports' variable. Use 'module.exports" instead.' in the Error List.  This will also appear if you hover over the `exports = {};` line above in the code window.  You should also get node/no-missing-import and node/no-unsupported-features/es-syntax errors for the `import typoFile from "./typo-file";` line.  Note that as usual clicking the link in the Code column in the Error List should take you to the help page for these errors.

## <a name="changesmadetodefaultconfig"></a>Changes Made to Default Configuration File

To create the [configuration file for Node](setupnodeconfig.md) the following changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc).  This is more complicated than for [other plugins](examples.md) because we have one rule, node/no-missing-import, that doesn't work in TypeScript, and one rule, node/no-unsupported-features/es-syntax, that needs to be configured differently for TypeScript.  There is some discussion of this in [issues on the node-eslint-plugin GitHub site](https://github.com/mysticatea/eslint-plugin-node/issues/236).

1. In the first "plugins" section, which refers to JavaScript and TypeScript files, add `"node",`. After these changes this part of the .eslintrc.js should look as below:
``` javascript
    "plugins": [
        "@typescript-eslint",
        "prettier",
        "node",
    ],
```
2. Add the rules below into the rules section of the JavaScript/TypeScript override.  That is, add them to the first "rules:" object in the file, after the prettier/prettier rule.
``` javascript
    // node plugin recommended rules, apply to JavaScript and TypeScript
    "node/no-deprecated-api": "error",                     // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-deprecated-api.md
    "node/no-exports-assign": "error",                     // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-exports-assign.md
    "node/no-extraneous-import": "error",                  // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-extraneous-import.md
    "node/no-extraneous-require": "error",                 // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-extraneous-require.md
    "node/no-missing-require": "error",                    // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-missing-require.md
    "node/no-unpublished-bin": "error",                    // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unpublished-bin.md
    "node/no-unpublished-import": "error",                 // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unpublished-import.md
    "node/no-unpublished-require": "error",                // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unpublished-require.md
    "node/no-unsupported-features/es-builtins": "error",   // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unsupported-features/es-builtins.md
    "node/no-unsupported-features/node-builtins": "error", // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unsupported-features/node-builtins.md
    "node/process-exit-as-throw": "error",                 // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/process-exit-as-throw.md
    "node/shebang": "error",                               // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/shebang.md
```
3. Add the rules below into the rules section of the JavaScript-only rules override.  That is, add them to the second "rules:" object in the file, after the valid-typeof rule:
``` javascript
    // node plugin recommended rules - no-missing-import doesn't work with normal TypeScript import/export syntax, so it's in
    // the JavaScript-only section, no-unsupported-features/es-syntax needs different configuration for TypeScript, so in both
    "node/no-missing-import": "error",        // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-missing-import.md
    "node/no-unsupported-features/es-syntax": "error",  // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unsupported-features/es-syntax.md
```
4. Add the rules below into the rules section of the TypeScript-only rules override.  That is, add them to the third "rules:" object in the file, after the @typescript-eslint/unbound-method rule.
``` javascript
    // node plugin recommended rules - https://github.com/mysticatea/eslint-plugin-node/issues/236
    "node/no-unsupported-features/es-syntax": ["error", { "ignores": ["modules"] }],  // https://github.com/mysticatea/eslint-plugin-node/blob/master/docs/rules/no-unsupported-features/es-syntax.md
```
Once all this is done the final .eslintrc.js file should then be as [shown on this link](setupnodeconfig.md).