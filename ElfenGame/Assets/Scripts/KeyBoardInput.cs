using UnityEngine;

public class KeyBoardInput : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            if (MainUIManager.manager != null && (!ChatManager.manager || !ChatManager.manager.isActive()))
            {
                MainUIManager.manager.OnPausePressed();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ChatManager.manager != null && ChatManager.manager.isActive())
            {
                ChatManager.manager.SetChatInvisible();
            }
            else if (MainMenuUIManager.manager != null)
            {
                MainMenuUIManager.manager.OnEscapePressed();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ChatManager.manager && ChatManager.manager.isActive())
            {
                ChatManager.manager.OnTabPressed();
            }
            else if (LoginUIManager.manager)
            {
                LoginUIManager.manager.OnTabPressed();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ChatManager.manager != null && ChatManager.manager.isActive())
            {
                ChatManager.manager.OnEnterPressed();
            }
            else if (LoginUIManager.manager != null)
            {
                LoginUIManager.manager.OnEnterPressed();
            }
        }

        if (Input.GetKeyDown("t"))
        {
            if (ChatManager.manager != null && !ChatManager.manager.isActive())
            {
                ChatManager.manager.setChatVisible();
            }
        }

        if (Input.GetKeyDown("`"))
        {
            if (LoginUIManager.manager)
            {
                LoginUIManager.manager.QuickLogin("fynn");
            }

            if (MainUIManager.manager)
            {
                Game.currentGame.nextPlayer(passed: true);
            }
        }

        if (Input.GetKeyDown("="))
        {
            if (LoginUIManager.manager)
            {
                LoginUIManager.manager.QuickLogin("maex");
            }
        }
        if (Input.GetKeyDown("\\"))
        {
            if (LoginUIManager.manager)
            {
                LoginUIManager.manager.QuickLogin("luca");
            }

        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G))
        {
            Lobby.StartGetSessionsTask();
        }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
