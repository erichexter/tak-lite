using System.Diagnostics;
using dpp.cot;
using Tak_lite.ViewModels;
using TheBentern.Tak.Client;
using Contact = dpp.cot.Contact;
using Point = dpp.cot.Point;

namespace Tak_lite.Service;

public class TakServiceInstance
{
    private TakClient _client;

    private readonly List<TakContact> _contacts = new();

    public Action<TakContact> Callback;
    private string uid;
    private TakContact _contact;

    public DateTime LastChanged { get; private set; }
    public bool IsConnected { get; set; }

    public TakServer ConfigurationServer { get; set; }

    public async void Connect()
    {

    }
    public async void Connect(string filepath)
    {
        uid = Guid.NewGuid().ToString();
        _client = new TakClient(filepath);
        try
        {
            IsConnected = true;
            await _client.ListenAsync(ReceivedCoTEvent);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            IsConnected = false;
        }
    }

    private Message LocationCot(TakContact contact)
    {
        return new Message
        {
            Event = new Event
            {
                Time = DateTime.UtcNow,
                Start = DateTime.UtcNow,
                Stale = DateTime.UtcNow.AddMinutes(5),
                Uid = uid,
                Version = "2.0",
                How = "m-g",
                Type = "a-f-G-E-V-C",
                Detail = new Detail
                {
                    Contact = new Contact { Callsign = contact.Callsign, Endpoint = "*:-1:stcp" },
                    Group = new Group { Name = contact.Team, Role = contact.Role },
                    Takv = new Takv
                        { Version = "1.0.0.0", Device = "Custom build", Os = "iOS", Platform = "WinTAK-CIV" },
                    Status = new Status { Battery = 100 },
                    PrecisionLocation = new PrecisionLocation { Geopointsrc = "GPS" }
                    //,Track = new Track(){ }
                },
                Point = new Point
                {
                    Lat =  contact.Point.Lat,
                    Lon = contact.Point.Lon
                }
            }
        };
    }

    private Task ReceivedCoTEvent(Event arg)
    {
        Debug.WriteLine("received tak cot message");
        if (arg == null)
            return Task.CompletedTask;

        Debug.WriteLine(arg.ToXmlString());

        var callsign = arg?.Detail?.Contact?.Callsign;

        if (_contacts.Exists(a => a.UUID == arg.Uid))
        {
            var takContact = _contacts.Single(a => a.UUID == arg.Uid);
            takContact.Point = arg?.Point;
            if (Callback != null)
                Callback(takContact);
        }
        else
        {
            var takContact = new TakContact
            {
                Callsign = callsign,
                Point = arg?.Point,
                Team = arg?.Detail?.Group?.Name,
                Role = arg?.Detail?.Group?.Role,
                UUID = arg?.Uid
            };

            _contacts.Add(takContact);

            if (Callback != null)
                Callback(takContact);
        }

        LastChanged = DateTime.UtcNow;;

        return Task.CompletedTask;
    }

    public void SendCot()
    {
        var msg = LocationCot(_contact);
        _client.SendAsync(msg);
    }
    public void UpdateLocation(Location location)
    {
        _contact.Point.Lat=location.Latitude;
        _contact.Point.Lon=location.Longitude;
        SendCot();
    }

    public void UpdateContact(TakContact contact)
    {
        _contact = contact;
        SendCot();
    }
}

public class TakContact
{
    public string Callsign { get; set; }
    public Point Point { get; set; }
    public string Role { get; set; }
    public string Team { get; set; }
    public string UUID { get; set; }
}


