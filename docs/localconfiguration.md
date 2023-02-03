# Local Configuration Files (.eslintrc.js)

## Background

By default the TypeScript Analyzer uses a [default configuration file](defaultconfig.md) for ESLint, .eslintrc.js, in a [standard location](defaultconfig.md#location).  This file is used for all linting in all projects, unless the project has a local configuration file that overrides it.

You may wish to use your own local configuration file with a custom set of rules for a specific project.  

## Creating Local Configuration

The TypeScript Analyzer is designed so that there are broadly two ways of creating a local configuration file:

1. Copy the default configuration file into your project, and then edit it as necessary.  The TypeScript Analyzer is designed to work with its default install and a local copy of the default configuration file, .eslintrc.js.  In general you'd copy this file into the root of your project, possibly alongside any package.json file. 
2. Create a minimal configuration file in your project that enables just the plugins and rules that you need.  This file can be in any of the [multiple accepted formats for ESLint configuration](https://eslint.org/docs/user-guide/configuring/configuration-files#configuration-file-formats).

## How the TypeScript Analyzer Uses Local Configuration

The TypeScript Analyzer will attempt to use any configuration file it finds locally rather than the default configuration.  The TypeScript Analyzer uses the [same rules that ESLint does to find configuration files](https://eslint.org/docs/user-guide/configuring/configuration-files#using-configuration-files), which means the [cascading and hierarchy rules in the ESLint documentation](https://eslint.org/docs/user-guide/configuring/configuration-files#cascading-and-hierarchy) still apply.  

The only difference is that if the TypeScript Analyzer does not find local configuration it falls back to using its own default configuration.

This search for a local configuration is the default behavior, but it can be turned off.  To do this go to Tools/Options/TypeScript Analyzer/ESLintand set the 'Enable local config (.eslintrc.js)' setting to False.  If you do this the TypeScript Analyzer will always use its own default configuration file and ignore any local configuration.

For those who know about ESLint, we are using the [deprecated personal configuration file functionality](https://eslint.org/docs/user-guide/configuring/configuration-files#personal-configuration-files-deprecated) to fall back to default configuration if no local configuration is found.  However, we redirect the search for the fallback file to our own folder, c:\Users\\{username\}\TypeScriptAnalyzerConfig.  

## Local Configuration and Installs

In general any local configuration should work with the [main install](installs.md), although if the new configuration file has additional plugins then a local install will be necessary: see the [plugins](plugins.md) and [installs](installs.md) documentation for more information.

[Additional more detailed documentation on configuration](configuration.md) of the TypeScript Analyzer is available.

## Walk Through of Creating Local Configuration Files as Copy of Default

Below are the detailed steps for creating a local configuration file that is a copy of the default for a Node Console Application.  These steps will work in Visual Studio 2022, Visual Studio 2019 or Visual Studio 2017, although things are a little more difficult in Visual Studio 2022.

- With the TypeScript Analyzer installed create a new Blank Node.js Console Application with TypeScript.    
- Create a JavaScript file called .eslintrc.js in the NodeConsoleApp folder.  To do this rightclick the project name, Add/New File..., enter .eslintrc.js, and click OK.
- Open the default configuration file using Tools/TypeScript Analyzer (ESLint)/Edit Default Config.
- Select the entire contents of the default configuration file (ctrl-a) and copy it (ctrl-c).  Paste it into your new local .eslintrc.js file (ctrl-v).  Save.  Close the default configuration file you copied from so you don't get confused.
- Go to Tools/Options/TypeScript Analyzer/ESLint and ensure that 'Enable local config (.eslintrc.js)' is set to True, which is the default.
- If you are in Visual Studio 2022 v17.4 or later then set 'Enable local node_modules' to False on the Tools/Options/TypeScript Analyzer/ESLint screen. You need to do this because the Node.js Console Application project includes ESLint locally, but not Prettier or other plugins the new configuration file needs.  Setting this setting forces the TypeScript Analyzer to use the npm packages (node_modules) that it is installed with, that do include Prettier etc., rather than the local packages.  You can set this setting back to True later by creating a [local installation](creatinglocalinstall.md).  You can leave this setting at its default 'True' in Visual Studio 2017 and Visual Studio 2019, since these have no local ESLint in their projects.

This creates a local configuration file.  To show that it is being used do the following steps:

- Open app.ts and replace the default text in app.ts with `var x = true`. You should get an @typescript-eslint/no-unused-vars warning for the x not being used elsewhere, a no-var error for using var instead of const, and a prettier/prettier warning because you have no semicolon at the end.
- In your new *local* .eslintrc.js file find the no-var rule.  Replace `"no-var": "error",` with `"no-var": "off",`.  Save.  
- Go back to app.ts and explicitly run the TypeScript Analyzer by rightclicking/Run TypeScript Analyzer (ESLint) on Code File.
- You should see that the no-var error disappears but the no-unused-vars and prettier warnings remain: we are using a local configuration that overrides the default.