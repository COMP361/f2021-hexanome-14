using UnityEngine;
using UnityEngine.UI;

public class VariationButton : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isSelected = false;
    public Button button;
    private Color normalColor;
    public Color selectedColor;

    public void variationClickedHandler()
    {
        if (!isSelected)
        {
            isSelected = true;
            var colors = button.colors;
            normalColor = colors.normalColor;
            colors.normalColor = selectedColor;
            colors.selectedColor = selectedColor;
            colors.highlightedColor = selectedColor;
            colors.pressedColor = selectedColor;
            button.colors = colors;
        }
        else
        {
            isSelected = false;
            var colors = button.colors;
            colors.normalColor = normalColor;
            colors.highlightedColor = normalColor;
            colors.pressedColor = normalColor;
            colors.selectedColor = normalColor;
            button.colors = colors;
        }
    }

}
