using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    #region singleton 

    private static LoginUIManager _instance;

    public static LoginUIManager manager
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LoginUIManager>();
            }
            return _instance;
        }
    }

    #endregion   

    [SerializeField] InputField usernameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] TextMeshProUGUI statusText;

    // Start is called before the first frame update
    void Start()
    {
        // General Initialization for whole game
        Lobby.Init();
        Texture2D glove = Resources.Load("glovecursor") as Texture2D;
        Cursor.SetCursor(glove, new Vector2(0, 0), CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log($"Logging in with username: {username} and password: {password}");

        OnAttemptToLogin();

        Lobby.user.AuthenticateUser(usernameInput.text, passwordInput.text);
        //Lobby.GetSessions();
    }

    public void OnTabPressed()
    {
        if (usernameInput.isFocused)
        {
            passwordInput.Select();
        }
        else
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

    public void QuickLogin(string v)
    {
        string password = "abc123_ABC123";
        Debug.Log($"Logging in with username: {v} and password: {password}");

        OnAttemptToLogin();

        Lobby.user.AuthenticateUser(v, password);

    }
}
