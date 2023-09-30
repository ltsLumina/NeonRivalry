using UnityEngine;

public class MainMenuController : MonoBehaviour
{
   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManagerExtended.LoadNextScene();
        }
    }
    
    public void OnButtonPress()
    {
        SceneManagerExtended.LoadNextScene();
    }
}
