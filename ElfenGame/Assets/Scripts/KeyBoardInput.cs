using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBoardInput : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public GameObject panel;

    // [SerializeField]
    private bool isPaused = false;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            isPaused = !isPaused;
        }

        panel.SetActive(isPaused);
    }


}
