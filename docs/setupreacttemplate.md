# Standalone TypeScript React Template and Standalone JavaScript React Template

## Background

Some [new project templates for JavaScript and TypeScript projects](https://devblogs.microsoft.com/visualstudio/the-new-javascript-typescript-experience-in-vs-2022-preview-3/) were added to Visual Studio 2022.

This article looks at the Standalone TypeScript React Template and the Standalone JavaScript React Template, and how the TypeScript Analyzer works with them.

## Standalone TypeScript React Project and ESLint

If you create a Standalone TypeScript React project or Standalone JavaScript React project in Visual Studio 2022 then it is already set up to lint using ESLint.  The npm packages needed (ESLint and various plugins) will be installed from package.json.  The ESLint configuration is hidden in the package.json file, where there is an 'eslintconfig' section as below.
``` json
  "eslintConfig": {
    "extends": [
      "react-app",
      "react-app/jest"
    ]
  },
```
These project templates are based on [create-react-app](https://create-react-app.dev/), which is a project that allows you to easily create a new React development set up in a standard format.  The Create React App project has [a page](https://create-react-app.dev/docs/setting-up-your-editor/) that shows how to make the ESLint set up above work with various integrated development environments.

## TypeScript Analyzer Works with No Further Configuration

**For the TypeScript Analyzer no further configuration is necessary for linting to work**.  The TypeScript Analyzer will see the configuration in package.json and the installed npm packages and use those to lint, displaying any errors in the Visual Studio code editor and Error List.

For this to work the settings 'Enable local config (.eslintrc.js)' and 'Enable local node_modules' must be set to their defaults of 'True' in Tools/Options/TypeScript Analyzer/ESLint.

### Linting

To see this we need to actually generate an error, since the initial code has none.

One rule that is enabled is [new-parens](https://eslint.org/docs/rules/new-parens), and the documentation shows [code that breaks this rule](https://eslint.org/docs/rules/new-parens#always) as below.  If you paste this code into a code file in the project, for example src/App.tsx in the TypeScript React project or src/App.js in the JavaScript React project, then the TypeScript Analyzer will run and show a warning for new-parens in the Error List, underlining the 'new ' in each line:
``` javascript
var person = new Person;
var person = new (Person);
```

### Fixing

**The TypeScript Analyzer can also fix this error.**  If you rightclick in the code editor and select 'Fix TypeScript Analyzer (ESLint) Errors in Code if Possible' on the context menu, then the problems will be fixed and the new-parens warnings will disappear.

## Prettier

Note that Prettier is NOT available by default if you create a Standalone React project in Visual Studio.  This is because the TypeScript Analyzer is using Create React App's linting rules and configuration as shown above, and that doesn't include Prettier.  

## Seeing Rules that are Enabled

One problem here is that our configuration just says 'react-app' and 'react-app/jest'.  We don't know which individual rules are in force without using Google.

The TypeScript Analyzer can help here.  Go to Tools/Options/TypeScript Analyzer/ESLint and set the options 'Enable logging' and 'Log first config' to True. Then lint a file, for example rightclick index.tsx in Solution Explorer/Run TypeScript Analyzer (ESLint).  

Now go to the Output pane in Visual Studio (View/Output) and select 'TypeScript Analyzer (ESLint, Prettier)' in the dropdown.

Log entries are generated and shown here when the TypeScript Analyzer lints.  This includes a dump of the configuration that was used if 'Log first config' is True.  It's put between entries 'CALCULATED CONFIG FOR FILE START' and 'CALCULATED CONFIG FOR FILE END' to make it easier to find.

The configuration generated at the time of writing is on [this link](setupreacttemplateconfig.md) and shows that by default a very long list of rules is enabled, including some [typescript-eslint plugin](https://github.com/typescript-eslint/typescript-eslint) rules.