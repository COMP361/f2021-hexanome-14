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


    public void SetFields(string createdBy, int nPlayers)
    {
        createdByText.text = $"CreatedBy: {createdBy}";
        nPlayersText.text = $"Players: {nPlayers}";
    }
}
