# Set Up for Vue Plugin

## Overview

ESLint has a plugin for linting and fixing in Vue projects, [eslint-plugin-vue](https://eslint.vuejs.org/).  As the [documentation says](https://eslint.vuejs.org/), 'this plugin allows us to check the `<template>` and `<script>` of .vue files with ESLint, as well as Vue code in `.js` files'.

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  Instructions on how to do this for the Vue projects in Visual Studio are below.

## Vue and Visual Studio Projects

Visual Studio has several Vue project templates, of two basic types:

1. 'Standalone JavaScript Vue Project' and 'Standalone TypeScript Vue Project'.  These project templates are only available in Visual Studio 2022.  The project created here is Vue 3.
2. 'Basic vue.js Web Application'.  These project templates available in Visual Studio 2017, 2019 and 2022, and in JavaScript or TypeScript versions, although there's no TypeScript version in Visual Studio 2022.  All of these templates are different, but similar obviously.  The JavaScript template in Visual Studio 2022 is Vue 3, and the rest are Vue 2.

Enabling the TypeScript Analyzer for the Standalone JavaScript/TypeScript Vue Projects is relatively straightforward and is described below.

The situation is more complex for Basic vue.js Web Applications.  Setting up the TypeScript Analyzer for these project types is described on a [separate page](setupvuebasicwebapp.md).

## Standalone JavaScript/TypeScript Vue Project in Visual Studio 2022

### Using the Default Linting from a Terminal

If in Visual Studio 2022 you create a Standalone JavaScript Vue Project or a Standalone TypeScript Vue Project then linting from a terminal window is set up for you as part of the project creation. 

ESLint and the eslint-plugin-vue plugin are installed for you, and ESLint is configured via an 'eslintConfig' property in package.json.  

If you run `npm run lint` from a terminal in the root of the project ESLint will run and show any errors.

### Enabling TypeScript Analyzer To Use the Default Linting

**The TypeScript Analyzer can use this ESLint configuration, and show linting errors in the code windows and Error List.**

**To enable this** you need to add .vue files to the list of files the Analyzer handles.  To do this go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',vue' to the existing list. After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,vue'.  All other settings on this screen should be set to the defaults.  In particular settings 'Enable local config' and 'Enable local node_modules' should be set to True.

**To test this is working**, in a Standalone JavaScript/TypeScript Vue Project open file src/App.vue.  On lines 12-14 currently there is a 'components' property of an object defined, with value `{ HelloWorld }`.  If you change the name here to, say, `HelloVueWorld` then you should see linting errors for the file including a [vue/no-unused-components error which comes from the plugin](https://eslint.vuejs.org/rules/no-unused-components.html). Errors in the template or style section will also be shown correctly.

Note that clicking the link in the Code column in the Error List should take you to the help page for this error. 

### Enabling Prettier

Prettier is **not** enabled by default in a Standalone JavaScript/TypeScript project.  We can enable it by following the steps below, which extend the existing configuration.

Please note you may not want to do this: Prettier doesn't work too well with .vue files, and the plugins that attempt to mitigate this have [issues on Windows](https://github.com/meteorlxy/eslint-plugin-prettier-vue/issues/29).

1. Doubleclick package.json in Solution Explorer to edit it.  Find the "eslintconfig" property.  This is the configuration for ESLint which the TypeScript Analyzer usually has in a .eslintrc.js file.  Replace the entire property and value with the code below.
For a Standalone **JavaScript** Vue Project:
``` json
"eslintConfig": {
    "root": true,
    "env": {
      "node": true
    },
    "plugins": [
      "prettier"
    ],
    "extends": [
      "plugin:vue/vue3-essential",
      "eslint:recommended"
    ],
    "parserOptions": {
      "parser": "@babel/eslint-parser"
    },
    "rules": {
      "prettier/prettier": [
        "warn",
        {
          "tabWidth": 2,
          "endOfLine": "lf",
          "printWidth": 80,
          "semi": true,
          "singleQuote": true,
          "quoteProps": "as-needed",
          "jsxSingleQuote": false,
          "trailingComma": "es5",
          "bracketSpacing": true,
          "arrowParens": "always"
        }
      ]
    }
},
```
For a Standalone **TypeScript** Vue Project:
``` json
"eslintConfig": {
    "root": true,
    "env": {
      "node": true
    },
    "plugins": [
      "prettier"
    ],
    "extends": [
      "plugin:vue/vue3-essential",
      "eslint:recommended",
      "@vue/typescript"
    ],
    "parserOptions": {
      "parser": "@typescript-eslint/parser"
    },
    "rules": {
      "prettier/prettier": [
        "warn",
        {
          "tabWidth": 2,
          "endOfLine": "lf",
          "printWidth": 80,
          "semi": true,
          "singleQuote": true,
          "quoteProps": "as-needed",
          "jsxSingleQuote": false,
          "trailingComma": "es5",
          "bracketSpacing": true,
          "arrowParens": "always"
        }
      ]
    }
},
```
2. Still in package.json, add the dependencies below to the end of the devDependencies section and save. These are the additional npm package dependencies that the TypeScript Analyzer needs to get Prettier to run:
``` json
    {{site.packageversions.eslintpluginprettier}}
    {{site.packageversions.prettier}}
```
3. Install the new npm packages. Rightclick the project in Solution Explorer/Open in Terminal, then in the terminal that appears execute the command `npm i`.

Now if you lint a .vue file in the project you will probably get some Prettier warnings.  As usual, you can rightclick/Fix TypeScript Analyzer (ESLint) Errors to format the file and remove these errors.

## Basic vue.js Web Application

Set up for the TypeScript Analyzer for Basic vue.js Web Applications is in a [separate document](setupvuebasicwebapp.md).