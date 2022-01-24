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

    private bool active = true;


    public void SetToDefaultColor()
    {
        Image image = GetComponent<Image>();
        if (gameSession.players.Contains(Lobby.myUsername))
        {
            image.color = new Color(89f / 255f, 231f / 255f, 230f / 255f, 74f / 255f);

        }
        else
        {
            image.color = new Color(238f / 255f, 100f / 255f, 100f / 255f, 74f / 255f);

        }
    }

    public void SetFields(Lobby.GameSession gameSession)
    {
        createdByText.text = $"CreatedBy: {gameSession.createdBy}";
        nPlayersText.text = $"Players: {gameSession.players.Count}";

        this.gameSession = gameSession;
        SetToDefaultColor();
        // EE6464
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!active)
            return;

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

    public void deactivate()
    {
        Image image = GetComponent<Image>();
        if (gameSession.players.Contains(Lobby.myUsername))
        {
            image.color = new Color(89f / 255f, 231f / 255f, 230f / 255f, 74f / 255f);

        }
        else
        {
            image.color = new Color(107f / 255f, 107f / 255f, 107f / 255f, 74f / 255f);

        }
        active = false;
    }

    public void activate()
    {
        active = true;
    }

}
