using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    public GameObject playerPrefab;

    [SerializeField]
    public GameObject leftPane;

    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerManagers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializePlayerManagers()
    {
        if (GameConstants.networkManager)
        {
            foreach (Photon.Realtime.Player p in GameConstants.networkManager.GetPlayers())
            {
                GameObject g = Instantiate(playerPrefab, leftPane.transform);
                PlayerManager pm = g.GetComponent<PlayerManager>();

                pm.initialize(p.UserId);
                pm.updateStats();
            }
        }
    }

    public void OnPausePressed()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
    }

    public void exitGameClicked()
    {
        GameConstants.networkManager.LeaveRoom();
    }
}
