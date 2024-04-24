using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;


public class SaveCheckpoint : MonoBehaviour
{
    [SerializeField] private ObjectivesManager objectives;
    [SerializeField] private UI_SkillTree skillTree;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveManager.Instance.SavePlayerTransform(other.transform);
            objectives.SaveObjectives();
        }
    }
}
