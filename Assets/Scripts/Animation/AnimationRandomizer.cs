using UnityEngine;

public class AnimationRandomizer : MonoBehaviour
{
    private void Start()
    {
        Randomize();
    }

    private void Randomize()
    {
        GameObject[] chattingNPCs = GameObject.FindGameObjectsWithTag("Chatting");
        
        foreach (GameObject npc in chattingNPCs)
        {
            Animator animator = npc.GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float randomStartTime = Random.Range(0f, 1f); // Generate a random start time

                // Play the animation from a random start time
                animator.Play(stateInfo.fullPathHash, -1, randomStartTime);
                animator.Update(0f);
            }
        }
    }
}