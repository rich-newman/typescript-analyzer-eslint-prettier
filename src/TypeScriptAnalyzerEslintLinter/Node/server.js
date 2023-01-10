"use strict";

const http = require("http"),
    fs = require("fs"),
    path = require("path");

var start = function (port) {
    http.createServer(function (req, res) {
        if (!req.url || req.url.length < 2) {
            res.writeHead(200, { "Content-Type": "text/plain" });
            res.end();
            return;
        }

        const route = req.url.substring(1);
        let body = "";

        if (route === "ping") {
            res.writeHead(200, { "Content-Type": "text/plain" });
            res.end("pong");
            return;
        }

        req.on("data", function (data) {
            body += data;
        });

        req.on("end", async function () {
            let log = () => {};
            let logOutput = "";
            try {
                if (body === "") return;
                if (route === "eslint") {
                    const data = JSON.parse(body);
                    // For outputting tsconfig or eslintrc contents we pass message as an object.  Node console.log handles this, although
                    // only to an nesting level of 2.  For the log output returned to VS we use JSON.stringify with no limit on nesting.
                    if (data.debug || data.enablelogging)
                        log = (message, outputMessage) => {
                            if (data.enablelogging) {
                                const logOutputMessage =
                                    outputMessage !== undefined
                                        ? outputMessage
                                        : message instanceof Object
                                            ? JSON.stringify(message, null, 2) // eslint-disable-line
                                            : message;                         // eslint-disable-line
                                logOutput += `[${new Date().toISOString()}] ${logOutputMessage}\r\n`;
                            }
                            if (data.debug)
                                console.log(
                                    `[${new Date().toISOString()}]`,
                                    message
                                );
                        };
                    log("Running request on port " + port);
                    const result = await linteslint(data, log);
                    const response = { result: result, log: logOutput };
                    res.writeHead(200, { "Content-Type": "application/json" });
                    res.write(JSON.stringify(response));
                } else {
                    throw Error("No linter found for " + route);
                }
            } catch (e) {
                log(e);
                const responseObject = {
                    exception: JSON.stringify(e, Object.getOwnPropertyNames(e)),
                    error: true,
                    message: e.message,
                    log: logOutput,
                };
                let response = JSON.stringify(responseObject);
                if (response.length > 65536 && logOutput !== "") {
                    responseObject.log =
                        "Log output not sent: too long to be returned as part of error";
                    response = JSON.stringify(responseObject);
                }
                res.writeHead(500, { "Content-Type": "application/json" });
                res.write(response);
            } finally {
                res.end();
            }
        });
    }).listen(port);
};

// If we're not passing text we can pass a list of files in data.files, or a list of tsconfigs in data.projectfiles, or both
// If we pass both there should be no individual .ts or .tsx files in data.files: just other extensions
// If we're passing text we will have the associated file in data.files[0],
// if we're using a tsconfig that additionally will be in data.projectfiles[0]
async function linteslint(data, log) {
    setWorkingDirectory(data.dirname, log);
    const dirName = data.dirname === "" ? __dirname : data.dirname;
    if (data.debug || data.enablelogging) logInitialValues(data, dirName, log);
    process.env["USERPROFILE"] = data.configfolder;
    let allResults = [];
    if (data.projectfiles.length > 0) {
        const results = await eslinttsconfigs(data, dirName, log);
        allResults = allResults.concat(results);
    }
    if (
        data.files.length > 0 &&
        (data.text === null ||
            (data.text !== null && data.projectfiles.length === 0))
    ) {
        const results = await calleslint(data, data.files, dirName, log, null);
        allResults = allResults.concat(results);
    }
    return allResults;
}

function setWorkingDirectory(dirName, log) {
    if (dirName !== "") {
        try {
            process.chdir(dirName);
        } catch (err) {
            log(
                "Unable to set working directory to '" +
                    dirName +
                    "'. Error: " +
                    err
            );
        }
    }
}

function logInitialValues(data, dirName, log) {
    log("LINTING RUN STARTED");
    log("Try to fix: " + data.fixerrors);
    log("Enable ignore: " + data.enableignore);
    log("Debug enabled: " + data.debug);
    log("Logging enabled: " + data.enablelogging);
    log("Local configuration enabled: " + data.enablelocalconfig);
    const baseConfigFolder =
        data.configfolder === "" ? "Not found" : data.configfolder;
    log("Base Config folder: " + baseConfigFolder);
    const ignoreFile = data.ignorefile === "" ? "Not found" : data.ignorefile;
    log("Ignore file: " + ignoreFile);
    log("Root Directory for Node: " + dirName);
    log("Extension install directory: " + __dirname);
    log("Working directory: " + process.cwd());
    if (data.logfilenames) {
        log("Files to lint:");
        data.files.forEach((f) => log(f));
        log("Project Files to lint:");
        data.projectfiles.forEach((f) => log(f));
    }
    //log(process.versions);
}

