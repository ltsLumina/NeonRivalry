using UnityEngine;

public class PrefabFactory<T> : IFactory<T> where T : MonoBehaviour
{
    private readonly GameObject prefab;

    public PrefabFactory(GameObject prefab) 
    {
        this.prefab = prefab;
    }

    public override T Create()
    {
        GameObject prefabInstance = GameObject.Instantiate(prefab);
        return prefabInstance.GetComponent<T>();
    }
}