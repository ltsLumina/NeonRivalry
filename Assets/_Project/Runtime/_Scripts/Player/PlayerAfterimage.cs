#region
using System.Collections;
using UnityEngine;
#endregion

public class PlayerAfterimage : MonoBehaviour
{
    [SerializeField] bool trailIsActive;
    [SerializeField] float activeTime = .1f;
    [SerializeField] float meshRefreshRate = 0.01f;
    [SerializeField] Material afterImageMeshMaterial;
    [SerializeField] float meshDestroyDelay = .2f;
    [SerializeField] Transform spawningPosition;

    bool trailHasBeenActivated;
    SkinnedMeshRenderer[] skinnedMeshRenderers;

    PlayerController playerController;

    void Awake() => playerController = GetComponent<PlayerController>();

    // Update is called once per frame
    void Update()
    {
        trailIsActive = playerController.ActivateTrail;

        if (trailIsActive && !trailHasBeenActivated)
        {
            trailHasBeenActivated = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null) skinnedMeshRenderers = GameObject.FindWithTag("CharacterBody").GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                var afterImage = new GameObject($"After Image {i} (Dash)", typeof(MeshRenderer), typeof(MeshFilter));

                afterImage.transform.SetPositionAndRotation(spawningPosition.position, spawningPosition.rotation);

                var meshRenderer = afterImage.GetComponent<MeshRenderer>();
                var meshFilter   = afterImage.GetComponent<MeshFilter>();

                var mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                meshFilter.mesh       = mesh;
                meshRenderer.material = afterImageMeshMaterial;

                Destroy(afterImage, meshDestroyDelay);
                Destroy(mesh, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }

        playerController.ActivateTrail = false;
        trailIsActive                  = false;
        trailHasBeenActivated          = false;
    }
}
