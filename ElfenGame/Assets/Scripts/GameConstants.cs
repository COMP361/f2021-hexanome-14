using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public const float dragZ = -5;
    public const float gridItemRelativeZ = -1;

    public const float pathColoringAlpha = 0.35f;
    public const float pathColoringHoverAlpha = 0.8f;

    public static string lobbyServiceUrl = "http://18.223.185.13:4242";

    private static NetworkManager _networkManager;
    private static MouseActivityManager _mouseActivityManager;
    private static Camera _mainCamera;

    private static LoginUIManager _loginUIManager;
    private static MainMenuUIManager _mainMenuUIManager;
    private static MainUIManager _mainUIManager;

    private static Dictionary<string, NewTown> _townDict;

    private static GameObject _roadGroup;
    private static GameObject _townGroup;



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
    public static Dictionary<string, NewTown> townDict
    {
        get
        {
            if (_townDict == null || _townDict.ContainsValue(null))
            {
                _townDict = new Dictionary<string, NewTown>();
                foreach (NewTown town in townGroup.GetComponentsInChildren<NewTown>())
                {
                    _townDict.Add(town.name, town);
                }
            }
            return _townDict;
        }

        set
        {
            _townDict = value;
        }
}

    public static NetworkManager networkManager
    {
        get
        {
            if (_networkManager == null)
            {
                _networkManager = Object.FindObjectOfType<NetworkManager>();
            }
            return _networkManager;
        }
    }
    public static MouseActivityManager mouseActivityManager
    {
        get
        {
            if (_mouseActivityManager == null)
            {
                _mouseActivityManager = Object.FindObjectOfType<MouseActivityManager>();

            }
            return _mouseActivityManager;
        }
    }
    public static LoginUIManager loginUIManager
    {
        get
        {
            if (_loginUIManager == null)
            {
                _loginUIManager = Object.FindObjectOfType<LoginUIManager>();
            }
            return _loginUIManager;
        }
    }
    public static MainMenuUIManager mainMenuUIManager
    {
        get
        {
            if (_mainMenuUIManager == null)
            {
                _mainMenuUIManager = Object.FindObjectOfType<MainMenuUIManager>();
            }
            return _mainMenuUIManager;
        }
    }
    public static MainUIManager mainUIManager
    {
        get
        {
            if (_mainUIManager == null)
            {
                _mainUIManager = Object.FindObjectOfType<MainUIManager>();
            }
            return _mainUIManager;
        }
    }
}
