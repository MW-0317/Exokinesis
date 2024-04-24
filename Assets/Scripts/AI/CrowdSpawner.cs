using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSpawner : MonoBehaviour
{
    public GameObject[] allNpcs;
    public GameObject npcHolder;
    public GameObject plane;
    public int number;

    // Start is called before the first frame update
    void Start()
    {
        allNpcs = GameObject.FindGameObjectsWithTag("NPC");
        Renderer r = plane.GetComponent<Renderer>();
        //r.bounds.max;
        Debug.Log(r.bounds.min + " " + r.bounds.max);
        for (int i = 0; i < number; i++)
        {
            Vector3 location = new Vector3(
                    Random.Range(r.bounds.min.x, r.bounds.max.x),
                    Random.Range(r.bounds.min.y, r.bounds.max.y),
                    Random.Range(r.bounds.min.z, r.bounds.max.z)
                );
            GameObject npc = allNpcs[Random.Range(0, allNpcs.Length)];
            GameObject npcCopy = Instantiate(npc, location, Quaternion.identity);
            npcCopy.transform.parent = npcHolder.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
