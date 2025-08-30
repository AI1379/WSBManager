using System;
using Avalonia.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WSBManager.Models;

namespace WSBManager.Views;

public class ConfigurationForm : UserControl, IActivatableView
{
    public static readonly StyledProperty<Configuration> ConfigurationProperty =
        AvaloniaProperty.Register<ConfigurationForm, Configuration>(
            nameof(Configuration),
            defaultBindingMode: BindingMode.TwoWay
        );

    private readonly StackPanel _stackPanel = new StackPanel();

    public Configuration Configuration
    {
        get => GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public ConfigurationForm()
    {
        Content = _stackPanel;

        this.WhenActivated(disposables =>
        {
            Debug.WriteLine("ConfigurationForm activated");

            this.GetObservable(ConfigurationProperty).Subscribe(_ =>
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Configuration));
                var writer = new System.IO.StringWriter();
                serializer.Serialize(writer, Configuration);
                var xml = writer.ToString();
                Debug.WriteLine($"Configuration changed to:\n{xml}");
                _stackPanel.Children.Clear();
                ReloadConfiguration();
            }).DisposeWith(disposables);
        });
    }

    private void ReloadConfiguration()
    {
        foreach (var prop in Configuration.GetType().GetProperties())
        {
            var panel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Margin = new Avalonia.Thickness(0, 5),
            };

            var label = new TextBlock { Text = prop.Name + ": ", };

            Control inputControl = prop.PropertyType switch
            {
                { } t when t == typeof(string) => new TextBox
                    { Text = prop.GetValue(Configuration) as string ?? string.Empty, Width = 300 },
                { } t when t == typeof(int) => CreateNumericUpDown((int)(prop.GetValue(Configuration) ?? 0)),
                { } t when t.IsEnum => CreateEnumComboBox(t, prop.GetValue(Configuration) ?? null),
                { } t when t == typeof(List<MappedFolder>) => CreateMappedFolderListBox(),
                _ => new TextBlock { Text = $"Unsupported type: {prop.PropertyType.Name}" }
            };

            Debug.WriteLine(
                $"Binding {prop.Name} of type {prop.PropertyType.Name} using control {inputControl.GetType().Name}");

            inputControl.Bind(
                GetBindingProperty(inputControl),
                new Binding(prop.Name)
                {
                    Source = Configuration,
                    Mode = BindingMode.TwoWay
                }
            );

            if (inputControl is ComboBox comboBox)
            {
                Debug.WriteLine($"ComboBox selected item before binding: {comboBox.SelectedItem}");
            }

            panel.Children.Add(label);
            panel.Children.Add(inputControl);
            _stackPanel.Children.Add(panel);
        }
    }

    private static NumericUpDown CreateNumericUpDown(int currentValue, int step = 128)
    {
        var numericUpDown = new NumericUpDown
        {
            Minimum = 1,
            Maximum = int.MaxValue,
            Value = currentValue,
            Increment = step,
        };
        return numericUpDown;
    }


    private ComboBox CreateEnumComboBox(Type enumType, object? currentValue)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Invalid enum type or value.");
        var comboBox = new ComboBox();
        Enum.GetValues(enumType).Cast<object>().ToList()
            .ForEach(enumValue => comboBox.Items.Add(enumValue));
        if (currentValue != null)
            comboBox.SelectedItem = currentValue;
        return comboBox;
    }

    // TODO: Use DataGrid or similar for better UX
    private static ListBox CreateMappedFolderListBox()
    {
        // TODO: Implement adding/removing items
        var listBox = new ListBox();
        listBox.Height = 100;
        listBox.SelectionMode = SelectionMode.Multiple;
        return listBox;
    }

    private static AvaloniaProperty GetBindingProperty(Control control) => control switch
    {
        TextBox => TextBox.TextProperty,
        NumericUpDown => NumericUpDown.ValueProperty,
        ComboBox => SelectingItemsControl.SelectedItemProperty,
        ListBox => ListBox.SelectedItemsProperty,
        _ => throw new NotSupportedException($"Control type {control.GetType().Name} is not supported for binding.")
    };
}