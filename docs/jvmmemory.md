# JavaScript Virtual Machine Running Out of Memory

## Summary

**If you are seeing lint runs failing with 'failed to communicate' errors, and you are linting a large project, it may be worth increasing the JavaScript VM Memory setting in Tools/Options/TypeScript Analyzer to 4096.**

## Detail

The Analyzer sets up a node process to run ESLint.  Node runs V8, a JavaScript virtual machine.  Node allocates some memory to this, but it's 1GB or less, and that may not be enough for linting a large project with a lot of files.

The problem here is that if ESLint runs out of memory then Node just crashes, with no chance to report errors back to Visual Studio. So your linting may just stop working with no obvious indication as to why.  You are likely to get a generic error message in the Error List like the one below, although clearly a message like this can be caused by other things.

'Failed to communicate with ESLint server: The underlying connection was closed: An unexpected error occurred on a receive. --> Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.'

You can tell node when you start it to allocate more memory to V8 with the 'max-old-space' command line parameter. In general you don't want to do this just for basic linting.  You probably don't want a large slice of the memory on your machine potentially being used for a linter.

However, if you are trying to lint all of a very large project you may well run into this problem.  To resolve it you can set the Tools/Options/TypeScript Analyzer/Eslint/`JavaScript VM Memory setting`.  If this setting is set to 0, the default, then node will use its default memory allocation.  If you set it to a high value, say 4096, then max-old-space will be set to 4096MB when node is started, and more memory will be available to Node/V8, which may allow your linting to run.