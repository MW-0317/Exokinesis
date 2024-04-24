using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimatorParameter : PlayableBehaviour
{
    public Animator animator;
    public float speed;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        if (animator != null)
        {
            // Optionally reset the parameter if needed
            animator.SetFloat("Speed", 0);
        }
    }
}
