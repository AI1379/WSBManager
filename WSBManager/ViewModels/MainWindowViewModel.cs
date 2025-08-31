using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using ReactiveUI;
using WSBManager.Models;

namespace WSBManager.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public SandboxInstanceViewModel SandboxInstanceViewModel { get; }

    public MainWindowViewModel(SandboxInstanceViewModel sandboxInstanceViewModel)
    {
        SandboxInstanceViewModel = sandboxInstanceViewModel;
        Debug.WriteLine("MainWindowViewModel initialized");
    }
}