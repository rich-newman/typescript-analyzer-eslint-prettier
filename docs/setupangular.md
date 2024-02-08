# Set Up for Angular Plugin

## Overview

ESLint has a [plugin for linting and fixing in Angular projects](https://github.com/angular-eslint/angular-eslint).  This lints both TypeScript and HTML files in an Angular project, and will lint inline HTML templates in TypeScript files.  

We can set up the TypeScript Analyzer to use this plugin in Visual Studio projects.  This shows both how to use a plugin, and how to apply it to files that are not currently linted.  Errors will be shown in the Visual Studio Error List and underlined in the code window.

Instructions on how to do this in both a 'Angular and ASP.NET Core' and a 'Standalone TypeScript Angular project' in Visual Studio 2022 are below.

## Instructions for a Angular and ASP.NET Core project in Visual Studio 2022

These instructions only work in Visual Studio 2022 at present.  Neither Visual Studio 2017 nor Visual Studio 2019 has an Angular and ASP.NET Core project template that works with the instructions below.

1. Create a new 'Angular and ASP.NET Core' project.  For testing purposes it's easier to NOT configure for HTTPS, so clear the checkbox on the 'Additional Information' screen, otherwise accept the defaults.
2. Build the project to ensure it builds.
3. Doubleclick package.json in Solution Explorer to edit it.  It's in the {appName}.client folder, where {appName} is the name of your project.  Add the dependencies below into the devDependencies and save.  Add them after the existing devDependencies.  These are the additional dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the angular-eslint plugin:
``` json
    {{site.packageversions.typescripteslintplugin}},
    {{site.packageversions.typescripteslintparser}},
    {{site.packageversions.eslint}},
    {{site.packageversions.eslintpluginprettier}},
    {{site.packageversions.prettier}},
    "@angular-eslint/builder": "^17.2.1",
    "@angular-eslint/eslint-plugin": "^17.2.1",
    "@angular-eslint/eslint-plugin-template": "^17.2.1",
    "@angular-eslint/schematics": "^17.2.1",
    "@angular-eslint/template-parser": "^17.2.1"
```
4. Install these npm packages.  Rightclick {appName}.client in Solution Explorer/Open in Terminal, then in the terminal that appears execute the command `npm i`.
5. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project in the {appName}.client folder.  To do this rightclick the {appName}.client folder in Solution Explorer, Add/New Item..., JavaScript File, enter .eslintrc.js in the Name: box, and click Add.
6. Copy the [file contents on this link](setupangularconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the Angular plugin.  The actual changes made are [detailed in another article](setupangularchangestodefaultconfig.md).
7. **Go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',html' to the existing list.**  After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,html'. 
8. On the Tools/Options/TypeScript Analyzer/ESLint screen opened in step 7, check that both 'Enable local config (.eslintrc.js)' and 'Enable local node_modules' are set to True, which is the default.

### Testing the Angular and ASP.NET Core project

You should now be set up correctly.  The remaining steps show this.

1. Open any .html file, for example {appName}.client/src/app/app.component.html, where {appName} is the name of your project.  Paste the code below into it at the end and save.
``` lang-html
    <p *ngIf="forecasts==false"><em>Loading...</em></p>
    <app-sizer ([size])="fontSizePx"></app-sizer>
```
If the Analyzer is working correctly you should see two @angular-eslint errors, one per line.  The first line should have a @angular-eslint/template/eqeqeq error on 'forecasts==false', the second line should have a @angular-eslint/template/banana-in-box error on '([size])="fontSizePx"'
2. To test it's working with TypeScript, open a TypeScript file, for example {appName}.client/src/app/app.component.ts and change the value of the selector to CamelCase.  For example change it from 'app-root' to 'AppRoot' in app.component.ts.  You should get a @angular-eslint/component-selector error 'The selector should be kebab-case...'.
3. To test it's working with inline HTML templates in TypeScript files, in your component TypeScript file from 2 replace the entire @Component attribute on the class with the code below:
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

Note that there is a fixer for the template/banana-in-box rule. To see this fix the app.component.ts file above (rightclick in code window/Fix TypeScript Analyzer (ESLint) Errors if Possible).  This should remove the banana-in-box error by reversing the order of the parentheses, and leave the eqeqeq error, which has no fixer.

## Standalone TypeScript Angular Project

Almost the exact same steps above will also work in Visual Studio's Standalone TypeScript Angular project type in Visual Studio 2022.  The differences are:

- You need to create a Standalone TypeScript Angular project type in Visual Studio 2022, clearly, then follow the same steps as above.
- In step 4, to install the npm packages you right click the project name in Solution Explorer not the {appName}.client folder, then 'Open in Terminal', and then execute command `npm i` in the window that appears.  There is no {appName}.client folder in the project.  
- In step 5, the .eslintrc.js file needs to be created at the root of the project, not in the {appName}.client folder.
- The src folder mentioned in the testing section is also at the root of the project, not in the {appName}.client folder.

Testing the resulting project should work exactly as described above for the Angular and ASP.NET Core project.

## Changes Made to Default Configuration File

To create the [configuration file for Angular](setupangularconfig.md) several changes were made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc).  These are [detailed in a separate article](setupangularchangestodefaultconfig.md).