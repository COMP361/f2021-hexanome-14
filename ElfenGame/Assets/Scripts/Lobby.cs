using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Lobby : MonoBehaviour
{

    private static readonly HttpClient client = new HttpClient();
    // Start is called before the first frame update
    void Start()
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("user", "bgp-client-name:bgp-client-pw");
        // Debug.Log(AuthenticateAsync());
        Debug.Log(AuthenticateAsync());
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

    async Task<Dictionary<string, string>> AuthenticateAsync()
    {
        using (var httpClient = new HttpClient())
        {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://18.116.53.177:4242/oauth/token"))
            {
                var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
                request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                request.Content = new StringContent("grant_type=password&username=maex&password=abc123_ABC123");
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                var response = await httpClient.SendAsync(request);

                var responseString = await response.Content.ReadAsStringAsync();

                string[] objects = responseString.Split(':');
                Debug.Log(objects);

                return null;
                //var data = JsonConvert.DeserializeObject<Token[]>(responseString);

                //var contentStream = await response.Content.ReadAsStreamAsync();
                //using var streamReader = new StreamReader(contentStream);
                //using var jsonReader = new JsonTextReader(streamReader);

                //JsonSerializer serializer = new JsonSerializer();

                //try
                //{
                //    return serializer.Deserialize<Dictionary<string, string>>(jsonReader);
                //}
                //catch (JsonReaderException)
                //{
                //    Debug.LogError("Invalid Json");
                //}
                //return null;
            }
        }
    }

    async void GetToken()
    {
        var values = new Dictionary<string, string>
        {
            //{ "user_oauth_approval", "true" },
            //{ "_csrf", "19beb2db-3807-4dd5-9f64-6c733462281b" },
            { "grant_type", "password" },
            { "username", "maex" },
            { "password", "abc123_ABC123" }
        };

        var content = new FormUrlEncodedContent(values);

        var response = await client.PostAsync("http://18.116.53.177:4242/oauth/token", content);

        var responseString = await response.Content.ReadAsStringAsync();
        
        Debug.Log(responseString);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
