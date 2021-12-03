using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSessionListItemScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI createdByText;
    [SerializeField]
    private TextMeshProUGUI nPlayersText;

    private Lobby.GameSession gameSession;
    private OnGameSessionClickedHandler handler;


    public void SetFields(Lobby.GameSession gameSession)
    {
        createdByText.text = $"CreatedBy: {gameSession.createdBy}";
        nPlayersText.text = $"Players: {gameSession.players.Count}";

        if (gameSession.players.Contains(Lobby.myUsername))
        {
            Image image = GetComponent<Image>();
            image.color = new Color(89f / 255f, 231f / 255f, 230f / 255f, 74f / 255f);
   
        }

        this.gameSession = gameSession;
        // EE6464
    }


    public void OnPointerClick(PointerEventData eventData)
    {

        if (handler != null)
        {
            handler.OnGameSessionClicked(gameSession);
        }

        Image image = GetComponent<Image>();
        image.color = new Color(102f / 255f, 236f / 255f, 77f / 255f, 74f / 255f);
    }

    public void SetOnGameSessionClickedHandler(OnGameSessionClickedHandler handler)
    {
        this.handler = handler;
    }

}
