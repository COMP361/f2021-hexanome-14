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
            GameConstants.mainUIManager.OnPausePressed();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameConstants.mainMenuUIManager != null)
            {
                GameConstants.mainMenuUIManager.OnEscapePressed();
            }
        }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

}
