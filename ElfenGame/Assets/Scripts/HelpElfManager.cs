using System;
using UnityEngine;
using UnityEngine.UI;

public class HelpElfManager : MonoBehaviour
{
    #region singleton 

    private static HelpElfManager _instance;

    public static HelpElfManager elf
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HelpElfManager>();
            }
            return _instance;
        }
    }

    #endregion {
    // Start is called before the first frame update

    private Text bubbleText;
    [SerializeField] private GameObject bubble;

    private HelpMessage defaultMessage;


    public string helpMessage
    {
        set
        {
            bubbleText.text = value;
        }
    }

    public void DisplayHelpMessage(string message)
    {
        helpMessage = message;
        bubble.SetActive(true);
    }

    public void HideHelpMessage()
    {
        bubble.SetActive(false);
    }

    public void setDefaultMessage(string newMessage)
    {
        defaultMessage.helpMessage = newMessage;
    }

    void Awake()
    {
        bubbleText = bubble.GetComponentInChildren<Text>();
        defaultMessage = GetComponent<HelpMessage>();
    }


    // Update is called once per frame
    void Update()
    {

    }

    internal void SetSprite(Sprite image)
    {
        GetComponent<Image>().sprite = image;
    }
}
