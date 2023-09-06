#region
using System;
using UnityEngine;
#endregion

public class GameManager : MonoBehaviour
{
    const int TARGET_FPS = 60;
    
    void Awake()
    {
        Application.targetFrameRate = TARGET_FPS; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManagerExtended.ReloadScene();
        }
    }
}
