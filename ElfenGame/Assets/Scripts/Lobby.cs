using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Security.Cryptography;
using Newtonsoft.Json;

public class Lobby : MonoBehaviour
{

    private static readonly HttpClient client = new HttpClient();
    static string accessToken;
    static string resetToken;
    static List<string> sessionIDs;
    public static List<GameSession> availableGames = new List<GameSession>();
    public static string myUsername;
    public static DateTime lastRenew;
    public static string lastHash = "-";


    // Start is called before the first frame update
    void Start()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("user", "bgp-client-name:bgp-client-pw");
        // Debug.Log(AuthenticateAsync());
        // await AuthenticateAsync("maex", "abc123_ABC123");
        // await RenewToken();
        //GetToken();
    }

    public class Token
    {
        public int access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }

    public class GameSession
    {

        public GameSession(string session_ID, List<string> players, string createdBy, string saveID)
        {
            this.session_ID = session_ID;
            this.players = players;
            this.createdBy = createdBy;
            this.saveID = saveID;
        }
        public string session_ID { get; private set; }

        public List<string> players { get; private set; }

        public string createdBy { get; private set; }

        public string saveID { get; private set; }

        public override string ToString()
        {
            return $"Id: {this.session_ID}, N players {this.players.Count}, createdby: {this.createdBy}";
        }
    }

    static public async Task AuthenticateAsync(string username, string password)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{GameConstants.lobbyServiceUrl}/oauth/token"))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                request.Content = new StringContent(String.Format("grant_type=password&username={0}&password={1}", username, password));
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var response = await httpClient.SendAsync(request);

                // response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    //var result = JsonConvert.DeserializeObject(responseString);
                    JObject json = JsonConvert.DeserializeObject<JObject>(responseString);
                    Debug.Log("Access Token Retreived: " + json["access_token"]);

                    accessToken = json["access_token"].ToString().Replace("+", "%2B");
                    resetToken = json["refresh_token"].ToString().Replace("+", "%2B");
                    myUsername = username;

                    await RenewToken();
                    SceneManager.LoadScene("Connecting");
                }
                else
                {
                    if (GameConstants.loginUIManager != null)
                    {
                        GameConstants.loginUIManager.OnLoginFailed();
                    }
                }

                // await getSessions();
            }
        }
    }

    private static string createMd5Hex(string data)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] dataHash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
        StringBuilder sb = new StringBuilder();
        foreach (byte b in dataHash)
        {
            sb.Append(b.ToString("x2").ToLower());
        }
        return sb.ToString();
    }

    public static async Task LongPollForUpdates(GameSessionsReceivedInterface callbackTarget)
    {
        var url = $"{GameConstants.lobbyServiceUrl}/api/sessions/?hash={lastHash}&location=18.116.53.177&access_token={accessToken}";
        //Debug.Log(lastHash);
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            using (var response = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead))
            {

                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        JObject json = JsonConvert.DeserializeObject<JObject>(responseString);

                        //Debug.Log("Access Token Retreived: " + json["access_token"]);

                        availableGames = new List<GameSession>();

                        lastHash = createMd5Hex(responseString);

                        // List<GameSession> allGames = new List<GameSession>();
                        foreach (JToken game in json["sessions"].Children())
                        {
                            var property = game as JProperty;
                            //Debug.Log(property);
                            //Debug.Log(property.Value["creator"]);
                            //Debug.Log(property.Value["players"]);


                            GameSession gameSession = new GameSession(session_ID: property.Name, players: property.Value["players"].ToObject<List<string>>(), createdBy: property.Value["creator"].ToString(), saveID: property.Value["savegameid"].ToString());

                            availableGames.Add(gameSession);
                            //Debug.Log(gameSession.ToString());
                            // Debug.Log(allGames);
                        }
                        //Debug.Log(availableGames);

                        if (callbackTarget != null)
                        {
                            callbackTarget.OnUpdatedGameListReceived(availableGames);
                        }
                    }
                    catch (JsonReaderException)
                    {
                        Debug.LogError($"Failed to parse json: {responseString}");
                    }
                }



            }
        }

        await LongPollForUpdates(callbackTarget);
    }

    public static async Task CreateSession(string savegameID = "")
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{GameConstants.lobbyServiceUrl}/api/sessions?location=18.116.53.177&access_token={accessToken}"))
            {
                request.Content = new StringContent("{\"game\":\"ElfenGame\", \"creator\":\"" + myUsername + "\", \"savegame\":\"" + savegameID + "\"}");
                Debug.Log($"Creating session: {request.Content.ToString()}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application / json");

                var response = await httpClient.SendAsync(request);


                Debug.Log(response);
            }
        }
    }

    // public static async Task CreateSaveGame(string savegameID = "")
    // {
    //     using (var httpClient = new HttpClient())
    //     {
    //         using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{GameConstants.lobbyServiceUrl}/api/savegames?location=

    // public static bool SessionListUpdated(List<GameSession> list1, List<GameSession> list2)
    // {
    //     bool updated = false;

    //     foreach (GameSession game1 in list1)
    //     {
    //         bool foundMatch = false;
    //         foreach (GameSession game2 in list2)
    //         {
    //             if (game1.session_ID == game2.session_ID)
    //             {
    //                 foundMatch = true;
    //             }
    //         }

    //         if (!foundMatch)
    //         {
    //             updated = true;
    //         }
    //     }

    //     foreach (GameSession game1 in list2)
    //     {
    //         bool foundMatch = false;
    //         foreach (GameSession game2 in list1)
    //         {
    //             if (game1.session_ID == game2.session_ID)
    //             {
    //                 foundMatch = true;
    //             }
    //         }

    //         if (!foundMatch)
    //         {
    //             updated = true;
    //         }
    //     }


    //     return updated;
    // }

    public static async Task LeaveSession(string sessionID)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"{GameConstants.lobbyServiceUrl}/api/sessions/{sessionID}/players/{myUsername}?access_token={accessToken}"))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                request.Headers.TryAddWithoutValidation("authorization", $"Basic {base64authorization}");
                var response = await httpClient.SendAsync(request);
            }
        }
    }

    public static async Task DeleteSession(string sessionID)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"{GameConstants.lobbyServiceUrl}/api/sessions/{sessionID}?access_token={accessToken}"))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                request.Headers.TryAddWithoutValidation("authorization", $"Basic {base64authorization}");
                var response = await httpClient.SendAsync(request);
            }
        }
    }

    // public static async Task GetSessions(GameSessionsReceivedInterface callbackTarget)
    // {
    //     using (var httpClient = new HttpClient())
    //     {
    //         using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"{GameConstants.lobbyServiceUrl}/api/sessions"))
    //         {
    //             var response = await httpClient.SendAsync(request);

    //             var responseString = await response.Content.ReadAsStringAsync();

    //             JObject json = JsonConvert.DeserializeObject<JObject>(responseString);
    //             //Debug.Log("Access Token Retreived: " + json["access_token"]);

    //             availableGames = new List<GameSession>();


    //             // List<GameSession> allGames = new List<GameSession>();
    //             foreach (var game in json["sessions"].Children())
    //             {
    //                 var property = game as JProperty;
    //                 //Debug.Log(property);
    //                 //Debug.Log(property.Value["creator"]);
    //                 //Debug.Log(property.Value["players"]);


    //                 GameSession gameSession = new GameSession() { session_ID = property.Name, players = property.Value["players"].ToObject<List<string>>(), createdBy = property.Value["creator"].ToString() };

    //                 availableGames.Add(gameSession);
    //                 //Debug.Log(gameSession.ToString());
    //                 // Debug.Log(allGames);
    //             }
    //             //Debug.Log(availableGames);

    //             callbackTarget.OnUpdatedGameListReceived(availableGames);

    //             //if (SessionListUpdated(newGames, availableGames))
    //             //{
    //             //    availableGames = newGames;

    //             //}

    //         }
    //     }
    // }


    public static async Task JoinSession(string sessionID)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), $"{GameConstants.lobbyServiceUrl}/api/sessions/{sessionID}/players/{myUsername}?access_token={accessToken}&location=18.116.53.177"))
            {
                var response = await httpClient.SendAsync(request);
                Debug.Log(response);
            }
        }
    }

    public static async Task RenewToken()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"{GameConstants.lobbyServiceUrl}/oauth/token"))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                request.Content = new StringContent($"grant_type=refresh_token&refresh_token={resetToken}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var response = await httpClient.SendAsync(request);

                var responseString = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<JObject>(responseString);
                Debug.Log("Access Token Retreived: " + json["access_token"]);
                accessToken = json["access_token"].ToString().Replace("+", "%2B");
                resetToken = json["refresh_token"].ToString().Replace("+", "%2B");
                Debug.Log(response);
            }
        }

        lastRenew = DateTime.Now;
    }

    // Update is called once per frame
    async void Update()
    {
        if ((DateTime.Now - lastRenew).Milliseconds > 100000)
        {
            Debug.Log("Renewing Token Automatically");
            lastRenew = DateTime.Now;
            await RenewToken();
        }
    }
}
