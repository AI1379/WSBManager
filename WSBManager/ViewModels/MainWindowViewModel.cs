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
}