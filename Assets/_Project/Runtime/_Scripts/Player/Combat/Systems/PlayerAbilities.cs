using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

//TODO: Psuedo-code for a class that holds all the possible player abilities.
//      This class will be used by the PlayerController class.
public class PlayerAbilities
{
    public static void Teleport(Vector2 direction, PlayerController target)
    {
        Vector3 newPos = target.transform.position + new Vector3(direction.x, 0, direction.y);
        target.transform.position = newPos;
        Debug.LogWarning("this does not work");
    }

    public static void Stun(float duration, PlayerController target)
    {
        // Apply a stun to target player, lasting for the specified duration.
        CoroutineHelper.StartCoroutine(StunRoutine(duration, target));
    }
    
    static IEnumerator StunRoutine(float duration, PlayerController target)
    {
        // Save a reference to the Rigidbody and the current constraints.
        var targetRB              = target.GetComponent<Rigidbody>();
        var constraintsBeforeStun = targetRB.constraints;
        
        // Apply the new constraints.
        targetRB.constraints = RigidbodyConstraints.FreezeAll;
        
        yield return new WaitForSeconds(duration);

        targetRB.constraints = constraintsBeforeStun;
        
        // Player is back to normal.
    }
    
    public static void Fade(PlayerController target)
    {
        Transform thisModel  = target.transform.GetChild(0);

        float thisTargetScaleX  = thisModel.localScale.x * -1; // target scale X

        float duration = 0.75f; // duration of the flip animation

        // flip this player if it's grounded and the X scale changes
        if (target.IsGrounded() && Math.Abs(thisModel.localScale.x - thisTargetScaleX) > 0.001f)
        {
            // Flip Animation for this player
            Sequence thisSequence = DOTween.Sequence();
            thisSequence.Join(thisModel.DOScaleZ(0f, duration  / 3)); // scale Z to 0 in the first half of the duration
            thisSequence.Append(thisModel.DOScaleZ(1, duration / 3)); // scale Z back to 1 in the second half of the duration
        }
    }

    public static void Heal(int amount)
    {
        // Heal this player.
    }

    // any other ability methods here
}
