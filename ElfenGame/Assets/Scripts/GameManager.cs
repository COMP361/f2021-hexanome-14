using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public const float dragZ = -5;
    public const float gridItemRelativeZ = -1;

    public const float pathColoringAlpha = 0.35f;
    public const float pathColoringHoverAlpha = 0.8f;

    public static GameManager _instance;

    [SerializeField]
    public MouseActivityManager mouseActivityManager;

    [SerializeField]
    public Camera mainCamera;

    [SerializeField]
    public GameObject roadGroup;

    [SerializeField]
    public GameObject townGroup;


    public Dictionary<string, NewTown> townDict = new Dictionary<string, NewTown>();


    void Awake()
    {

        foreach (NewTown town in townGroup.GetComponentsInChildren<NewTown>())
        {
            townDict.Add(town.name, town);
        }

        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        } else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }


}