namespace Tak_lite.Service;

public class TakService
{
    private readonly DataService _dataService;
    private TakContact _contact;
    public List<TakServiceInstance> Servers { get; private set; }=new();

    public TakService(DataService dataService)
    {
        _dataService = dataService;
        
    }

    public bool IsConnected()
    {
        return Servers.Any(a => a.IsConnected);
    }

    public void LoadServer()
    {
        Servers.Clear();
        Servers.AddRange(_dataService.GetAppSettings().Servers.Select(a=>new TakServiceInstance(){ConfigurationServer = a,Callback = Callback,DisconnectCallback = DisconnectCallback}));
    }


    public Action<string> DisconnectCallback;
    public Action<TakContact> Callback;
    

    public void Connect()
    {
        
        foreach (var server in Servers)
        {
            if (!server.IsConnected && server.ConfigurationServer.Enabled)
            {
                server.Connect();
            }
        }
    }

    public void UpdateLocation(Location location)
    {
        foreach (var server in Servers)
        {
            if (server.IsConnected && server.ConfigurationServer.Enabled)
            {
                server.UpdateLocation(location);
            }
        }
    }
    public void SendCot()
    {
        foreach (var server in Servers)
        {
            if (server.IsConnected && server.ConfigurationServer.Enabled)
            {
                server.SendCot();
            }
        }
    }

    public void UpdateContact(TakContact contact)
    {
        _contact = contact;
        foreach (var server in Servers)
        {
            if (server.IsConnected && server.ConfigurationServer.Enabled)
            {
                server.UpdateContact(contact);
            }
        }
    }
}