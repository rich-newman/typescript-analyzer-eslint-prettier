# Editing tsconfig.json

The typescript-eslint plugin has some issues around noticing that a tsconfig.json file has changed between linting runs.  This can cause problems if you are editing the tsconfig, particularly if you are adding or removing files included in the project.  Obviously this is only an issue if you are set up to USE tsconfig.json, with Tools/Options/'TypeScript: Lint using tsconfig.json' set to True.

The problem shows up as an error in the Error List that says 'Parsing error: "parserOptions.project" has been set for @typescript-eslint/parser.
The file does not match your project config'.  This can happen if you add a file into the files tsconfig.json includes, or stop excluding one. 

Depending on the circumstances this may go away of its own accord if you save the tsconfig.json again and re-run the lint, or, in the worst case, it will only fix itself if you restart Visual Studio.

If you are seeing this problem there is a workaround. This is to add `createDefaultProgram: true` to the parserOptions section of your .eslintrc.js file.  This should make the error go away on the next lint.  

You should only do this WHILE you are editing the tsconfig as it apparently slows down the linting considerably.