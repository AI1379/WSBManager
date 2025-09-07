using System;
using System.Collections.Generic;
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
    Guid Start(Configuration config);

    /// <summary>
    /// List all running sandboxes
    /// </summary>
    List<Guid> List();

    /// <summary>
    /// Execute a new sandbox with the given parameters
    /// </summary>
    void Exec(Guid id, string command, RunMode mode, string? directory);

    /// <summary>
    /// Stop a running sandbox by its ID
    /// </summary>
    void Stop(Guid id);

    /// <summary>
    /// Share a folder with the sandbox
    /// </summary>
    void Share(Guid id, string hostPath, string sandboxPath, bool readOnly = true);

    /// <summary>
    /// Connect to a running sandbox
    /// </summary>
    void Connect(Guid id);

    /// <summary>
    /// Get the ip address of the sandbox
    /// </summary>
    string Ip(Guid id);
}

public class SandboxCommandBuilder(bool rawOutput = false)
{
    private string RawFlag => rawOutput ? "--raw" : "";
    private string BaseName => "wsb";

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
            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, xmlSetting);
            xmlSerializer.Serialize(xmlWriter, config);
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

    public Guid Start(Configuration config)
    {
        Console.WriteLine("Mock Start called");
        Console.WriteLine(_builder.Start(config));
        return Guid.NewGuid();
    }

    public List<Guid> List()
    {
        Console.WriteLine("Mock List called");
        Console.WriteLine(_builder.List());
        return new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
    }

    public void Exec(Guid id, string command, RunMode mode, string? directory)
    {
        Console.WriteLine($"Mock Exec called with id={id}, command={command}, mode={mode}, directory={directory}");
        Console.WriteLine(_builder.Exec(id, command, mode, directory));
    }

    public void Stop(Guid id)
    {
        Console.WriteLine($"Mock Stop called with id={id}");
        Console.WriteLine(_builder.Stop(id));
    }

    public void Share(Guid id, string hostPath, string sandboxPath, bool readOnly = true)
    {
        Console.WriteLine(
            $"Mock Share called with id={id}, hostPath={hostPath}, sandboxPath={sandboxPath}, readOnly={readOnly}");
        Console.WriteLine(_builder.Share(id, hostPath, sandboxPath, readOnly));
    }

    public void Connect(Guid id)
    {
        Console.WriteLine($"Mock Connect called with id={id}");
        Console.WriteLine(_builder.Connect(id));
    }

    public string Ip(Guid id)
    {
        Console.WriteLine($"Mock Ip called with id={id}");
        Console.WriteLine(_builder.Ip(id));
        return "127.0.0.1";
    }
}

public class SandboxExecutor : ISandboxExecutor
{
    public Guid Start(Configuration config)
    {
        throw new NotImplementedException();
    }

    public List<Guid> List()
    {
        throw new NotImplementedException();
    }

    public void Exec(Guid id, string command, RunMode mode, string? directory)
    {
        throw new NotImplementedException();
    }

    public void Stop(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Share(Guid id, string hostPath, string sandboxPath, bool readOnly = true)
    {
        throw new NotImplementedException();
    }

    public void Connect(Guid id)
    {
        throw new NotImplementedException();
    }

    public string Ip(Guid id)
    {
        throw new NotImplementedException();
    }
}