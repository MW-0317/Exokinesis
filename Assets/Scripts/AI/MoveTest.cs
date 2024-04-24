using UnityEngine;
using UnityEngine.AI;

public class MoveTest : MonoBehaviour
{
    public Transform goal;
    private Vector3 lastPos;
    NavMeshAgent agent;

    public bool TravelTo(Vector3 pos)
    {
        return agent.SetDestination(pos);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPos = goal.position;
        agent.destination = goal.position;
    }

    private void Update()
    {
        // Currently just follows goal transform
        if (lastPos != goal.position)
        {
            lastPos = goal.position;
            TravelTo(lastPos);
        }
    }
}
