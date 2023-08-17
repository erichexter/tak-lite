using Eco.FrameworkImpl.Ocl;
using Tak_lite.ViewModels;

namespace Tak_lite;
[QueryProperty(nameof(SetId),"id")]
public partial class ConfigTakServerDetailPage : ContentPage
{
    private readonly ConfigTakServerDetailViewModel _vm;

    public ConfigTakServerDetailPage(ConfigTakServerDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    public string SetId  
    {  
        set
        {
            _vm.LoadFromId(value);
        }  
    }  
}