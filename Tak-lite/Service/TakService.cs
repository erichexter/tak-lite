namespace Tak_lite.Service;

public class TakService
{
    private TakContact _contact;
    public List<TakServiceInstance> Servers { get; private set; }=new();

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