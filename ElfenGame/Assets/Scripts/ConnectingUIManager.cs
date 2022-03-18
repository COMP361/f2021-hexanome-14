using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectingUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionStatus;

    // Start is called before the first frame update
    void Start()
    {
        if (GameConstants.networkManager)
        {
            GameConstants.networkManager.Connect();
        }

        if (GameConstants.playfabManager)
        {
            GameConstants.playfabManager.Login(Lobby.myUsername);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameConstants.networkManager)
        {
            connectionStatus.text = GameConstants.networkManager.getNetworkState();
            //Debug.Log(connectionStatus.text);
            if (!GameConstants.networkManager.isConnected())
                GameConstants.networkManager.Connect();
        }

        if (GameConstants.networkManager && GameConstants.networkManager.getNetworkState() == "ConnectedToMasterServer")
            SceneManager.LoadScene("MainMenu");
    }
}
