# Changelog

These are the changes to each version that have been released
on Visual Studio Marketplace.

Version numbering started at 2.0 to avoid confusion with the old 
TypeScript Analyzer which was based on TSLint rather than ESLint.  
The old TypeScript Analyzer had version numbers with major version 
number of 1.

## 2.5

**2023-01-23**

- Add documentation explaining why the TypeScript Analyzer shows no linting errors in VS2022 v17.4 and later for Node.js Console Applications and TypeScript Console Applications.  The reason is that the TypeScript Analyzer looks for and uses any local configuration in a project, and these project types are configured locally with ESLint but no linting rules.
- Add 'Enable local config (.eslintrc.js)' option, which by default is True.  If set to False the TypeScript Analyzer will ignore any local configuration files and always use its own .estlintrc.js that comes with the TypeScript Analyzer installation.  This is the file that can be edited from menu option Tools/TypeScript Analyzer (ESLint)/'Edit Default Config (c:\Users{username}\TypeScriptAnalyzerConfig.eslintrc.js)'.
- This means that if both the new 'Enable local config (.eslintrc.js)' option, and the existing 'Enable local node_modules' option are set to False then the TypeScript Analyzer will use its own configuration regardless of what is set up locally.  By doing this it's possible to override the config that does nothing in the new project templates and to get linting errors back.
- Update to latest versions of dependent packages, including ESLint 8.31.0.

## 2.4

**2022-10-07**

- Add Fix on Save option, which allows Prettier formatting when a file is saved, issue [#1](https://github.com/rich-newman/typescript-analyzer-eslint-prettier/issues/1).
- Update to latest versions of dependent packages, including ESLint 8.24.0.

## 2.3

**2022-07-05**

- Update to latest versions of dependent npm packages, including ESLint 8.19.0
- Set the node working directory in the web server to the local install folder if there is one, rather than defaulting to the open solution folder.  This helps some packages to find their own config files, notably the Babel parser.  Display the working directory in the logs.  More sophisticated ways of setting the working directory may be necessary, this will be added to the road map.
- Give a better error message if a local install uses a version of ESLint before 7.0.0.  The way ESLint is called from code changed fundamentally in v7.0.0 and as a result the Analyzer cannot work with versions before this.

## 2.2

**2022-06-23**

- Fix bug that means the tagger doesn't underline correctly in .vue files.
- Add links to help files for Vue errors.

## 2.1

**2022-06-04**

- Fix bug that crashes the webserver if linting using tsconfigs and there are comments in the JSON.
- Target .NET Framework 4.6 in Linter project rather than 4.6.1 so Visual Studio 2017 can run it without a targeting pack.
- Added documentation on using the Analyzer with Visual Studio's Angular project types.

## 2.0

**2022-05-16**

- Initial release
