using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsClick : MonoBehaviour
{

    public GameObject game;
    public GameObject game1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OptionClicked()
    {
        game1.SetActive(false);
        game.SetActive(true);
                    
    }
}
