using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WSBManager.Services;

namespace WSBManager.ViewModels;

public class SandboxInstanceViewModel : ReactiveObject, IActivatableViewModel
{
    public enum SandboxState
    {
        Stopped,
        Starting,
        Running,
        Stopping,
        Error
    }

    public ViewModelActivator Activator { get; } = new();
    public SandboxConfigurationViewModel SandboxConfigurationViewModel { get; }
    [Reactive] public string Title { get; set; }

    private readonly ISandboxExecutor _sandboxExecutor;

    [Reactive] private Guid InstanceId { get; set; } = Guid.Empty;
    [Reactive] private string SandboxIp { get; set; } = string.Empty;
    [Reactive] public SandboxState State { get; set; } = SandboxState.Stopped;

    private const string NullFlag = "N/A";
    [Reactive] public string InstanceIdText { get; set; } = "Instance UUID: N/A";
    [Reactive] public string SandboxIpText { get; set; } = "Sandbox IP: N/A";

    public ReactiveCommand<Unit, Unit> Start => ReactiveCommand.CreateFromTask(StartSandbox);
    public ReactiveCommand<Unit, Unit> Stop => ReactiveCommand.CreateFromTask(StopSandbox);
    public ReactiveCommand<Unit, Unit> Connect => ReactiveCommand.CreateFromTask(ConnectSandbox);

    public SandboxInstanceViewModel(
        SandboxConfigurationViewModel sandboxConfigurationViewModel,
        ISandboxExecutor sandboxExecutor,
        EditableItem<string> tabTitle)
    {
        SandboxConfigurationViewModel = sandboxConfigurationViewModel;
        _sandboxExecutor = sandboxExecutor ?? throw new ArgumentNullException(nameof(sandboxExecutor));
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

            this.WhenAnyValue(x => x.InstanceId)
                .Subscribe(_ =>
                    InstanceIdText = $"Instance UUID: {(InstanceId == Guid.Empty ? NullFlag : InstanceId)}")
                .DisposeWith(disposables);

            this.WhenAnyValue(x => x.SandboxIp)
                .Subscribe(_ => SandboxIpText = $"Sandbox IP: {(SandboxIp == string.Empty ? NullFlag : SandboxIp)}")
                .DisposeWith(disposables);
        });

        Debug.WriteLine("SandboxInstanceViewModel initialized with EditableItem");
    }

    private async Task StartSandbox()
    {
        if (State == SandboxState.Running)
        {
            Console.WriteLine("Sandbox is already running.");
            return;
        }

        try
        {
            State = SandboxState.Starting;
            InstanceId = await _sandboxExecutor.Start(SandboxConfigurationViewModel.Configuration);
            SandboxIp = await _sandboxExecutor.Ip(InstanceId);
            State = SandboxState.Running;
            Console.WriteLine($"Sandbox started with ID: {InstanceId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting sandbox: {ex.Message}");
        }
    }

    private async Task StopSandbox()
    {
        if (State == SandboxState.Stopped)
        {
            Console.WriteLine("Sandbox is not running.");
            return;
        }

        try
        {
            State = SandboxState.Stopping;
            await _sandboxExecutor.Stop(InstanceId);
            InstanceId = Guid.Empty;
            SandboxIp = string.Empty;
            State = SandboxState.Stopped;
            Console.WriteLine($"Sandbox with ID: {InstanceId} stopped.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping sandbox: {ex.Message}");
        }
    }

    private async Task ConnectSandbox()
    {
        if (State == SandboxState.Stopped)
        {
            Console.WriteLine("Sandbox is not running. Cannot connect.");
            return;
        }

        try
        {
            await _sandboxExecutor.Connect(InstanceId);
            Console.WriteLine($"Connected to sandbox with ID: {InstanceId}, IP: {SandboxIp}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to sandbox: {ex.Message}");
        }
    }
}