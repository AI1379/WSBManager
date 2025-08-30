using System.Collections.Generic;

namespace WSBManager.Models;

public class Configuration
{
    // Resharper disable InconsistentNaming
    public PropertyStatus vGPU { get; set; } = PropertyStatus.Enable;
    public PropertyStatus Networking { get; set; } = PropertyStatus.Enable;
    public PropertyStatus AudioInput { get; set; } = PropertyStatus.Enable;
    public PropertyStatus VideoInput { get; set; } = PropertyStatus.Enable;
    public PropertyStatus ProtectedClient { get; set; } = PropertyStatus.Enable;
    public PropertyStatus PrinterRedirection { get; set; } = PropertyStatus.Enable;
    public PropertyStatus ClipboardRedirection { get; set; } = PropertyStatus.Enable;
    public List<MappedFolder> MappedFolders { get; set; } = new List<MappedFolder>();
    public int MemoryInMB { get; set; } = 2048;
    public string? LogonCommand { get; set; } = @"C:\path\to\your\application.exe";

    // Resharper restore InconsistentNaming
}