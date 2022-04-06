using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectingUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI connectionStatus;

    // Start is called before the first frame update
    void Start()
    {
        SaveAndLoad.UpdateLocalSavedIds();
        if (NetworkManager.manager)
        {
            NetworkManager.manager.Connect();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.manager)
        {
            connectionStatus.text = NetworkManager.manager.getNetworkState();
            //Debug.Log(connectionStatus.text);
            if (!NetworkManager.manager.isConnected())
                NetworkManager.manager.Connect();
        }

        if (NetworkManager.manager && NetworkManager.manager.getNetworkState() == "ConnectedToMasterServer")
            SceneManager.LoadScene("MainMenu");
    }
}
