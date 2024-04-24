using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{
    [SerializeField] private float _timeUntilBored;
    [SerializeField] private int _numberOfBoredAnimations;
    private bool _isBored;
    private float _idleTime;
    private int _boredAnimation;
    private int _lastBoredAnimation = -1;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool isIdle = animator.GetFloat("Speed") < 0.01f;
        
        if (!_isBored && isIdle)
        {
            _idleTime += Time.deltaTime;

            if (_idleTime > _timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                _isBored = true;
    
                // Choose a new bored animation, ensuring it's not the same as the last one
                do
                {
                    _boredAnimation = Random.Range(1, _numberOfBoredAnimations + 1);
                }
                while (_boredAnimation == _lastBoredAnimation);

                _lastBoredAnimation = _boredAnimation;

                _boredAnimation = _boredAnimation * 2 - 1;
                animator.SetFloat("IdleAnimation", _boredAnimation - 1);
            }
        }
        // If a bored animation is about to end, reset to default idle
        else if (stateInfo.normalizedTime % 1 > 0.98)
        {
            ResetIdle();
        }
        
        else if (!isIdle) ResetIdle();
         
        animator.SetFloat("IdleAnimation", _boredAnimation, 0.5f, Time.deltaTime);
    }

    private void ResetIdle()
    {
        if (_isBored)
        {
            _boredAnimation--;
        }
        
        _isBored = false;
        _idleTime = 0;
    }
}
