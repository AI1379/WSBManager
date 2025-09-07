using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WSBManager.Models;
using WSBManager.Services;

namespace WSBManager.ViewModels;

public class SandboxConfigurationViewModel : ReactiveObject
{
    [Reactive] public string ConfigurationFilePath { get; set; } = string.Empty;
    [Reactive] public Configuration Configuration { get; set; } = new Configuration();

    public ReactiveCommand<Unit, Unit> LoadConfigurationFromFile { get; }
    public ReactiveCommand<Unit, Unit> SaveConfigurationToFile { get; }
    public ReactiveCommand<Unit, Unit> SelectConfigurationFile { get; }

    private readonly IFileService _fileService;

    public SandboxConfigurationViewModel(IFileService fileService)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        LoadConfigurationFromFile = ReactiveCommand.CreateFromTask(LoadConfiguration);
        SaveConfigurationToFile = ReactiveCommand.CreateFromTask(SaveConfiguration);
        SelectConfigurationFile = ReactiveCommand.CreateFromTask(SelectConfiguration);
    }

    private async Task LoadConfiguration()
    {
        if (ConfigurationFilePath == string.Empty)
        {
            Console.WriteLine("Configuration file path not set");
            await SelectConfiguration();

            // We already called LoadConfiguration in SelectConfiguration, so just return here.
            return;
        }

        Console.WriteLine("Loading configuration from file: " + ConfigurationFilePath);
        ConfigurationFilePath = ConfigurationFilePath.Trim(' ', '\"', '\'');
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
        try
        {
            using var reader = new System.IO.StreamReader(ConfigurationFilePath);
            Configuration = serializer.Deserialize(reader) as Configuration ?? new Configuration();
            Debug.WriteLine("Configuration loaded successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
        }
    }

    private async Task SaveConfiguration()
    {
        if (ConfigurationFilePath == string.Empty)
        {
            Console.WriteLine("Configuration file path not set");
            await SaveConfigurationAs();

            // We do not call SaveConfiguration in SaveConfigurationAs, so we continue to save after setting the path.
        }

        if (ConfigurationFilePath == string.Empty)
        {
            Console.WriteLine("Configuration file path not set");
            return;
        }

        Console.WriteLine("Saving configuration to file: " + ConfigurationFilePath);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));

        // Use 'await using' to ensure the stream is properly disposed of asynchronously.
        await using var writer = new System.IO.StreamWriter(ConfigurationFilePath);
        serializer.Serialize(writer, Configuration);
    }

    private async Task SelectConfiguration()
    {
        var result = await _fileService.OpenFileAsync("Select Configuration File",
            "WSB Files",
            ["*.wsb", "*.xml"]);

        Console.WriteLine($"Selected Configuration File: {result}");

        if (result == null)
        {
            Console.WriteLine("File selection was cancelled or failed.");
            return;
        }

        ConfigurationFilePath = result;

        await LoadConfiguration();
    }

    private async Task SaveConfigurationAs()
    {
        var result = await _fileService.SaveFileAsync(
            "configuration.wsb",
            "Save Configuration File",
            "*.wsb");

        Console.WriteLine($"Selected Save File: {result}");

        if (result == null)
        {
            Console.WriteLine("File save was cancelled or failed.");
            return;
        }

        ConfigurationFilePath = result;
    }
}