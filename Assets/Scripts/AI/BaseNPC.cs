using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[Serializable]
public class Location
{
    public string name;
    public Transform gameObject;
}

public class BaseNPC : MonoBehaviour
{
    public SplineContainer spline;
    public GameObject player;

    private Vector3 lastPos;
    public NavMeshAgent agent;

    public AITaskGrouping tasks;
    private ChasePlayer chaseTask;
    private Question questionTask;
    private Interact interactTask;
    private Pathing pathingTask;

    private bool patrols;
    public TMP_Text questioningUI;
    public TMP_Text chasingUI;

    // Useful for UI
    [Header("Player Detection")]
    public float EyeHeight = 1.65f;
    public float SeeDistance = 15.0f;
    public float Sensitivity = 0.1f;
    public float HearDistance = 3.0f;
    public float seenValue { get; private set; } = 0.0f;
    public float heardValue { get; private set; } = 0.0f;

    [Header("TravelTo Places")]
    public List<Location> locations;

    [Header("Agent Settings")]
    public float wanderingSpeed = 2.0f;
    public float chasingSpeed = 11.0f;

    [Header("AI")]
    public bool AIEnabled = true;
    public bool isPatrolling = true;
    public bool isLookingForPlayer = true;

    private int defaultMask;
    private int canInteractMask;

    private Head head;

    public bool wantsToInteract { get { return !interactTask.isComplete; } }
    public bool isMoving
    {
        get
        {
            return !agent.isStopped;
        }
        set
        {
            agent.isStopped = !value;
        }
    }

    public void EnableAI()
    {
        AIEnabled = true;
        agent.enabled = true;
        isMoving = true;
    }

    public bool TravelTo(Vector3 pos)
    {
        if (lastPos == pos) { return true; }
        return agent.SetDestination(pos);
    }

    public void TravelToLocation(string name)
    {
        Location location = locations.Find(x => x.name.Equals(name));
        if (location.gameObject == null) return;
        tasks.AddTask(new AITimer(0, 30.0f));
        tasks.AddTask(new TravelToGameObject(0, location.gameObject));
    }

    public void HeardAudio()
    {
        if (!chaseTask.isComplete) return;
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (isLookingForPlayer && distance < HearDistance)
            heardValue += Sensitivity * 50;
    }

    public bool IsAtDestination()
    {
        if (agent.destination == null) return false;
        Vector2 position = new Vector2(transform.position.x, transform.position.z);
        Vector2 destination = new Vector2(agent.destination.x, agent.destination.z);
        float distance = Vector2.Distance(position, destination);
        return distance < 1.0f;
    }

    public void LookAt(Vector3 position)
    {
        if (head != null) head.LookAt(position);
    }

    public void UpdateTasks(float deltaTime)
    {
        if (!AIEnabled) return;
        tasks.UpdateTasks(deltaTime, this);
    }

    public bool Interact()
    {
        bool interact = wantsToInteract;
        interactTask.Complete();
        return interact;
    }

