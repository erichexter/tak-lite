using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TakClientTests;

public class UnitTest1
{
    [Fact]
    public void loadcert()
    {
        var certstring =
            "MIIEUzCCAzugAwIBAgIEN/PPMDANBgkqhkiG9w0BAQsFADB4MQswCQYDVQQGEwJY\nWDEOMAwGA1UECBMFc3RhdGUxDTALBgNVBAcTBGNpdHkxDDAKBgNVBAoTA1RBSzEM\nMAoGA1UECxMDVEFLMS4wLAYDVQQDEyV1ZnMudGFrLXNlcnZlci5jb20tSW50ZXJt\nZWRpYXJ5LUNBLTAxMB4XDTIzMTAxMjA2MDUxNloXDTIzMTExMTE4MDUxNlowKzEM\nMAoGA1UEChMDVEFLMQwwCgYDVQQLEwNUQUsxDTALBgNVBAMTBGl0YWswggIiMA0G\nCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQCc/RkO72ketg00mayLgJH86MyM5IAp\nuxb2TrvKS0J76RuCbZ6S4ALi9Zb1GBOLuAA5FgB40DT5BfshObpBPbgJ9USWE6/V\nalAtFyy/CXcxpmVrQWxCeZGrTo/zpPEN1doAGTZ3DB8C05ndui4aHo3ox3QRRkKd\nDlH7kspuK+3laYR3Wz0oZQoSp6UvjsCNoiBAwhvrHI9m+ehCDy36d5AML3JEYyjg\nuD6NUeb0PRlYjZo/oLZwaSn9I1iCeZwXO/l9PBb6XUr47Tb8bcFpOuMbKHyNJD1e\n50LvLdvovV37lWDeU0COudiA4MDVO6Y8w6xdgyfhtRiH/PpaZGLdY5LDKss3E8K5\n3bn1hzmpZTdN3pNrDk+YRzYT0gfeY76DDJss1aZjySekXBEpTzvXuhJPYd0b3SOg\nigHStcdO4F9gasTFpOht+7+ufjgpm6tkNdFC57CHuvKo3DU0r58HJb+JfOvybBRL\n8a9XYc8ILbVW4tntZWF1XZWsJb59JIUhE1EAbebly7DiEWdnvOsB5roTFr/7lHmY\ndl7ws3U5nu1vENvfG7C4prloctZ2rf1EaECqJkWxjbPibwi5odNaJUFUyJa0W7yX\nZRvERnRcd9u1Nxb0+XQPLJaDjyETPtHUhkKhgfxejDlACNd1S1aBAtkB2kJobYMO\nT3OLPjhixUHXqQIDAQABozIwMDAeBgNVHSUEFzAVBggrBgEFBQcDAgYJKoZIhvcN\nAQkHMA4GA1UdDwEB/wQEAwIDyDANBgkqhkiG9w0BAQsFAAOCAQEAXGsyAyZyU1XK\nqPk63QCd1Q+nskjVQ+z6lWJhxtI9QlugNaVUoqZS083/U4t21xxjAvKZAb4ZjjjJ\n7i8ou0MpgVZDnJaT1zbQ7T/lJch9OVpqR/JPzHQ7u0hk3EU+6MAgQVpdfKjcRhvW\nat8b/NvDTyf7/fmhPjbz6Zh4/+OFZutYWqwqinmf0YRPq7/LjbF3bRiu9MDFm62U\nIcZ3LlEWoR8DGu8nlDhHRemtnb+C8t4bpn/6ka6UDxYo1XilXF4yF6pXIETXr/CI\nYEzJykXD9LGv/1qdS070viVr1Fv9oha4dJxpuAM/pDEETdMSOjZT6+rWljjOez7S\nOOLkUi74hw==";
        // convert certstring to byte array
        var result = LoadCertificateFromPem(certstring, "atakatak");
    }

    private static byte[] LoadCertificateFromPem(string certstring, string password)
    {
        var bytes = Convert.FromBase64String(certstring);
        var cert = new X509Certificate2(bytes, string.Empty,
            X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

        return cert.Export(X509ContentType.Pkcs12, password);
    }

    [Fact]
    public async void CertificateEnrollment()
    {
        var username = "itak";
        var password = "Itak1234567890!";
        var certPassword = "atakatak";
        var uid = Guid.NewGuid().ToString();
        var url = "https://ufs.tak-server.com:8446";
        var uri = new Uri(url);

        
        var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(
                        $"{username}:{password}")));
        var orgPath = "/Marti/api/tls/config";
        var body = await client.GetStringAsync(orgPath);
        using TextReader reader = new StringReader(body);
        var config = (certificateConfig)new XmlSerializer(typeof(certificateConfig)).Deserialize(reader);

