using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectiveReached : MonoBehaviour
{
    private IntroUI _ui;
    public int objectiveNum;
    private bool _entered = false;

    private void Awake()
    {
        _ui = GameObject.Find("PlayerArmature").GetComponent<IntroUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_entered) return;
        
        switch (objectiveNum)
        {
            case 3:
                if (!GameManager.Instance.puzzleCompleted) return;
                _ui.Objective3Complete();
                _entered = true;
                break;
            case 4:
                _ui.Objective4Complete();
                _entered = true;
                break;
            case 5:
                _ui.Objective5Complete();
                _entered = true;
                break;
            case 6:
                _ui.Objective6Complete();
                _entered = true;
                break;
        }
    }
}
