# Set Up for Vue Plugin

## Overview

ESLint has a plugin for linting and fixing in Vue projects, [eslint-plugin-vue](https://eslint.vuejs.org/).  As the [documentation says](https://eslint.vuejs.org/), 'this plugin allows us to check the `<template>` and `<script>` of .vue files with ESLint, as well as Vue code in `.js` files'.

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  Instructions on how to do this for the Vue projects in Visual Studio are below.

## Vue and Visual Studio Projects

Visual Studio has a couple of Vue project templates:

1. Standalone JavaScript/TypeScript Vue Project.  This project template is only available in Visual Studio 2022.  The project created here is Vue 3.
2. Basic vue.js Web Application, using either JavaScript or TypeScript.  This project template is available in Visual Studio 2017, 2019 and 2022.  This creates a Vue 2 project. The latest and now default version of Vue is Vue 3.  

## Standalone JavaScript/TypeScript Vue Project in Visual Studio 2022

### Enabling the Default Linting in the TypeScript Analyzer

If you create a Standalone Vue Project in Visual Studio 2022 then ESLint is installed and linting is available.  The eslint-plugin-vue plugin is installed as part of the project creation.  If you run `npm run lint` from a terminal in the root of the project ESLint will run and show any errors.

**The TypeScript Analyzer can use this ESLint configuration, and show linting errors in the code windows and Error List.**

**To enable this** go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',vue' to the existing list. After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,vue'.

**To test this is working**, open file src/App.vue.  On lines 12-14 currently there is a 'components' property of an object defined, with value `{ HelloWorld }`.  If you change the name here to, say, `HelloVueWorld` then you should see linting errors for the file including a [vue/no-unused-components error which comes from the plugin](https://eslint.vuejs.org/rules/no-unused-components.html).  Note that clicking the link in the Code column in the Error List should take you to the help page for this error.  Errors in the template section will also be shown correctly.

### Drawbacks of the Default Linting in the Project

There are some drawbacks to using this default linting.  The main one is that there is no Prettier, and no formatting rules are enabled at all.  Also the default lint has a limited set of rules, and which rules are enabled is not immediately obvious.

To set up your own configuration with Prettier follow the steps below.

### Full Instructions for a Standalone JavaScript/TypeScript Vue Project in Visual Studio 2022

1. Create a new Standalone TypeScript Vue Project in Visual Studio 2022.  Build the project.
2. If you haven't already, go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',vue' to the existing list. After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,vue'.
3. Doubleclick package.json in Solution Explorer to edit it. Add the dependencies below into the devDependencies and save. Add them after the existing devDependencies. These are the additional dependencies that the TypeScript Analyzer needs to get Prettier to run:
``` json
    "eslint-plugin-prettier": "4.0.0",
    "prettier": "2.6.2"
```
4. Install these npm packages. Rightclickthe project in Solution Explorer/Open in Terminal, then in the terminal that appears execute the command `npm i`.
5. Go back to package.json, and find the "eslintconfig" property.  Replace the entire property and value with the code below:
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





