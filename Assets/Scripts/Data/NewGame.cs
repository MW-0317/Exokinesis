using System;
using System.Collections;
using System.Collections.Generic;
using CI.QuickSave;
using UnityEngine;

public class NewGame : MonoBehaviour
{
    public static NewGame Instance;

    public bool isNewGame;
    public bool shouldReload;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
