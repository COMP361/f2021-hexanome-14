using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Card : MonoBehaviour
{
    [SerializeField] public Image cardImage;

    private Image background;
    public CardEnum cardType;

    public bool selected;



    public void Initialize(CardEnum e){
        cardType = e;
        selected = false;
        background = GetComponent<Image>();

        cardImage.sprite = e.GetSprite();
        background.color = GameConstants.blue;
    }


    public void OnClickCard(){

        selected = !selected;
        Debug.Log("Clicked, now it is : " + selected);

        if (selected){
            background.color = GameConstants.green;
        } else {
            background.color = GameConstants.blue;
        }
    }

}