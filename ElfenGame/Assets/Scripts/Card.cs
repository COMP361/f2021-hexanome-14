using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] public Image cardImage;

    private Image background;
    public CardEnum cardType;

    public Toggle toggle;

    public bool selected;

    public void Awake()
    {
        background = GetComponent<Image>();
        selected = false;
        background.color = GameConstants.blue;

        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });

            ToggleGroup group = GetComponentInParent<ToggleGroup>();
            if (group != null)
            {
                toggle.group = group;
            }
        }
    }

    public void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(delegate { ToggleValueChanged(toggle); });
        }
    }

    private void ToggleValueChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            selected = true;
            background.color = GameConstants.red;
        }
        else
        {
            selected = false;
            background.color = GameConstants.blue;
        }
    }

    public void Initialize(CardEnum e)
    {
        cardType = e;

        cardImage.sprite = e.GetSprite();
        
    }


    public void OnClickCard()
    {

        selected = !selected;
        Debug.Log("Clicked, now it is : " + selected);

        if (selected)
        {
            background.color = GameConstants.green;
        }
        else
        {
            background.color = GameConstants.blue;
        }
    }

}