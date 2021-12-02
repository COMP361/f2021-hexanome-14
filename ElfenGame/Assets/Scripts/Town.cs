using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Town : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private float alphaHitThreshold = 0.1f;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("on Drop");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("onPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("onPointerExit");
    }


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaHitThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
