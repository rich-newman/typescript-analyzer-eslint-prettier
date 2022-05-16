# Ignoring Files

The TypeScript Analyzer ignores files in [exactly the same confusing way ESLint does](https://eslint.org/docs/user-guide/configuring/ignoring-code).  The two main approaches are:

1. Include an ignorePatterns entry in the .eslintrc.js configuration file.  This is our recommended approach.
2. Create a .eslintignore file in your project's root directory.

Other approaches such as [hiding files to be ignored in a local package.json](https://eslint.org/docs/user-guide/configuring/ignoring-code#using-eslintignore-in-packagejson) will also work.

## ignorePatterns in .eslintrc.js

We recommend using ignorePatterns in [.eslintrc.js configuration files](https://eslint.org/docs/user-guide/configuring/ignoring-code#ignorepatterns-in-config-files) if you want to ignore files.  You can put, e.g.,`"ignorePatterns": ["**/*.ts", "myfile.md"],` at the top-level in the file, after `"root": true,` and files that match the patterns in the array will be ignored for linting.

Note however that you can't easily do this directly in the [default configuration file](defaultconfig.md), .eslintrc.js in c:\Users\\{username\}\TypeScriptAnalyzerConfig.  This is because ESLint uses the path of the configuration file as a starting point for any search for files to ignore in this file, and this is unrelated to your project path.

Instead [create a local .eslintrc.js file in your project](localconfiguration.md), and include ignorePaths in that if you want to specifically ignore files.

## .eslintignore

If you want to use a .eslintignore file then you again need to create a local .eslintrc.js file in your project at the root, and then put a .eslintignore file in the same folder.  The default configuration file won't work if a .eslintignore file is found without a local .eslintrc.js, and your linting will have no rules.  The [ESLint documentation](https://eslint.org/docs/user-guide/configuring/ignoring-code#the-eslintignore-file) shows how to use a .eslintignore file.

## Advantages of ignorePatterns

The advantages or using ignorePatterns are: 

1. You don't need another file (.eslintignore)
2. The TypeScript Analyzer and ESLint  work better with ignorePatterns  

ESLint can only use one .eslintignore file for a lint session, whereas it will use the appropriate .eslintrc.js file for each individual file it's linting.  This applies, for example. when the TypeScript Analyzer lints an entire solution in one run.  

The TypeScript Analyzer looks for a .eslintignore by searching in the folder and any parent folder of the first file it's linting in this linting run.  Whilst this should work for individual files, if you lint an entire solution it will use this ignore file for every file being linted.

## Disabling Ignore

ESLint has a [command-line parameter, no-ignore, that disables ignoring files](https://eslint.org/docs/user-guide/command-line-interface#options).  This means files described in ignorePatterns or .eslintignore will NOT be ignored if this parameter is passed.

The TypeScript Analyzer allows you to set a very similar flag for its lint runs.  This is [Tools/Options/TypeScript Analyzer/ESLint/Enable ignore](settings.md#enableignore).  By default this is True, but if you set it to False ignore is disabled. 

This can be useful for ruling out ignore patterns if a file is not being linted and you think it should.

## Logging

If you want to see what the TypeScript Analyzer thinks is being ignored in a linting run then enable logging with [Tools/Options/TypeScript Analyzer/ESLint/Enable logging](settings.md#logging).  Then also enable the option 'Log first config' in the same place.  After a linting run the configuration output in the Output pane, TypeScript Analyzer dropdown, will have an ignorePatterns section showing what ESLint is ignoring.  It has this same section whether the ignore is coming from a .eslintignore file or an ignorePatterns section in the actual .eslintrc.js.

## node_modules and dot-files

In general **node_modules folders containing npm packages do not get linted**.  [ESLint ignores node_modules folders](https://eslint.org/docs/user-guide/configuring/ignoring-code#the-eslintignore-file) and any contents by default, and the TypeScript Analyzer only lints files that are in your project.  In general it's better to NOT put node_modules folders into Visual Studio projects. 

Similarly **dot-files and dot-folders do not get linted**.  dot-files are files whose names start with a dot.  Normally an exception to this rule is .eslintrc.js, which does get linted.  However, Prettier makes such a mess of the [default configuration file](defaultconfig.md), .eslintrc.js, that we have used ignorePatterns in that file to ensure that it is ignored by the TypeScript Analyzer.

If you DO want to lint something in node_modules you can by including the node_modules folder in your project, setting Tools/Options/Enable Ignore to False, and linting by using the context menu in Solution Explorer.  In spite of [what the documentation says](https://eslint.org/docs/user-guide/configuring/ignoring-code#the-eslintignore-file) if you explicitly ask ESLint to lint a file in node_modules it will do it if the ignore is disabled.  Be warned that if you try to lint an entire node_modules folder you may lint thousands of files and it will be slow.  You may also [run out of memory](jvmmemory.md).