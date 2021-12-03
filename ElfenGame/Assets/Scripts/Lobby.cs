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

public class Lobby : MonoBehaviour
{

    private static readonly HttpClient client = new HttpClient();
    static string accessToken;
    static string resetToken;
    static List<string> sessionIDs;
    public static List<GameSession> availableGames = new List<GameSession>();
    public static string myUsername;
    // Start is called before the first frame update
    void Start()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("user", "bgp-client-name:bgp-client-pw");
        // Debug.Log(AuthenticateAsync());
        AuthenticateAsync("maex", "abc123_ABC123");
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
        public string session_ID { get; set; }

        public List<string> players { get; set; }

        public string createdBy { get; set; }

        public string ToString()
        {
            return $"Id: {this.session_ID}, N players {this.players.Count}, createdby: {this.createdBy}";
        }
    }

    static public async Task AuthenticateAsync(string username, string password)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://18.116.53.177:4242/oauth/token"))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                request.Content = new StringContent(String.Format("grant_type=password&username={0}&password={1}",username, password));
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var response = await httpClient.SendAsync(request);

                // response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject json = JObject.Parse(responseString);
                    Debug.Log("Access Token Retreived: " + json["access_token"]);

                    accessToken = json["access_token"].ToString().Replace("+", "%2B");
                    resetToken = json["refresh_token"].ToString().Replace("+", "%2B");
                    myUsername = username;
                    SceneManager.LoadScene("MainMenu");
                }
                else
                {

                }

                // await getSessions();
            }
        }
    }

    public static async Task CreateSession()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), $"http://18.116.53.177:4242/api/sessions?location=18.116.53.177&access_token={accessToken}"))
            {
                request.Content = new StringContent("{\"game\":\"ElfenGame\", \"creator\":\"maex\", \"savegame\":\"\"}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);


                Debug.Log(response);
            }
        }
    }

    public static bool SessionListUpdated(List<GameSession> list1, List<GameSession> list2)
    {
        bool updated = false;

        foreach (GameSession game1 in list1)
        {
            bool foundMatch = false;
            foreach (GameSession game2 in list2)
            {
                if (game1.session_ID == game2.session_ID)
                {
                    foundMatch = true;
                }
            }

            if (!foundMatch)
            {
                updated = true;
            }
        }

        foreach (GameSession game1 in list2)
        {
            bool foundMatch = false;
            foreach (GameSession game2 in list1)
            {
                if (game1.session_ID == game2.session_ID)
                {
                    foundMatch = true;
                }
            }

            if (!foundMatch)
            {
                updated = true;
            }
        }


        return updated;
    }

    public static async Task GetSessions(GameSessionsReceivedInterface callbackTarget)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "http://18.116.53.177:4242/api/sessions"))
            {
                var response = await httpClient.SendAsync(request);

                var responseString = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(responseString);
                //Debug.Log("Access Token Retreived: " + json["access_token"]);

                availableGames = new List<GameSession>();


                // List<GameSession> allGames = new List<GameSession>();
                foreach (var game in json["sessions"].Children())
                {
                    var property = game as JProperty;
                    //Debug.Log(property);
                    //Debug.Log(property.Value["creator"]);
                    //Debug.Log(property.Value["players"]);


                    GameSession gameSession = new GameSession() { session_ID = property.Name, players = property.Value["players"].ToObject<List<string>>(), createdBy = property.Value["creator"].ToString()};
                    
                    availableGames.Add(gameSession);
                    //Debug.Log(gameSession.ToString());
                    // Debug.Log(allGames);
                }
                //Debug.Log(availableGames);

                callbackTarget.OnUpdatedGameListReceived(availableGames);

                //if (SessionListUpdated(newGames, availableGames))
                //{
                //    availableGames = newGames;
                    
                //}

            }
        }
    }


    public static async Task joinSession(string sessionID)
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), $"http://127.0.0.1:4242/api/sessions/{sessionID}/players/{myUsername}?access_token={accessToken}"))
            {
                var response = await httpClient.SendAsync(request);
                Debug.Log(response);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
