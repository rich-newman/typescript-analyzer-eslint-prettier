# Set Up for Vue Plugin - Basic vue.js Web Application

## Overview

This document discusses the Basic vue.js Web Application project template in Visual Studio 2017 and 2019, and concludes you should no longer use it.

This is a legacy project template that has been around for a while.  We have [separate documentation for setting up the TypeScript Analyzer](setupvue.md) with the more recent 'Standalone Vue Project' and 'Vue and ASP.NET Core' templates which are in Visual Studio 2022 only.  If you want to set up a Vue solution in Visual Studio we suggest you use these templates.

There are different Basic vue.js Web Application project templates for both JavaScript and TypeScript in Visual Studio 2019 and Visual Studio 2017.  These templates were at one stage in Visual Studio 2022 but have been removed.

**At the time of writing none of the Basic vue.js Web Application project templates works very well in any version of Visual Studio, and as a result the guidance here has been withdrawn and we don't recommend that you use these project templates.**  The exact problems with each template type are described below.

## Visual Studio 2019 JavaScript and TypeScript

At the time of writing the Basic vue.js Web Application projects in Visual Studio 2019 won't build if you have the latest LTS version of Node installed.

You get an error '0308010C:digital envelope routines::unsupported' which is related to changes the Node team made in v17 to remove a security problem with SSL.

These templates did work with v16 of Node, but as a result of the errors with later secure versions we now recommend you don't attempt to use these templates, and the instructions for their use with the TypeScript Analyzer have been removed.

## Visual Studio 2017, JavaScript and TypeScript

Visual Studio 2017 is no longer fully supported by Microsoft.  It is [in extended support.](https://devblogs.microsoft.com/visualstudio/support-ends-for-older-versions-of-visual-studio-feb2022/#:~:text=Visual%20Studio%202017%3A%20mainstream%20support,baseline%20to%20remain%20under%20support.)  Neither of the Vue templates in Visual Studio 2017 for new projects works well, and we suggest you do not use them.

- Basic vue.js Web Application, TypeScript will install and build, but will not run from Visual Studio.

- Basic vue.js Web Application, JavaScript will not install correctly.  This is because its peer dependencies are inconsistent, and since version 7 of npm the installer will refuse to install if this is the case.  As in the instructions for Visual Studio 2019, JavaScript we can force an install with the `npm i -legacy-peer-deps` command, after which the project created will work.  However, the packages installed are quite old and will not work with the TypeScript Analyzer.