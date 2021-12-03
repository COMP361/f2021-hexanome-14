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
    static GameSession[] availableGames;
    // Start is called before the first frame update
    void Start()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("user", "bgp-client-name:bgp-client-pw");
        // Debug.Log(AuthenticateAsync());
        // AuthenticateAsync("maex", "abc123_ABC123");
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
        public int session_ID { get; set; }

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

                    accessToken = json["access_token"].ToString();
                    resetToken = json["refresh_token"].ToString();

                    SceneManager.LoadScene("MainMenu");
                }
                else
                {

                }

                // await getSessions();
            }
        }
    }

    async Task CreateSession()
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

    public static async Task GetSessions()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "http://18.116.53.177:4242/api/sessions"))
            {
                var response = await httpClient.SendAsync(request);

                var responseString = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(responseString);
                Debug.Log("Access Token Retreived: " + json["access_token"]);


                // GameSession gs = new GameSession() { session_ID = json[]};


                string[] objects = responseString.Split('\"');

                List<string> ids = new List<string>();
                foreach (string item in objects)
                {
                    // var result = int.TryParse(item, out int temp);
                    if (Regex.IsMatch(item, @"^\d+$"))
                        {
                            ids.Add(item);
                        }

                }
                sessionIDs = ids;

                Debug.Log(sessionIDs);
                Regex regex = new Regex(@"\[.*?\]");
                List<List<string>> players = new List<List<string>>();
                foreach (Match match in regex.Matches(responseString))
                {
                    players.Add(match.Value.Split(new char[] { ',' }).ToList());
                }
                var matches = regex.Matches(responseString);


                Debug.Log(players);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
