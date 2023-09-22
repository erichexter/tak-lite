
using Camera.MAUI;
using Camera.MAUI.ZXingHelper;
using Tak_lite.ViewModels;

namespace Tak_lite;

public partial class ConfigAresPage : ContentPage
{
	public ConfigAresPage(ConfigAresDetailViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
        vm.Load();
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