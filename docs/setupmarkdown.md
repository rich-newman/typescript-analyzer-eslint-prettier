# Set Up for Markdown Plugin

## Overview

ESLint has a [plugin to lint and fix Markdown files](https://github.com/leo-buneev/eslint-plugin-md).  It lints Markdown, not JavaScript or TypeScript inside the markdown.  

We can set up the TypeScript Analyzer to use this plugin on Markdown files in Visual Studio projects.  This shows both how to use a plugin, and how to apply it to files that are not currently linted.  Errors will be shown in the Visual Studio Error List and underlined in the code window.

Instructions on how to do this in a TypeScript Node.js Console Application are below.

## Instructions

1. Create a new Blank Node.js Console Application (TypeScript).
2. Doubleclick package.json in Solution Explorer to edit it.  Replace the existing devDependencies section with the code below and save.  These are the dependencies that the [TypeScript Analyzer needs locally](installs.md#localinstall), plus the new plugin, eslint-plugin-md:
``` json
"devDependencies": {
    "@types/node": "16.11.64",
    "@typescript-eslint/eslint-plugin": "5.39.0",
    "@typescript-eslint/parser": "5.39.0",
    "eslint": "8.24.0",
    "eslint-plugin-prettier": "4.2.1",
    "prettier": "2.7.1",
    "typescript": "4.8.4",
    "eslint-plugin-md": "1.0.19"
}
```
3. Install these npm packages by rightclicking 'npm' in Solution Explorer and running 'Install npm Packages' in VS2019 or VS2022, or 'Install Missing npm Packages' in VS2017.
4. Create a new [local configuration file](localconfiguration.md) called .eslintrc.js in the project.  To do this rightclick the project name in Solution Explorer, Add/New File..., enter .eslintrc.js, and click OK.
5. Copy the [file contents on this link](setupmarkdownconfig.md) into your new file and save.  This is the usual default configuration file for the TypeScript Analyzer modified to enable the Markdown plugin.  The actual changes made are [detailed at the end of this article](setupmarkdown.md#changesmadetodefaultconfig).
6. **Go to Tools/Options/TypeScript Analyzer/ESLint and under 'File extensions to lint' add ',md' to the existing list.**  After the change the 'File extensions to lint' setting should look like 'js,jsx,ts,tsx,mjs,cjs,md'.
7. Open README.md, copy the header that's in there and paste it on a new line, so it's duplicated.  The TypeScript Analyzer should generate md/remark warnings 'Don't use multiple top level headings' and 'Do not use headings with similar content'.

## <a name="changesmadetodefaultconfig"></a>Changes Made to Default Configuration File

To create the [configuration file for Markdown](setupmarkdownconfig.md) one change was made to the [code in the default configuration file](defaultconfig.md#defaulteslintrc):

This was to add the section below as an element in the first overrides array, after the 'TypeScript-only Rules' entry.  This is fundamentally the same entry as is specified in the [documentation for the plugin](https://github.com/leo-buneev/eslint-plugin-md#usage), although we haven't explicitly specified the parser.  You can specify the parser as on the link if you want to, it will work the same.  The completed file should look like [the code on this link](setupmarkdownconfig.md):
``` javascript
        {
            "files": ["*.md"],
            "extends": ["plugin:md/recommended"],
            "rules": {
                "md/remark": "warn",
            },
        },
```
## Configuring Individual Rules

The md/remark rule enabled above wraps and enables a number of [remark rules for Markdown](https://github.com/remarkjs/remark-lint#rules).  If you want to configure the individual Markdown rules then you need to use the [syntax described in the plugin documentation](https://github.com/leo-buneev/eslint-plugin-md#supported-rules).  In particular you need to ensure you enable the plugin 'preset-link-markdown-style-guide'.  Otherwise the default recommended rules may get disabled.

For example, to disable the rule [maximum-line-length](https://github.com/remarkjs/remark-lint/tree/main/packages/remark-lint-maximum-line-length) and to configure the rule [ordered-list-marker-value](https://github.com/remarkjs/remark-lint/tree/main/packages/remark-lint-ordered-list-marker-value) to accept ordered numbers in a list rather than repeated ones, you would replace the code above in the section [Changes Made to Default Configuration File](setupmarkdown.md#changesmadetodefaultconfig) with the code below:
``` javascript
        {
            "files": ["*.md"],
            "extends": ["plugin:md/recommended"],
            "rules": {
                "md/remark": [
                    "warn",
                    {
                        "plugins": [
                            "preset-lint-markdown-style-guide",
                            "frontmatter",
                            ["lint-maximum-line-length", false],
                            ["lint-ordered-list-marker-value", [1, "ordered"]]
                        ],
                    },
                ],
            },
        },
```