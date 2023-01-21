# Troubleshooting

If the TypeScript Analyzer isn't working, or isn't working as expected, there are a couple of steps you can take to troubleshoot.

## Logging

The first thing to do if you are having issues with the TypeScript Analyzer is to enable logging.

You can to this by going to Tools/Options/TypeScript Analyzer/ESLint/'Enable Logging' and setting the value to True.  You can turn on other logging options here to give more details.

The next time the TypeScript Analyzer is run it will output log details of what it is doing into the Visual Studio Output pane (View/Output) if you select 'TypeScript Analyzer (ESlint, Prettier)' in the 'Show output from:' dropdown in that pane.

In particular this will show if the TypeScript Analyzer is encountering any errors when linting, 
will show the configuration that the TypeScript Analyzer is using, and will show any linting errors it finds in the run.

## Forcing the TypeScript Analyzer to Use Its Default Configuration

The TypeScript Analyzer installation includes its own set of npm packages, including ESLint and Prettier, and a default configuration file, .eslintrc.js.  

These can be overridden by including your own local npm packages in a node_modules folder, and by including your own configuration locally in any format that [ESLint allows it](https://eslint.org/docs/latest/user-guide/configuring/configuration-files#configuration-file-formats). 

The TypeScript Analyzer will by default automatically use any local configuration or packages that it finds, even if these cause it not to work well. More details on how this works can be found on the [Installs documentation page](installs.md), and the [Local Configuration documentation page](localconfiguration.md).

The TypeScript Analyzer also has settings that can be set from within Visual Studio via Tools/Options/TypeScript Analyzer.

The second thing you can do if you are having issues is to force the TypeScript Analyzer to revert to using default settings, the default configuration file, and the npm modules it came with.

To do this and test:
1. Reset the TypeScript Analyzer Settings from the Visual Studio menus at the top: Tools/TypeScript Analyzer (ESLint)/'Reset TypeScript Analyzer (ESLint) Settings'.  This sets the default configuration file, .eslintrc.js, back to the default version from installation, and sets all Tools/Options/TypeScript Analyzer/ESLint settings back to their defaults.  This will overwrite any manual changes made to the .eslintrc.js file at c:\Users\\{userName\}\TypeScriptAnalyzerConfig\\.eslintrc.js.
2. Go to Tools/Options/TypeScript Analyzer/ESLint on the Visual Studio menus, and set the two settings 'Enable local config (.eslintrc.js)' and 'Enable local node_modules' to False. 'Enable local config (.eslintrc.js)' set to false forces the TypeScript Analyzer to use the default .eslintrc.js that was reset in step 1, and to ignore any local configuration. 'Enable local node_modules' set to false forces the TypeScript Analyzer to use the npm packages that were installed with the TypeScript Analyzer, and to ignore any local packages in a local node_modules folder. 
3. In any TypeScript or JavaScript file in your project in Visual Studio enter the line `console.writeline('test')`, with no semicolon at the end.  You should get a prettier error in the Error List ' Replace `'Hello·world')` with `"Hello·world");'

These steps should usually get the TypeScript Analyzer to display linting errors in the Error List.  You can then reverse these steps to see what is causing any problem locally.