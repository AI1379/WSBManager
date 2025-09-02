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
using WSBManager.Services;

namespace WSBManager.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private readonly SourceList<EditableItem<string>> _tabsSource = new();
    private readonly ReadOnlyObservableCollection<EditableItem<string>> _tabs;
    public ReadOnlyObservableCollection<EditableItem<string>> Tabs => _tabs;

    public ReactiveCommand<Unit, Unit> AddTabCmd { get; }
    public ReactiveCommand<EditableItem<string>, Unit> RemoveTabCmd { get; }
    [Reactive] public int SelectedTabIndex { get; set; } = -1;

    private readonly ObservableCollection<SandboxInstanceViewModel> _sandboxInstances = [];
    [Reactive] public SandboxInstanceViewModel? SandboxInstanceViewModel { get; set; }

    private readonly IViewModelFactory _viewModelFactory;

    public MainWindowViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));

        _tabsSource.Connect()
            .Bind(out _tabs)
            .Subscribe();

        AddTabCmd = ReactiveCommand.Create(AddNewTab);
        RemoveTabCmd = ReactiveCommand.Create<EditableItem<string>>(RemoveTab);

        AddNewTab();
        SwitchToTab(0);

        this.WhenAnyValue(x => x.SelectedTabIndex)
            .Where(x => x != -1 && x < _tabs.Count)
            .Subscribe(SwitchToTab);

        Debug.WriteLine("MainWindowViewModel initialized");
    }

    private void SwitchToTab(int index)
    {
        SandboxInstanceViewModel = _sandboxInstances[index];
        Debug.WriteLine($"Switched to tab index: {index}, Name: {_tabs[index].Value}");
    }

    private void AddNewTab()
    {
        var newTabName = $"Tab {_tabs.Count + 1}";
        _tabsSource.Add(new EditableItem<string>(newTabName));
        _sandboxInstances.Add(_viewModelFactory.Create<SandboxInstanceViewModel>());
        SelectedTabIndex = _tabs.Count - 1;
        Debug.WriteLine($"Added new tab: {newTabName}");
    }

    private void RemoveTab(EditableItem<string> tab)
    {
        var index = _tabs.IndexOf(tab);
        if (index < 0 || index >= _tabs.Count || _tabs.Count == 1) return;

        _sandboxInstances.RemoveAt(index);
        _tabsSource.RemoveAt(index);

        Debug.WriteLine($"Removed tab at index: {index}");
        Debug.WriteLine($"Current selected index: {SelectedTabIndex}");
    }
}