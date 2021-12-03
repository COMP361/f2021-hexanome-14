using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour, GameSessionsReceivedInterface
{
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject availableGamesView;
    [SerializeField] private GameObject sessionPrefab;

    public void OnGameLaunched()
    {
        connectionStatusText.gameObject.SetActive(true);
    }

    public void OnConnect()
    {
        networkManager.Connect();
    }

    public void SetConnectionStatus(string status)
    {
        connectionStatusText.text = status;
    }

    public void OnUpdatedGameListReceived(List<Lobby.GameSession> gameSessions)
    {
        RemoveAllGameSessions();

    }

    private void RemoveAllGameSessions()
    {
        foreach (Transform child in availableGamesView.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    private void AddGameSession(Lobby.GameSession gameSession)
    {
        GameObject newSession = Instantiate(sessionPrefab, availableGamesView.transform);
        
    }
}
