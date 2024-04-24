using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public static OutlineManager Instance { get; private set; }

    private Outline _currentOutlinedObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool RequestOutline(Outline outline)
    {
        if (_currentOutlinedObject == null || _currentOutlinedObject == outline)
        {
            _currentOutlinedObject = outline;
            return true;
        }

        return false;
    }

    public void ClearOutline(Outline outline)
    {
        if (_currentOutlinedObject == outline)
        {
            _currentOutlinedObject = null;
        }
    }
}
