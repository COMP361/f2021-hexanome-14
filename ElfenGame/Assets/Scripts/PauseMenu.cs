using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{

    private bool isPaused = false;
    [SerializeField] public GameObject pauseMenuUI;


    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown("q"))
    //    {
    //        isPaused = !isPaused;
    //    }
    //    pauseMenuUI.SetActive(isPaused);
    //}


    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            // if pause menu is on
            if (isPaused)
            {
                //then resume game
                Resume();
            }
            // currently playing game
            else
            {
                //pause
                Pause();
            }
            
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        SceneLoader sceneLoader = new SceneLoader();
        sceneLoader.LoadScene("MainMenu");
        Debug.Log("Loading MainMenu");
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }



}



