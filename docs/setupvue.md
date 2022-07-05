# Set Up for Vue Plugin

## Overview

ESLint has a plugin for linting and fixing in Vue projects, [eslint-plugin-vue](https://eslint.vuejs.org/).  As the [documentation says](https://eslint.vuejs.org/), 'this plugin allows us to check the `<template>` and `<script>` of .vue files with ESLint, as well as Vue code in `.js` files'.

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  Instructions on how to do this for the Vue projects in Visual Studio are below.

## Vue and Visual Studio Projects

Visual Studio has a couple of Vue project templates:

1. Standalone JavaScript/TypeScript Vue Project.  This project template is only available in Visual Studio 2022.  The project created here is Vue 3.
2. Basic vue.js Web Application, using either JavaScript or TypeScript.  This project template is available in Visual Studio 2017, 2019 and 2022.  This creates a Vue 2 project. The latest and now default version of Vue is Vue 3.  

## Standalone JavaScript/TypeScript Vue Project in Visual Studio 2022

If you create a Standalone TypeScript Vue Project or Standalone JavaScript Vue Project in Visual Studio 2022 then ESLint is installed and linting is available.  The eslint-plugin-vue plugin is installed as part of the project creation.  ESLint is configured from an 'eslintConfig' property in package.json.  If you run `npm run lint` from a terminal in the root of the project ESLint will run and show any errors.

**The TypeScript Analyzer can use this ESLint configuration, and show linting errors in the code windows and Error List.**

**To enable this** go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',vue' to the existing list. After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,vue'.

**To test this is working**, open file src/App.vue.  On lines 12-14 currently there is a 'components' property of an object defined, with value `{ HelloWorld }`.  If you change the name here to, say, `HelloVueWorld` then you should see linting errors for the file including a [vue/no-unused-components error which comes from the plugin](https://eslint.vuejs.org/rules/no-unused-components.html). Errors in the template or style section will also be shown correctly.

Note that clicking the link in the Code column in the Error List should take you to the help page for this error. 

## Enabling Prettier for Standalone Vue Projects

Prettier is **not** enabled by default in a Standalone JavaScript/TypeScript project.  We can enable it by following the steps below, which extend the existing configuration:

1. Doubleclick package.json in Solution Explorer to edit it.  Find the "eslintconfig" property.  This is the configuration for ESLint which the TypeScript Analyzer usually has in a .eslintrc.js file.  Replace the entire property and value with the code below:
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
2. Still in package.json, add the dependencies below into the devDependencies section and save. Add them after the existing devDependencies. These are the additional npm package dependencies that the TypeScript Analyzer needs to get Prettier to run:
``` json
    "eslint-plugin-prettier": "4.0.0",
    "prettier": "2.6.2"
```
3. Install the new npm packages. Rightclick the project in Solution Explorer/Open in Terminal, then in the terminal that appears execute the command `npm i`.

Now if you lint a .vue file in the project you will probably get some Prettier warnings.  As usual, you can rightclick/Fix TypeScript Analyzer (ESLint) Errors to format the file and remove these errors.

### Full Configuration