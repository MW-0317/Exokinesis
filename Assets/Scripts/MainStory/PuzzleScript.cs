using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScript : MonoBehaviour
{
    public float[] correctRotations;
    
    [SerializeField] private bool isPlaced = false;
    
    private float[] _rotations = { 0, 90, 180, 270 };
    private int _possibleRotations = 1;
    
    private PuzzleManager _puzzleManager;

    private void Awake()
    {
        _puzzleManager = GameObject.Find("Canvas").GetComponent<PuzzleManager>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _possibleRotations = correctRotations.Length;
        int rand = Random.Range(0, _rotations.Length);
        transform.eulerAngles = new Vector3(0, 0, _rotations[rand]);

        if (_possibleRotations > 1)
        {
            if (Mathf.RoundToInt(transform.eulerAngles.z) == Mathf.RoundToInt(correctRotations[0]) || Mathf.RoundToInt(transform.eulerAngles.z) == Mathf.RoundToInt(correctRotations[1]))
            {
                isPlaced = true;
                _puzzleManager.CorrectMove();
            }
        }
        else
        {
            if (Mathf.RoundToInt(transform.eulerAngles.z) == Mathf.RoundToInt(correctRotations[0]))
            {
                isPlaced = true;
                _puzzleManager.CorrectMove();
            }
        }
    }

    public void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        
        if (_possibleRotations > 1)
        {
            if (Mathf.RoundToInt(transform.eulerAngles.z) == Mathf.RoundToInt(correctRotations[0]) || Mathf.RoundToInt(transform.eulerAngles.z) == Mathf.RoundToInt(correctRotations[1]) && isPlaced == false)
            {
                isPlaced = true;
                _puzzleManager.CorrectMove();
            }
            else if (isPlaced)
            {
                isPlaced = false;
                _puzzleManager.IncorrectMove();
            }
        }
        else
        {
            if (Mathf.RoundToInt(transform.eulerAngles.z) == Mathf.RoundToInt(correctRotations[0]) && isPlaced == false)
            {
                isPlaced = true;
                _puzzleManager.CorrectMove();
            }
            else if (isPlaced)
            {
                isPlaced = false;
                _puzzleManager.IncorrectMove();
            }
        }
    }
}
