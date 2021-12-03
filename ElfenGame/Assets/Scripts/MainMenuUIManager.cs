using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private TextMeshProUGUI scrollViewElementPrefab;

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

    public void UpdateAvailableRoomList(List<string> roomNames)
    {
        Debug.LogError("Rooms updated");

        foreach (string roomName in roomNames)
        {
            Debug.LogError($"Room found: {roomName}");
        }
    }
}
