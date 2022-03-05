using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Card : MonoBehaviour
{
    public CardEnum cardType;

    public bool selected;

    public Card(CardEnum e){
        cardType = e;
        selected = false;

    }

    public void OnClickCard(GameObject go ){
        selected = !selected;
        Debug.Log("Clicked, now it is : " + selected);
        if (selected){
            go.transform.GetChild(0).gameObject.SetActive(false);
            go.transform.GetChild(1).gameObject.SetActive(true);
        } else {
            go.transform.GetChild(0).gameObject.SetActive(true);
            go.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

}