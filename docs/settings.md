# TypeScript Analyzer Settings

The TypeScript Analyzer settings can be found in the same place as the settings for other things in Visual Studio.  This is on the menu Tools/Options/TypeScript Analyzer/ESLint:

![Tools/Options/TypeScript Analyzer/ESLint](assets\images\options.jpg)

## Resetting the Settings to Defaults

There is a menu option to reset all these settings to their original default value, and at the same time reset the [default configuration file](defaultconfig.md) to its original default value.

This option is Tools/TypeScript Analyzer (ESLint)/Reset TypeScript Analyzer (ESLint) Settings...

## Individual Settings

The individual settings on the Settings screen are described below.

### Basic

#### Enable TypeScript Analyzer (ESLint)

If this is set to false then the TypeScript Analyzer remains installed but is disabled everywhere.  No menu options or linting errors will appear. 

#### <a name="fixonsave"></a>Fix and Format on Save

If this is set to True then the Analyzer will run its fix process whenever a file is saved.  If Prettier is enabled this means that Prettier will run whenever a file is saved, and the saved file contents will be formatted.

This is set up so it only runs when a file is explicitly saved using the menus, toolbars or control + S.  When Visual Studio saves a file for you it is not expected to run.  Occasions when Visual Studio saves files are when you build with unsaved files, or rename an unsaved file.

#### <a name="runonbuild"></a>Run on Build

The default for this is False.  If it is set to True then whenever a Visual Studio project or solution is built then the TypeScript Analyzer will run on whatever is being built before the build takes place.  

If there are any linting errors then the build will not take place until they are fixed or until this option is turned off again.  There will be a 'Build failed because of ESLint errors' message in the status bar.

#### Show Prettier Errors in Error List

The default for this is True, which means Prettier errors are shown in the Visual Studio Error List as warnings with code 'prettier/prettier'.  They are also underlined in green as warnings in the Visual Studio code editor.

There can be a lot of these errors, particularly in a project where Prettier has not been used previously.  See image below.

You can hide the Prettier errors in the Error List, and prevent the underlining of Prettier errors in the Visual Studio code windows, by setting this option to false.

Note that if you do set this option to false then Prettier is still enabled, and Prettier errors will still be corrected if you fix. The setting just prevents these errors cluttering up the Error List and code windows.

![Basic lint](assets\images\prettiererrors.jpg)

#### Show red/green underlining

The default value for this is True.  If the setting is set to False then ESLint errors and warnings will still appear in the Error List but code underlining in red and green in the Visual Studio code editor windows will be disabled.

#### <a name="usetsconfig"></a>TypeScript: lint using tsconfig.json

By default the TypeScript Analyzer handles TypeScript files in the same way as all other files.  This means that it hands individual .ts and/or .tsx files to ESLint to lint.  However, it is possible to lint using tsconfig.json 'project' files.  If `TypeScript: lint using tsconfig.json` is set to True then the TypeScript Analyzer will attempt to automatically find tsconfig.json files to hand to ESLint for linting.

The exact rules for how the TypeScript Analyzer tries to find tsconfig.json files to hand to ESLint are fairly complex.  [Full details of how this works are available](tsconfigrules.md).  However, broadly, for an individual file it looks in the current folder and then all parent folders until it finds a tsconfig.json.  It hands that to ESLint and then filters the results to only show errors for the file that was originally linted.  For linting a solution, project or folder it looks in the the appropriate container and hands all tsconfig.json files it finds to ESLint, and doesn't filter the results at all.

The **main reason for enabling this setting** and using tsconfig.json files to lint is that doing so enable type-aware rules from the TypeScript ESLint plugin to be used without the need for specific configuration in your configuration file.  [More details on this are available](typeinformation.md).

### ESLint Configuration

#### <a name="enableignore"></a>Enable ignore

