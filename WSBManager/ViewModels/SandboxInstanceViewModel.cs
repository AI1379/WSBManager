using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WSBManager.ViewModels;

public class SandboxInstanceViewModel : ReactiveObject
{
    [Reactive] public string DisplayName { get; set; } = "New Sandbox";
}