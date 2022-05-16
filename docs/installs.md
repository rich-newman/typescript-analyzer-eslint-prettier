# Installs

## Main Install

### Background

The TypeScript Analyzer uses [node](https://nodejs.org/en/) to run [ESLint](https://eslint.org/) and [Prettier](https://prettier.io/).  To do this it installs its own version of node, and [npm packages](https://docs.npmjs.com/about-packages-and-modules) for ESLint, prettier, TypeScript and associated ESLint plugins.  These are used only by the TypeScript Analyzer.

### Main Install Path

As usual for Visual Studio extensions the TypeScript Analyzer (ESLint) install can be found in a subfolder of a path like 'C:\Users\\{username\}\AppData\Local\Microsoft\VisualStudio\17.0_xxxxxxxx\Extensions'.  The subfolders of the Extensions folder contain separate extensions, and are randomly named.  The code for running node is found in a Node subfolder of the extension folder for the TypeScript Analyzer.  The npm packages needed are installed in a node_modules subfolder of this.

## What's Installed

The current npm packages that are installed with the TypeScript Analyzer are as below.

```json
    "@typescript-eslint/eslint-plugin": "5.23.0",
    "@typescript-eslint/parser": "5.23.0",
    "eslint": "8.15.0",
    "eslint-plugin-prettier": "4.0.0",
    "prettier": "2.6.2",
    "typescript": "4.6.4"
```

These packages can be seen in a package.json in the code base.  Note that the extension does not use this package.json to do an install when the extension is started or installed.  The exact versions are distributed with the extension.

## <a name="localinstall"></a>Local Install

### Background

The main install will work as long as you are happy to only use the packages in the section above.  If all you want is slightly different configuration, such as different sets of rules enabled, for different projects then you can continue to use the main install but use a local configuration file, .eslintrc.js.

However, there are some reasons why you might want to use different packages to those above, in which case you need to create a local install.

### Why Use a Local Install?

There are a couple of reasons to use a local install:

**You can use your own versions of the packages** locally, with either later or earlier versions.  

There are some exceptions: you can't use eslint version 6 or earlier, for example.  This is because the eslint team changed the way in which eslint is called from code between version 6 and version 7, meaning the code to launch eslint in the Analyzer won't work.  As usual in npm world, it may be difficult to work out which versions will work together.

**You can also install additional plugins**, and configure the Analyzer to use them, again with the caveat that you need versions that will play together nicely.

If you are working in a project that already has npm packages installed it should hopefully be possible to extend the existing package.json so that the TypeScript Analyzer will use the local install.

### Creating a Local Install

[Creating a local install is documented on a separate page.](creatinglocalinstall.md)

### Using a Local Install

Whenever the TypeScript Analyzer is asked to lint it looks for a local install of these npm packages. **If the TypeScript Analyzer finds a local install it uses it in preference to its own install**.  

This search for a local install is quite naive.  The Analyzer looks in the folder it is run in for a package.json, and then for a node_modules folder with an eslint folder beneath that folder.  If it finds such a node_modules folder then it will use it for all npm packages.  If it can't find one it looks in the parent folder for the same, and then up the folder tree to the root.  If it can't find an appropriate node_modules folder then it will use its own install.
