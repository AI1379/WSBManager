using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using ReactiveUI;
using WSBManager.Models;

namespace WSBManager.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private int _clickCount;

    public ObservableCollection<string> Items { get; } = [];

    public string TextBoxContent { get; set; } = "Hello, World!";


    public string MainWindowText =>
        $"Welcome to WSBManager! Click count: {_clickCount}, TextBox content: {TextBoxContent}";

    public void IncrementClickCount()
    {
        _clickCount++;
        Items.Add(TextBoxContent);
        this.RaisePropertyChanged(nameof(MainWindowText));
    }

    public string TestXml()
    {
        var configuration = new Configuration();
        configuration.MappedFolders.Add(new MappedFolder
            { HostFolder = "C:\\HostFolder1", SandboxFolder = "C:\\SandboxFolder1", ReadOnly = false });
        configuration.LogonCommand = @"C:\SandboxFolder1\runme.exe";
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
        };
        var textWriter = new System.IO.StringWriter();
        using var xmlWriter = XmlWriter.Create(textWriter, settings);
        serializer.Serialize(xmlWriter, configuration);
        var str = textWriter.ToString();
        return str;
    }
}