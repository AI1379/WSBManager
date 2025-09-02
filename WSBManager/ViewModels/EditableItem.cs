using ReactiveUI;

namespace WSBManager.ViewModels;

public class EditableItem<T>(T value) : ReactiveObject
{
    private T _value = value;

    public T Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}