using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    public GameObject playerPrefab;

    [SerializeField]
    public GameObject leftPane;

    [SerializeField]
    public GameObject cardPanel;

    [SerializeField]
    public GameObject cardPrefab;
    [SerializeField]
    public GameObject confirmButton;
  


    private bool isPaused = false;
    private bool isViewingCards = false;
    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerManagers();
        Player pm = playerPrefab.GetComponent<Player>();

        if (GameConstants.networkManager && GameConstants.networkManager.IsMasterClient())
        {
            Game.currentGame.Init();
	    }
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
                Player pm = g.GetComponent<Player>();

                pm.Initialize(p.UserId);
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

    public void OnShowCardHandPressed()
    {
        isViewingCards = !isViewingCards;
        cardPanel.SetActive(isViewingCards);
    }

    public void UpdateCardHand()
    { 
   
	    foreach (Card card in cardPanel.GetComponentsInChildren<Card>())
        {
            Destroy(card.gameObject);
		}

        Transform cardGroup = cardPanel.GetComponentInChildren<GridLayoutGroup>().transform;	
	    foreach (CardEnum c in Player.GetLocalPlayer().mCards)
        {
            GameObject g = Instantiate(cardPrefab, cardGroup);
            Card card = g.GetComponent<Card>();

            card.Initialize(c);

        }
    }
}
