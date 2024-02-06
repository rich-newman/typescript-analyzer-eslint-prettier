# Setting Parsers

The [TypeScript ESLint plugin to ESLint](https://github.com/typescript-eslint/typescript-eslint) is installed with the TypeScript Analyzer.  This doesn't work unless you explicitly set the parser in config to the full path of the TypeScript ESLint parser.  This is true even if, as with the TypeScript Analyzer, the parser is where you'd expect in the same node_modules folder as the TypeScript ESLint plugin itself.  

Unfortunately because Visual Studio extensions are not installed to a set path the TypeScript Analyzer has no way of knowing the path to the parser before install.  This means we can't specify the parser in the default configuration file, because we don't know where it will be.  

As a result we set it in code on every linting run.  To do this we use the [baseConfig property of the options object provided to ESLint](https://eslint.org/docs/developer-guide/nodejs-api#-new-eslintoptions).  The code is as below.  dirName will be where the code is running, which is a subfolder of the install path for the extension.

```javascript
    options.baseConfig = {
        parser: dirName + "\\node_modules\\@typescript-eslint\\parser\\dist\\index.js",
    };
```

This is a longwinded way of saying that the **TypeScript Analyzer sets the parser explicitly to the TypeScript ESLint parser included with its own install, even though this isn't in the default configuration file**. 

Of course **a different parser can be specified**, either in the default configuration file or in a local file.  It can be specified for all files, or for individual sets of files separately.  The baseConfig above is immediately overridden if ESLint finds any property to override it in non-base configuration.

## Note re Path for Setting Parsers

Note that in version 6.0 and later of typescript-eslint (which the Analyzer now uses) the typescript-eslint package was changed so that simply referencing the parser folder no longer worked.  That is, if we'd put "\\node_modules\\@typescript-eslint\\parser" in the code above then ESLint would not be able to find a parser.  You need to add the "\\dist\\index.js".  This is only true of the typescript-eslint parser if you are setting it by path.  For other parsers the root folder of the package should work in the syntax above.