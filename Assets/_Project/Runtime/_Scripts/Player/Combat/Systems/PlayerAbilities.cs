using System.Collections;
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

    public static void Heal(int amount)
    {
        // Heal this player.
    }

    // any other ability methods here
}
