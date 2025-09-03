using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WSBManager.Services;

namespace WSBManager.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public ObservableCollection<TabViewModel> Tabs { get; } = new();

    public ReactiveCommand<Unit, Unit> AddTabCmd { get; }
    public ReactiveCommand<TabViewModel, Unit> RemoveTabCmd { get; }
    [Reactive] public int SelectedTabIndex { get; set; } = -1;

    private readonly ObservableCollection<SandboxInstanceViewModel> _sandboxInstances = [];
    [Reactive] public SandboxInstanceViewModel? CurrentInstance { get; set; }

    private readonly IViewModelFactory _viewModelFactory;

    public MainWindowViewModel(IViewModelFactory viewModelFactory)
    {
        _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));

        AddTabCmd = ReactiveCommand.Create(AddNewTab);
        RemoveTabCmd = ReactiveCommand.Create<TabViewModel>(RemoveTab);

        AddNewTab();
        SwitchToTab(0);

        this.WhenAnyValue(x => x.SelectedTabIndex)
            .Where(x => x != -1 && x < Tabs.Count)
            .Subscribe(SwitchToTab);

        Debug.WriteLine("MainWindowViewModel initialized");
    }

    private void SwitchToTab(int index)
    {
        CurrentInstance = _sandboxInstances[index];
        Debug.WriteLine($"Switched to tab index: {index}, Name: {Tabs[index].Title}");
    }

    private void AddNewTab()
    {
        var newTabName = $"Tab {Tabs.Count + 1}";
        Tabs.Add(new TabViewModel(newTabName));
        _sandboxInstances.Add(_viewModelFactory.Create<SandboxInstanceViewModel>());
        SelectedTabIndex = Tabs.Count - 1;
        Debug.WriteLine($"Added new tab: {newTabName}");
    }

    private void RemoveTab(TabViewModel tab)
    {
        var index = Tabs.IndexOf(tab);
        if (index < 0 || index >= Tabs.Count || Tabs.Count == 1) return;

        _sandboxInstances.RemoveAt(index);
        Tabs.RemoveAt(index);

        // Extra check to ensure the new tab is right
        SwitchToTab(SelectedTabIndex);

        Debug.WriteLine($"Removed tab at index: {index}");
        Debug.WriteLine($"Current selected index: {SelectedTabIndex}");
    }
}