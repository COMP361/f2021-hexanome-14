using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    public GameObject playerPrefab;

    [SerializeField]
    public GameObject leftPane;

    public void addPlayer()
    {
        GameObject player = Instantiate(playerPrefab, leftPane.transform);
        //player.transform.SetParent(leftPane.transform, worldPositionStays: false);
    }

}
