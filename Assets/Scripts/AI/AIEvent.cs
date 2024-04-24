using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AIEvent
{
    public bool EventOccured { get; protected set; } = false;
    public Vector2 Position;
    public GameObject gameObject;
    public float MaxDistance;
    public float Timer;

    public abstract void Handle(BaseNPC npc);
}

public class AISoundEvent : AIEvent
{
    public AISoundEvent(Vector2 position)
    {
        this.Position = position;
    }

    public override void Handle(BaseNPC npc)
    {
        float distance = Vector3.Distance(npc.transform.position, gameObject.transform.position);
        if (distance > npc.HearDistance) return;
        AIInvestigateTask investigateTask = new(0, this.gameObject.transform);
        npc.tasks.AddTask(investigateTask);
    }
}

public class AIEventManager
{
    public List<AIEvent> events;
    public AIEventManager()
    {
        events = new();
    }

    public void OnSoundEvent(Vector2 position)
    {
        AISoundEvent soundEvent = new AISoundEvent(position);
        events.Add(soundEvent);
    }

    public void Handle(BaseNPC npc)
    {
        for (int i = 0; i < events.Count; i++)
        {
            events[i].Handle(npc);
        }
    }

    public void Clear()
    {
        events.Clear();
    }
}
