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

    public Lobby.GameSession gameSession;
    private OnGameSessionClickedHandler handler;

    private Image background;

    private bool active = true;

    public void Awake()
    {
        background = GetComponent<Image>();
    }
    public void SetToDefaultColor()
    {
        // Image image = GetComponent<Image>();
        // if (gameSession.players.Contains(GameConstants.username))
        // {
        //     image.color = GameConstants.blueFaded;

        // }
        // else
        // {
        //     image.color = GameConstants.redFaded;

        // }
        if (background == null)
        {
            background = GetComponent<Image>();
        }
        background.color = GameConstants.blueFaded;
    }

    public void SetFields(Lobby.GameSession gameSession)
    {
        createdByText.text = $"CreatedBy: {gameSession.createdBy}";

        string players_string = "Players: ";

        for (int i = 0; i < gameSession.players.Count; i++)
        {
            players_string += gameSession.players[i];
            if (i != gameSession.players.Count - 1)
            {
                players_string += ", ";
            }
        }
        nPlayersText.text = players_string;
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
        image.color = GameConstants.greenFaded;
    }

    public void SetOnGameSessionClickedHandler(OnGameSessionClickedHandler handler)
    {
        this.handler = handler;
    }

    public void deactivate()
    {
        active = false;
    }

    public void activate()
    {
        active = true;
    }

    internal void SetSelectedColor()
    {
        background.color = GameConstants.greenFaded;
    }

    internal void SetUnselectedColor()
    {
        background.color = GameConstants.greyFaded;
    }
}
