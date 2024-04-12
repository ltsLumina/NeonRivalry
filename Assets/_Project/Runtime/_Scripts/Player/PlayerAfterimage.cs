using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerAfterimage : MonoBehaviour   
{
    [SerializeField] private bool trailIsActive = false;
    [SerializeField] private float activeTime = .1f;
    [SerializeField] private float meshRefreshRate = 0.01f;
    [SerializeField] private Material afterImageMeshMaterial;
    [SerializeField] private float meshDestroyDelay = .2f;
    [SerializeField] private Transform spawningPosition;

    private ObjectPool dashPool;

    private bool trailHasBeenActivated;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    private PlayerController playerController;
    private void Awake()
    {
        dashPool = GetComponent<ObjectPool>();
        playerController = GetComponent<PlayerController>();  
    }
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
        while(timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if(skinnedMeshRenderers == null)
                skinnedMeshRenderers = GameObject.FindWithTag("CharacterBody").GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject afterImage = new GameObject();
                
                afterImage.transform.SetPositionAndRotation(spawningPosition.position, spawningPosition.rotation);

                MeshRenderer meshRenderer = afterImage.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = afterImage.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                meshFilter.mesh = mesh;
                meshRenderer.material = afterImageMeshMaterial;

                Destroy(afterImage, meshDestroyDelay);
                Destroy(mesh, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }
        playerController.ActivateTrail = false;
        trailIsActive = false;
        trailHasBeenActivated = false;
    }
}