async function eslinttsconfigs(data, dirName, log) {
    let allResults = [];
    for (const tsconfigFile of data.projectfiles) {
        const parserOptions = {
            tsconfigRootDir: path.dirname(tsconfigFile),
            project: [tsconfigFile],
            sourceType: "module",
        };
        const filesInTsconfig = gettsconfigContents(
            tsconfigFile,
            dirName,
            data,
            log
        );
        const results = await calleslint(
            data,
            data.text === null ? filesInTsconfig : data.files,
            dirName,
            log,
            parserOptions
        );
        allResults = allResults.concat(results);
    }
    return allResults;
}

function gettsconfigContents(tsconfigFile, dirName, data, log) {
    // The replace deals with node leaving the BOM in the string when reading a file
    const contents = fs
        .readFileSync(tsconfigFile, "utf8")
        .replace(/^\uFEFF/, "");
    const typescript = require(dirName + "\\node_modules\\typescript");
    const jsonContents = typescript.parseConfigFileTextToJson(
        tsconfigFile,
        contents
    );
    const tsconfigFilePath = path.dirname(tsconfigFile);
    const tsResult = typescript.parseJsonConfigFileContent(
        jsonContents.config,
        typescript.sys,
        tsconfigFilePath
    );
    if (data.logtsconfig) {
        log("TSCONFIG.JSON CONTENTS START");
        log(tsResult);
        log("TSCONFIG.JSON CONTENTS END");
    }
    return tsResult.fileNames || [];
}

// files are either regular files, or a list of tsconfigs if we're using tsconfig
async function calleslint(data, files, dirName, log, parserOptions) {
    const ESLint = createESLint(dirName, log);
    const options = createOptions(data, dirName, log, parserOptions);
    const eslint = new ESLint(options);
    if (fs.existsSync(data.msconfigfile)) deleteMsConfigFile(data, log);
    if (data.logfirstconfig) await logConfigForFirstFile(files, eslint, log);
    //throwTestError(log);
    log("Before call to ESLint");
    const results =
        data.text === null
            ? await eslint.lintFiles(files)
            : await eslint.lintText(data.text, { filePath: files[0] }); // files[0] is a tsconfig if we're using those
    log("After call to ESLint");
    await logResults(eslint, data, results, log);
    log("After logging results");
    if (data.fixerrors) await fixErrors(ESLint, results, log);
    return results;
}

function createESLint(dirName, log) {
    const pathToRequire = dirName + "\\node_modules\\eslint";
    log("Path for ESLint: " + pathToRequire);
    const { ESLint } = require(pathToRequire);
    if (ESLint === null || ESLint === undefined)
        throw "ERROR: Unable to load ESLint. Is ESLint version >= 7.0.0 available to the Analyzer?";
    log("Successfully Loaded ESLint version: " + ESLint.version);
    return ESLint;
}

function createOptions(data, dirName, log, parserOptions) {
    const options = {
        resolvePluginsRelativeTo: dirName,
        ignore: data.enableignore,
        useEslintrc: data.enablelocalconfig,
    };
    options.overrideConfig = {};
    if (!data.enablelocalconfig)
        options.overrideConfigFile = data.configfolder + "\\.eslintrc.js";
    // parserOptions is only set if we are linting with tsconfig.json in this run
    if (parserOptions) options.overrideConfig.parserOptions = parserOptions;
    // We set the TypeScript parser in the install as the default parser in the base config
    // This is because the typescript-eslint plugin needs it specified even if it's in the same node_modules as its own code
    // Any parser at top-level in actual config will override this
    options.baseConfig = {
        parser: dirName + "\\node_modules\\@typescript-eslint\\parser",
    };
    if (data.fixerrors) {
        options.fix = true;
    }
    setIgnorePath(options, data, log);
    return options;
}

function setIgnorePath(options, data, log) {
    if (data.ignorefile !== "" && fs.existsSync(data.ignorefile)) {
        options.ignorePath = data.ignorefile;
        log("Ignore file set: " + data.ignorefile);
    }
}

// We don't expect this to be called: we only call if the file exists, and the C# backs it up
// and removes it anyway
function deleteMsConfigFile(data, log) {
    try {
        fs.unlinkSync(data.msconfigfile);
        log("Deleted Microsoft ESLint config file: " + data.msconfigfile);
    } catch (e) {
        // prettier-ignore
        log("Failed to delete Microsoft ESLint config file: " + data.msconfigfile + ". " + e);
    }
}

