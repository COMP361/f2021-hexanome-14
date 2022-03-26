using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskRunner : MonoBehaviour
{
    #region singleton 

    private static TaskRunner _instance;

    public static TaskRunner runner
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TaskRunner>();
            }
            return _instance;
        }
    }

    #endregion
    public List<Action> toRun = new List<Action>();

    public static void runOnMainThread(Action action)
    {
        runner.toRun.Add(action);
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }
    void Update()
    {
        if (toRun.Count > 0)
        {
            foreach (Action a in toRun)
            {
                try
                {
                    a();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            toRun.Clear();
        }
    }
}
