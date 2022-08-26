# Using ESLint Plugins

Most ESLint plugins should work with the TypeScript Analyzer. 

We have some **detailed instructions** for setting up the [React plugin](setupreact.md), [Angular plugin](setupangular.md), [Vue plugin](setupvue.md), the [Markdown plugin](setupmarkdown.md), the [Node plugin](setupnode.md) and the [JSDoc plugin](setupjsdoc.md).

Broadly there are three actions, described below, that may need to be taken to get a plugin working in the TypeScript Analyzer.  The example instructions above show in detail how to do this for the specific plugins covered:

## 1. Install the plugin

To install the plugin you need to create a local installation of the npm packages the TypeScript Analyzer uses and add your plugin package to it.

More detailed information on [installs](installs.md) and [how to create a local install for your plugin](creatinglocalinstall.md) is available.  

However the summary is that you add the npm packages you and the TypeScript Analyzer need to a local package.json and then install them as usual.

## 2. Configure the plugin

This means editing an ESLint configuration file, .eslintrc.js or similar.  How to edit this file will be described in the plugin documentation.

For this it is better to use a local configuration file, which can just be the [default configuration](defaultconfig.md) copied locally and then edited to enable the plugin.

More information is available [on creating a local configuration file](localconfiguration.md), [configuration in general](configuration.md), and [the default configuration file](defaultconfig.md).

## 3. Tell the TypeScript Analyzer if there are additional files that need to be passed to the linter

You only need to do this if the plugin you are using lints files with extensions that the TypeScript Analyzer doesn't usually lint.  For example, if you want to use eslint-plugin-md to lint Markdown files then the TypeScript Analyzer needs to know to pass any files with extension .md to ESLint.

By default the TypeScript Analyzer only passes files with extensions in the list 'js,jsx,ts,tsx,mjs,cjs' to ESLint.  Additional extensions can be added to the list via a [TypeScript Analyzer setting, Tools/Options/TypeScript Analyzer/ESLint/'File Extensions to Lint'](settings.md#fileextensionstolint).

## Examples

The [detailed instructions for using the Markdown plugin](setupmarkdown.md) show how to do all three of the steps above.  The other [examples](examples.md) don't require step 3.
