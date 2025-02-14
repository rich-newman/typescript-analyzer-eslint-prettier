﻿# Developing the TypeScript Analyzer

This page explains how to perform some tasks relating to developing the TypeScript Analyzer (ESLint), and some associated issues.

## Running the TypeScript Analyzer in Visual Studio in Debug

**NOTE: for Visual Studio 2022 the instructions below only work if you have v17.8 or earlier installed.  The latest version of Visual Studio throws a null pointer exception on build. We are trying to find a fix for this at the time of writing. It's possible to [install v17.8 by using the command line](https://stackoverflow.com/a/78637995/4166522).**

To run the TypeScript Analyzer in debug ensure [node is installed](https://nodejs.org/en/download/), clone the repository, and open the appropriate solution with Visual Studio.  The 'appropriate solution' is TypeScriptAnalyzerEslint VS2022.sln if you are running Visual Studio 2022, or TypeScriptAnalyzerEslint.sln for Visual Studio 2019.  Both of these files are in the root folder.  The TypeScript Analyzer cannot be run in debug from Visual Studio 2017.

Now ensure TypeScriptAnalyzerESlintVsix64 is your startup project in VS2022, or TypeScriptAnalyzerEslintVsix in VS2019.  

You need the usual settings to run a VSIX in debug for this project, but these should be set correctly from the checkout if you have a default install location for Visual Studio.  To check this open the properties of the startup project you just set, and go to the Debug tab.  Make sure that the [start action is correctly set](https://bideveloperextensions.github.io/features/VSIXextensionmodel/) to 'Start external program', with a path to devenv.exe for the version of Visual Studio you are using, and that Command line arguments are set to '/rootsuffix Exp'.

Set a breakpoint and start debugging (F5).  It will open a new instance of Visual Studio in the experimental instance in which you can make use of TypeScript Analyzer until your breakpoint will be hit.

Once you have the solution building and running in either Visual Studio 2019 or 2022 then you should be able to run and debug the unit tests from the Test Explorer window.

## Issue: Project won't Build and Long User Names

If your Windows username is more than 15 characters you will find that the solution won't build with an error message 'Problem occurred while extracting the vsix to the experimental extensions path. Could not find a part of the path {path}'.  

This is due to a combination of the crapness of npm, the fact that paths longer than 259 characters don't work on Windows, the fact that the extension team decided to put the display name of the project in the path in the experimental instance and the fact that that stops the solution building even though once it's built they don't put that name in the path.

A workaround is to open the source.extension.vsixmanifest designer in Visual Studio, and change the display name to 'T', which should be short enough regardless of your user name.

## Debugging server.js

[server.js](https://github.com/rich-newman/typescript-analyzer-eslint-prettier/blob/main/src/TypeScriptAnalyzerEslintLinter/Node/server.js) is the JavaScript node code that runs the web server that runs ESLint.  It's possible to debug this, either from the Visual Studio experimental instance with Visual Studio running in debug, or more simply by creating a Node Console App that runs it.

### Debugging server.js with Visual Studio Running

- Run a debug build of the TypeScript Analyzer as above. You don’t have to start it in debug, although you can: Start without Debugging is also fine.
- Get it to lint and show the debug window.
- The window will have a line like ‘Debugger listening on ws://127.0.0.1:9229/c846f953-e308-4604-b588-d3270a8ad5ca’
- Start Edge and enter edge://inspect in the address bar.
- You should get a Devices screen. Click ‘Open Dedicated DevTools for Node’
- Now click Connection, and click on localhost:9229 if it’s there, or add it and click on it if not.
- If you now go to Console it should be showing the console output.
- If you click on server.js at the right end of a console line it should take you to that line in the server.js file on Sources.
- Now you can set breakpoints and step.

The debugger will reconnect on a later run if you leave it open.

### Debugging server.js from a Node Console Application

- Create a Node Console App (JavaScript)
- Open the [server.js](https://github.com/rich-newman/typescript-analyzer-eslint-prettier/blob/main/src/TypeScriptAnalyzerEslintLinter/Node/server.js) file and copy/paste this entire file into app.js
- Paste the dependencies section from [package.json in TypeScriptAnalyzerEslintLinter/Node](https://github.com/rich-newman/typescript-analyzer-eslint-prettier/blob/main/src/TypeScriptAnalyzerEslintLinter/Node/package.json) into your package.json.  
- Restore packages (rightclick npm, Install npm packages).
- Set up appropriate paths to lint in the 'const data = ' section at the bottom of app.js.  
- The Node Console App should now run in debug in Visual Studio, and you should be able to put breakpoints in the main code in VS.
- If you get an 'You have used a rule which requires parserServices to be generated.' error it means you need to pass a tsconfig.json in projectfiles. 

## Running the GitHub Pages Site Locally

Feb 2024 Edit: DON'T INSTALL a later version of Ruby than 3.1.4 when following the instructions below, as at the time of writing the Slate template used by the site doesn't work with it.  The instructions below work if you install Ruby 3.1.4 instead.

The docs project in this solution is our GitHub pages site.  You can make changes to this and preview them locally if you have Jekyll installed.

To install Jekyll there are [instructions on the Jekyll site](https://jekyllrb.com/docs/installation/windows/). Follow ‘Installation via RubyInstaller’, steps 1-4. Basically you run the installer for Ruby+Devkit 3.1.4-1, including the optional bit at the end where you accept the defaults by hitting Enter in the Command Prompt that appears.  Then you do `gem install jekyll bundler`. And then `jekyll -v`.

Once Jekyll is installed, and assuming you have the code checked out locally and loaded in Visual Studio, you can launch the site locally in a browser.

To do this:
- Rightclick the docs project in Solution Explorer and select 'Open in Terminal'.
- The first time you do this you will need to run 'bundle install' at this point.
- In the terminal that appears run command 'bundle exec jekyll serve'.
- This should run the web server and show an address to enter into a browser to see the site, usually <http://127.0.0.1:4000>.  If you start a browser and type in this URL you should see the site.  Don't use ctrl-c to copy the URL out of the terminal by the way, as that's the command to stop the web server.

## Dogfooding the TypeScript Analyzer Solution

The TypeScript Analyzer solution is set up so that if the Analyzer is installed locally it will lint the Markdown and JavaScript files in the project.  For this to work for the Markdown it needs some set up as below.  These are broadly the same steps you need to [get the Analyzer to lint Markdown](setupmarkdown.md) in general.

This linting uses the package.json and .eslintrc.js files that are included in the Solution in Solution Items.  If you look inside .eslintrc.js you will see that some of the default Markdown rules have been disabled.

### Steps to Lint the TypeScript Analyzer Solution

1. On a computer where you have cloned [the GitHub project for the TypeScript Analyzer](https://github.com/rich-newman/typescript-analyzer-eslint-prettier) ensure the [TypeScript Analyzer (ESLint, Prettier) extension](https://marketplace.visualstudio.com/items?itemName=RichNewman.TypeScriptAnalyzerEslintPrettier) is installed.  You can do this inside Visual Studio through Extensions/Manage Extensions and search for 'TypeScript Analyzer' of course.
2. Rightclick the solution (root) in Solution Explorer/Open in Terminal, and run `npm i` in the terminal that appears.  There is a package.json in the root of the project that contains the npm packages the Analyzer needs to run.
3. Go to Tools/Options/TypeScript Analyzer/ESLint and under the setting 'File extensions to lint' add ',md'.  So the full setting should be 'js,jsx,ts,tsx,mjs,cjs,md'.

To test this works copy and paste the top-level heading in a .md file, so that the heading is duplicated, and save the file.  You should get a `no-multiple-toplevel-headings` error.

If you revert the change above and run the TypeScript Analyzer on the entire solution it should have no errors.  It will tell you via messages that .eslintrc.js has been ignored.
