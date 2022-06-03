# Running in Folder View

## Background

In Visual Studio Solution Explorer it is possible to display files and folders rather than a Solution and Projects.  You can do this by clicking the 'Switch between solutions and available views' icon in the Solution Explorer toolbar, and then doubleclicking Folder View:

![Folder View](assets\images\folderview.jpg)

If you do this Solution Explorer shows all folders and files regardless of whether they are in a Visual Studio project or not:

![Folder View Files](assets\images\folderviewfiles.jpg)

## TypeScript Analyzer and Folder View

The TypeScript Analyzer continues to work in Folder View:

- If you doubleclick a file to open it, and it is a file the Analyzer can lint, then it will be linted and any errors shown.
- The context menu items in the Visual Studio code file window continue to work (Run TypeScript Analyzer (ESLint) on Code File/Fix TypeScript Analyzer (ESLint) Errors in Code File if Possible)
- In Visual Studio 2019 and Visual Studio 2022 the context menu options in Solution Explorer will work for individual files and folders.  At the time of writing this does not work in Visual Studio 2017: Microsoft changed the way this works between Visual Studio 2017 and 2019.
- If the 'TypeScript: lint using tsconfig.json' setting is True then the TypeScript Analyzer will apply the [same rules for linting](tsconfigrules.md) as it does in the regular Solution Explorer view.