        var o = config.nameEntries[0].value;
        var ou = config.nameEntries[1].value;

        using (var privateKey = RSA.Create(2048))
        {
            var request = new CertificateRequest(new X500DistinguishedName($"CN={username};O={o};OU={ou}"), privateKey,
                HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            var bytes = request.CreateSigningRequest();
            var bodyreq = Convert.ToBase64String(bytes);
            var res = await client.PostAsync($"/Marti/api/tls/signClient/v2?clientUid={uid}&version=1.0",
                new StringContent(bodyreq));
            var resstr = await res.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<CertResponse>(resstr);

            var certBytes = LoadCertificateFromPem(response.signedCert, certPassword);
            var cert = new X509Certificate2(certBytes, certPassword);
            var certWithKey = cert.CopyWithPrivateKey(privateKey);
            var certWithKeyBytes = certWithKey.Export(X509ContentType.Pkcs12);
            var ca0 = LoadCertificateFromPem(response.ca0, certPassword);
            var ca1 = LoadCertificateFromPem(response.ca1, certPassword);

            MakeConnectionPackage(uri, ca0, ca1, certWithKeyBytes, certPassword, $"{uid}.zip");

            //var client1 = new TcpClient();
            //client1.Connect(uri.DnsSafeHost, 8089);
            //var sslStream = new SslStream(client1.GetStream(), false, CertificateValidationCallback);

            //X509CertificateCollection certcollection = new();
            //certcollection.Add(new X509Certificate2(certWithKey.Export(X509ContentType.Pkcs12)));
            //await sslStream.AuthenticateAsClientAsync(uri.DnsSafeHost, certcollection,SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.Tls11, false);
        }
    }

