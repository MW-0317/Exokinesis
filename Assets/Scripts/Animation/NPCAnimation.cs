using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class NPCAnimation : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _agent;

    public float SpeedChangeRate = 10f;

    private const float speedOffset = 0.1f;
    private float _currentSpeed;
    private float _animationBlend;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Use NavMeshAgent's velocity and desiredVelocity to calculate target and current speed
        float targetSpeed = _agent.desiredVelocity.magnitude;
        float currentHorizontalSpeed = new Vector3(_agent.velocity.x, 0.0f, _agent.velocity.z).magnitude;

        // Acceleration/deceleration calculation
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * SpeedChangeRate);
            _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
        }
        else
        {
            _currentSpeed = targetSpeed;
        }
        
        _animationBlend = Mathf.Lerp(_animationBlend, _currentSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // Normalize the speed against the max speed of the agent for the animation parameter
        float normalizedSpeed = (_agent.speed > 0) ? _animationBlend / _agent.speed : 0;
        _animator.SetFloat("Speed", normalizedSpeed);
        
        if (_agent.speed > 2) _animator.SetBool("isChasing", true);
        if (_agent.speed <= 2) _animator.SetBool("isChasing", false);
    }
}