using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBoardInput : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            if (GameConstants.mainUIManager != null)
            {
                GameConstants.mainUIManager.OnPausePressed();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameConstants.chatManager != null && GameConstants.chatManager.isActive())
            {
                GameConstants.chatManager.SetChatInvisible();
            } else if (GameConstants.mainMenuUIManager != null)
            {
                GameConstants.mainMenuUIManager.OnEscapePressed();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (GameConstants.chatManager != null && GameConstants.chatManager.isActive())
            {
                GameConstants.chatManager.OnTabPressed();
            } else if (GameConstants.loginUIManager != null)
            {
                GameConstants.loginUIManager.OnTabPressed();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (GameConstants.chatManager != null && GameConstants.chatManager.isActive())
            {
                GameConstants.chatManager.OnEnterPressed();
            } else if (GameConstants.loginUIManager != null)
            {
                GameConstants.loginUIManager.OnEnterPressed();
            }
        }

        if (Input.GetKeyDown("t"))
        {
            if (GameConstants.chatManager != null && !GameConstants.chatManager.isActive())
            {
                GameConstants.chatManager.setChatVisible();
            }
        }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