async function logConfigForFirstFile(files, eslint, log) {
    if (files.length > 0) {
        const config = await eslint.calculateConfigForFile(files[0]);
        log("CALCULATED CONFIG FOR FILE START");
        log(config);
        log("CALCULATED CONFIG FOR FILE END");
    }
}

//function throwTestError(log) {
//    var err = Error("It's a Test Error");
//    log("Before throwing error");
//    throw err;
//}

async function logResults(eslint, data, results, log) {
    let message, outputMessage;
    if (data.enablelogging) {
        const vsFormatter = await eslint.loadFormatter("visualstudio");
        outputMessage = "RESULTS:\r\n" + vsFormatter.format(results);
    }
    if (data.debug) {
        const stylishFormatter = await eslint.loadFormatter("stylish");
        message = stylishFormatter.format(results);
    }
    log(message, outputMessage);
}

async function fixErrors(ESLint, results, log) {
    log("Fixing errors");
    await ESLint.outputFixes(results);
    log("After fixing errors");
}

if (__filename.split("\\").pop() !== "app.js") start(process.argv[2]);

// For debugging create a Node Console App (JavaScript), paste this entire file into app.js, paste the dependencies section from
// package.json in TypeScriptAnalyzerEslintLinter/Node into your package.json and restore packages, then set up appropriate paths to lint in the const
// data section below.  It should now run, and you should be able to put breakpoints in the main code above in VS.
// NOTE: if you get an 'You have used a rule which requires parserServices to be generated.' error it means you need to
// to pass tsconfig.json in projectfiles.  You can also debug in Edge using edge://inspect.  See the readme.
// https://gist.github.com/mikeal/1840641#gistcomment-2896667
const net = require("net");
function getPort(port = 80) {
    const server = net.createServer();
    return new Promise((resolve, reject) =>
        server
            .on("error", (error) =>
                error.code === "EADDRINUSE"
                    ? server.listen(++port)
                    : reject(error)
            )
            .on("listening", () => server.close(() => resolve(port)))
            .listen(port)
    );
}

async function testServer() {
    const port = await getPort();
    start(port);
    console.log("Webserver started on port " + port);
    //const file = "C:\\Dotnet\\NodejsConsoleApp1\\NodejsConsoleApp1\\app.js";
    //const textString = fs
    //    .readFileSync(file, "utf8")
    //    .replace(/^\uFEFF/, "");
    const data = JSON.stringify({
        configfolder: "C:\\Users\\Rich Newman\\TypeScriptAnalyzerConfig",
        msconfigfile: "C:\\Users\\Rich Newman\\.eslintrc",
        //configFolder: "",
        //files: ["C:\\Dotnet\\NodejsConsoleApp3TestTypeScript\\NodejsConsoleApp3TestTypeScript\\JavaScript1.js",
        //    "C:\\Dotnet\\NodejsConsoleApp3TestTypeScript\\NodejsConsoleApp3TestTypeScript\\TypeScript1.ts"],
        text: null, // Make null to read the file from disk
        //projectfiles: ["C:\\Dotnet\\NodejsConsoleApp3TestTypeScript\\NodejsConsoleApp3TestTypeScript\\tsconfig.json"],
        //files: [],
        files: [
            "C:\\Dotnet\\NodejsConsoleApp9\\NodejsConsoleApp9\\app.js",
            "C:\\Dotnet\\NodejsConsoleApp9\\NodejsConsoleApp9\\test\\apptest.js",
        ],
        projectfiles: [],
        fixerrors: false,
        // dirname is folder that contains node_modules, full path. If dirname is empty string we use the installed node_modules.
        dirname: __dirname,
        debug: true, // Logs to console window if true: set to false and use console.log if you want to find your output more easily
        enablelogging: true, // If true sets the log property of the response object to the log output
        logfilenames: true,
        logfirstconfig: true,
        logtsconfig: false,
    });

    const options = {
        hostname: "localhost",
        port: port,
        path: "/eslint",
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Content-Length": data.length,
        },
    };

    const req = http.request(options, () => {
        //console.log("RESPONSE FROM WEBSERVER:");
        //console.log(`statusCode: ${res.statusCode}`);
        //res.on("data", d => {
        //    console.log(d.toString());
        //})
    });

    req.on("error", (error) => {
        console.error(error);
    });

    req.write(data);
    req.end();
}

if (__filename.split("\\").pop() === "app.js") testServer();
