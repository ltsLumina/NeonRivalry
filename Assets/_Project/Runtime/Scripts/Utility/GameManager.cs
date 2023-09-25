#region
using UnityEngine;
#endregion

/*--------------------------------------
 Use Time.fixedDeltaTime instead of Time.deltaTime when working with the State Machine.
--------------------------------------*/
public class GameManager : SingletonPersistent<GameManager>
{
    // The target frame rate of the game. It is set to 60 FPS as fighting games typically run at 60 FPS.
    const int TARGET_FPS = 60;

    protected override void Awake()
    {
        base.Awake();
        
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
