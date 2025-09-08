using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WSBManager.Models;

namespace WSBManager.Services;

public enum RunMode
{
    ExistingLogin,
    System
};

public interface ISandboxExecutor
{
    /// <summary>
    /// Start a sandbox with the Configuration object
    /// </summary>
    Task<Guid> Start(Configuration config);

    /// <summary>
    /// List all running sandboxes
    /// </summary>
    Task<List<Guid>> List();

    /// <summary>
    /// Execute a new sandbox with the given parameters
    /// </summary>
    Task Exec(Guid id, string command, RunMode mode, string? directory);

    /// <summary>
    /// Stop a running sandbox by its ID
    /// </summary>
    Task Stop(Guid id);

    /// <summary>
    /// Share a folder with the sandbox
    /// </summary>
    Task Share(Guid id, string hostPath, string sandboxPath, bool readOnly = true);

    /// <summary>
    /// Connect to a running sandbox
    /// </summary>
    Task Connect(Guid id);

    /// <summary>
    /// Get the ip address of the sandbox
    /// </summary>
    Task<string> Ip(Guid id);
}

public class SandboxCommandBuilder(bool rawOutput = false, bool withBaseName = true)
{
    private string RawFlag => rawOutput ? "--raw" : "";
    private string BaseName => withBaseName ? "wsb" : "";

    public string Start(Configuration? config)
    {
        var xmlString = string.Empty;
        if (config != null)
        {
            var xmlSerializer = new XmlSerializer(typeof(Configuration));
            var xmlSetting = new XmlWriterSettings
            {
                Indent = false,
                NewLineHandling = NewLineHandling.None,
                OmitXmlDeclaration = true,
            };
            var emptyNs = new XmlSerializerNamespaces([XmlQualifiedName.Empty]);
            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, xmlSetting);
            xmlSerializer.Serialize(xmlWriter, config, emptyNs);
            xmlString = stringWriter.ToString();
            xmlString = $"--config \"{xmlString}\"";
        }

        return $"{BaseName} start {RawFlag} {xmlString}";
    }

    public string List() => $"{BaseName} list {RawFlag}";

    public string Exec(Guid id, string command, RunMode mode, string? directory)
    {
        var directoryParam = $"-d \"{directory}\"" ?? "";
        return $"{BaseName} exec {RawFlag} --id {id} -c {command} -r {mode.ToString()} {directoryParam}";
    }

    public string Stop(Guid id) => $"{BaseName} stop {RawFlag} --id {id}";

    public string Share(Guid id, string hostPath, string sandboxPath, bool readOnly)
    {
        var readOnlyFlag = readOnly ? "" : "--allow-write";
        return
            $"{BaseName} share {RawFlag} --id {id} -f \"{hostPath}\" -s \"{sandboxPath}\" {readOnlyFlag}";
    }

    public string Connect(Guid id) => $"{BaseName} connect {RawFlag} --id {id}";

    public string Ip(Guid id) => $"{BaseName} ip {RawFlag} --id {id}";
}

public class MockSandboxExecutor : ISandboxExecutor
{
    private readonly SandboxCommandBuilder _builder = new(rawOutput: true);

    public async Task<Guid> Start(Configuration config)
    {
        Console.WriteLine("Mock Start called");
        Console.WriteLine(_builder.Start(config));
        await Task.Delay(0); // Simulate async work
        return Guid.NewGuid();
    }

    public async Task<List<Guid>> List()
    {
        Console.WriteLine("Mock List called");
        Console.WriteLine(_builder.List());
        await Task.Delay(0); // Simulate async work
        return new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
    }

    public async Task Exec(Guid id, string command, RunMode mode, string? directory)
    {
        Console.WriteLine($"Mock Exec called with id={id}, command={command}, mode={mode}, directory={directory}");
        Console.WriteLine(_builder.Exec(id, command, mode, directory));
        await Task.Delay(0); // Simulate async work
    }

    public async Task Stop(Guid id)
    {
        Console.WriteLine($"Mock Stop called with id={id}");
        Console.WriteLine(_builder.Stop(id));
        await Task.Delay(0); // Simulate async work
    }

