using System.Collections;
using UnityEngine;

//TODO: Psuedo-code for a class that holds all the possible player abilities.
//      This class will be used by the PlayerController class.
public class PlayerAbilities : MonoBehaviour
{
    public void Teleport(float distance, PlayerController target)
    {
        Vector3 newPos = target.transform.position + new Vector3(distance, 0, 0);
        target.transform.position = newPos;
    }

    public void Stun(float duration, PlayerController target)
    {
        // Apply a stun to target player, lasting for the specified duration.
        StartCoroutine(StunRoutine(target));
    }

    IEnumerator StunRoutine(PlayerController target)
    {
        // Save a reference to the Rigidbody and the current constraints.
        var targetRB              = target.GetComponent<Rigidbody>();
        var constraintsBeforeStun = targetRB.constraints;
        
        // Apply the new constraints.
        targetRB.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(1.5f);

        targetRB.constraints = constraintsBeforeStun;
        
        // Player is back to normal.
    }

    public void Heal(int amount)
    {
        // Heal this player.
    }

    // any other ability methods here
}
