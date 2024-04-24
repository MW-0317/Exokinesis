using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level2Start : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GameObject.Find("PlayerArmature").GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Initialize());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.firstLevel2)
        {
            _playerInput.gameObject.GetComponent<IntroUI>().ShowChapter2();
            _playerInput.gameObject.GetComponent<IntroUI>().ShowLvl2Obj1();
            
            GameManager.Instance.firstLevel2 = false;
        }
    }

    private IEnumerator Initialize()
    {
        _playerInput.gameObject.GetComponent<CharacterController>().enabled = false;
        _playerInput.gameObject.transform.position = new Vector3(9.504f, 0.01f, -13.92f);
        _playerInput.gameObject.GetComponent<CharacterController>().enabled = true;

        GameManager.Instance.isLevel2 = true;
        if (GameManager.Instance.firstLevel2) NewGame.Instance.isNewGame = true;

        yield return new WaitForSeconds(0.5f);
        
        _playerInput.SwitchCurrentActionMap("NoInput");
        
        GameManager.Instance.level2Loaded = true;
        GameManager.Instance.puzzleCompleted = false;

        yield return new WaitForSeconds(2f);
        _playerInput.SwitchCurrentActionMap("Player");
        NewGame.Instance.isNewGame = false;
    }
}
