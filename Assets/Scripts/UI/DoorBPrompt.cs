using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorBPrompt : MonoBehaviour
{
    public GameObject openCanvas;
    public GameObject lockedCanvas;
    public AutoDoor autoDoor;
    public TMP_Text securityText;
    public bool level1 = false;
    public bool level2 = false;
    public bool level3 = false;
    public bool manualControl = false;
    public bool stayLocked = false;
    public bool broken = false;
    
    private bool playerInTrigger = false;

    private void Start()
    {
        UpdateLevelText();
    }

    private void Update()
    {
        if (autoDoor.isLevel3 && !GameManager.Instance.hasKeycard1)
        {
            lockedCanvas.SetActive(true);
        }
        if (autoDoor.isLevel2 && !GameManager.Instance.hasKeycard2)
        {
            lockedCanvas.SetActive(true);
        }
        if (autoDoor.isLevel1 && !GameManager.Instance.hasKeycard3)
        {
            lockedCanvas.SetActive(true);
        }
        if (autoDoor.isManualAccess && !GameManager.Instance.puzzleCompleted)
        {
            lockedCanvas.SetActive(true);
        }
        if (autoDoor.alwaysLocked)
        {
            lockedCanvas.SetActive(true);
        }
        if (autoDoor.isBroken)
        {
            lockedCanvas.SetActive(true);
        }
        
        if (playerInTrigger)
        {
            if (!autoDoor.isLevel3 || autoDoor.isLevel3 && GameManager.Instance.hasKeycard1)
            {
                lockedCanvas.SetActive(false);
                openCanvas.SetActive(!autoDoor.isOpen);
            }
            else if (autoDoor.isLevel2 && GameManager.Instance.hasKeycard2)
            {
                lockedCanvas.SetActive(false);
                openCanvas.SetActive(!autoDoor.isOpen);
            }
            else if (autoDoor.isLevel1 && GameManager.Instance.hasKeycard3)
            {
                lockedCanvas.SetActive(false);
                openCanvas.SetActive(!autoDoor.isOpen);
            }
            else if (autoDoor.isManualAccess && GameManager.Instance.puzzleCompleted)
            {
                lockedCanvas.SetActive(false);
                openCanvas.SetActive(!autoDoor.isOpen);
            }
            else if (autoDoor.isBroken)
            {
                openCanvas.SetActive(!autoDoor.isOpen);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (autoDoor.isLevel3 && !GameManager.Instance.hasKeycard1)
        {
            return;
        }
        if (autoDoor.isLevel2 && !GameManager.Instance.hasKeycard2)
        {
            return;
        }
        if (autoDoor.isLevel1 && !GameManager.Instance.hasKeycard3)
        {
            return;
        }
        if (autoDoor.isManualAccess && !GameManager.Instance.puzzleCompleted)
        {
            return;
        }
        if (autoDoor.alwaysLocked)
        {
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            // Immediately adjust canvas visibility
            openCanvas.SetActive(!autoDoor.isOpen);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (autoDoor.isLevel3 && !GameManager.Instance.hasKeycard1)
        {
            return;
        }
        if (autoDoor.isLevel2 && !GameManager.Instance.hasKeycard2)
        {
            return;
        }
        if (autoDoor.isLevel1 && !GameManager.Instance.hasKeycard3)
        {
            return;
        }
        if (autoDoor.isManualAccess && !GameManager.Instance.puzzleCompleted)
        {
            return;
        }
        if (autoDoor.alwaysLocked)
        {
            return;
        }
        
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            openCanvas.SetActive(false);
        }
    }
    
    private void UpdateLevelText()
    {
        if(level1)
        {
            securityText.text = "LEVEL 1";
        }
        else if(level2)
        {
            securityText.text = "LEVEL 2";
        }
        else if(level3)
        {
            securityText.text = "LEVEL 3";
        }
        else if (manualControl)
        {
            securityText.text = "REMOTE";
        }
        else if (stayLocked)
        {
            securityText.text = "RESTRICTED";
        }
        else if (broken)
        {
            securityText.text = "NEEDS REPAIR";
        }
        else
        {
            securityText.text = "";
        }
    }
}
