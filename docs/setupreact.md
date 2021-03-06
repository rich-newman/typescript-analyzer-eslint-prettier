# Set Up for React Plugin

## Overview

To lint a React project we need to set up the [React plugin for ESLint](https://github.com/jsx-eslint/eslint-plugin-react).  For the TypeScript Analyzer this means setting up a [local install](creatinglocalinstall.md) with the plugin, and a [configuration file](configuration.md) with appropriate rules enabled.

Detailed instructions on how to do this in a Node.js Console application are below, separately for [TypeScript](setupreact.md#typescript) and [JavaScript](setupreact.md#javascript) projects.

## <a name="typescript"></a>Instructions for TypeScript

1. Create a new Blank Node.js Console Application (TypeScript).
2. Doubleclick package.json in Solution Explorer to edit it.  Replace the existing devDependencies section with the code below and save.  These are the dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the new plugin, eslint-plugin-react:
``` json
"devDependencies": {
    "@types/node": "16.11.43",
    "@typescript-eslint/eslint-plugin": "5.30.5",
    "@typescript-eslint/parser": "5.30.5",
    "eslint": "8.19.0",
    "eslint-plugin-prettier": "4.2.1",
    "prettier": "2.7.1",
    "typescript": "4.7.4",
    "eslint-plugin-react": "7.30.1"
}
```
3. Install these npm packages by rightclicking 'npm' in Solution Explorer and running 'Install npm Packages' in VS2019 or VS2022, or 'Install Missing npm Packages' in VS2017.
4. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project.  To do this rightclick the project name, Add/New File..., enter .eslintrc.js, and click OK.
5. Copy the [file contents on this link](setupreactconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable React.  The actual changes made are [detailed at the end of this article](setupreact.md#changesmadetodefaultconfig).
6. Test the rules work.  Add a new 'TypeScript JSX File' to the project and add the code below to that.  To do this rightclick the project in Solution Explorer, Add/New Item..., 'TypeScript JSX File', and accept the default name by clicking the Add button.
``` jsx
function Hello({ name }) {
  return <div>Hello {name}</div>;
}
```
You should get a react/prop-types error in the Error List, with underlining on the first 'name'.

## <a name="javascript"></a>Instructions for JavaScript

1. Create a new Blank Node.js Console Application (JavaScript).
2. Edit package.json.  You need to create a new devDependencies entry at the same level as "author".  So just replace the `"author": {"name": ""},` entry with what's below and save.  These are the dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the new plugin, eslint-plugin-react:
``` json
"author": {
    "name": ""
},
"devDependencies": {
    "@types/node": "16.11.43",
    "@typescript-eslint/eslint-plugin": "5.30.5",
    "@typescript-eslint/parser": "5.30.5",
    "eslint": "8.19.0",
    "eslint-plugin-prettier": "4.2.1",
    "prettier": "2.7.1",
    "typescript": "4.7.4",
    "eslint-plugin-react": "7.30.1"
}
```
3. Install these npm packages by rightclicking 'npm' in Solution Explorer and running 'Install npm Packages' in VS2019 or VS2022, or 'Install Missing npm Packages' in VS2017.
4. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project.  To do this rightclick the project name, Add/New File..., enter .eslintrc.js, and click OK.
5. Copy the [file contents on this link](setupreactconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable React.  The actual changes made are [detailed at the end of this article](setupreact.md#changesmadetodefaultconfig).
6. Test the rules work.  In app.js paste in the code below:
``` tsx
function Hello({ name }) {
  return <div>Hello {name}</div>;
}
```
You should get a react/prop-types error in the Error List, with underlining on the first 'name'.

## <a name="changesmadetodefaultconfig"></a>Changes Made to Default Configuration File

To create the [configuration file for React](setupreactconfig.md) the following changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc):

1. In the first "plugins" section, which refers to JavaScript and TypeScript files, add `"react"`. Immediately after the plugins section add `"settings": { "react": { "version": "detect" } },`.  After these changes this part of the .eslintrc.js should look as below:
``` javascript
    "plugins": [
        "@typescript-eslint",
        "prettier",
        "react"
    ],
    "settings": { "react": { "version": "detect" } },
```
2. Add the rules below into the rules section of the JavaScript/TypeScript override.  That is, add them to the first "rules:" object in the file, after the prettier/prettier rule.  The final .eslintrc.js file should then be as [shown on this link](setupreactconfig.md).  Make sure you save it:
``` javascript
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

    // Some additional React rules - MS enable these in the default ESLint config
    "react/no-danger": "warn",                // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-danger.md
    "react/no-did-mount-set-state": "warn",   // https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/no-did-mount-set-state.md               
```