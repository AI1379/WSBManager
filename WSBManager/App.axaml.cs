using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using WSBManager.Services;
using WSBManager.ViewModels;

namespace WSBManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Views.MainWindow();
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow) ??
                           throw new InvalidOperationException("Failed to get TopLevel from MainWindow.");

            var services = new ServiceCollection();

            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<SandboxInstanceViewModel>();
            services.AddTransient<SandboxConfigurationViewModel>();

            services.AddSingleton<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<IFileService>(_ => new FileService(topLevel));

            // Currently the SandboxExecutor is not implemented yet, so we use the mock service.
            // services.AddSingleton<ISandboxExecutor, MockSandboxExecutor>();
            services.AddSingleton<ISandboxExecutor, SandboxExecutor>();

            var serviceProvider = services.BuildServiceProvider();

            desktop.MainWindow.DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>();
        }

        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

        base.OnFrameworkInitializationCompleted();
    }
}