# Set Up for Angular Plugin

## Overview

ESLint has a [plugin for linting and fixing in Angular projects](https://github.com/angular-eslint/angular-eslint).  This lints both TypeScript and HTML files in an Angular project, and will lint inline HTML templates in TypeScript files.  

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  This shows both how to use a plugin, and how to apply it to files that are not currently linted.  Errors will be shown in the Visual Studio Error List and underlined in the code window.

Instructions on how to do this in a **ASP.NET Core with Angular project** are below.

## Instructions

1. Create a new ASP.NET Core with Angular project.
2. Doubleclick package.json in Solution Explorer to edit it.  It's in the ClientApp folder.  Add the dependencies below into the devDependencies and save.  Add them after the existing devDependencies.  These are the additional dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the angular-eslint plugin:
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
3. Install these npm packages.  Rightclick ClientApp in Solution Explorer/Open in Terminal, then in the terminal that appears execute the command `npm i`.
4. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project in the ClientApp folder.  To do this rightclick the ClientApp folder in Solution Explorer, Add/New Item..., JavaScript File, enter .eslintrc.js in the Name: box, and click Add.
5. Copy the [file contents on this link](setupangularconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the Angular plugin.  The actual changes made are [detailed in another article](setupangularchangestodefaultconfig.md).
6. **Go to Tools/Options/TypeScript Analyzer/ESLint and under File extensions to lint add ',html' to the existing list.**  After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,html'.  

 You should now be set up correctly.  The remaining steps show this.

7. Open any .html file, for example ClientApp/src/app/app.component.html, and paste the code below into it and save.
``` lang-html
    <p *ngIf="forecasts==false"><em>Loading...</em></p>
    <app-sizer ([size])="fontSizePx"></app-sizer>
```
If the Analyzer is working correctly you should see two @angular-eslint errors, one per line.  The first line should have a @angular-eslint/template/eqeqeq on 'forecasts==false', the second line should have a @angular-eslint/template/banana-in-box error on '([size])="fontSizePx"'
8. To test it's working with TypeScript, open a TypeScript file, for example ClientApp/src/app/app.component.ts and change the value of the selector to CamelCase.  For example change it from 'app-root' to 'AppRoot' in app.component.ts.  You should get a @angular-eslint/component-selector error 'The selector should be kebab-case'.
9. To test it's working with inline HTML templates in TypeScript files open a component TypeScript file, for example, src/app/counter/counter.component.ts.  Replace the @Component attribue on the class with the (very unrealistic) code below:
``` javascript
    @Component({
      selector: 'app-test-name',
      template: `
        <p *ngIf="forecasts == false"><em>Loading...</em></p>
        <app-sizer ([size])="fontSizePx"></app-sizer>
        {{ !(foo | async) }}
      `
    })
```
This code is taken from the @angular-eslint docs examples of how to break the rules.  You should get template/eqeqeq, template/banana-in-box, and template/no-negated-async errors in the inline template.

Note that there is a fixer for the template/banana-in-box rule, but the @angular-eslint plugin doesn't seem to work too well. If you fix the test file above (rightclick in code window/Fix TypeScript Analyzer (ESLint) Errors if Possible) it will remove the space after app-sizer.  After that all your errors disappear in spite of the fact that only banana-in-box is actually fixed.  Put the space back to get the other two errors appearing again.

## Standalone TypeScript Angular Project

The exact same steps as above will also work in Visual Studio's Standalone TypeScript Angular project type.

## Changes Made to Default Configuration File

To create the [configuration file for Angular](setupangularconfig.md) several changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc).  These are [detailed in a separate article](setupangularchangestodefaultconfig.md).

