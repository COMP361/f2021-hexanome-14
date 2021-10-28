using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMovementTiles : MonoBehaviour
{

    [SerializeField]
    List<MovementTileSO> mTiles;

    [SerializeField]
    GameObject movementTilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        foreach (MovementTileSO tile in mTiles)
        {
            GameObject newTile = Instantiate(movementTilePrefab, transform);
            newTile.GetComponent<MovementTileScript>().SetTileSO(tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
