using UnityEngine;

public class GameObjectFactory : IFactory<GameObject>
{
    public override GameObject Create()
    {
        GameObject go = new GameObject();
        return go;
    }
}