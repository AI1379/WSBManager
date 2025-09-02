using System.Diagnostics;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WSBManager.ViewModels;

public class SandboxInstanceViewModel : ReactiveObject
{
    public SandboxConfigurationViewModel SandboxConfigurationViewModel { get; }

    public SandboxInstanceViewModel(SandboxConfigurationViewModel sandboxConfigurationViewModel)
    {
        SandboxConfigurationViewModel = sandboxConfigurationViewModel;
        Debug.WriteLine("SandboxInstanceViewModel initialized");
    }
}