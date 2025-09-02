using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Xml;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WSBManager.Models;

namespace WSBManager.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly SourceList<EditableItem<string>> _tabsSource = new();
    private readonly ReadOnlyObservableCollection<EditableItem<string>> _tabs;
    public ReadOnlyObservableCollection<EditableItem<string>> Tabs => _tabs;

    public ReactiveCommand<Unit, Unit> AddTab { get; }
    [Reactive] public int SelectedTabIndex { get; set; } = -1;

    public SandboxInstanceViewModel SandboxInstanceViewModel { get; }

    public MainWindowViewModel(SandboxInstanceViewModel sandboxInstanceViewModel)
    {
        SandboxInstanceViewModel = sandboxInstanceViewModel;

        _tabsSource.Connect()
            .Bind(out _tabs)
            .Subscribe();

        AddTab = ReactiveCommand.Create(AddNewTab);

        Debug.WriteLine("MainWindowViewModel initialized");
    }

    private void AddNewTab()
    {
        var newTabName = $"Tab {_tabs.Count + 1}";
        _tabsSource.Add(new EditableItem<string>(newTabName));
        Debug.WriteLine($"Current tabs: {string.Join(", ", _tabs.Select(t => t.Value))}");
        Debug.WriteLine($"Selected tab index before adding: {SelectedTabIndex}");
        Debug.WriteLine($"Added new tab: {newTabName}");
    }
}