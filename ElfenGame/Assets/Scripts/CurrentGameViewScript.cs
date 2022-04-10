using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentGameViewScript : MonoBehaviour
{
    public TextMeshProUGUI sessionText, createdByText, playersText, saveIDText;

    public void SetSession(Lobby.GameSession session)
    {
        sessionText.text = $"Session Id: {session.session_ID}";
        createdByText.text = $"Created by: {session.createdBy}";
        playersText.text = $"Players: {String.Join(", ", session.players)}";
        saveIDText.text = $"Save ID: {session.saveID}";
    }

}
