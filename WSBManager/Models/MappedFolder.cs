namespace WSBManager.Models;

public class MappedFolder
{
    public string HostFolder { get; set; } = string.Empty;
    public string SandboxFolder { get; set; } = string.Empty;
    public bool ReadOnly { get; set; } = false;
}