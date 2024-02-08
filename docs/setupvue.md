# Set Up for Vue Plugin

## Overview

ESLint has a plugin for linting and fixing in Vue projects, [eslint-plugin-vue](https://eslint.vuejs.org/).  As the [documentation says](https://eslint.vuejs.org/), 'this plugin allows us to check the `<template>` and `<script>` of .vue files with ESLint, as well as Vue code in `.js` files'.

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  Instructions on how to do this for the Vue projects in Visual Studio are below.

## Vue and Visual Studio Projects

Visual Studio has several Vue project templates, of two basic types:

1. 'Standalone JavaScript Vue Project' and 'Standalone TypeScript Vue Project'.  These project templates are only available in Visual Studio 2022.  The project created here is Vue 3.
2. 'Basic vue.js Web Application'.  These project templates available in Visual Studio 2017, 2019 and 2022, and in JavaScript or TypeScript versions, although there's no TypeScript version in Visual Studio 2022.  All of these templates are different, but similar obviously.  The JavaScript template in Visual Studio 2022 is Vue 3, and the rest are Vue 2.

Enabling the TypeScript Analyzer for the Standalone JavaScript/TypeScript Vue Projects is described below.

None of the Basic vue.js Web Application types works at the time of writing, and as a result we recommend you don't attempt to use these templates.  More details on this are on a [separate page](setupvuebasicwebapp.md).

## Standalone JavaScript/TypeScript Vue Project in Visual Studio 2022

### Using the Default Linting from a Terminal

If in Visual Studio 2022 you create a Standalone JavaScript Vue Project or a Standalone TypeScript Vue Project then linting from a terminal window is set up for you as part of the project creation. 

ESLint and the eslint-plugin-vue plugin are installed for you, and ESLint is configured via a .eslintrc.cjs file in the root of the project.

If you run `npm run lint` from a terminal in the root of the project ESLint will run and show any errors.

### Enabling TypeScript Analyzer To Use the Default Linting

**The TypeScript Analyzer can use this ESLint configuration, and show linting errors in the code windows and Error List.**

**To enable this** you need to add .vue files to the list of files the Analyzer handles.  To do this go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',vue' to the existing list. After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,vue'.  All other settings on this screen should be set to the defaults.  In particular settings 'Enable local config' and 'Enable local node_modules' should be set to True.

You also need to have built the project at least once to ensure it works and to install the npm packages.

**To test this is working**, in a Standalone JavaScript/TypeScript Vue Project open file src/App.vue.  On line 11 currently there is a 'HelloWorld' component declared with attribute msg:

```html
      <HelloWorld msg="You did it!" />
```

An easy way to generate an error from the eslint-plugin-vue is to simply duplicate the attribute.  That is, change this code to:

```html
      <HelloWorld msg="You did it!" msg="You did it!" />
```

If you do this change you should get a [vue/no-duplicate-attributes](https://eslint.vuejs.org/rules/no-duplicate-attributes.html) and a [vue/no-parsing-error](https://eslint.vuejs.org/rules/no-parsing-error.html) error on the second msg attribute.  As usual, clicking on the Code in the Error List will take you to the documentation for these errors.

### Enabling Prettier

Prettier is **not** enabled by default in a Standalone JavaScript/TypeScript project.  We can enable it by following the steps below, which extend the existing configuration.

Please note you may not want to do this: Prettier doesn't work too well with .vue files, and the plugins that attempt to mitigate this have [issues on Windows](https://github.com/meteorlxy/eslint-plugin-prettier-vue/issues/29).

1. Doubleclick .eslintrc.cjs in Solution Explorer to edit it.  Replace the entire contents of the file with the code below.
For a Standalone **JavaScript** Vue Project:
``` javascript
/* eslint-env node */
module.exports = {
  root: true,
  extends: ['plugin:vue/vue3-essential', 'eslint:recommended'],
  env: {
    node: true,
  },
  plugins: ['prettier'],
  parserOptions: {
    ecmaVersion: 'latest',
  },
  rules: {
    'prettier/prettier': [
      'warn',
      {
        tabWidth: 2,
        endOfLine: 'lf',
        printWidth: 80,
        semi: true,
        singleQuote: true,
        quoteProps: 'as-needed',
        jsxSingleQuote: false,
        trailingComma: 'es5',
        bracketSpacing: true,
        arrowParens: 'always',
      },
    ],
  },
};
```
For a Standalone **TypeScript** Vue Project:
``` javascript
/* eslint-env node */
require('@rushstack/eslint-patch/modern-module-resolution');
module.exports = {
  root: true,
  extends: [
    'plugin:vue/vue3-essential',
    'eslint:recommended',
    '@vue/eslint-config-typescript',
  ],
  env: {
    node: true,
  },
  plugins: ['prettier'],
  parserOptions: {
    parser: '@typescript-eslint/parser',
    ecmaVersion: 'latest',
  },
  rules: {
    'prettier/prettier': [
      'warn',
      {
        tabWidth: 2,
        endOfLine: 'lf',
        printWidth: 80,
        semi: true,
        singleQuote: true,
        quoteProps: 'as-needed',
        jsxSingleQuote: false,
        trailingComma: 'es5',
        bracketSpacing: true,
        arrowParens: 'always',
      },
    ],
  },
};
```
2. Doubleclick package.json to open it and add the dependencies below to the end of the devDependencies section and save. These are the additional npm package dependencies that the TypeScript Analyzer needs to get Prettier to run:
``` json
    {{site.packageversions.eslintpluginprettier}},
    {{site.packageversions.prettier}}
```
3. Install the new npm packages. Rightclick the project in Solution Explorer/Open in Terminal, then in the terminal that appears execute the command `npm i`.

Now if you lint a .vue file in the project you will probably get some Prettier warnings.  As usual, you can rightclick/Fix TypeScript Analyzer (ESLint) Errors to format the file and remove these errors.