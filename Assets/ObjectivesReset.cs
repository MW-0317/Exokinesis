using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivesReset : MonoBehaviour
{
    [SerializeField] private ObjectivesManager objectivesManager;
    
    // Start is called before the first frame update
    void Start()
    {
        if (NewGame.Instance.isNewGame || GameManager.Instance.isLevel2)
        {
            objectivesManager.ResetObjectives();
            objectivesManager.LoadObjectives();
        }
    }
}
