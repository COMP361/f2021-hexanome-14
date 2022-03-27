using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageIndicatorScript : MonoBehaviour
{

    [SerializeField] private Canvas messageIndicator;
    [SerializeField] private TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ChatManager.manager != null && !ChatManager.manager.isActive() && ChatManager.manager.newMessage()!=0)
        {
            if (ChatManager.manager.newMessage() < 9)
            {
                txt.text = "" + ChatManager.manager.newMessage();
            }
            else
            {
                txt.text = "9+";
            }
            messageIndicator.gameObject.SetActive(true);
        }

    }
}
