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

public class LobbyService : MonoBehaviour
{

    private static readonly HttpClient client = new HttpClient();
    static string accessToken;
    static string resetToken;
    static List<string> sessionIDs;
    public static string myUsername;
    public static DateTime lastRenew;


    public async static void initGameService()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("user", "bgp-client-name:bgp-client-pw");
        // Debug.Log(AuthenticateAsync());
        await AuthenticateAsync("ElfenGame", "abc123_ABC123");

        // await DeleteGame();
        await RegisterGame(); //TODO: reset this to 2
    }

    private static async Task DeleteGame()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"{GameConstants.lobbyServiceUrl}/api/gameservices/ElfenGame?access_token={accessToken}"))
            {
                using (var response = await httpClient.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Log("Game deleted");
                    }
                    else
                    {
                        Debug.Log("Game not deleted");
                    }
                }
            }
        }
    }

    public class Token
    {
        public int access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }

    public static async Task RegisterGame()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), $"{GameConstants.lobbyServiceUrl}/api/gameservices/ElfenGame?access_token={accessToken}"))
            {

                request.Content = new StringContent("{\"location\": \"\", \"maxSessionPlayers\": \"6\",\"minSessionPlayers\": \"2\",\"name\": \"ElfenGame\",\"displayName\": \"ElfenGame\",\"webSupport\": \"false\"}");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Game registered");
                }
                else
                {
                    // Get error message
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.Log($"Game not registered: {content}");
                }

            }
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
                    JObject json = JObject.Parse(responseString);
                    Debug.Log("Service Token Retreived: " + json["access_token"]);

                    accessToken = json["access_token"].ToString().Replace("+", "%2B");
                    resetToken = json["refresh_token"].ToString().Replace("+", "%2B");

                }
                else
                {
                }

                // await getSessions();
            }
        }
    }

}
