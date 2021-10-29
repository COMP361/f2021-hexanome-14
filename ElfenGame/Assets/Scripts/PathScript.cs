using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour, IDragOver
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDragEnter()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.65f);
    }

    public void OnDragExit()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
}
