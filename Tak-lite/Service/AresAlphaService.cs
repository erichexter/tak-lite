using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tak_lite.Service;

public class AresAlphaService
{
    private string token;
    public bool LoggedIn { get; set; } = false;

    public async Task<LoginResult> Login(string username, string password)
    {
        var apiBaseUrl = "https://api.ares-alpha.com/api/v1/";
        var apiUrl = apiBaseUrl + "login";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Ares%20alpha", "363"));

        try
        {
            // Create a JSON object to send
            var jsonContent = new
            {
                Username = username,
                Password = password
            };

            var jsonString = JsonConvert.SerializeObject(jsonContent);
            var body = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, body);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var loginResult = JsonSerializer.Deserialize<LoginResult>(responseContent);
                token = loginResult.value.token;
                LoggedIn = true;
                return loginResult;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        return null;
    }

    public void Logout()
    {
        token = "";
        LoggedIn = false;
    }

    public async Task<Game> JoinGame(string playerId,string sponsorId,string gameID)
    {
        var apiBaseUrl = "https://api.ares-alpha.com/api/v1/";
        var apiUrl = apiBaseUrl + "join-game";
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Ares%20alpha", "363"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // Create a JSON object to send
            var gameRequest = new
            {
                PlayerId = playerId,
                SponsorId = sponsorId,
                GameId = gameID,
                Level = 1,
                PlatformType = 2,
                GroupName = "",
                CreateGroup = "false",
                SquadColor = ""

            };

            var gameRequestJson = JsonConvert.SerializeObject(gameRequest);
            var body = new StringContent(gameRequestJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(apiUrl, body);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var gameres = JsonSerializer.Deserialize<JoinGameResult>(responseContent);

                return gameres.value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }

        return null;
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

public class JoinGameResult
{
    public bool hasError { get; set; }
    public object errorMessage { get; set; }
    public Game value { get; set; }
}


public enum JoinGameLevel
{
    Soldier = 0,
    SquadLeader = 1,
    PlatoonLeader = 2,
    CompanyLeader = 3
}



public class Game
{
    public string squadId { get; set; }
    public string squadName { get; set; }
    public string squadColor { get; set; }
    public string teamId { get; set; }
    public string teamName { get; set; }
    public string teamColor { get; set; }
    public string platoonId { get; set; }
    public string platoonName { get; set; }
    public string companyId { get; set; }
    public string companyName { get; set; }
    public string gameId { get; set; }
    public string gameName { get; set; }
    public int assetLifespan { get; set; }
    public int refreshLocationPeriod { get; set; }
    public bool haveMedics { get; set; }
    public bool haveReferees { get; set; }
    public string mapId { get; set; }
    public bool isQuickGame { get; set; }
    public string creatorGameId { get; set; }
    public int poiMinLevel { get; set; }
    public int squadColorsType { get; set; }
}
