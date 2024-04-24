using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Ending2 : MonoBehaviour
{
    [SerializeField] private GameObject endingCutscene;
    [SerializeField] private FadingController fadingController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private GameObject UI;
    [SerializeField] private Material skyboxMaterial;
    private bool _entered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_entered || !GameManager.Instance.altEnding) return;
        
        playerInput.SwitchCurrentActionMap("Cutscene");
        StartCoroutine(StartCutscene());
        _entered = true;
    }

    IEnumerator StartCutscene()
    {
        fadingController.FadeOut(2f);
        musicManager.FadeMusicOut(4f);

        yield return new WaitForSeconds(2f);
        
        fadingController.FadeIn(2f);
        endingCutscene.GameObject().SetActive(true);

        yield return new WaitForSeconds(8f);
        RenderSettings.skybox = skyboxMaterial;

        yield return new WaitForSeconds(33f);
        
        fadingController.FadeOut(1f);
        UI.SetActive(false);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadSceneAsync("MainMenu");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}