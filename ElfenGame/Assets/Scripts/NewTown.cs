using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTown : MonoBehaviour, IDragOver
{

    public void OnDragEnter()
    {
        //Not a built in method

        //Debug.Log("Dragged Into :" + name);
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
    }

    public void OnDragExit()
    {
        //Not a built in method

        //Debug.Log("Dragged Out of :" + name);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
