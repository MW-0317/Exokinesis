using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ObjectGrabbable : MonoBehaviour
{
    public Vector3 roomMinBoundary;
    public Vector3 roomMaxBoundary;
    
    private float _originalFOV;
    private Rigidbody _objectRigidbody;
    private Transform _objectGrabPointTransform;
    private Dictionary<int, int> _pickedUpObjects = new();
    [SerializeField] private int xpToGive;
    [SerializeField] private float newFOV = 22.0f;
    [SerializeField] private float zoomDuration = 0.5f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private CinemachineVirtualCamera playerFollowCamera;

    private void Awake()
    {
        _objectRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (playerFollowCamera != null)
        {
            _originalFOV = playerFollowCamera.m_Lens.FieldOfView;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 center = (roomMinBoundary + roomMaxBoundary) / 2;
        Vector3 size = roomMaxBoundary - roomMinBoundary;
        
        Gizmos.DrawWireCube(center, size);
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this._objectGrabPointTransform = objectGrabPointTransform;
        _objectRigidbody.useGravity = false;
        _objectRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        StartCoroutine(ChangeFOV(newFOV, zoomDuration));
    }

    public void Drop()
    {
        this._objectGrabPointTransform = null;
        _objectRigidbody.useGravity = true;
        _objectRigidbody.constraints = RigidbodyConstraints.None;
        
        StartCoroutine(ChangeFOV(_originalFOV, zoomDuration));
    }
    
    public void MoveToPosition(Vector3 newPosition)
    {
        if (_objectGrabPointTransform != null)
        {
            _objectRigidbody.MovePosition(newPosition);
        }
    }
    
    public void AddKinesisXp()
    {
        int objectId = GetInstanceID();
        if (!_pickedUpObjects.ContainsKey(objectId))
        {
            _pickedUpObjects[objectId] = 1; // Mark as picked up once
            LevelManager.Instance.AddXP("Telekinesis", xpToGive);
        }
        else
        {
            int count = _pickedUpObjects[objectId];
            int xpToAdd = Mathf.Max(1, 15 - count); // Gives diminishing returns for each additional pickup
            _pickedUpObjects[objectId] = count + 10;
            LevelManager.Instance.AddXP("Telekinesis", xpToAdd);
        }
    }

    public void PrisonCellOpened()
    {
        StartCoroutine(CellOpened(_originalFOV, zoomDuration));
    }

    private void FixedUpdate()
    {
        if (_objectGrabPointTransform != null)
        {
            Vector3 targetPosition = _objectGrabPointTransform.position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPosition);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, distance, layerMask))
            {
                Vector3 adjustedPosition = hit.point - direction * 0.05f; // Adjust position slightly before the collision point

                _objectRigidbody.MovePosition(adjustedPosition);
                
                // Prevent sticking on walls
                Vector3 pushAwayFromWall = hit.normal * 0.10f; // Adjust as necessary
                _objectRigidbody.MovePosition(adjustedPosition + pushAwayFromWall);
            }
            else
            {
                _objectRigidbody.MovePosition(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f));
            }
        }
    }

    private IEnumerator ChangeFOV(float targetFOV, float duration)
    {
        float time = 0;
        float startFOV = playerFollowCamera.m_Lens.FieldOfView;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            playerFollowCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        playerFollowCamera.m_Lens.FieldOfView = targetFOV;
    }

    private IEnumerator CellOpened(float targetFOV, float duration)
    {
        float time = 0;
        float startFOV = playerFollowCamera.m_Lens.FieldOfView;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            playerFollowCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            yield return null;
        }

        playerFollowCamera.m_Lens.FieldOfView = targetFOV;
        
        Destroy(gameObject);
    }
}
