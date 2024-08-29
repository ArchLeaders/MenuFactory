using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MenuFactory.Abstractions;
using MenuFactory.Sample.Models;
using System.Diagnostics;

namespace MenuFactory.Sample.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly DefaultMenu _defaultMenu = new();

    public ShellViewModel(InputElement visualRoot)
    {
        _menuFactory = new AvaloniaMenuFactory(visualRoot);
        _menuFactory.AddMenuGroup(_defaultMenu);
    }

    [ObservableProperty]
    private IMenuFactory _menuFactory;

    [ObservableProperty]
    private bool _showExtraMenuItems = false;

    [RelayCommand]
    private void AddMenuItem()
    {
        _defaultMenu.DynamicMenuItems.Add($"Item {_defaultMenu.DynamicMenuItems.Count + 1}");
    }

    partial void OnShowExtraMenuItemsChanged(bool oldValue, bool newValue)
    {
        Debug.WriteLine(newValue);

        if (newValue) {
            MenuFactory.AddMenuGroup<ExtrasMenu>();
        }
        else {
            MenuFactory.RemoveMenuGroup<ExtrasMenu>();
        }
    }
}
