using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using WSBManager.ViewModels;

namespace WSBManager.Views;

public partial class SandboxInstance : ReactiveUserControl<SandboxInstanceViewModel>
{
    public SandboxInstance()
    {
        InitializeComponent();
    }
}