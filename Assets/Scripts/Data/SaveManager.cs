using System;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using CI.QuickSave;
using StarterAssets;
using UnityEngine.SceneManagement; // Make sure you have QuickSave imported

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!NewGame.Instance.isNewGame) LoadGameManager();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!NewGame.Instance.isNewGame) LoadPlayerTransform();
        if (NewGame.Instance.isNewGame)
        {
            SavePlayerTransform(transform);
        }
    }

    public void SavePlayerTransform(Transform playerTransform)
    {
        var transformData = new TransformData(playerTransform.position, playerTransform.rotation, playerTransform.localScale);
        var writer = QuickSaveWriter.Create("PlayerData");
        
        writer.Write("TransformData", transformData)
              .Commit();
    }

    public void SaveGameManager()
    {
        var writer = QuickSaveWriter.Create("PlayerData");

        writer.Write("GameManagerSettings", GameManager.Instance.GetAllSettings())
              .Write("Level2", GameManager.Instance.isLevel2) // For main menu
              .Commit();
    }

    public bool LoadPlayerTransform()
    {
        ThirdPersonController playerTransform = FindObjectOfType<ThirdPersonController>();
        
        var reader = QuickSaveReader.Create("PlayerData");
        if (reader.Exists("TransformData") && playerTransform != null)
        {
            TransformData data = reader.Read<TransformData>("TransformData");
            playerTransform.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            playerTransform.transform.rotation = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);
            playerTransform.transform.localScale = new Vector3(data.scale[0], data.scale[1], data.scale[2]);
            return true;
        }
        else
        {
            Debug.LogWarning("No save data found for player transform");
            return false;
        }
    }

    public void LoadGameManager()
    {
        var reader = QuickSaveReader.Create("PlayerData");
        if (reader.Exists("GameManagerSettings"))
        {
            Dictionary<string, object> settings = reader.Read<Dictionary<string, object>>("GameManagerSettings");
            GameManager.Instance.SetAllSettings(settings);
        }

    }
}

[System.Serializable]
public struct TransformData
{
    public float[] position;
    public float[] rotation;
    public float[] scale;

    public TransformData(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.position = new float[] { position.x, position.y, position.z };
        this.rotation = new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
        this.scale = new float[] { scale.x, scale.y, scale.z };
    }
}
