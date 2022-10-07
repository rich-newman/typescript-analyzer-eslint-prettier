# Set Up for Vue Plugin - Basic vue.js Web Application

## Overview

This document describes how to set up the TypeScript Analyzer to work with projects created using the Basic vue.js Web Application project template in Visual Studio.

This is a legacy project template that has been around for a while.  We have [separate documentation for setting up the TypeScript Analyzer](setupvue.md) with the more recent 'Standalone Vue Project' template which is in Visual Studio 2022 only.

There are different templates for both JavaScript and TypeScript in each of Visual Studio 2022, Visual Studio 2019 and Visual Studio 2017, the exception being that there is no TypeScript template in Visual Studio 2022.  

This article deals separately with the templates for each Visual Studio version and language.

## Formatting - Prettier

As discussed on the main Vue page, Prettier doesn't work particularly well with the default Vue projects.  As a result there are no formatting rules in any of the configurations discussed below.

## Visual Studio 2022, JavaScript (Vue 3)

Follow the steps below to enable the TypeScript Analyzer for a Basic vue.js Web Application project, JavaScript in Visual Studio 2022:

1. Create a Basic Vue.js Web Application, JavaScript project in Visual Studio 2022.
2. Replace the contents of package.json with the JSON below.  You may want to only copy/replace from "version" onwards to keep the name unaltered.  Note that the non-ESLint packages in 'dependencies' and 'devDependencies' should be the same ones as are in the original package.json. The only changes we are making are to add in the packages for linting and an ESLint configuration section in eslintConfig. Also note that the 'rules' section has been created solely so that we get no warnings or errors with the base project.  If you want to use the full recommended set of rules the individual rules can be removed from this section, and the code fixed for the resulting linting errors.
``` json
{
  "name": "vuejs-app1",
  "version": "0.1.0",
  "private": true,
  "scripts": {
    "serve": "vue-cli-service serve",
    "build": "vue-cli-service build"
  },
  "dependencies": {
    "core-js": "^3.6.5",
    "vue": "^3.0.0"
  },
  "devDependencies": {
    "@vue/cli-plugin-babel": "~4.5.0",
    "@vue/cli-service": "~4.5.0",
    "@vue/compiler-sfc": "^3.0.0",
    "eslint": "8.24.0",
    "eslint-plugin-vue": "9.6.0",
    "vue-eslint-parser": "9.1.0",
    "@babel/eslint-parser": "7.19.1"
 },
  "eslintConfig": {
    "root": true,
    "env": {
      "node": true
    },
    "extends": [
      "plugin:vue/vue3-recommended",
      "eslint:recommended"
    ],
    "rules": {
      "vue/max-attributes-per-line": [
        "warn",
        { "singleline": { "max": 3 } }
      ],
      "vue/html-indent": [ "warn", 2 ],
      "vue/html-closing-bracket-spacing": [
        "warn",
        { "selfClosingTag": "never" }
      ],
      "vue/require-default-prop": "off",
      "vue/no-multiple-template-root": "off"
    }
  }
}
```
3. Rightclick npm in Solution Explorer/Install npm packages.
4. Go to the menu option Tools/Options/TypeScript Analyzer.  Under 'File Extensions to lint' add ',vue' at the end, if vue is not already in the list.  It's also worth setting Logging/Enable Logging to True so you can see if it's worked. Click OK.
5. Rightclick the project name in Solution Explorer/Run TypeScript Analyzer (ESLint).
6. On the View menu select Output to bring up the Output window if it's not already open.  In the 'Show output form:' dropdown select 'TypeScript Analyzer (ESLint, Prettier)'.  You should see log output ending with 'RESULTS: no problems' if there are none.
7. To test it works in package.json change the line at the end `"vue/no-multiple-template-root": "off"` to `"vue/no-multiple-template-root": "warn"`.  Again rightclick the project name in Solution Explorer/Run TypeScript Analyzer (ESLint).  In the Error List you should see a warning about the vue/no-multiple-template-root rule you just enabled.  Doubleclicking the description will take you to the error in the code.  Clicking in the Code column should take you to the documentation page for the rule on the internet.
8. Set the vue/no-multiple-template rule back to 'off' in package.json.  On Tools/Options/TypeScript Analyzer (ESLint) set Logging/Enable Logging back to False.
9. Do Debug/Start without Debugging to do a build and run the project.  This should show the test web page and prove that the project still works.

