# Set Up for Angular Plugin

## Overview

ESLint has a [plugin for linting and fixing in Angular projects](https://github.com/angular-eslint/angular-eslint).  This lints both TypeScript and HTML files in an Angular project.  

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  This shows both how to use a plugin, and how to apply it to files that are not currently linted.  Errors will be shown in the Visual Studio Error List and underlined in the code window.

Instructions on how to do this in a ASP.NET Core with Angular project are below.

## Instructions

1. Create a new ASP.NET Core with Angular project.
2. Doubleclick package.json in Solution Explorer to edit it.  It's in the ClientApp folder.  Add the dependencies below into the devDependencies and save.  These are the additional dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the angular-eslint plugin:
``` json
    "@typescript-eslint/eslint-plugin": "5.23.0",
    "@typescript-eslint/parser": "5.23.0",
    "eslint": "8.15.0",
    "eslint-plugin-prettier": "4.0.0",
    "prettier": "2.6.2",
    "@angular-eslint/builder": "13.2.1",
    "@angular-eslint/eslint-plugin": "13.2.1",
    "@angular-eslint/eslint-plugin-template": "13.2.1",
    "@angular-eslint/schematics": "13.2.1",
    "@angular-eslint/template-parser": "13.2.1"
```
3. Install these npm packages by rightclicking 'npm' in Solution Explorer and running 'Install npm Packages' in VS2019 or VS2022, or 'Install Missing npm Packages' in VS2017.
4. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project.  To do this rightclick the project name, Add/New File..., enter .eslintrc.js, and click OK.
5. Copy the [file contents on this link](setupangularconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the Angular plugin.  The actual changes made are [detailed at the end of this article](setupangular.md#changesmadetodefaultconfig).
6. **Go to Tools/Options/TypeScript Analyzer/ESLint and under File extensions to lint add ',html' to the existing list.**  After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,html'.
7. Open any .html file, for example, ClientApp/src/app/app.component.html, and paste the code below into it and save.
``` lang-html
<p *ngIf="forecasts==false"><em>Loading...</em></p>
<app-sizer ([size])="fontSizePx"></app-sizer>
```
If the Analyzer is working correctly you should see two @angular-eslint errors, one per line.  The first line should have a @angular-eslint/template/eqeqeq on 'forecasts==false', the second line should have a @angular-eslint/template/banana-in-box error on '([size])="fontSizePx"'

## <a name="changesmadetodefaultconfig"></a>Changes Made to Default Configuration File

To create the [configuration file for Angular](setupangularconfig.md) the following changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc):

1. The section below was added as an element in the first overrides array, after the 'TypeScript-only Rules' entry.  This enables the linting rules for HTML files.
``` javascript
    {
      "files": ["*.html"],
      "parser": "@angular-eslint/template-parser",
      "rules": {
        "@angular-eslint/template/banana-in-box": "error",
        "@angular-eslint/template/eqeqeq": "error",
        "@angular-eslint/template/no-negated-async": "error",
      }
    }
```
2. In the TypeScript-only rules entry a plubins reference was added as below, after the "files" property:
``` javascript
      "plugins": [
        "@angular-eslint",
        //"@angular-eslint/template",
      ],
```
3. In the TypeScript-only rules entry the rules below were added:
``` javascript
        "@angular-eslint/component-class-suffix": "error",
        "@angular-eslint/contextual-lifecycle": "error",
        "@angular-eslint/directive-class-suffix": "error",
        "@angular-eslint/no-conflicting-lifecycle": "error",
        "@angular-eslint/no-empty-lifecycle-method": "error",
        "@angular-eslint/no-host-metadata-property": "error",
        "@angular-eslint/no-input-rename": "error",
        "@angular-eslint/no-inputs-metadata-property": "error",
        "@angular-eslint/no-output-native": "error",
        "@angular-eslint/no-output-on-prefix": "error",
        "@angular-eslint/no-output-rename": "error",
        "@angular-eslint/no-outputs-metadata-property": "error",
        "@angular-eslint/use-lifecycle-interface": "warn",
        "@angular-eslint/use-pipe-transform-interface": "error",
        "@angular-eslint/directive-selector": [
          "error",
          {
            "type": "attribute",
            "prefix": "app",
            "style": "camelCase"
          }
        ],
        "@angular-eslint/component-selector": [
          "error",
          {
            "type": "element",
            "prefix": "app",
            "style": "kebab-case"
          }
        ],
```
The completed file should look like [the code on this link](setupangularconfig.md):