const config = {
    "root": true,
    "ignorePatterns": [".eslintrc.js", "_site/*"],

    "overrides": [
        {
            "files": ["*.md"],
            "plugins": ["md"],
            "parser": "markdown-eslint-parser",
            "rules": {
                "md/remark": [
                    "warn",
                    {
                        "plugins": [
                            "preset-lint-markdown-style-guide",
                            "frontmatter",
                            ["lint-maximum-line-length", false],
                            ["lint-maximum-heading-length", false],
                            ["lint-no-duplicate-headings", false],  // Perfectly valid 'Background' subheadings get flagged
                            ["lint-ordered-list-marker-value", [1, "ordered"]]
                        ],
                    },
                ],
            },
        },
    ],
};

module.exports = config;