using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public const float dragZ = -5;
    public const float gridItemRelativeZ = -1;

    public const float pathColoringAlpha = 0.35f;
    public const float pathColoringHoverAlpha = 0.8f;

    public static Color greenFaded = new Color(102f / 255f, 236f / 255f, 77f / 255f, 74f / 255f);
    public static Color redFaded = new Color(238f / 255f, 100f / 255f, 100f / 255f, 74f / 255f);
    public static Color blueFaded = new Color(89f / 255f, 231f / 255f, 230f / 255f, 74f / 255f);
    public static Color greyFaded = new Color(107f / 255f, 107f / 255f, 107f / 255f, 74f / 255f);

    public static Color green = new Color(102f / 255f, 236f / 255f, 77f / 255f);
    public static Color red = new Color(238f / 255f, 100f / 255f, 100f / 255f);
    public static Color blue = new Color(89f / 255f, 231f / 255f, 230f / 255f);

    public static Color white = new Color(1.0f, 1.0f, 1.0f);
    public static Color grey = new Color(107f / 255f, 107f / 255f, 107f / 255f);

    public static string lobbyServiceUrl = "http://18.223.185.13:4242";

    private static MouseActivityManager _mouseActivityManager;
    private static Camera _mainCamera;


    private static Dictionary<string, NewTown> _townDict;
    private static Dictionary<string, PathScript> _roadDict;

    private static List<string> _townNames;

    private static GameObject _roadGroup;
    private static GameObject _townGroup;
    private static GameObject _tileGroup;



    public static Camera mainCamera
    {
        get
        {
            if (_mainCamera == null)
            {
                _mainCamera = Object.FindObjectOfType<Camera>();
            }
            return _mainCamera;
        }
    }
    public static GameObject roadGroup
    {
        get
        {
            if (_roadGroup == null)
            {
                _roadGroup = GameObject.Find("Roads");
            }
            return _roadGroup;
        }
    }
    public static GameObject townGroup
    {
        get
        {
            if (_townGroup == null)
            {
                _townGroup = GameObject.Find("Towns");
            }
            return _townGroup;
        }
    }
    public static GameObject tileGroup
    {
        get
        {
            if (_tileGroup == null)
            {
                _tileGroup = GameObject.Find("TileArea");
            }
            return _tileGroup;
        }
    }

    public static List<string> townNames
    {
        get
        {
            if (_townNames == null)
            {
                _townNames = new List<string>();
                TextAsset textAsset = Resources.Load("TownList") as TextAsset;
                string[] lines = textAsset.text.Split('\n');
                foreach (string line in lines)
                {
                    _townNames.Add(line);
                }
            }
            return _townNames;
        }
    }
    public static Dictionary<string, NewTown> townDict
    {
        get
        {
            if (_townDict == null || _townDict.ContainsValue(null))
            {
                _townDict = new Dictionary<string, NewTown>();
                if (townGroup != null)
                {
                    foreach (NewTown town in townGroup.GetComponentsInChildren<NewTown>())
                    {
                        _townDict.Add(town.name, town);
                    }
                }
            }
            return _townDict;
        }

        set
        {
            _townDict = value;
        }
    }

    public static Dictionary<string, PathScript> roadDict
    {
        get
        {
            if (_roadDict == null || _roadDict.ContainsValue(null))
            {
                _roadDict = new Dictionary<string, PathScript>();
                if (roadGroup != null)
                {
                    foreach (PathScript road in roadGroup.GetComponentsInChildren<PathScript>())
                    {
                        _roadDict.Add(road.gameObject.name, road);
                    }
                }
            }
            return _roadDict;
        }

        set
        {
            _roadDict = value;
        }
    }
}
