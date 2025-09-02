using System.Diagnostics;
using ReactiveUI;

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