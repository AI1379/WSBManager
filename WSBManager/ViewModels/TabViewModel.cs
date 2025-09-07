using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WSBManager.ViewModels;

public class TabViewModel : ReactiveObject
{
    [Reactive] public string Title { get; set; }
    [Reactive] public bool IsEnabled { get; set; } = false;
    public ReactiveCommand<Unit, Unit> ToggleCmd { get; set; }

    public TabViewModel(EditableItem<string> tabTitle, bool isEnabled = false)
    {
        IsEnabled = isEnabled;
        // The outputScheduler must be set to the main scheduler to avoid threading issues with UI updates
        ToggleCmd = ReactiveCommand.Create(Toggle, outputScheduler: AvaloniaScheduler.Instance);

        var tabTitle1 = tabTitle;
        Title = tabTitle1.Value;

        this.WhenAnyValue(x => x.Title)
            .Skip(1)
            .Subscribe(t => tabTitle1.Value = t);
        tabTitle1.WhenAnyValue(x => x.Value)
            .Subscribe(title => Title = title);
    }

    private void Toggle()
    {
        IsEnabled = !IsEnabled;
    }
}