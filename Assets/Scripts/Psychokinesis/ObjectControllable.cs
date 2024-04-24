using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ObjectControllable : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private IntroUI introUI;
    [SerializeField] private LoreManager loreManager;
    public bool HasBeenMoved { get; private set; } = false;
    public bool isKeycard1 = false;
    public bool isKeycard2 = false;
    public bool isKeycard3 = false;
    public bool isHackable = false;
    public bool isMap = false;
    public bool isLore1 = false;
    public bool isLore2 = false;
    public bool isLore3 = false;
    public bool isBossLore = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    public void Move()
    {
        _animator.enabled = true;
        HasBeenMoved = true;
        
        var outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
        
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        var child = gameObject.transform.GetChild(0);
        if (child != null) child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void GetKeycard1()
    {
        _animator.enabled = true;
        HasBeenMoved = true;

        var outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        
        gameObject.SetActive(false);
        
        introUI.Keycard1Obtained();
    }

    public void GetKeycard2()
    {
        _animator.enabled = true;
        HasBeenMoved = true;

        var outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        
        gameObject.SetActive(false);
        
        introUI.Objective2Complete();
    }

    public void GetKeycard3()
    {
        _animator.enabled = true;
        HasBeenMoved = true;

        var outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;

        gameObject.SetActive(false);
        
        introUI.Keycard3Obtained();
    }

    public void StartHacking()
    {
        _animator.enabled = true;
        HasBeenMoved = true;
        
        var outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        
        introUI.ShowHackingMenu();
        GameManager.Instance.isHacking = true;
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        StartCoroutine(StopHacking());
    }

    IEnumerator StopHacking()
    {
        yield return new WaitUntil(() => GameManager.Instance.puzzleCompleted);
        
        gameObject.GetComponent<AudioSource>().enabled = true; // Play "complete" sfx

        yield return new WaitForSeconds(0.5f);
        introUI.CloseHackingMenu();
        GameManager.Instance.isHacking = false;

        yield return new WaitForSeconds(1f);
        LevelManager.Instance.AddXP("Psychokinesis", 50);
        
        introUI.Hacking1Complete();
    }

    public void GetMap1()
    {
        _animator.enabled = true;
        HasBeenMoved = true;
        
        GameManager.Instance.hasMap = true;
        
        var outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        
        gameObject.SetActive(false);
        
        introUI.MapObtained();
    }

    public void ShowLore(int loreNum)
    {
        _animator.enabled = true;
        HasBeenMoved = true;
        
        var outline = GetComponent<Outline>();
        if (outline != null) outline.enabled = false;

        if (loreManager != null)
        {
            loreManager.SetText(loreNum);
            loreManager.Pause();
        }
    }
}