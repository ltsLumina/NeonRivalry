#region
using System.Collections;
using System.Linq;
using UnityEngine;
#endregion

public class SkinSelector : MonoBehaviour
{
    [SerializeField] Material shelbyOriginalSkin;
    [SerializeField] Material shelbyAlternateSkin;
    [SerializeField] Material shelbyWeaponSkin;
    [SerializeField] Material shelbyWeaponAlternateSkin;
    
    [SerializeField] Material morpheOriginalSkin;
    [SerializeField] Material morpheAlternateSkin;
    [SerializeField] Material morpheWeaponSkin;
    [SerializeField] Material morpheWeaponAlternateSkin;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerManager.Players.Count == 2);
        
        // if there is only one player of each character, set the original skin
        if (PlayerManager.Players[0].Character.CharacterPrefab.name == "Shelby")
        {
            PlayerManager.Players[0].gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = shelbyOriginalSkin;
            GameObject weapon = FindChildWithTag(PlayerManager.Players[0].gameObject, "Weapon");
            if (weapon != null) weapon.GetComponent<SkinnedMeshRenderer>().material = shelbyWeaponSkin;
        }
        else if (PlayerManager.Players[0].Character.CharacterPrefab.name == "Morphe")
        {
            PlayerManager.Players[0].gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = morpheOriginalSkin;
            GameObject weapon = FindChildWithTag(PlayerManager.Players[0].gameObject, "Weapon");
            if (weapon != null) weapon.GetComponent<SkinnedMeshRenderer>().material = morpheWeaponSkin;
        }

        // If there are two players with the same character, assign the alternate skin to the second player
        if (PlayerManager.Players[0].Character.CharacterPrefab.name == "Shelby" && PlayerManager.Players[1].Character.CharacterPrefab.name == "Shelby")
        {
            PlayerManager.Players[1].gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = shelbyAlternateSkin;
            GameObject weapon = FindChildWithTag(PlayerManager.Players[1].gameObject, "Weapon");
            if (weapon != null) weapon.GetComponent<SkinnedMeshRenderer>().material = shelbyWeaponAlternateSkin;
        }
        else if (PlayerManager.Players[0].Character.CharacterPrefab.name == "Morphe" && PlayerManager.Players[1].Character.CharacterPrefab.name == "Morphe")
        {
            PlayerManager.Players[1].gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = morpheAlternateSkin;
            GameObject weapon = FindChildWithTag(PlayerManager.Players[1].gameObject, "Weapon");
            if (weapon != null) weapon.GetComponent<SkinnedMeshRenderer>().material = morpheWeaponAlternateSkin;
        }
    }

    // Method to find a child GameObject with a specific tag
    static GameObject FindChildWithTag(GameObject parent, string tag)
    {
        // Sort out all the skinned mesh renderers in the parent object
        SkinnedMeshRenderer[] skinnedMeshRenderers = parent.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        // Check if any of the skinned mesh renderers have the tag we are looking for
        return (from skinnedMeshRenderer in skinnedMeshRenderers where skinnedMeshRenderer.CompareTag(tag) select skinnedMeshRenderer.gameObject).FirstOrDefault();
    }
}
