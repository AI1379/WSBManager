using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WSBManager.ViewModels;

public class SandboxInstanceViewModel : ReactiveObject, IActivatableViewModel
{
    public SandboxConfigurationViewModel SandboxConfigurationViewModel { get; }
    [Reactive] public string Title { get; set; }

    public SandboxInstanceViewModel(
        SandboxConfigurationViewModel sandboxConfigurationViewModel,
        EditableItem<string> tabTitle)
    {
        SandboxConfigurationViewModel = sandboxConfigurationViewModel;
        var tabTitle1 = tabTitle;
        Title = tabTitle1.Value;

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.Title)
                .Skip(1)
                .Subscribe(title => tabTitle1.Value = title)
                .DisposeWith(disposables);

            tabTitle1.WhenAnyValue(x => x.Value)
                .Subscribe(title => Title = title)
                .DisposeWith(disposables);
        });

        Debug.WriteLine("SandboxInstanceViewModel initialized with EditableItem");
    }

    public ViewModelActivator Activator { get; } = new();
}