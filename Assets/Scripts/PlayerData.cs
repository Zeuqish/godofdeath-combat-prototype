using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    public Dictionary<string, bool> playerFlags {private set; get;}


    void Awake()
    {
        if (instance == null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void setPlayerFlag(string flag, bool value)
    {
        playerFlags.Add(flag, value);
    }

}
