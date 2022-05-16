using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TypeScriptAnalyzerEslintLinter
{
    public class NodeServer
    {
        private const string BASE_URL = "http://localhost";
        // process is static, so even though we new the Linter and the NodeServer on every lint call
        // we reuse the same process if it's alive and kicking
        private static Process process;
        private static readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        public int BasePort { get; private set; }

        public async Task<string> CallServerAsync(string path, ServerPostData postData, Func<string, bool, Task> logger, int jvmMemory)
        {
            try
            {
                await EnsureNodeProcessIsRunningAsync(logger, jvmMemory);
                MsEslintConfigFile.Disable();

                string url = $"{BASE_URL}:{BasePort}/{path.ToLowerInvariant()}";
                string json = JsonConvert.SerializeObject(postData);
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    Benchmark.Log("Before server call");
                    var result = await client.UploadStringTaskAsync(url, json);
                    Benchmark.Log($"After server call");
                    return result;
                }
            }
            catch (WebException ex)
            {
                try
                {
                    if (ex.Response == null)
                    {
                        await logger(ex.ToString(), true);
                        // e.g. if firewall blocks connection: "Unable to connect to the remote server"
                        return "Failed to communicate with ESLint server: " + ex.Message + (ex.InnerException != null ? " --> " + ex.InnerException.Message : "");
                    }
                    StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream());
                    string resp = await streamReader.ReadToEndAsync();
                    dynamic obj = JsonConvert.DeserializeObject(resp);
                    if (obj.error == true)
                    {
                        string messageFromServer = obj.message;
                        bool isTypeScriptRunTimeInfoException = IsTypeScriptRunTimeInfoException(messageFromServer);
                        string logOutput = obj.log;
                        if (!string.IsNullOrEmpty(logOutput)) await logger(logOutput, false);
                        await logger("ESLint reported error" + (isTypeScriptRunTimeInfoException ? analyzerTypeInfoNeededMessage : "")
                            + ": ", true);
                        await LogException(logger, obj);
                        return "ESLint webserver error" + (isTypeScriptRunTimeInfoException ? analyzerTypeInfoNeededMessage : "")
                            + ". For more details see Output window: " + (messageFromServer ?? "");
                    }
                    await logger("WebException without error from ESLint. Response: " + resp, true);
                    return "Internal error";
                }
                catch (Exception innerEx)
                {
                    // This should never happen
                    await logger("Invalid response from ESLint server: " + innerEx.ToString(), true);
                    return "Internal error. For more details see Output window.";
                }
                finally
                {
                    Down();
                }
            }
            catch (Exception ex)
            {
                // e.g. if EXE not found or fails to start due to port-in-use error
                Down();
                await logger(ex.ToString(), true);
                return "Failed to start ESLint server: " + ex.Message + (ex.InnerException != null ? " --> " + ex.InnerException.Message : "");
            }
        }

        private static async Task LogException(Func<string, bool, Task> logger, dynamic obj)
        {
            try
            {
                string exceptionString = obj.exception;
                dynamic exception = JsonConvert.DeserializeObject(exceptionString);
                await logger(JsonConvert.SerializeObject(exception, Formatting.Indented).Replace("\\n", "\n"), true);
            }
            catch (Exception ex)
            {
                await logger("Unable to log exception from server: " + ex.Message, true);
            }
        }

        private readonly string runTimeInfoExceptionMessage = "You have used a rule which requires parserServices to be generated. " +
            "You must therefore provide a value for the \"parserOptions.project\" property for @typescript-eslint/parser";
        private bool IsTypeScriptRunTimeInfoException(string messageFromServer) 
            => messageFromServer?.Contains(runTimeInfoExceptionMessage) ?? false;
        private readonly string analyzerTypeInfoNeededMessage =
            " caused by a rule included in config that needs type information, but use of tsconfig not enabled.  " +
            "To fix this set \'Tools/Options/TypeScript Analyzer/TypeScript/Lint with tsconfig.json files\' to True";

        public void Down()
        {
            if (process != null)
            {
                try
                {
                    if (!process.HasExited)
                        process.Kill();
                }
                finally
                {
                    process.Dispose();
                    process = null;
                }
            }
        }

        private async Task EnsureNodeProcessIsRunningAsync(Func<string, bool, Task> logger, int jvmMemory)
        {
            await locker.WaitAsync();
            try
            {
                if (process != null && !process.HasExited) return;
                try
                {
                    Down();
                    SelectAvailablePort();
                    await logger("TypeScript Analyzer initialization, port selected: " + this.BasePort, false);

                    ProcessStartInfo start = new ProcessStartInfo(Path.Combine(ExecutionPath, "node.exe"))
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = $"\"{Path.Combine(ExecutionPath, "server.js")}\" {BasePort}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    if (jvmMemory != 0) start.Arguments = $"--max_old_space_size={jvmMemory} " + start.Arguments;
#if DEBUG
                    start.WindowStyle = ProcessWindowStyle.Normal;
                    start.CreateNoWindow = false;
                    // Note: a second process won't start if we already have a debug build running because of the line below.
                    // Inspect always uses the same port (9229). Run one of the builds as a release build, or take the line out.
                    start.Arguments = "--no-deprecation --inspect " + start.Arguments;
#endif
                    process = Process.Start(start);

                    // Give the node server some time to initialize
                    await Task.Delay(100);

                    if (process.HasExited)
                        throw new Exception($"ESLint server failed to start: {start.FileName} {start.Arguments}");
                }
                catch (Exception)
                {
                    Down();
                    throw;
                }
            }
            finally { locker.Release(); }
        }

        private void SelectAvailablePort()
        {
            // Creates the Socket to send data over a TCP connection.

            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    socket.Bind(endPoint);
                    IPEndPoint endPointUsed = (IPEndPoint)socket.LocalEndPoint;
                    BasePort = endPointUsed.Port;
                }
            }
            catch (SocketException)
            {
                /* Couldn't get an available IPv4 port */
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
                        socket.Bind(endPoint);
                        IPEndPoint endPointUsed = (IPEndPoint)socket.LocalEndPoint;
                        BasePort = endPointUsed.Port;
                    }
                }
                catch (SocketException)
                {
                    /* Couldn't get an available IPv6 port either */
                }
            }
        }

        private static string GetNodeDirectory()
        {
            string toolsDir = Environment.GetEnvironmentVariable("VS140COMNTOOLS");

            if (!string.IsNullOrEmpty(toolsDir))
            {
                string parent = Directory.GetParent(toolsDir).Parent.FullName;
                return Path.Combine(parent, @"IDE\Extensions\Microsoft\Web Tools\External\Node");
            }

            return string.Empty;
        }

        public static readonly string ExecutionPath = Path.Combine(AssemblyDirectory, Constants.NODE_FOLDER_NAME); // + Constants.VERSION);

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
