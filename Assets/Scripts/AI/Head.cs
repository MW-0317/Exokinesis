using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public void LookAt(Vector3 position)
    {
        transform.LookAt(position);
    }
}
