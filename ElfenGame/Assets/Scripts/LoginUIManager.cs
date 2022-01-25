using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{

    [SerializeField] InputField usernameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] TextMeshProUGUI statusText;

    // Start is called before the first frame update
    void Start()
    {
        LobbyService.initGameService();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log($"Logging in with username: {username} and password: {password}");

        OnAttemptToLogin();

        await Lobby.AuthenticateAsync(usernameInput.text, passwordInput.text);
        //Lobby.GetSessions();
    }

    public void OnTabPressed()
    {
        if (usernameInput.isFocused)
        {
            passwordInput.Select();
        } else
        {
            usernameInput.Select();
        }
    }

    public void OnEnterPressed()
    {
        OnLoginClicked();
    }

    public void OnAttemptToLogin()
    {
        statusText.text = "Attempting to Login";
        statusText.color = GameConstants.green;
    }

    public void OnLoginFailed()
    {
        statusText.text = "Login Failed. Please Try Again";
        statusText.color = GameConstants.red;
    }
}
