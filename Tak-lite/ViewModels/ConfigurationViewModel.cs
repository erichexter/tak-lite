using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tak_lite;

public partial class ConfigurationViewModel : ObservableObject
{
    public ConfigurationViewModel()
    {
    }

    [ObservableProperty] private string role;
    [ObservableProperty] private string callsign;
    [ObservableProperty] private string team;


    [RelayCommand]
    public void SaveCommand()
    {

    }

}