## Visual Studio 2019, TypeScript (Vue 2)

Follow the steps below to enable the TypeScript Analyzer for a Basic vue.js Web Application project, TypeScript in Visual Studio 2019:

1. Create a Basic Vue.js Web Application, TypeScript project in Visual Studio 2019.
2. Replace the contents of package.json with the JSON below.  You may want to only copy/replace from "author" onwards to keep the name and description unaltered.  Note that the non-ESLint packages in 'dependencies' and 'devDependencies' should be the same ones as are in the original package.json.  The only changes we are making are to add in the packages for linting and an ESLint configuration section in eslintConfig.  Also note that the 'rules' section has been created solely so that we get no warnings or errors with the base project.  If you want to use the full recommended set of rules the individual rules can be removed from this section, and the code fixed for the resulting linting errors.
``` json
{
  "name": "vuejs-app1",
  "version": "0.1.0",
  "private": true,
  "scripts": {
    "serve": "vue-cli-service serve",
    "build": "vue-cli-service build"
  },
  "description": "VuejsApp1",
"author": {
    "name": ""
  },
  "dependencies": {
    "vue": "2.6.12",
    "vue-class-component": "7.2.6",
    "vue-property-decorator": "9.1.2"
  },
  "devDependencies": {
    "@vue/cli-plugin-babel": "4.5.13",
    "@vue/cli-plugin-typescript": "4.5.13",
    "@vue/cli-service": "4.5.13",
    "vue-template-compiler": "2.6.12",
	"eslint-plugin-vue": "9.6.0",
    "vue-eslint-parser": "9.1.0",
    "@typescript-eslint/eslint-plugin": "5.39.0",
    "@typescript-eslint/parser": "5.39.0",
    "eslint": "8.24.0",
    "typescript": "4.8.4",
    "@vue/eslint-config-typescript": "11.0.2"
  },
  "postcss": {
    "plugins": {
      "autoprefixer": {}
    }
  },
  "eslintConfig": {
    "root": true,
    "env": {
      "node": true
    },
    "extends": [
      "plugin:vue/recommended",
      "eslint:recommended",
      "@vue/typescript"
    ],
    "parserOptions": {
      "parser": "@typescript-eslint/parser"
    },
    "rules": {
      "vue/max-attributes-per-line": [
        "warn",
        { "singleline": { "max": 2 } }
      ],
      "vue/html-indent": [ "warn", 4 ]
    }
  },
  "browserslist": [
    "> 1%",
    "last 2 versions",
    "not ie <= 8"
  ]
}
```
3. Rightclick npm in Solution Explorer/Install npm packages.
4. Go to the menu option Tools/Options/TypeScript Analyzer.  Under 'File Extensions to lint' add ',vue' at the end, if vue is not already in the list.  It's also worth setting Logging/Enable Logging to True so you can see if it's worked. Click OK.
5. Rightclick the project name in Solution Explorer/Run TypeScript Analyzer (ESLint).
6. On the View menu select Output to bring up the Output window if it's not already open.  In the 'Show output form:' dropdown select 'TypeScript Analyzer (ESLint, Prettier) 2019'.  You should see log output ending with 'RESULTS: no problems' if there are none.
7. To test it works in package.json delete the line towards the end `"vue/html-indent": [ "warn", 4 ]`.  Also delete the comma at the end of the line before.  Again rightclick the project name in Solution Explorer/Run TypeScript Analyzer (ESLint).  In the Error List you should see several warnings about the vue/html-indent rule you just deleted, in files App.vue and Home.vue.  Doubleclicking a description will take you to the error in the code.  Clicking in the Code column should take you to the documentation page for the rule on the internet.  In the Output window you should see the same warnings after RESULTS, under 'TypeScript Analyzer (ESLint, Prettier)' in the dropdown.
8. Add the line `"vue/html-indent": [ "warn", 4 ]` back into package.json.  If you haven't restarted Visual Studio you can do this by hitting Ctrl-z (undo) a few times.  On Tools/Options/TypeScript Analyzer (ESLint) set Logging/Enable Logging back to False.
9. Do Debug/Start without Debugging to do a build and run the project.  This should show the test web page and prove that the project still works.

