using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!NewGame.Instance.isNewGame)
        {
            Destroy(gameObject);
        }
    }
}
