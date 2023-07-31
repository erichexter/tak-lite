using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigTakServerDetailPage : ContentPage
{
    public ConfigTakServerDetailPage(ConfigTakServerDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}