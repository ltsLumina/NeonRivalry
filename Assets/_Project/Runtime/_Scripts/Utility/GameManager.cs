#region
using UnityEngine;
#endregion

/*--------------------------------------
 Use Time.fixedDeltaTime instead of Time.deltaTime when working with the State Machine.
--------------------------------------*/
public class GameManager : MonoBehaviour
{
    // The target frame rate of the game. It is set to 60 FPS as fighting games typically run at 60 FPS.
    const int TARGET_FPS = 60;

    void Awake()
    {
        Application.targetFrameRate = TARGET_FPS;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        
        // Press escape (while in the editor) to unlock the cursor.
        if (Input.GetKeyDown(KeyCode.Escape) && Application.isEditor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManagerExtended.ReloadScene();
        }
        
        // If the user is in a build, pressing the escape key will quit the game.
        if (Input.GetKeyDown(KeyCode.Escape) && !Application.isEditor)
        {
            Application.Quit();
        }
    }
}
