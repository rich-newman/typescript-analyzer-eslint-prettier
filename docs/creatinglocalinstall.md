﻿# <a name="createlocalinstall"></a>Creating a Local Install

The steps below describe how to create a local installation of the TypeScript Analyzer (ESLint).  More details on what this is and why you might want to do this are on the [Installs documentation page](installs.md).

## 1. Add devDependencies Section to package.json File

To create a local install you first need to [add appropriate dependencies to a package.json file](https://docs.npmjs.com/specifying-dependencies-and-devdependencies-in-a-package-json-file) in a devDependencies section.

You need a devDependencies section in your package.json that contains the npm packages below:
``` json
"devDependencies": {
    {{site.packageversions.typesnode}},
    {{site.packageversions.typescripteslintplugin}},
    {{site.packageversions.typescripteslintparser}},
    {{site.packageversions.eslint}},
    {{site.packageversions.eslintpluginprettier}},
    {{site.packageversions.prettier}},
    {{site.packageversions.typescript}}
}
```

If you don't have a package.json in your project you need [to create one](https://docs.npmjs.com/creating-a-package-json-file#creating-a-default-packagejson-file).  You can do this by starting a command prompt in the root of your project and executing the command `npm init -y`.  This assumes you have node and npm installed.  If you don't [you need to install them](https://nodejs.org/en/download/).

If you already have a package.json in your project then you can add the dependencies to it.

If you want to use **additional ESLint plugins** with the TypeScript Analyzer add them into this package.json under devDependencies as well. 

## 2. Install the Packages

To install the packages, depending on your project type, either rightclick package.json in Solution Explorer and select 'Restore packages', or rightclick the npm entry in Solution Explorer and select 'Install npm packages', or open a command prompt or Powershell terminal and run `npm i`.

When the Analyzer next runs it will see the new folder and use that in preference to its own npm modules.

Note that the TypeScript Analyzer will only use a local install if the 'Enable local node_modules' setting is True in Tools/Options/TypeScript Analyzer.  True is the default setting. 

**It can be useful to turn logging on** in the Output window for this: Tools/Options/TypeScript Analyzer, set Logging Enabled to true.  When the Analyzer runs it will show which version of ESLint it is using in the logging output.  It will show the path used for a local install under 'Root directory for Node:'.  It will also usually show any problems with the new configuration.

**The Analyzer will not use any globally installed npm packages**.  That is, it will not use packages installed as `npm i -g {packagename}`.  It either uses a local install or its own install.

## <a name="localconfiguration"></a>3. Set Up Local Configuration

If you follow the steps above under 'Creating a Local Install' then the TypeScript Analyzer will by default continue to use the [default configuration file](defaultconfig.md), .eslintrc.js, in c:\Users\\{username\}\TypeScriptAnalyzerConfig.

As [described in the configuration documentation](localconfiguration.md) you can additionally create a local configuration file.  The easiest thing to do is to create a .eslintrc.js file in the root of your project, alongside your package.json, and to copy in the contents of the default .eslintrc.js configuration file.

## Results

The TypeScript Analyzer is designed so that if you create a local install as above that uses the same versions of the npm packages as the Analyzer users, and a local configuration file which is a copy of the default configuration file, then the Analyzer should run and lint in exactly the same way as if it were using its own install.  The difference is now you can change things and they only affect your project.
