using Avalonia.ReactiveUI;
using WSBManager.ViewModels;

namespace WSBManager.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}