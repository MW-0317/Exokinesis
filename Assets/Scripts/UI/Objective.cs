using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class Objective
{
    public string name;
    public string description;
    public string rewards;
    public bool isCompleted;

    [Header("Events")]
    public UnityEvent objectiveCompleteEvent;

    public Objective(string name, string description, string rewards)
    {
        this.name = name;
        this.description = description;
        this.rewards = rewards;
    }
}
