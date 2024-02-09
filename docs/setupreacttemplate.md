# Standalone React Projects/React and ASP.NET Core Projects

## Background

Visual Studio 2022 has a 'Standalone JavaScript React Project' template, a 'Standalone TypeScript React Project' template, and 'React and ASP.NET Core' templates for both JavaScript and TypeScript.

If you create a solution from any of these templates then ESLint is configured in the resulting solution, including rules from [react-hooks](https://github.com/facebook/react/blob/main/packages/eslint-plugin-react-hooks/README.md) and [react-refresh](https://github.com/ArnaudBarre/eslint-plugin-react-refresh).  The TypeScript Analyzer can use this configuration.  Note that this configuration does not include more general rules from eslint-plugin-react.  

More details are below.

## Which Project Are We Setting Up ESLint In?

In the discussion below we will refer to the 'client project', which is the JavaScript or TypeScript project in which we are setting up ESLint.  With the Standalone projects this is the sole project in the solution.  However, with the React and ASP.NET Core project we have two projects in our solution: an ASP.NET Core project in C#, which we can't lint with ESLint because it's the wrong language, and a JavaScript or TypeScript project.  Here, obviously, the client project is the JavaScript or TypeScript project.

## Using the Default Linting from a Terminal

If in Visual Studio 2022 you create any of these project types then linting from a terminal window is set up for you as part of the project creation. 

ESLint and the associated plugins are installed for you, and ESLint is configured via a .eslintrc.cjs file in the root of the project.

If you run `npm run lint` from a terminal in the root of the client project ESLint will run and show any errors.

## TypeScript Analyzer Works with No Further Configuration

**For the TypeScript Analyzer no further configuration is necessary for linting to work**.  The TypeScript Analyzer will see the configuration in .eslintrc.cjs and the installed npm packages and use those to lint, displaying any errors in the Visual Studio code editor and Error List.

For this to work the settings 'Enable local config (.eslintrc.js)' and 'Enable local node_modules' must be set to their **defaults** of 'True' in Tools/Options/TypeScript Analyzer/ESLint.

You also need to have built the solution at least once to ensure it works and to install the npm packages.

## Testing This Is Working

**To test this is working**, open file src/App.tsx in the client project.  At the very end of the file add the line below:

```javascript
      export const foo = () => { };
```

This is one of the examples that [fails the only-export-components rule](https://github.com/ArnaudBarre/eslint-plugin-react-refresh?tab=readme-ov-file#fail) of react-refresh. You should see 'foo' underlined in green, since this is a warning, and if you hover should see the message 'Fast refresh only works when a file only exports components. Use a new file to share constants or functions between components.'

To test the refresh-hooks rules, first ensure that useEffect is being imported: for a standalone project you will need to change the first line to the line below:

```javascript
      import { useState, useEffect } from 'react'
```

Then at the start of function App add the code below, which is taken from [the documentation](https://legacy.reactjs.org/docs/hooks-rules.html):

```javascript
    if (name !== '') {
        useEffect(function persistForm() {
            localStorage.setItem('formData', name);
        });
    }
```
This should generate a react-hooks/rules-of-hooks error on useEffect: 'React Hook "useEffect" is called conditionally. React Hooks must be called in the exact same order in every component render.'  