using System.Diagnostics;
using System.Text.Json.Serialization;
using ABI.Windows.Devices.PointOfService.Provider;
using dpp.cot;
using TheBentern.Tak.Client;

namespace Tak_lite;

public partial class MainPage : ContentPage
{
    private Location location;
    private TakClient client;

	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

    
	private async void OnCounterClicked(object sender, EventArgs e)
    {
        
        var ls = new LocationService();
        location = await ls.GetCurrentLocation();

        client = new TakClient("c:\\atak.zip");
        //await client.ConnectAsync();
        //await client.SendAsync(getEvent());

        
        await client.ListenAsync(ReceivedCoTEvent);
    }

    private Message getEvent()
    {
        return new Message()
        {
            Event = new Event()
            {
                Detail = new Detail()
                {
                    Contact = new dpp.cot.Contact(){Callsign = "foobar"}

                },
                Point = new dpp.cot.Point
                {
                    Lat = location.Latitude,Lon = location.Longitude
                }
            }
        };
    }

    private Task ReceivedCoTEvent(Event arg)
    {
        
        Debug.WriteLine(arg?.Detail?.Contact?.Callsign + $" {arg.Point.Lat} {arg.Point.Lon}");
        return Task.CompletedTask;
    }
}

