using UnityEngine;
using TMPro;

public class MessageIndicatorScript : MonoBehaviour
{

    [SerializeField] private GameObject messageIndicator;
    [SerializeField] private TextMeshProUGUI txt;

    // Start is called before the first frame update
    public void UpdateNumMessages(int numMessages)
    {
        if (numMessages > 0)
        {
            messageIndicator.SetActive(true);
            if (numMessages > 9)
            {
                txt.text = "9+";
            }
            else
            {
                txt.text = numMessages.ToString();
            }
        }
        else
        {
            messageIndicator.SetActive(false);
        }
    }
}
