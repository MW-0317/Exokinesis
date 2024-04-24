using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrowdAI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GameObject target;
    public GameObject[] allTargets;
    
    // Start is called before the first frame update
    void Start()
    {
        FindTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && Vector3.Distance(this.transform.position, target.transform.position) < 0.5f)
        {
            FindTarget();
        }
    }

    public void FindTarget()
    {
        if (target != null) target.transform.tag = "Target";
        
        allTargets = GameObject.FindGameObjectsWithTag("Target");
        target = allTargets[Random.Range(0, allTargets.Length)];
        // target.transform.tag = "Untagged";
        
        navMeshAgent.destination = target.transform.position;
    }
}