The TypeScript Analyzer ignores files in [exactly the same confusing way ESLint does](https://eslint.org/docs/user-guide/configuring/ignoring-code).  This behavior can be disabled by setting the 'Enable ignore' setting to False.  To be clear, if the setting is False it means that anything set up to be ignored will NOT be ignored.  This is roughly equivalent to using the `no-ignore` command line parameter with ESLint.

[More details on ignoring files is available.](ignoringfiles.md)

#### Enable local config (.eslintrc.js)

The TypeScript Analyzer install includes a file, .eslintrc.js, that is used to configure ESLint.  This file can be brought up and edited from the Visual Studio menu: Tools/TypeScript Analyzer (ESLint)/'Edit Default Config (c:\Users\{username}\TypeScriptAnalyzerConfig\\.eslintrc.js)'.  However, by default before linting the TypeScript Analyzer allows ESLint to look for local configuration to replace the configuration in this file.  [ESLint does this using its usual fairly complicated rules](https://eslint.org/docs/latest/user-guide/configuring/configuration-files#configuration-file-formats).

If this 'Enable local config' setting is set to False then the TypeScript Analyzer does NOT allow ESLint to use any local configuration, instead forcing ESLint to use the default configuration file mentioned above, c:\Users\{username}\TypeScriptAnalyzerConfig\\.eslintrc.js.  Technically the TypeScript Analyzer does this by setting the overrideConfigFile option in ESLint to point to the default file, and also setting the useEslintrc option to false.

#### Enable local node_modules

The TypeScript Analyzer install includes ESLint, TypeScript and associated npm packages to allow linting, all in a node_modules folder as usual.  However, by default before linting the TypeScript Analyzer searches for a local installation of ESLint, TypeScript etc. to use.

If this 'Enable local node_modules' setting is set to False the search for a local installation is NOT carried out, forcing the Analyzer to use its installed node_modules folder.  This can be useful if you're unsure if problems with local npm packages are causing issues with linting.

More details on [installs](installs.md) and [creating a local install](creatinglocalinstall.md) are available.

#### <a name="fileextensionstolint"></a>File extensions to lint

The TypeScript Analyzer will only lint files with file extensions that are in the list provided under this setting.  This means that if you want to use a plugin to lint other types of file you will need to edit this list.

[An example of how to do this to lint Markdown (.md) files](setupmarkdown.md) using the [eslint-plugin-md plugin](https://github.com/leo-buneev/eslint-plugin-md#readme) is available.

#### JavaScript VM memory (MB)

The TypeScript Analyzer runs node with default memory settings.  By default, node allocates only limited memory to the JavaScript virtual machine that runs ESLint.  This means that if it lints very large projects it can crash with out of memory errors.  This setting can be used to allocate more memory in this case.

In general this setting should be left at the default value of 0.

[More details on how to identify when the TypeScript Analyzer is running out of memory, and what to do about it, are available.](jvmmemory.md)

### Extended

#### Interval stopping typing to lint (ms)

If you are typing in a file that can be linted in the Visual Studio code editor then the TypeScript Analyzer will lint it after waiting for a fixed period after you stop typing.  By default this is a 3 seconds, or 3000 milliseconds.  This period can be configured to something shorter or longer using this setting.

#### Pattern to match tsconfig file names

This setting is only used if `TypeScript: lint using tsconfig.json` is set to True.  In this case the pattern in this setting will be used to identify tsconfig.json files to be used for linting.  More details on this setting can be found in the [documentation on using tsconfig.json files](tsconfigrules.md).

### <a name="logging"></a>Logging

#### Enable logging

The TypeScript Analyzer can log what happens during linting.  By default this is disabled as it slows down Visual Studio.

To enable logging set this setting to True. 

If you then lint something a log will be generated. To see this bring up the Visual Studio Output pane (View/Output).  In the 'Show output from:' dropdown at the top select 'TypeScript Analyzer (ESLint, Prettier)'.

The additional logging settings below only apply if this setting is set to True.

#### Log file names

If logging is enabled and this setting is set to True then as part of the logging in the Output pane the TypeScript Analyzer will show a list of all files it is linting and, separately, all project files (tsconfig.jsons) that it is linting.  If you are linting a very large solution and looking at the logs you may want to set this setting to false to prevent a very long list of files being generated.

#### Log first config

If logging is enabled and this setting is set to true then the contents of the configuration file (.eslintrc.js) associated with the first file being linted will be logged in addition to the usual logging.  In fact, the results of ESLint's calculateConfigForFile for that file are output if this setting is true.

#### Log tsconfig.json contents

If logging is enabled, this setting is set to true, and the TypeScript Analyzer is linting with tsconfig.json files then the contents of any tsconfig.json files that are being used to lint will be logged in addition to the usual logging.  For more details on linting with tsconfig.json files see the [documentation on the setting 'TypeScript: lint using tsconfig.json' above.](settings.md#usetsconfig)