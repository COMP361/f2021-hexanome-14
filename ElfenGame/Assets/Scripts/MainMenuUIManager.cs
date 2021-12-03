using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private NetworkManager networkManager;


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
}
