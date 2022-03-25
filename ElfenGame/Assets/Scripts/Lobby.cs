using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Security.Cryptography;
using Newtonsoft.Json;

public class Lobby
{
    public static Lobby user, gameservice;
    private static bool initWasRun = false;
    public static List<GameSession> availableGames = new List<GameSession>();

    private string accessToken, refreshToken;
    private string lastHash = "-";

    private bool renewingToken = false;
    private DateTime lastRenewed = DateTime.FromOADate(0);
    private List<Action> onRenewDone = new List<Action>();

    private List<SavedGame> savedGames = new List<SavedGame>();

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

    public struct SavedGame
    {
        public string saveID;
        public List<string> players;
    }

    public static void Init()
    {
        if (initWasRun)
            return;

        initWasRun = true;
        user = new Lobby();
        gameservice = new Lobby();

        gameservice.AuthenticateGameService();
        gameservice.GetSessions();
    }


    #region Authentication

    private void SetTokens(string json)
    {
        JObject jsonObject = JObject.Parse(json);
        accessToken = System.Net.WebUtility.UrlEncode(jsonObject["access_token"].ToString());
        refreshToken = System.Net.WebUtility.UrlEncode(jsonObject["refresh_token"].ToString());
    }
    public void AuthenticateAsync(string username, string password, Action<bool, string> callback)
    {
        string url = "/oauth/token";
        string json = $"grant_type=password&username={username}&password={password}";

        Task task = LobbySendAsync(url, HttpMethod.Post, json: json, use_token: false, first_refresh: false, encode_media: true, auth: true, callback: callback);
    }
    public void AuthenticateUser(string username, string password)
    {
        AuthenticateAsync(username, password,
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log($"Successfully authenticated as {username}");
                SetTokens(msg);
                GameConstants.username = username;
                SceneManager.LoadScene("Connecting");
            }
            else
            {
                Debug.Log($"Failed to authenticate as {username}. Response: {msg}");
                LoginUIManager.manager.OnLoginFailed();
            }
        });
    }

    private void AuthenticateGameService()
    {
        AuthenticateAsync(GameConstants.service_username, GameConstants.service_password,
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log($"Successfully authenticated as {GameConstants.service_username}");
                SetTokens(msg);
            }
            else
            {
                Debug.LogError($"Failed to authenticate Service Account!");
                Application.Quit();
            }
        });
    }
    private void RenewToken(Action onSucess = null)
    {
        if (renewingToken)
        {
            if (onSucess != null)
                onRenewDone.Add(onSucess);
            return;
        }
        renewingToken = true;
        string url = "/oauth/token";
        string json = $"grant_type=refresh_token&refresh_token={refreshToken}";

        Task task = LobbySendAsync(url, HttpMethod.Post, json: json, use_token: false, first_refresh: false, encode_media: true, auth: true, callback:
        (bool success, string msg) =>
        {
            renewingToken = false;
            if (success)
            {
                Debug.Log("Successfully renewed token");
                lastRenewed = DateTime.Now;
                SetTokens(msg);
                if (onSucess != null)
                {
                    onSucess();
                }
                foreach (Action action in onRenewDone)
                {
                    Debug.Log("Found Action in onRenewDone");
                    action();
                }
                onRenewDone.Clear();
            }
            else
            {
                Debug.Log("RenewToken failed: " + msg);
            }
        });
    }

    #endregion

    #region Sessions
    public void CreateSession(string savegameID = "")
    {
        string url = "/api/sessions";
        string q_params = "location=0.0.0.0";
        string json = "{\"game\":\"ElfenGame\", \"creator\":\"" + GameConstants.username + "\", \"savegame\":\"" + savegameID + "\"}";

        Task task = LobbySendAsync(url, HttpMethod.Post, json: json, q_params: q_params,
        callback:
        (bool success, string response) =>
        {
            if (success)
            {
                Debug.Log("New Game Session: " + response);
                HandleLocalPlayerGameCreated(response);
            }
            else
            {
                Debug.LogError($"Failed to create session: {response}");
            }
        });
    }
    public void LaunchSession(string sessionID)
    {
        string url = "/api/session/" + sessionID;
        Task task = LobbySendAsync(url, HttpMethod.Post,
        callback:
        (bool success, string response) =>
        {
            if (success)
            {
                Debug.Log("Successfully launched session");
                MainMenuUIManager.manager.CreateGameWithOptions();
            }
            else
            {
                Debug.LogError("Failed to launch session");
                MainMenuUIManager.manager.CreateGameWithOptions(); // TODO: Remove when done debugging
            }
        });
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

    public void GetSessions()
    {
        string url = "/api/sessions/";
        string q_params = "hash=" + lastHash;
        Task task = LobbySendAsync(url, HttpMethod.Get, q_params: q_params, long_poll: true, use_token: false,
        callback:
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log("GetSessions: " + msg);
                JObject json = JsonConvert.DeserializeObject<JObject>(msg);


                availableGames = new List<GameSession>();

                lastHash = createMd5Hex(msg);

                foreach (JToken game in json["sessions"].Children())
                {
                    var property = game as JProperty;

                    GameSession gameSession = new GameSession(session_ID: property.Name, players: property.Value["players"].ToObject<List<string>>(), createdBy: property.Value["creator"].ToString(), saveID: property.Value["savegameid"].ToString());
                    availableGames.Add(gameSession);
                }

                if (MainMenuUIManager.manager != null)
                {
                    MainMenuUIManager.manager.OnUpdatedGameListReceived(availableGames);
                }
            }
            else
            {
                Debug.Log("GetSessions failed: " + msg);
            }
            GetSessions();
        });
    }

    private static void HandleLocalPlayerGameCreated(string sessionID)
    {
        if (MainMenuUIManager.manager != null)
        {
            MainMenuUIManager.manager.OnGameCreated(sessionID);
        }
    }

    public void LeaveSession(string sessionID)
    {
        string url = "/api/sessions/" + sessionID + "/players/" + GameConstants.username;
        Task task = LobbySendAsync(url, HttpMethod.Delete, auth: true, callback:
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log("Successfully left session: " + sessionID);
            }
            else
            {
                Debug.Log("LeaveSession failed: " + msg);
            }
        });
    }

    public void DeleteSession(string sessionID)
    {
        string url = "/api/sessions/" + sessionID;
        Task task = LobbySendAsync(url, HttpMethod.Delete, auth: true, callback:
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log("Successfully deleted session: " + sessionID);
            }
            else
            {
                Debug.Log("DeleteSession failed: " + msg);
            }
        });
    }

    public void JoinSession(string sessionID)
    {
        string url = "/api/sessions/" + sessionID + "/players/" + GameConstants.username;
        string q_params = "location=0.0.0.0";
        Task task = LobbySendAsync(url, HttpMethod.Put, q_params: q_params, auth: true, callback:
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log("Successfully joined session: " + sessionID);
            }
            else
            {
                Debug.Log("JoinSession failed: " + msg);
            }
        });
    }
    #endregion

    #region SavedGames

    public void PutSavedGame(string saveid, List<string> playerNames)
    {
        string url = "/api/gameservices/ElfenGame/savegames/" + saveid;
        string json = "{\"players\": " + JsonConvert.SerializeObject(playerNames) + ", \"gamename\": \"ElfenGame\", \"savegameid\": \"" + saveid + "\"}";
        Task task = LobbySendAsync(url, HttpMethod.Put, json: json, callback:
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log("Successfully saved game: " + saveid);
            }
            else
            {
                Debug.Log("PutSavedGame failed: " + msg);
            }
        });
    }

    public void GetSavedGames()
    {
        string url = "/api/gameservices/ElfenGame/savegames";
        Task task = LobbySendAsync(url, HttpMethod.Get, callback:
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log("Successfully retrieved saved games: " + msg);
                JObject json = JsonConvert.DeserializeObject<JObject>(msg);
                savedGames = new List<SavedGame>();
                foreach (JToken game in json.Children())
                {
                    var property = game as JProperty;
                    SavedGame savedGame = new SavedGame { saveID = property.Value["savegameid"].ToString(), players = property.Value["players"].ToObject<List<string>>() };
                    if (savedGame.players.Contains(GameConstants.username))
                        savedGames.Add(savedGame); // Only add games that the user is in
                }
            }
            else
            {
                Debug.Log("GetSavedGames failed: " + msg);
            }
        });
    }

    public void DeleteSavedGame(string saveid)
    {
        string url = "/api/gameservices/ElfenGame/savegames/" + saveid;
        Task task = LobbySendAsync(url, HttpMethod.Delete, callback:
        (bool success, string msg) =>
        {
            if (success)
            {
                Debug.Log("Successfully deleted saved game: " + saveid);
            }
            else
            {
                Debug.Log("DeleteSavedGame failed: " + msg);
            }
        });
    }

    #endregion

    public async Task LobbySendAsync(string url, HttpMethod method, Action<bool, string> callback = null, string q_params = "", string json = "", bool use_token = true, bool first_refresh = true, bool encode_media = false, bool auth = false, bool long_poll = false)
    {
        HttpClient client = new HttpClient();
        if (long_poll)
            client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

        string endpoint = GameConstants.lobbyServiceUrl + url;
        if (q_params != "")
            endpoint += "?" + q_params;

        if (use_token)
            endpoint += (endpoint.Contains('?') ? "&" : "?") + "access_token=" + accessToken;

        Debug.Log($"Sending request to {endpoint}");

        HttpRequestMessage request = new HttpRequestMessage(method, endpoint);

        if (auth)
        {
            var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("bgp-client-name:bgp-client-pw"));
            request.Headers.TryAddWithoutValidation("Authorization", "Basic " + base64authorization);
        }

        if (json != "")
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        if (encode_media)
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

        var response = await client.SendAsync(request);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && use_token && first_refresh)
        {
            if ((DateTime.Now - lastRenewed).TotalSeconds > GameConstants.tokenResetRate)
            {
                RenewToken(onSucess:
                 () =>
                    {
                        _ = LobbySendAsync(url, method, callback, q_params, json, use_token, false, encode_media, auth, long_poll);
                    });
            }

        }
        else
        {
            if (callback != null)
                callback(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        }
    }

}
