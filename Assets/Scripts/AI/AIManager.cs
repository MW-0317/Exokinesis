using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    //public Transform testPos;
    public bool AIEnabled = true;
    public AIEventManager aiEvents;

    private bool AudioEvent = false;

    void Start()
    {
        aiEvents = new AIEventManager();
    }

    public void OnAudio(float volume)
    {
        AudioEvent = true;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        foreach (Transform npcT in this.GetComponentInChildren<Transform>())
        {
            BaseNPC npc = npcT.gameObject.GetComponent<BaseNPC>();

            // Updates tasks, runs every tick/frame
            if (npc == null || !npc.agent.enabled) continue;
            if (!AIEnabled)
            {
                npc.agent.isStopped = true;
                continue;
            }
            npc.agent.isStopped = false;
            aiEvents.Handle(npc);
            npc.UpdateTasks(deltaTime); 

            // Updates Behavior Tree, adds tasks
            npc.AIUpdate();

            if (AudioEvent)
                npc.HeardAudio();
        }
        aiEvents.Clear();
        AudioEvent = false;
    }

    public void OnSoundEvent(Vector2 position)
    {
        aiEvents.OnSoundEvent(position);
    }
}
