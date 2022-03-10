using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisibleTokenDisplay : MonoBehaviour
{
    [SerializeField]
    public Text nameText, numHiddenText;

    [SerializeField]
    public GameObject tileGroup;

    [SerializeField]
    public GameObject tileHolderPrefab;

    public void SetName(string username)
    {
        nameText.text = $"{username}'s tokens";
    }

    public void SetVisible(List<MovementTile> visibleTiles)
    {
        foreach (TileHolderScript thscript in tileGroup.GetComponentsInChildren<TileHolderScript>())
        {
            Destroy(thscript.gameObject);
	    }

        foreach (MovementTile mt in visibleTiles)
        {
            GameObject go = Instantiate(tileHolderPrefab, tileGroup.transform);
            TileHolderScript thscript = go.GetComponent<TileHolderScript>();
            thscript.SetIsSelectable(false);
            if (GameConstants.mainUIManager && GameConstants.mainUIManager.mTileDict != null && GameConstants.mainUIManager.mTileDict.ContainsKey(mt))
                thscript.SetTile(GameConstants.mainUIManager.mTileDict[mt]);
	    }
    }

    public void SetNumHidden(int numHidden)
    {
        numHiddenText.text = numHidden.ToString();
    }

    public void closeWindow()
    {
        gameObject.SetActive(false);
    }

    public void openWindow()
    {
        gameObject.SetActive(true);    
    }

}
