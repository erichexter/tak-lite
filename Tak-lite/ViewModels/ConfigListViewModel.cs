using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tak_lite.ViewModels;

public partial class ConfigListViewModel : ObservableObject
{

    [RelayCommand]
    public async Task Preferences()
    {
        await Shell.Current.GoToAsync(nameof(ConfigPreferencesPage));
    }

    [RelayCommand]
    public async Task Servers()
    {
        await Shell.Current.GoToAsync(nameof(ConfigTakServerListPage));
    }
}