    public async Task Share(Guid id, string hostPath, string sandboxPath, bool readOnly = true)
    {
        Console.WriteLine(
            $"Mock Share called with id={id}, hostPath={hostPath}, sandboxPath={sandboxPath}, readOnly={readOnly}");
        Console.WriteLine(_builder.Share(id, hostPath, sandboxPath, readOnly));
        await Task.Delay(0); // Simulate async work
    }

    public async Task Connect(Guid id)
    {
        Console.WriteLine($"Mock Connect called with id={id}");
        Console.WriteLine(_builder.Connect(id));
        await Task.Delay(0); // Simulate async work
    }

    public async Task<string> Ip(Guid id)
    {
        Console.WriteLine($"Mock Ip called with id={id}");
        Console.WriteLine(_builder.Ip(id));
        await Task.Delay(0); // Simulate async work
        return "127.0.0.1";
    }
}

public class SandboxExecutor : ISandboxExecutor
{
    private static async Task<JsonObject> ExecuteCommand(string command, string arguments)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new Exception($"Exit code: {process.ExitCode}. StdErr: {error}");
        }

        if (string.IsNullOrWhiteSpace(output))
        {
            // Return empty JsonObject if no output
            return new JsonObject();
        }

        return JsonNode.Parse(output)?.AsObject() ?? new JsonObject();
    }

    private readonly string _baseCommand = "wsb";
    private readonly SandboxCommandBuilder _builder = new(rawOutput: true, withBaseName: false);

    public async Task<Guid> Start(Configuration config)
    {
        var command = _builder.Start(config);
        var result = await ExecuteCommand(_baseCommand, command);
        if (result.TryGetPropertyValue("Id", out var idNode) && Guid.TryParse(idNode?.ToString(), out var id))
        {
            return id;
        }

        Debug.WriteLine($"Error parsing json object: {result}");
        throw new Exception("Failed to parse sandbox ID from command output.");
    }

    public async Task<List<Guid>> List()
    {
        var command = _builder.List();
        var result = await ExecuteCommand(_baseCommand, command.Replace("wsb ", ""));

        if (result.TryGetPropertyValue("WindowsSandboxEnvironments", out var idsNode) && idsNode is JsonArray idsArray)
        {
            return idsArray.OfType<JsonObject>()
                .Select(obj => obj.TryGetPropertyValue("Id", out var idValue) ? idValue?.ToString() : null)
                .Where(idStr => idStr != null && Guid.TryParse(idStr, out var temp))
                .Select(idStr => Guid.Parse(idStr!))
                .ToList();
        }

        Debug.WriteLine($"Error parsing json object: {result}");
        throw new Exception("Failed to parse sandbox IDs from command output.");
    }

    public async Task Exec(Guid id, string command, RunMode mode, string? directory)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }

    public async Task Stop(Guid id)
    {
        var command = _builder.Stop(id);
        await ExecuteCommand(_baseCommand, command);
        Debug.WriteLine($"Sandbox {id} stopped.");
    }

    public async Task Share(Guid id, string hostPath, string sandboxPath, bool readOnly = true)
    {
        await Task.Delay(0);
        throw new NotImplementedException();
    }

    public async Task Connect(Guid id)
    {
        var command = _builder.Connect(id);
        await ExecuteCommand(_baseCommand, command);
        Debug.WriteLine($"Connected to sandbox {id}.");
    }

    public async Task<string> Ip(Guid id)
    {
        var command = _builder.Ip(id);
        var result = await ExecuteCommand(_baseCommand, command);
        if (result.TryGetPropertyValue("Networks", out var networks)
            && networks is JsonArray { Count: > 0 } networksArray)
        {
            var firstNetwork = networksArray[0]?.AsObject();
            if (firstNetwork != null && firstNetwork.TryGetPropertyValue("IpV4Address", out var ipNode))
            {
                return ipNode?.ToString() ?? string.Empty;
            }
        }

        Debug.WriteLine($"Error parsing json object: {result}");
        throw new Exception("Failed to parse sandbox IP from command output.");
    }
}