using System.Reactive;
using System.Reactive.Linq;
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
        ToggleCmd = ReactiveCommand.Create(Toggle, outputScheduler: RxApp.MainThreadScheduler);
    }

    private void Toggle()
    {
        Dispatcher.UIThread.Post(() => IsEnabled = !IsEnabled);
    }
}