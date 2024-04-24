using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class AITimer : AITask
{
    protected float timer = 0.0f;
    protected float maxTime;

    public AITimer() : this(0, 1.0f)
    { 
    }

    public AITimer(int priority, float maxTime) : base(priority)
    {
        this.maxTime = maxTime;
    }

    public void increment(float dt)
    {
        timer += dt;
        isComplete = timer >= maxTime;
        if (isComplete) succeeded = true;
    }

    public override bool Abort()
    {
        return isComplete;
    }

    public override void Update(float dt, BaseNPC npc)
    {
        increment(dt);
    }

    public override void Reset()
    {
        timer = 0.0f;
        isComplete = false;
    }
}

public abstract class AITask
{
    public int priority {  get; set; }
    public bool isComplete = false;
    public bool succeeded = false;
    public bool failed = false;
    public bool paused = false;

    public AITask(int priority)
    {
        this.priority = priority;
        isComplete = false;
    }

    public virtual bool Abort()
    {
        return false;
    }

    public virtual void Update(float deltaTime, BaseNPC npc)
    {
        if (isComplete || paused) return;
    }

    public virtual void Reset()
    {
        isComplete = false;
        succeeded = false;
        failed = false;
    }

    public virtual void Complete()
    {
        isComplete = true;
    }
}

public class Pathing : AIRepeatedTaskGrouping
{
    public SplineContainer spline;
    private Vector3 lastPathPosition;

    public Pathing(int priority, SplineContainer spline) : base(priority)
    {
        this.spline = spline;
        for (int i = 0; i < spline[0].Count; i++)
        {
            Vector3 newVector = spline.transform.rotation * (Vector3)spline[0][i].Position + spline.transform.position;
            AITask task = new TravelTo(0, newVector);
            
            //Debug.DrawLine(newVector, newVector + Vector3.up * 10, Color.green, 1000.0f);
            tasks.Add(task);
        }
    }

    public override void UpdateTasks(float deltaTime, BaseNPC npc)
    {
        base.UpdateTasks(deltaTime, npc);
        if (paused) npc.agent.SetDestination(npc.transform.position);
    }
}

public class TravelTo : AITask
{
    public Vector3 position { get; set; }
    public TravelTo(int priority, Vector3 position) : base(priority)
    {
        this.position = position;
    }

    public override void Update(float deltaTime, BaseNPC npc)
    {
        base.Update(deltaTime, npc);
        npc.TravelTo(position);
        if (npc.IsAtDestination()) isComplete = true;
    }
}

public class TravelToGameObject : AITask
{
    public Transform gameObjectTransform { get; set; }
    public TravelToGameObject(int priority, Transform gameObjectTransform) : base(priority)
    {
        this.gameObjectTransform = gameObjectTransform;
    }

    public override void Update(float deltaTime, BaseNPC npc)
    {
        base.Update(deltaTime, npc);
        npc.TravelTo(gameObjectTransform.position);
        if (npc.IsAtDestination())
        {
            isComplete = true;
            succeeded = true;
        }
    }
}

// Pickup Object
// Question
public class Question : AIParallelTaskGrouping
{
    public Vector3 lookAtPosition { get; set; }
    public Question(int priority) : base(priority)
    {
    }

    public override void Update(float deltaTime, BaseNPC npc)
    {
        if (isComplete)
        {
            npc.isMoving = true;
        }
        base.Update(deltaTime, npc);

        if (!isComplete)
        {
            npc.isMoving = false;
            npc.agent.SetDestination(npc.transform.position);
            npc.LookAt(lookAtPosition);
        }
    }
}

// Interact

public class Interact : AIParallelTaskGrouping
{
    public Interact(int priority) : base(priority)
    {
    }

    public override void Update(float deltaTime, BaseNPC npc)
    {
        base.Update(deltaTime, npc);
    }

    public override void Reset()
    {
        base.Reset();
    }
}

public class ChasePlayer : AIParallelTaskGrouping
{
    public Transform playerTransform { get; set; }
    public ChasePlayer(int priority, Transform player) : base(priority)
    {
        TravelToGameObject playerObjectTask = new TravelToGameObject(priority, player);
        tasks.Add(playerObjectTask);
    }

    public override void Update(float deltaTime, BaseNPC npc)
    {
        base.Update(deltaTime, npc);
    }

    public override void Reset()
    {
        base.Reset();
    }
}

public class AITaskGrouping : AITask
{
    protected List<AITask> tasks;
    public int Count { get { return tasks.Count; } }

    public AITaskGrouping(int priority) : base(priority)
    {
        tasks = new();
    }

    public void AddTask(AITask task)
    {
        //tasks.Append(task);
        bool added = false;
        for (int i = 0; i < tasks.Count; i++)
        {
            AITask t = tasks[i];
            if (task.priority >= t.priority)
            {
                tasks.Insert(i, task);
                added = true;
                break;
            }
        }

        if (!added)
        {
            tasks.Add(task);
        }
    }

    public override void Update(float deltaTime, BaseNPC npc)
    {
        UpdateTasks(deltaTime, npc);
    }

    public virtual void UpdateTasks(float deltaTime, BaseNPC npc)
    {
        if (tasks.Count == 0 || paused)
            return;

        if (tasks.First().isComplete)
        {
            tasks.RemoveAt(0);
            if (tasks.Count == 0)
                return;
        }

        tasks.First().Update(deltaTime, npc);
    }

    public override void Reset()
    {
        base.Reset();
        for (int i = 0; i < tasks.Count; i++)
            tasks[i].Reset();
    }
}

public class AIParallelTaskGrouping : AITaskGrouping
{
    public AIParallelTaskGrouping(int priority) : base(priority)
    {
    }

    public override void UpdateTasks(float deltaTime, BaseNPC npc)
    {
        if (isComplete || paused) return;
        if (tasks.Count == 0)
        {
            isComplete = true;
            succeeded = true;
            return;
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            AITask task = tasks[i];
            if (task.Abort() || task.failed)
            {
                isComplete = true;
                failed = true;
                return;
            }

            if (task.isComplete && task.succeeded)
            {
                isComplete = true;
                succeeded = true;
                return;
            }

            if (task.isComplete)
            {
                tasks.RemoveAt(i);
                i--;
                continue;
            }
            task.Update(deltaTime, npc);
        }
    }
}

public class AIRepeatedTaskGrouping : AITaskGrouping
{
    private int index = 0;

    public AIRepeatedTaskGrouping(int priority) : base(priority)
    {
        
    }

    public override void UpdateTasks(float deltaTime, BaseNPC npc)
    {
        if (tasks.Count == 0 || paused)
            return;

        if (tasks[index].isComplete)
        {
            tasks[index].isComplete = false;
            index = (index + 1) % tasks.Count;
        }

        tasks[index].Update(deltaTime, npc);
    }

    public override void Reset()
    {
        base.Reset();
        index = 0;
    }
}

public class AIInvestigateTask : AITaskGrouping
{
    public AIInvestigateTask(int priority, Transform gameObjectTransform) : base(priority)
    {
        AITask travel = new TravelToGameObject(priority, gameObjectTransform);
        tasks.Add(travel);
    }
}