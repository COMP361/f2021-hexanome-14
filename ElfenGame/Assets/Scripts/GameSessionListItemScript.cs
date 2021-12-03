using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSessionListItemScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI createdByText;
    [SerializeField]
    private TextMeshProUGUI nPlayersText;


    public void SetFields(Lobby.GameSession gameSession)
    {
        createdByText.text = $"CreatedBy: {gameSession.createdBy}";
        nPlayersText.text = $"Players: {gameSession.players.Count}";

        if (gameSession.players.Contains(Lobby.myUsername))
        {
            Image image = GetComponent<Image>();
            image.color = new Color(89f / 255f, 231f / 255f, 230f / 255f, 74f / 255f);
   
        }
        // EE6464
    }
}
