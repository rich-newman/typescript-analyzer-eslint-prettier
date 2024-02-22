# Linting TypeScript using Rules that Need Type Information

## Background: Type-Aware Rules and the TypeScript ESLint Plugin

The TypeScript ESLint plugin has two sorts of rules for linting TypeScript:
- There are basic rules that don't need to know anything about the structure of the project they are in.  
- There are rules that require type information that DO need to understand the project structure.  These rules are marked as 'requires type information' with a check in the 'type checked' column on the [plugin's rules page](https://typescript-eslint.io/rules/).  These rules are also known as 'type-aware rules'.

For the rules that require type information to work you have to be using tsconfig.json files to structure your project, and you have to tell the linter about the tsconfig.json files.

## Default TypeScript Analyzer Behavior

By default the Analyzer only uses TypeScript rules that do NOT need type information.  If you include a rule in your [default configuration file](defaultconfig.md) (.eslintrc.js file) that needs type information without doing anything else you will get an error as below:

'ESLint error caused by a rule included in config that needs type information, but use of tsconfig not enabled.'

## Making the TypeScript Analyzer Work with Type-Aware Rules

You have two options to make a rule with type information work, assuming you have appropriate tsconfig.json files set up:

1. Easiest is to tell the Analyzer to use tsconfig files, and let it do the rest of the work for you.  To do this set 'Tools/Options/TypeScript Analyzer/TypeScript/[TypeScript: lint using tsconfig.json](settings.md#usetsconfig)' to True.  For any subsequent lint of TypeScript files the Analyzer will attempt to find appropriate tsconfig.json files and give them to the linter.  A drawback is that it may get this wrong if you have an unusual set-up.  We have [documented the rules the Analyzer uses to find tsconfig.json files](tsconfigrules.md).
2. Alternatively you can explicitly tell the linter which tsconfig.json file to use by setting up local .eslintrc.* configuration file with a parserOptions section.  This is documented on the [TypeScript ESLint plugin page on linting with type information](https://typescript-eslint.io/docs/linting/type-linting/).

A full example of linting with a rule that needs type information is below.  This shows how to use both of the options above.

## Advantages of a Configuration File

Whilst fixing the problem with a configuration file (option 2 above) is more difficult, it has some advantages:  

- The local configuration file can be checked into a repository with the code. This means the configuration is available immediately to anyone who checks out the project.  The Tools/Options flag can't be checked into a repo, so will need to be set separately on every machine.
- The local configuration file only applies to this project.  The Tools/Options flag is a global setting for the machine.

## Example of How to Lint with a Rule that Needs Type Information

This example sets up a Node TypeScript Console Application in Visual Studio, and enables the rule '[unbound-method](https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/unbound-method.md)' that needs type information.  We do this for both of the options described above for getting the rules to work: setting the flag in Tools/Options or editing a .eslintrc.js file.

### Basic Set Up

- Create a new Blank Node.js Console Application with TypeScript in Visual Studio.
- If you are using Visual Studio 2022 after version 17.4 you will need to go to Tools/Options/TypeScript Analyzer and set both 'Enable local config (.eslintrc.js)' and 'Enable local node_modules' to False, which forces the TypeScript Analyzer to use its own configuration. [More information on this is available](https://rich-newman.github.io/typescript-analyzer-eslint-prettier/noteonvs2022templates.html).
- Go to Tools/TypeScript Analyzer (ESLint)/Edit Default Config.  This brings up the base .eslintrc.js configuration file for the Analyzer.  Search in this for 'unbound-method' rule, which is commented out.  Comment it back in (remove the //) and save the file. 
- Open app.ts and paste the code below into app.ts after the existing code. This is taken from the [page on the unbound-method rule](https://github.com/typescript-eslint/typescript-eslint/blob/main/packages/eslint-plugin/docs/rules/unbound-method.md).
```typescript
const arith = {
    double(x: number): number {
        return x * 2;
    },
};
const { double } = arith;
```
- Save the file.  You will get the error 'ESLint webserver error caused by a rule included in config that needs type information, but use of tsconfig not enabled.'  

We've temporarily broken all linting: ESLint is very unforgiving of config problems and will just throw if it finds something wrong.  We want the code above to correctly generate the unbound-method linting error.

As described above, there are two ways of fixing the webserver error:

### Fixing with the Analyzer Flag

- Go to Tools/Options/TypeScript Analyzer/'TypeScript: lint using tsconfig.json' and set it to True.  Close the dialog.
- Lint app.ts by rightclicking on it in Solution Explorer/Run TypeScript Analyzer (ESLint), or just save the file.  You should see 'double' in `const { double } = arith` underlined in red and the associated error be the @typescript-eslint/unbound-method error.

### Fixing with a Configuration File

- Set the Tools/Options/TypeScript Analyzer/'TypeScript: lint using tsconfig.json' flag we set above to True back to False. 
- Save app.ts or rightclick in Solution Explorer/Run TypeScript Analyzer.  We should get the webserver error again.
- Create a new JavaScript file called .eslintrc.js in the root of your Node Console Application, at the same level as app.ts.  To do this rightclick the project name in Solution Explorer/Add/New File... and enter '.eslintrc.js' and click OK.
- Copy the contents of the base .eslintrc.js file into this new file and save.  The base .eslintrc.js should already be open in the editor from above, or open it with Tools/TypeScript Analyzer (ESLint)/Edit Default Config.
- Find the line `// TypeScript-only rules` in the new file you've created.  This is the section that sets up configuration for TypeScript files.  Underneath that line, and above the line `"files": ["*.ts", "*.tsx"],` add the code below
```json
"parserOptions": {
    "tsconfigRootDir": "{Paste path to tsconfig.json folder here}",
    "project": ["./tsconfig.json"]
},
```
- Replace '{Paste path to tsconfig.json folder here}' in the code you just pasted in with the path to the folder your tsconfig.json is in.  Unfortunately this has to have forward slashes (or doubled backslashes) as separators to work.  It doesn't need to include the tsconfig.json file name, which is specified in the second line.  So it will be something like `tsconfigRootDir: "C:/Dotnet/NodejsConsoleApp1/NodejsConsoleApp1",`.  Save the file.
- If you are using Visual Studio 2022 v17.4 or later go to Tools/Options/TypeScript Analyzer and set 'Enable local config (.eslintrc.js)' to True.  Leave 'Enable local node_modules' set to False.
- Lint app.ts by rightclicking on it in Solution Explorer/Run TypeScript Analyzer (ESLint), or just save the file.  You should see 'double' in `const { double } = arith` underlined in red and the associated error be the @typescript-eslint/unbound-method error.