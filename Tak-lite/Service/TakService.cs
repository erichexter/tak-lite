using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using dpp.cot;
//using Javax.Security.Cert;
using TheBentern.Tak.Client;
using Point = dpp.cot.Point;

namespace Tak_lite.Service
{
    public class TakService
    {
        private string uid;
        private  TakClient _client;

        private List<TakContact> _contacts=new ();

        public Action<TakContact> Callback;

        //public async Task CopyFileToAppDataDirectory(string filename)
        //{
        //    // Open the source file
        //    using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(filename);

        //    // Create an output filename
        //    string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);

        //    // Copy the file to the AppDataDirectory
        //    using FileStream outputStream = File.Create(targetFile);
        //    await inputStream.CopyToAsync(outputStream);
        //}
        public async void Connect(string filepath)
        {
            uid=Guid.NewGuid().ToString();
            //await CopyFileToAppDataDirectory("atak.zip");

            _client = new TakClient( filepath);
            try
            {
                
                //await _client.ConnectAsync();
                //await _client.SendAsync(getEvent());
                await _client.ListenAsync(ReceivedCoTEvent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private Message LocationCot(Location location)
        {
            return new Message()
            {
                Event = new Event()
                {
                    Time = DateTime.UtcNow,
                    Start = DateTime.UtcNow,
                    Stale = DateTime.UtcNow.AddMinutes(5),
                    Uid = uid,
                    Version = "2.0",
                    How = "m-g",
                    Type = "a-f-G-E-V-C",
                    Detail = new Detail()
                    {
                        Contact = new dpp.cot.Contact() { Callsign = "foobar" ,Endpoint = "*:-1:stcp"}
                        ,Group = new Group(){Name = "Green",Role = "TM"}
                        ,Takv = new Takv(){Version = "1.0.0.0",Device = "Custom build",Os = "iOS",Platform = "WinTAK-CIV"}
                        ,Status = new Status(){Battery = 100}
                        ,PrecisionLocation = new PrecisionLocation(){Geopointsrc = "GPS"}
                        //,Track = new Track(){ }
                    },
                    Point = new dpp.cot.Point
                    {
                        Lat = location.Latitude,
                        Lon = location.Longitude,
                        
                    }
                }
            };
        }

        private Task ReceivedCoTEvent(Event arg)
        {
            Debug.WriteLine("received tak cot message");
            if(arg==null )
                return Task.CompletedTask;

            Debug.WriteLine(arg.ToXmlString());

            var callsign = arg?.Detail?.Contact?.Callsign;
            
            if (_contacts.Exists(a => a.UUID == arg.Uid))
            {
                var takContact = _contacts.Single(a => a.UUID==arg.Uid);
                takContact.Point = arg?.Point;
                if (Callback != null)
                    Callback(takContact);

            }
            else
            {
                var takContact = new TakContact()
                {
                    Callsign = callsign,
                    Point = arg?.Point,
                    Team=arg?.Detail?.Group?.Name,
                    Role=arg?.Detail?.Group?.Role,
                    UUID=arg?.Uid
                };

                _contacts.Add(takContact);
                
                if (Callback != null)
                    Callback(takContact);

            }
            LastChanged= DateTime.Now;

            return Task.CompletedTask;
        }

        public DateTime LastChanged { get; private set; }

        public void UpdateLocation(Location location)
        {
            var msg = LocationCot(location);
            Debug.WriteLine(msg.ToXmlString());
            _client.SendAsync(msg);
        }
    }

    public class TakContact
    {
        public string Callsign { get; set;}
        public Point Point { get; set;}
        public string Role { get; set; }
        public string Team { get; set; }
        public string UUID { get; set; }
    }
}
