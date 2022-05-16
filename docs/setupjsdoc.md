# Set Up for JSDoc Plugin

## Overview

ESLint has a [plugin to lint JSDoc comments in JavaScript code](https://www.npmjs.com/package/eslint-plugin-jsdoc).  The instructions below show how to set this up so it can be used in the TypeScript Analyzer.

## Instructions

1. Create a new Blank Node.js Console Application (JavaScript)
2. Doubleclick package.json in Solution Explorer to edit it.  You need to create a new devDependencies entry at the same level as "author".  So just replace the `"author": {"name": ""},` entry with what's below.  These are the dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the new plugin, eslint-plugin-jsdoc:
``` json
"author": {
    "name": ""
},
"devDependencies": {
    "@types/node": "16.11.35",
    "@typescript-eslint/eslint-plugin": "5.23.0",
    "@typescript-eslint/parser": "5.23.0",
    "eslint": "8.15.0",
    "eslint-plugin-prettier": "4.0.0",
    "prettier": "2.6.2",
    "typescript": "4.6.4",
    "eslint-plugin-jsdoc": "39.2.9"
}
```
3. Install these npm packages by rightclicking 'npm' in Solution Explorer and running 'Install npm Packages' in VS2019 or VS2022, or 'Install Missing npm Packages' in VS2017.
4. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project.  To do this rightclick the project name, Add/New File..., enter .eslintrc.js, and click OK.
5. Copy the [file contents on this link](setupjsdocconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the JSDoc plugin.  The actual changes made are [detailed at the end of this article](setupjsdoc.md#changesmadetodefaultconfig).
6. Test the rules work.  Open app.js, and replace the code with the code below.  This is taken from the [plugin docs](https://www.npmjs.com/package/eslint-plugin-jsdoc#rules), and is the first example of code that fails with the plugin enabled.

``` javascript
/**
 * @access foo
 */
function quux (foo) {

}
```
You should get a jsdoc/check-access warning 'Missing valid JSDoc @access level' in the Error List, along with other errors and warnings.  This will also appear if you hover over the first character in the line ` * @access foo` in the code window.

## <a name="changesmadetodefaultconfig"></a>Changes Made to Default Configuration File

To create the [configuration file for JSDoc](setupjsdocconfig.md) the following changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc):

1. In the first "plugins" section, which refers to JavaScript and TypeScript files, add `"jsdoc"`. 
2. Immediately after the plugins section add `"extends": ["plugin:jsdoc/recommended"],`.

After these changes this part of the .eslintrc.js should look as below.  The full completed file should look like [the code on this link](setupjsdocconfig.md):
``` javascript
    "plugins": [
        "@typescript-eslint",
        "prettier",
        "jsdoc"
    ],
    "extends": ["plugin:jsdoc/recommended"],
```