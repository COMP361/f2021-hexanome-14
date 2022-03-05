using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUIScript : MonoBehaviour
{
    [SerializeField]
    public GameObject cardObject;

    public void OnClickCard(Card c)
    {
        Debug.Log("Hello");
    }
}
