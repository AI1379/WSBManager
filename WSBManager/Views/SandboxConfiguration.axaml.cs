using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using WSBManager.ViewModels;

namespace WSBManager.Views;

public partial class SandboxConfiguration : ReactiveUserControl<SandboxConfigurationViewModel>
{
    public SandboxConfiguration()
    {
        InitializeComponent();
        DataContext = ViewModel = new SandboxConfigurationViewModel();
        this.WhenActivated(disposables => { Debug.WriteLine("Sandbox configuration activated"); });
    }
}