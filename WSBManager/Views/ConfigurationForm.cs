using System;
using System.Collections;
using Avalonia.Controls;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using ReactiveUI;
using WSBManager.Models;

namespace WSBManager.Views;

public class ConfigurationForm : UserControl, IActivatableView
{
    public static readonly StyledProperty<Configuration> ConfigurationProperty =
        AvaloniaProperty.Register<ConfigurationForm, Configuration>(
            nameof(Configuration),
            defaultBindingMode: BindingMode.TwoWay
        );

    private readonly Grid _grid = new Grid()
    {
        ShowGridLines = true,
    };

    public Configuration Configuration
    {
        get => GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public ConfigurationForm()
    {
        Content = _grid;

        _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        this.WhenActivated(disposables =>
        {
            Debug.WriteLine("ConfigurationForm activated");

            this.GetObservable(ConfigurationProperty).Subscribe(_ =>
            {
                _grid.Children.Clear();
                ReloadConfiguration();
            }).DisposeWith(disposables);
        });
    }

    private void ReloadConfiguration()
    {
        var currentRow = 0;
        foreach (var prop in Configuration.GetType().GetProperties())
        {
            var label = new TextBlock
            {
                Text = prop.Name + ": ",
                VerticalAlignment = VerticalAlignment.Center
            };

            Control inputControl = prop.PropertyType switch
            {
                { } t when t == typeof(string) => CreateTextBox(prop.Name, prop.GetValue(Configuration) as string),
                { } t when t == typeof(int) => CreateNumericUpDown(prop.Name, (int)(prop.GetValue(Configuration) ?? 0)),
                { } t when t.IsEnum => CreateEnumComboBox(prop.Name, t, prop.GetValue(Configuration) ?? null),
                { } t when t != typeof(string) && typeof(IList).IsAssignableFrom(t) =>
                    CreateDataGrid(prop, prop.GetValue(Configuration) as IList, t),
                _ => new TextBlock { Text = $"Unsupported type: {prop.PropertyType.Name}" }
            };

            label.Margin = new Thickness(5);
            inputControl.Margin = new Thickness(5);
            Grid.SetRow(label, currentRow);
            Grid.SetRow(inputControl, currentRow);
            Grid.SetColumn(label, 0);
            Grid.SetColumn(inputControl, 1);
            _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _grid.Children.Add(label);
            _grid.Children.Add(inputControl);

            currentRow++;
        }
    }

    private TextBox CreateTextBox(string propName, string? text = null, string? watermark = null)
    {
        var textBox = new TextBox
        {
            Text = text ?? string.Empty,
            Watermark = watermark ?? $"Enter {propName}...",
        };

        textBox.Bind(
            TextBox.TextProperty,
            new Binding(propName)
            {
                Source = Configuration,
                Mode = BindingMode.TwoWay
            }
        );
        return textBox;
    }

    private NumericUpDown CreateNumericUpDown(string propName, int currentValue, int step = 128)
    {
        var numericUpDown = new NumericUpDown
        {
            Minimum = 1,
            Maximum = int.MaxValue,
            Value = currentValue,
            Increment = step,
        };

        numericUpDown.Bind(
            NumericUpDown.ValueProperty,
            new Binding(propName)
            {
                Source = Configuration,
                Mode = BindingMode.TwoWay
            }
        );

        return numericUpDown;
    }


    private ComboBox CreateEnumComboBox(string propName, Type enumType, object? currentValue)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Invalid enum type or value.");
        var comboBox = new ComboBox();
        Enum.GetValues(enumType).Cast<object>().ToList()
            .ForEach(enumValue => comboBox.Items.Add(enumValue));
        if (currentValue != null)
            comboBox.SelectedItem = currentValue;

        comboBox.Bind(
            SelectingItemsControl.SelectedItemProperty,
            new Binding(propName)
            {
                Source = Configuration,
                Mode = BindingMode.TwoWay
            }
        );
        return comboBox;
    }

    private StackPanel CreateDataGrid(PropertyInfo prop, IList? list, Type listType)
    {
        var propName = prop.Name;
        ArgumentNullException.ThrowIfNull(list);

        var panel = new StackPanel();
        var itemsSource = list.Cast<object>().ToList();
        var dataGrid = new DataGrid
        {
            AutoGenerateColumns = true,
            ItemsSource = itemsSource,
        };

        dataGrid.Bind(
            DataGrid.ItemsSourceProperty,
            new Binding(propName)
            {
                Source = Configuration,
                Mode = BindingMode.TwoWay
            }
        );

        var addButton = new Button { Content = "Add", Margin = new Thickness(5) };
        addButton.Click += (_, _) =>
        {
            Debug.WriteLine($"Adding {propName}");
            var elementType = listType.GenericTypeArguments[0];
            var newItem = Activator.CreateInstance(elementType) ?? throw new NullReferenceException();
            var configListRef = prop.GetValue(Configuration) as IList ?? throw new NullReferenceException();
            configListRef.Add(newItem);
            dataGrid.ItemsSource = configListRef.Cast<object>().ToList();
        };

        var removeButton = new Button { Content = "Remove", Margin = new Thickness(5) };
        removeButton.Click += (_, _) =>
        {
            if (dataGrid.SelectedItem == null) return;
            Debug.WriteLine($"Removing selected item from {propName}");
            var configListRef = prop.GetValue(Configuration) as IList ?? throw new NullReferenceException();
            configListRef.Remove(dataGrid.SelectedItem);
            dataGrid.ItemsSource = configListRef.Cast<object>().ToList();
        };

        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };
        buttonPanel.Children.Add(addButton);
        buttonPanel.Children.Add(removeButton);

        panel.Children.Add(dataGrid);
        panel.Children.Add(buttonPanel);

        return panel;
    }
}