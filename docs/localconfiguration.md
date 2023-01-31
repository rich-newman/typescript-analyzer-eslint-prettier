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

For those who know about ESLint, we are using the [deprecated personal configuration file functionality](https://eslint.org/docs/user-guide/configuring/configuration-files#personal-configuration-files-deprecated) to fall back to default configuration if no local configuration is found.  However, we redirect the search for the fallback file to our own folder, c:\Users\\{username\}\TypeScriptAnalyzerConfig.  

## Local Configuration and Installs

In general any local configuration should work with the [main install](installs.md), although if the new configuration file has additional plugins then a local install will be necessary: see the [plugins](plugins.md) and [installs](installs.md) documentation for more information.

[Additional more detailed documentation on configuration](configuration.md) of the TypeScript Analyzer is available.

## Walk Through of Creating Local Configuration Files as Copy of Default

Below are the detailed steps for creating a local configuration file that is a copy of the default for a Node Console Application.

- With the TypeScript Analyzer installed create a new Node Console Application with TypeScript.    
- If you are using VS2017, VS2019 or a version of VS2022 before 17.4 replace the default text in app.ts with `var x = true`. You will get a no-unused-vars warning for the x not being used elsewhere, and a no-var error for using var instead of const.  
- If you are using a version of Visual Studio 2022 after 17.4 you will get no errors at this stage: Microsoft have changed the local template so ESLint is enabled locally, but no rules are in force, and the Analyzer uses the valid local configuration.  [More information on this is available.](noteonvs2022templates.md).  To change this go to Tools/Options/TypeScript Analyzer/ESLint on the Visual Studio menus, and set the two settings 'Enable local config (.eslintrc.js)' and 'Enable local node_modules' to False.  Now replace the default text in app.ts with `var x = true`. You will get a no-unused-vars warning for the x not being used elsewhere, and a no-var error for using var instead of const. 
- Create a JavaScript file called .eslintrc.js in the NodeConsoleApp folder.
- Open the default configuration file using Tools/TypeScript Analyzer (ESLint)/Edit Default Config.
- Select the entire contents of the default configuration file (ctrl-a) and copy it (ctrl-c).  Paste it into your new local .eslintrc.js file (ctrl-v).  Save.
- In your new local .eslintrc.js file find the no-var rule.  Replace `"no-var": "error",` with `"no-var": "off", `.  Save.
- If you are using a version of Visual Studio 2022 after 17.4 go to Tools/Options/TypeScript Analyzer/ESLint and set 'Enable local config (.eslintrc.js)' back to True.
- Go back to app.ts and explicitly run the TypeScript Analyzer by rightclicking/Run TypeScript Analyzer (ESLint) on Code File.
- You should see that the no-var error disappears but the no-unused-vars warning remains: we are using a local configuration that overrides the default.