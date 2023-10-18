using Camera.MAUI;
using Eco.FrameworkImpl.Ocl;
using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigTakServerListPage : ContentPage
{
    private readonly ConfigTakServerListViewModel _vm;

    public ConfigTakServerListPage(ConfigTakServerListViewModel vm)
	{
		InitializeComponent();
		BindingContext=vm;
        _vm = vm;
    }

    private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
    {
        await _vm.Edit(((TakServer)e.Item).Id);
    }

    private void CameraView_OnCamerasLoaded(object sender, EventArgs e)
    {
        if (cameraView.NumCamerasDetected > 0)
        {
            if (cameraView.NumMicrophonesDetected > 0)
                cameraView.Microphone = cameraView.Microphones.First();
            cameraView.Camera = cameraView.Cameras.First();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (await cameraView.StartCameraAsync() == CameraResult.Success)
                {
                }
                
            });
        }
    }
}