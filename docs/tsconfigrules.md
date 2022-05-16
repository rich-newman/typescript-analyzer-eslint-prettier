# Rules For Finding tsconfig.json Files

The rules around how the TypeScript Analyzer finds tsconfig.json files if `TypeScript: lint with tsconfig.json` is set to True are a little complicated.  For completeness full details are below.

## Linting Individual .ts or .tsx Files

- The TypeScript Analyzer can be run for an individual .ts or .tsx file, either from Solution Explorer, by opening or saving a file, or by rightclicking in the Visual Studio code window.  
- In these cases the TypeScript Analyzer tries to find the nearest associated tsconfig.json file.  If there is one or more in the same folder it will use them, otherwise it will search up the folder tree looking in each parent folder until it finds one or more in the same folder.  When an appropriate tsconfig.json or group of them is found they are passed to ESLint.  If no tsconfig.json file is found no linting takes place.  
- Linting results are then filtered so that only errors for the original file are displayed, if any.

## Linting Groups of Files

- If the TypeScript Analyzer is run for an entire solution, an individual Visual Studio project, or an individual folder then it finds all tsconfig.json files in any project in the solution, in the project, or in the folder and any subfolder and hands them all to ESLint.
- The TypeScript Analyzer can be run for one tsconfig.json by right-clicking it in Solution Explorer and selecting 'Run TypeScript Analyzer'.  In this case just the one tsconfig.json file is passed to ESLint.
- In all the cases above a tsconfig.json will only be passed to ESLint if it is included in a Visual Studio project.
- In all the cases above a tsconfig.json, or list of them, is passed to ESLint, it lints all contents of them, and all errors are reported with no filtering applied.  This means individual files (.ts or .tsx) do NOT have to be included in a Visual Studio project.

## Linting Multiple Selections in Solution Explorer

- If more than one item is selected in Solution Explorer then the rules above are applied and a union of all tsconfig.jsons found is passed to ESLint.  Results are only filtered to the individual files if **all** files are individual .ts or .tsx files.

## Identifying tsconfig.json Files

- The TypeScript Analyzer identifies tsconfig.json files using the pattern detailed in Tools/Options/TypeScript Analyzer/ESLint/`Pattern to match tsconfig file names`.
- By default the pattern is `tsconfig.json,tsconfig.*.json`, where the * means a wildcard. So the Analyzer will consider a file called 'tsconfig.json' or a file called 'tsconfig.foo.json' as tsconfig.json files for these purposes.

It's clearly possible in some of the scenarios above that one TypeScript file might be included in more than one tsconfig.json passed to ESLint.  If there are any exact duplicate errors in the results then only one copy of the error is shown in the Visual Studio Error List.  Otherwise all errors are shown.

In all cases a tsconfig.json file will only be handed to ESLint if it is in a Visual Studio project.  However, individual files do not have to be included in a Visual Studio project to be linted if the TypeScript Analyzer is set to use tsconfig.json files.  
