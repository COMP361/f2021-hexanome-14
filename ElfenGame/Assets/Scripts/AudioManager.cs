using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    #region singleton 

    private static AudioManager _instance;

    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    public static AudioManager manager
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
            }
            return _instance;
        }
    }

    #endregion   
    public AudioMixer mixer;

    public void SetVolume(float volume)
    {
        mixer.SetFloat("volume", volume);
    }

    public float GetVolume()
    {
        float volume;
        mixer.GetFloat("volume", out volume);
        return volume;
    }
}
