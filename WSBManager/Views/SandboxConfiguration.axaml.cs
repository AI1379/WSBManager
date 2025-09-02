using System.Diagnostics;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WSBManager.ViewModels;

namespace WSBManager.Views;

public partial class SandboxConfiguration : ReactiveUserControl<SandboxConfigurationViewModel>
{
    public SandboxConfiguration()
    {
        InitializeComponent();
        this.WhenActivated(disposables => { Debug.WriteLine("Sandbox configuration activated"); });
    }
}