using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class AltEndingTrigger : MonoBehaviour
{
    [SerializeField] private GameObject altCutscene;
    private bool _entered = false;
    private PlayerInput _playerInput;
    private GameObject _player;
    private Animator _animator;
    private ThirdPersonController _controller;

    private void Awake()
    {
        _playerInput = GameObject.Find("PlayerArmature").GetComponent<PlayerInput>();
        _player = _playerInput.gameObject;
        _animator = _playerInput.gameObject.GetComponent<Animator>();
        _controller = _playerInput.gameObject.GetComponent<ThirdPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_entered)
        {
            _playerInput.SwitchCurrentActionMap("Cutscene");
            StartCoroutine(StartCutscene());
            
            GameManager.Instance.altEnding = true; // Enable second ending
            _entered = true;
        }
    }

    private IEnumerator StartCutscene()
    {
        if (_animator.GetBool("Crouch"))
        {
            _controller.Crouching();
            yield return new WaitForSeconds(0.5f);
        }
        
        altCutscene.SetActive(true);
        
        yield return new WaitForSeconds(5f);
        
        _controller.Crouching();
        _player.transform.localPosition = new Vector3(-0.8906f, -1.4901f, 1.6584f);
        _player.transform.localRotation = Quaternion.Euler(0f, 499.93f, 0f);

        yield return new WaitForSeconds(16f); // Duration of cutscene (minus 5)
        
        _playerInput.SwitchCurrentActionMap("Player");
    }
}
