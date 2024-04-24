using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    private FadingController _fadingController;
    private PlayerInput _playerInput;
    private MusicManager _musicManager;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool level3 = false;
    [SerializeField] private Vector3 level3Position;
    
    private static bool _isInitialized = false;

    private void Awake()
    {
        _fadingController = GameObject.Find("FadingCanvas").GetComponent<FadingController>();
        _playerInput = GameObject.Find("PlayerArmature").GetComponent<PlayerInput>();
        _musicManager = GameObject.Find("PlayerArmature").GetComponent<MusicManager>();
        
        if (!_isInitialized)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_fadingController);

            _isInitialized = true;
        }
    }

    // private void Update()
    // {
    //     if (_playerInput.gameObject.transform.position.y <= -2f)
    //     {
    //         level3Position = new Vector3(9.504f, 0.01f, -13.92f);
    //         _playerInput.SwitchCurrentActionMap("NoInput");
    //         StartCoroutine(Teleport());
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        _playerInput.SwitchCurrentActionMap("NoInput");
        StartCoroutine(!level3 ? Load() : Teleport());
    }

    private IEnumerator Load()
    {
        _fadingController.FadeOut(1.5f);
        _musicManager.FadeMusicOut(4f);
        
        yield return new WaitForSeconds(1.5f);

        // Load the scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator Teleport()
    {
        _fadingController.FadeOut(1.5f);
        _musicManager.FadeMusicOut(4f);
        
        yield return new WaitForSeconds(1.5f);
        
        _playerInput.gameObject.GetComponent<CharacterController>().enabled = false;
        _playerInput.gameObject.transform.position = level3Position;
        _playerInput.gameObject.GetComponent<CharacterController>().enabled = true;

        yield return new WaitForSeconds(1f);
        _fadingController.FadeIn(4f);

        yield return new WaitForSeconds(2f);
        _playerInput.SwitchCurrentActionMap("Player");
    }
}
