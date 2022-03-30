using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public string helpMessage;
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOver();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit();
    }

    public void OnMouseOver()
    {
        HelpElfManager.elf.DisplayHelpMessage(helpMessage);
    }

    public void OnMouseExit()
    {
        HelpElfManager.elf.HideHelpMessage();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
