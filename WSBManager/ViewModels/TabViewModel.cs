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

    public TabViewModel(string title, bool isEnabled = false)
    {
        Title = title;
        IsEnabled = isEnabled;
        // The outputScheduler must be set to the main scheduler to avoid threading issues with UI updates
        ToggleCmd = ReactiveCommand.Create(Toggle, outputScheduler: AvaloniaScheduler.Instance);
    }

    private void Toggle()
    {
        IsEnabled = !IsEnabled;
    }
}