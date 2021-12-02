using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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


    void Awake()
    {
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
