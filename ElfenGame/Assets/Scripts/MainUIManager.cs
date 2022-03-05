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
        PlayerManager pm = playerPrefab.GetComponent<PlayerManager>();
        
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

    public void OnShowCardHandPressed()
    {
        Transform cardsGroup =  cardPanel.transform.GetChild(0);
        PlayerManager pm = playerPrefab.GetComponent<PlayerManager>();
        if (!isViewingCards){
            isViewingCards = true;
            cardPanel.SetActive(true);
            confirmButton.SetActive(true);

            
            /*
            GameObject g = Instantiate(cardPrefab, cardPanel.transform);
            g.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("T01");
            g.GetComponent<SpriteRenderer>().sortingOrder = 2;
            */
            pm.mCards = new List<Card>();
            pm.mCards.Add(new Card(CardEnum.T02));
            pm.mCards.Add(new Card(CardEnum.T03));
            pm.mCards.Add(new Card(CardEnum.T04));
            pm.mCards.Add(new Card(CardEnum.T05));
            pm.mCards.Add(new Card(CardEnum.T06));
            pm.mCards.Add(new Card(CardEnum.T07));
            pm.mCards.Add(new Card(CardEnum.witch));

        
            foreach ( Card card in pm.mCards){
                GameObject g = Instantiate(cardPrefab, cardPanel.transform);
                g.transform.SetParent(cardsGroup);
                CardEnum c = card.cardType;
                g.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CardEnum.GetName(c.GetType(), c));
                g.GetComponent<SpriteRenderer>().sortingOrder = 2;
                UnityEngine.UI.Button button = g.transform.GetChild(2).GetComponent<Button>();
                button.onClick.AddListener(() => card.OnClickCard(g));
            }
        //pm.mCards;
        }else {

            //unselect all the cards
            foreach ( Card card in pm.mCards){
                card.selected = false;
            }
            
            // delete all the children and unset the plane 
            foreach (Transform child in cardsGroup.transform) {
                GameObject.Destroy(child.gameObject);
            }
            isViewingCards = false;
            cardPanel.SetActive(false);
            confirmButton.SetActive(false);
        }
        
        
    }

    public void OnSelectPressed(){
        PlayerManager pm = playerPrefab.GetComponent<PlayerManager>();
        foreach ( Card card in pm.mCards){
            if (card.selected){
                Debug.Log(CardEnum.GetName(card.cardType.GetType(), card.cardType));
            }
        }
    }
}
