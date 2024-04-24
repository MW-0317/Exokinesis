using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleManager : MonoBehaviour
{
    public GameObject pipesHolder;
    public GameObject[] pipes;

    [SerializeField] private int totalPipes = 0;
    [SerializeField] private int correctedPipes = 0;
    [SerializeField] private GameObject firstSelected;

    // Start is called before the first frame update
    void Start()
    {
        totalPipes = pipesHolder.transform.childCount;

        pipes = new GameObject[totalPipes];

        for (int i = 0; i < pipes.Length; i++)
        {
            pipes[i] = pipesHolder.transform.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.isHacking)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void CorrectMove()
    {
        correctedPipes += 1;

        if (correctedPipes == totalPipes)
        {
            GameManager.Instance.puzzleCompleted = true;
        }
    }

    public void IncorrectMove()
    {
        correctedPipes -= 1;
    }
}
