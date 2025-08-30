using System;
using System.Diagnostics;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WSBManager.Models;

namespace WSBManager.ViewModels;

public class SandboxConfigurationViewModel : ReactiveObject
{
    [Reactive] public string ConfigurationFilePath { get; set; } = @"C:\path\to\config.wsb";
    [Reactive] public Configuration Configuration { get; set; } = new Configuration();

    public ReactiveCommand<Unit, Unit> LoadConfigurationFromFile { get; }
    public ReactiveCommand<Unit, Unit> SaveConfigurationToFile { get; }

    public SandboxConfigurationViewModel()
    {
        LoadConfigurationFromFile = ReactiveCommand.Create(LoadConfiguration);
        SaveConfigurationToFile = ReactiveCommand.Create(SaveConfiguration);
    }

    private void LoadConfiguration()
    {
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

    private void SaveConfiguration()
    {
        Console.WriteLine("Saving configuration to file: " + ConfigurationFilePath);
        // TODO: Implement saving configuration to file
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
        var writer = new System.IO.StringWriter();
        serializer.Serialize(writer, Configuration);
        var xml = writer.ToString();
        Debug.WriteLine($"Current Configuration: {xml}");
    }
}