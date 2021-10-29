using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public const float dragZ = -5;
    public const float gridItemRelativeZ = -1;

    public static GameObject gameManager = GameObject.Find("GameManager");

    public static MouseActivityManager mouseActivityManager = gameManager.GetComponent<MouseActivityManager>();

    public static Camera mainCamera = mouseActivityManager.cam;

    public static GameObject roadGroup = GameObject.Find("Roads");

    //public static List<Elf> elves;

}
