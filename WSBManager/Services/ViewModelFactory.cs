using System;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace WSBManager.Services;

public interface IViewModelFactory
{
    T Create<T>() where T : ReactiveObject;
}

public class ViewModelFactory(IServiceProvider serviceProvider) : IViewModelFactory
{
    public T Create<T>() where T : ReactiveObject
    {
        return serviceProvider.GetRequiredService<T>();
    }
}