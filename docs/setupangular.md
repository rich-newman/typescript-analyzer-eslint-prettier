# Set Up for Angular Plugin

## Overview

ESLint has a [plugin for linting and fixing in Angular projects](https://github.com/angular-eslint/angular-eslint).  This lints both TypeScript and HTML files in an Angular project, and will lint inline HTML templates in TypeScript files.  

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  This shows both how to use a plugin, and how to apply it to files that are not currently linted.  Errors will be shown in the Visual Studio Error List and underlined in the code window.

Instructions on how to do this in a **ASP.NET Core with Angular project in Visual Studio 2022** are below.

## Instructions for a ASP.NET Core with Angular project in Visual Studio 2022

These instructions only work in Visual Studio 2022 at present.  Visual Studio 2017 doesn't have an ASP.NET Core with Angular native project template.  Visual Studio 2019 does have an ASP.NET Core with Angular project template, but it uses old versions of Angular and other packages, and we are in npm dependency hell trying to work out compatible versions of packages.

1. Create a new ASP.NET Core with Angular project.  For testing purposes it's easier to NOT configure for HTTPS, so clear the checkbox on the 'Additional Information' screen.
2. Build the project to ensure it builds.
3. Doubleclick package.json in Solution Explorer to edit it.  It's in the ClientApp folder.  Add the dependencies below into the devDependencies and save.  Add them after the existing devDependencies.  These are the additional dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the angular-eslint plugin:
``` json
    "@typescript-eslint/eslint-plugin": "5.50.0",
    "@typescript-eslint/parser": "5.50.0",
    "eslint": "8.33.0",
    "eslint-plugin-prettier": "4.2.1",
    "prettier": "2.8.3",
    "@angular-eslint/builder": "14.1.2",
    "@angular-eslint/eslint-plugin": "14.1.2",
    "@angular-eslint/eslint-plugin-template": "14.1.2",
    "@angular-eslint/schematics": "14.1.2",
    "@angular-eslint/template-parser": "14.1.2"
```
4. Install these npm packages.  Rightclick ClientApp in Solution Explorer/Open in Terminal, then in the terminal that appears execute the command `npm i`.
5. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project in the ClientApp folder.  To do this rightclick the ClientApp folder in Solution Explorer, Add/New Item..., JavaScript File, enter .eslintrc.js in the Name: box, and click Add.
6. Copy the [file contents on this link](setupangularconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the Angular plugin.  The actual changes made are [detailed in another article](setupangularchangestodefaultconfig.md).
7. **Go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',html' to the existing list.**  After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,html'. 

### Testing the ASP.NET Core with Angular project

You should now be set up correctly.  The remaining steps show this.

1. Open any .html file, for example ClientApp/src/app/app.component.html, and paste the code below into it and save.
``` lang-html
    <p *ngIf="forecasts==false"><em>Loading...</em></p>
    <app-sizer ([size])="fontSizePx"></app-sizer>
```
If the Analyzer is working correctly you should see two @angular-eslint errors, one per line.  The first line should have a @angular-eslint/template/eqeqeq on 'forecasts==false', the second line should have a @angular-eslint/template/banana-in-box error on '([size])="fontSizePx"'
2. To test it's working with TypeScript, open a TypeScript file, for example ClientApp/src/app/app.component.ts and change the value of the selector to CamelCase.  For example change it from 'app-root' to 'AppRoot' in app.component.ts.  You should get a @angular-eslint/component-selector error 'The selector should be kebab-case'.
3. To test it's working with inline HTML templates in TypeScript files in your component TypeScript file from 2 replace the entire @Component attribute on the class with the code below:
``` javascript
    @Component({
      selector: 'app-test-name',
      template: `
       <p *ngIf="forecasts == false"><em>Loading...</em></p>
       <app-sizer ([size])="(fontSizePx)"></app-sizer>
     `
    })
```
You should get template/eqeqeq and template/banana-in-box errors in the inline template.  This code is taken from the @angular-eslint docs examples of how to break the rules.

Note that there is a fixer for the template/banana-in-box rule, but the @angular-eslint plugin doesn't seem to work too well here. If you fix the app.component.ts file above (rightclick in code window/Fix TypeScript Analyzer (ESLint) Errors if Possible) it will remove the space after app-sizer and before the fixed code `[(size)])="(fontSizePx)"`.  After that all your errors disappear in spite of the fact that only banana-in-box is actually fixed.  Put the space back to get the other error appearing again.  That is, make the second line be `<app-sizer [(size)])="(fontSizePx)"></app-sizer>`.

## Standalone TypeScript Angular Project

Similar steps to above will also work in Visual Studio's Standalone TypeScript Angular project type in Visual Studio 2022.  The differences are:

- In step 3 the dependencies you need to add into devDependencies in package.json are as below.  The package.json file is in the root of the project.
``` json
    "@typescript-eslint/eslint-plugin": "5.50.0",
    "@typescript-eslint/parser": "5.50.0",
    "eslint": "8.33.0",
    "eslint-plugin-prettier": "4.2.1",
    "prettier": "2.8.3",
    "@angular-eslint/builder": "13.2.1",
    "@angular-eslint/eslint-plugin": "13.2.1",
    "@angular-eslint/eslint-plugin-template": "13.2.1",
    "@angular-eslint/schematics": "13.2.1",
    "@angular-eslint/template-parser": "13.2.1"
```
- In step 4, to install these you right click the project name in Solution Explorer, then 'Open in Terminal', and then execute command `npm i` in the window that appears.  
- The .eslintrc.js file needs to be created at the root of the project, not in the ClientApp folder.
- The src folder mentioned elsewhere is also at the root of the project, not in the ClientApp folder.

Testing the resulting project should work exactly as described above for the ASP.NET Core with Angular project.

## Changes Made to Default Configuration File

To create the [configuration file for Angular](setupangularconfig.md) several changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc).  These are [detailed in a separate article](setupangularchangestodefaultconfig.md).