using System;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace WSBManager.Services;

public interface IViewModelFactory
{
    T Create<T>() where T : ReactiveObject;
    T Create<T>(object arg1) where T : ReactiveObject;
}

public class ViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    public T Create<T>() where T : ReactiveObject
    {
        return serviceProvider.GetRequiredService<T>();
    }

    public T Create<T>(object arg1) where T : ReactiveObject
    {
        return arg1 == null
            ? throw new ArgumentNullException(nameof(arg1))
            : ActivatorUtilities.CreateInstance<T>(serviceProvider, arg1);
    }
}