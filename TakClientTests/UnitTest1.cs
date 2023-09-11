using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace TakClientTests
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            string apiBaseUrl = "https://api.ares-alpha.com/api/v1/";
            string apiUrl = apiBaseUrl + "login"; // Replace with your API endpoint URL

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
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonContent);

                // Create the HTTP content with JSON data
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

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
                        new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(r), Encoding.UTF8,
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

}