    private static void MakeConnectionPackage(Uri uri, byte[] ca0, byte[] ca1, byte[] certBytes, string certPassword,
        string filename)
    {
        var pref = @$"<?xml version='1.0' encoding='ASCII' standalone='yes'?>
                    <preferences>
                    <preference version='1' name='cot_streams'>
                    <entry key='count' class='class java.lang.Integer'>1</entry>
                    <entry key='description0' class='class java.lang.String'>{uri.DnsSafeHost}</entry>
                    <entry key='enabled0' class='class java.lang.Boolean'>true</entry>
                    <entry key='connectString0' class='class java.lang.String'>{uri.DnsSafeHost}:8089:ssl</entry>
                    </preference>
                    <preference version='1' name='com.atakmap.app_preferences'>
                    <entry key='displayServerConnectionWidget' class='class java.lang.Boolean'>true</entry>
                    <entry key='caLocation' class='class java.lang.String'>cert/ca.p12</entry>
                    <entry key='caPassword' class='class java.lang.String'>{certPassword}</entry>
                    <entry key='certificateLocation' class='class java.lang.String'>cert/user.p12</entry>
                    <entry key='clientPassword' class='class java.lang.String'>{certPassword}</entry>
                    </preference>
                    </preferences>";

        using (var memoryStream = new MemoryStream())
        {
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var prefFile = archive.CreateEntry("server.pref");
                using (var entryStream = prefFile.Open())
                using (var streamWriter = new StreamWriter(entryStream))
                {
                    streamWriter.Write(pref);
                }

                var certFile = archive.CreateEntry("cert/ca.p12");
                using (var entryStream = certFile.Open())
                {
                    entryStream.Write(ca0);
                }

                var certFile1 = archive.CreateEntry("cert/ca1.p12");
                using (var entryStream = certFile1.Open())
                {
                    entryStream.Write(ca1);
                }

                var certFile2 = archive.CreateEntry("cert/user.p12");
                using (var entryStream = certFile2.Open())
                {
                    entryStream.Write(certBytes);
                }
            }

            using (var fileStream = new FileStream(@$"C:\Temp\{filename}", FileMode.Create))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.CopyTo(fileStream);
            }
        }
    }

    private bool CertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        if (true)
            return true;

        return sslPolicyErrors == SslPolicyErrors.None;
    }


    [Fact]
    public async void Test1()
    {
        var apiBaseUrl = "https://api.ares-alpha.com/api/v1/";
        var apiUrl = apiBaseUrl + "login"; // Replace with your API endpoint URL

        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Ares%20alpha", "363"));
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CFNetwork", "1410.0.3"));
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Darwin", "22.6.0"));

        try
        {
            // Create a JSON object to send
            var jsonContent = new
            {
                Username = "eric.hexter@gmail.com",
                Password = "brrgzy"
            };

            // Serialize the JSON object to a string
            var jsonString = JsonConvert.SerializeObject(jsonContent);

            // Create the HTTP content with JSON data
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseObject = JsonSerializer.Deserialize<LoginResult>(responseContent);
                //responseObject.value.id
                //   "get-player"

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", responseObject.value.token);

                var r = new
                {
                    PlayerId = responseObject.value.id,
                    GameId = (string)null!
                };

                var ree = await httpClient.PostAsync(apiBaseUrl + "get-player",
                    new StringContent(JsonConvert.SerializeObject(r), Encoding.UTF8,
                        "application/json"));
                var con = await ree.Content.ReadAsStringAsync();
                Debug.WriteLine(con);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}

//https://ares-alpha.com/show-qr/QVJFUy9Kb2luVGVhbS9Tb2xkaWVyL2YwZWMyNmI0LTI0Y2UtNDIyZC04MzI5LTdlNDgzYjZiYTEyYS8yMzYzZjZiZi0zYmExLTQ0M2EtODdlYy02NDQ2NDIyZjZjZDM=
//ARES/JoinTeam/SquadLeader/f0ec26b4-24ce-422d-8329-7e483b6ba12a/2363f6bf-3ba1-443a-87ec-6446422f6cd3
//ARES/JoinTeam/Soldier/f0ec26b4-24ce-422d-8329-7e483b6ba12a/2363f6bf-3ba1-443a-87ec-6446422f6cd3

public class LoginResult
{
    public bool hasError { get; set; }
    public object errorMessage { get; set; }
    public LoginUser value { get; set; }
}

public class LoginUser
{
    public string id { get; set; }
    public string username { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string club { get; set; }
    public string nickname { get; set; }
    public string country { get; set; }
    public string city { get; set; }
    public string token { get; set; }
    public DateTime expirationTimestamp { get; set; }
    public string[] groups { get; set; }
    public int keepDataPeriod { get; set; }
    public bool metricSystem { get; set; }
    public bool centerMap { get; set; }
}

public class UserResult
{
    public bool hasError { get; set; }
    public object errorMessage { get; set; }
    public User value { get; set; }
}

public class User
{
    public string id { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string salt { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string club { get; set; }
    public string nickname { get; set; }
    public string country { get; set; }
    public bool isAdmin { get; set; }
    public bool isActive { get; set; }
    public bool canCreateEvents { get; set; }
    public string city { get; set; }
    public DateTime timestamp { get; set; }
    public int keepDataPeriod { get; set; }
    public bool privacyOptionA { get; set; }
    public bool privacyOptionB { get; set; }
    public bool privacyOptionC { get; set; }
    public string createdOnPlatform { get; set; }
    public bool metricSystem { get; set; }
    public bool centerMap { get; set; }
    public int credits { get; set; }
}

// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks />
[SerializableAttribute]
[DesignerCategory("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "com.bbn.marti.config")]
[XmlRootAttribute(Namespace = "com.bbn.marti.config", IsNullable = false)]
public class certificateConfig
{
    private nameEntriesNameEntry[] nameEntriesField;

    /// <remarks />
    [XmlArrayAttribute(Namespace = "http://bbn.com/marti/xml/config")]
    [XmlArrayItemAttribute("nameEntry", IsNullable = false)]
    public nameEntriesNameEntry[] nameEntries
    {
        get => nameEntriesField;
        set => nameEntriesField = value;
    }
}

/// <remarks />
[SerializableAttribute]
[DesignerCategory("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://bbn.com/marti/xml/config")]
public class nameEntriesNameEntry
{
    private string nameField;

    private string valueField;

    /// <remarks />
    [XmlAttributeAttribute]
    public string name
    {
        get => nameField;
        set => nameField = value;
    }

    /// <remarks />
    [XmlAttributeAttribute]
    public string value
    {
        get => valueField;
        set => valueField = value;
    }
}

/// <remarks />
[SerializableAttribute]
[DesignerCategory("code")]
[XmlTypeAttribute(AnonymousType = true, Namespace = "http://bbn.com/marti/xml/config")]
[XmlRootAttribute(Namespace = "http://bbn.com/marti/xml/config", IsNullable = false)]
public class nameEntries
{
    private nameEntriesNameEntry[] nameEntryField;

    /// <remarks />
    [XmlElementAttribute("nameEntry")]
    public nameEntriesNameEntry[] nameEntry
    {
        get => nameEntryField;
        set => nameEntryField = value;
    }
}

public class CertResponse
{
    public string signedCert { get; set; }
    public string ca0 { get; set; }
    public string ca1 { get; set; }
}