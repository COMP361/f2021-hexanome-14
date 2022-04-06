using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavedGameListItemScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI saveidText;
    [SerializeField]
    private TextMeshProUGUI playersText;

    private string saveid;

    private Image background;

    public void Awake()
    {
        background = GetComponent<Image>();
    }

    public void SetFields(string saveid, string[] players)
    {
        saveidText.text = $"Save ID: {saveid}";
        string players_string = "Players: ";

        for (int i = 0; i < players.Length; i++)
        {
            players_string += players[i];
            if (i != players.Length - 1)
            {
                players_string += ", ";
            }
        }
        playersText.text = players_string;
        this.saveid = saveid;
        SetToDefaultColor();
        // EE6464
    }

    public void SetToDefaultColor()
    {
        background.color = GameConstants.blueFaded;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        MainMenuUIManager.manager.LoadGameItemSelected(saveid);

        Image image = GetComponent<Image>();
        image.color = GameConstants.greenFaded;
    }

}

