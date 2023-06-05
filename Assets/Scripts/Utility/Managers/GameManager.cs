using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerController player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManagerExtended.ReloadScene();
        }
    }
}
