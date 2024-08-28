using CommunityToolkit.Mvvm.ComponentModel;
using MenuFactory.Abstractions;
using MenuFactory.Sample.Models;
using System.Diagnostics;

namespace MenuFactory.Sample.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    public ShellViewModel()
    {
        MenuFactory.AddMenuGroup<DefaultMenu>();
    }

    [ObservableProperty]
    private IMenuFactory _menuFactory = new AvaloniaMenuFactory();

    [ObservableProperty]
    private bool _showExtraMenuItems = false;

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
