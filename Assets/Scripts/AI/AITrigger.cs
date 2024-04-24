using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITrigger : MonoBehaviour
{
    public BaseNPC npc;
    public DialogueQueue queue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !queue.allowedPassage)
            npc.isLookingForPlayer = true;
    }

    public void StartChasing()
    {
        npc.isLookingForPlayer = true;
    }
}