    public void AIUpdate()
    {
        agent.isStopped = !AIEnabled || tasks.Count == 0;
        if (!agent.enabled) return;
        if (!AIEnabled)
            return;

        if (spline != null)
            pathingTask.paused = !isPatrolling;

        // Check if player nearby, for sound if needed
        float distance = Vector3.Distance(player.transform.position, transform.position);

        RaycastHit interactHit;
        if (Physics.Raycast(transform.position + transform.up * EyeHeight, transform.forward, out interactHit, SeeDistance, defaultMask + canInteractMask))
        {
            Debug.DrawRay(transform.position + transform.up * EyeHeight, transform.forward * interactHit.distance, Color.green);
            if (interactHit.collider.CompareTag("Door")
                && interactTask.isComplete)
            {
                interactTask.Reset();
                tasks.AddTask(interactTask);
            }
        }
        else
        {
            Debug.DrawRay(transform.position + transform.up * EyeHeight, transform.forward * SeeDistance, Color.yellow);
        }
        bool seenThisFrame = false;
        
            
        if (isLookingForPlayer && CouldSeePlayer() && seenValue <= 1.0f && chaseTask.isComplete)
        {
            // if (name == "Warden")
            //     Debug.Log("HERE (for no reason whatsoever)");
            RaycastHit hitT;
            Vector3 eyeHeight = new Vector3(0.0f, EyeHeight, 0.0f);
            Vector3 diffVector = (player.transform.position - transform.position - eyeHeight / 2).normalized;
            if (Physics.Raycast(transform.position + eyeHeight, diffVector, out hitT, SeeDistance, defaultMask + canInteractMask)
                && hitT.transform.tag == "Player")
            {
                questionTask.lookAtPosition = hitT.transform.position;
                seenValue += Sensitivity * Time.deltaTime * 100;
                Debug.DrawRay(transform.position + eyeHeight, diffVector * hitT.distance, Color.green);
                seenThisFrame = true;
            }
            else
            {
                Debug.DrawRay(transform.position + eyeHeight, diffVector * SeeDistance, Color.yellow);
            }
        }

        if (!seenThisFrame)
            seenValue = 0.0f;
        // Debug.Log(seenValue);
        if (seenValue > 1.0f || heardValue > 1.0f)
        {
            GameManager.Instance.guardChasing = true;
            if (chasingUI != null) chasingUI.enabled = true;
            if (questioningUI != null) questioningUI.enabled = false;
            agent.speed = chasingSpeed;

            questionTask.isComplete = true;
            questionTask.succeeded = true;
            chaseTask.Reset();
            tasks.AddTask(chaseTask);
            seenValue = 0.0f;
            heardValue = 0.0f;
        }
        else if ((seenValue > 0.1f || heardValue > 0.1f) && questionTask.isComplete)
        {
            if (chasingUI != null) chasingUI.enabled = false;
            if (questioningUI != null) questioningUI.enabled = true;
            
            questionTask.Reset();
            tasks.AddTask(questionTask);

            StartCoroutine(Questioning());
        }
        if (chaseTask.succeeded)
        {
            // End Game Event
            GameManager.Instance.PrepareForRespawn();
            NewGame.Instance.shouldReload = true;
            GameManager.Instance.GameOverReset(); // Lose progress
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
        if (chaseTask.failed)
        {
            // Got Away Event
            if (chasingUI != null) chasingUI.enabled = false;
            agent.speed = wanderingSpeed;
            GameManager.Instance.guardChasing = false;
        }
    }

    public bool CouldSeePlayer()
    {
        Vector3 a = transform.forward;
        Vector3 b = (player.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(a, b);

        return dot >= 0.5;
    }

    private bool findHead(GameObject gameObject)
    {
        foreach (Transform headT in gameObject.GetComponentInChildren<Transform>())
        {
            head = headT.gameObject.GetComponent<Head>();
            if (head != null) return true;
            if (findHead(headT.gameObject)) return true;
        }
        return false;
    }

    private IEnumerator Questioning()
    {
        float startTime = Time.time;

        while (Time.time - startTime < 5f)
        {
            if (seenValue > 0.1f && heardValue > 0.1f)
            {
                yield break; // Stop the coroutine if seen/heard
            }
            yield return null;
        }

        // Passed
        if (questioningUI != null) questioningUI.enabled = false;
        questionTask.isComplete = true;
        if (spline != null && pathingTask != null && patrols)
        {
            isPatrolling = true;
            pathingTask.paused = false;
        }
        seenValue = 0f;
        heardValue = 0f;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPos = transform.position;
        agent.speed = wanderingSpeed;

        // if (!findHead(this.gameObject))
        //     Debug.Log("Head for NPC not found");

        tasks = new(0);

        chaseTask = new(1, player.transform);
        chaseTask.AddTask(new AITimer(0, 10.0f));
        chaseTask.isComplete = true;

        interactTask = new(9);
        interactTask.AddTask(new AITimer(0, 1.0f));
        interactTask.isComplete = true;

        questionTask = new(8);
        questionTask.AddTask(new AITimer(0, 3.0f));
        questionTask.isComplete = true;

        if (spline != null)
        {
            pathingTask = new Pathing(0, spline);
            tasks.AddTask(pathingTask);
        }

        defaultMask = LayerMask.GetMask("Default");
        canInteractMask = LayerMask.GetMask("canInteract");

        patrols = isPatrolling;
    }

    private void Update()
    {
    }
}