## Visual Studio 2019, JavaScript (Vue 2)

The situation for this template is a little more complicated.  A Visual Studio project created by this template includes ESLint, but at version 6.8.0.  The ESLint team fundamentally changed the way ESLint can be called from code in version 7.0.0.  This means the TypeScript Analyzer cannot work with versions of ESLint prior to 7.0.0.

We can address this by bumping the version of ESLint used by a project created by this template to 7.0.0.  However, the [peer dependencies](https://nodejs.org/en/blog/npm/peer-dependencies/) of package @vue/cli-plugin-eslint, which is also in the project, suggest that the package should be hosted in a version of ESLint < 7.0.0.  The package does appear to work with ESLint 7.0.0 in spite of this warning.

As a result of this the steps below do work to create a JavaScript project, but come with a health warning that the project or the TypeScript Analyzer may not function in all circumstances. 

Follow the steps below to enable the JavaScript Analyzer for a Basic vue.js Web Application project, JavaScript in Visual Studio 2019:

1. Create a Basic Vue.js Web Application, JavaScript project in Visual Studio 2019.
2. In package.json change the line `"eslint": "6.8.0",` to `"eslint": "7.0.0",`.
3. Rightclick the project name in Solution Explorer/Open in Terminal.  In the terminal that comes up execute the command `npm i -legacy-peer-deps`. We need to install with the additional flag so that npm doesn't enforce the peer dependencies discussed above.
4. Go to the menu option Tools/Options/TypeScript Analyzer.  Under 'File Extensions to lint' add ',vue' at the end, if vue is not already in the list.  It's also worth setting Logging/Enable Logging to True so you can see if it's worked. Click OK.
5. Rightclick the project name in Solution Explorer/Run TypeScript Analyzer (ESLint).
6. On the View menu select Output to bring up the Output window if it's not already open.  In the 'Show output form:' dropdown select 'TypeScript Analyzer (ESLint, Prettier) 2019'.  You should see log output ending with 'RESULTS: no problems' if there are none.
7. To test it works in package.json change the line `"rules": {},
` to the code below:
``` json
    "rules": {
      "vue/html-closing-bracket-spacing": [
        "warn",
        { "selfClosingTag": "never" }
      ]
    },
```
Again rightclick the project name in Solution Explorer/Run TypeScript Analyzer (ESLint).  In the Error List you should see a warning about the vue/html-closing-bracket-spacing rule you just enabled.  Doubleclicking the description will take you to the error in the code.    Clicking in the Code column should take you to the documentation page for the rule on the internet.
8. Take the vue/html-closing-bracket-spacing rule out of package.json.  On Tools/Options/TypeScript Analyzer (ESLint) set Logging/Enable Logging back to False.
9. Do Debug/Start without Debugging to do a build and run the project.  This should show the test web page and prove that the project still works.

## Visual Studio 2017, JavaScript and TypeScript

Visual Studio 2017 is no longer fully supported by Microsoft.  It is [in extended support.](https://devblogs.microsoft.com/visualstudio/support-ends-for-older-versions-of-visual-studio-feb2022/#:~:text=Visual%20Studio%202017%3A%20mainstream%20support,baseline%20to%20remain%20under%20support.)  **Neither of the Vue templates in Visual Studio 2017 for new projects works well, and we suggest you do not use them.**

**Basic vue.js Web Application, TypeScript** will install and build, but will not run from Visual Studio.  If you can get it going, or have an old version that works, the Analyzer can be made to work with it by following the same steps as above for Visual Studio 2019, TypeScript: add in the same  ESLint npm packages and eslintConfig to the package.json that's created, leaving the other package versions as they are, and follow the rest of the steps.

**Basic vue.js Web Application, JavaScript** will not install correctly.  This is because its peer dependencies are inconsistent, and since version 7 of npm the installer will refuse to install if this is the case.  As in the instructions for Visual Studio 2019, JavaScript we can force an install with the `npm i -legacy-peer-deps` command, after which the project created will work.  However, the packages installed are quite old and will not work with the TypeScript Analyzer.

