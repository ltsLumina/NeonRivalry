#region
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public static class SceneManagerExtended
{
    public static int Intro => 0;
    public static int MainMenu => 1;
    public static int CharacterSelect => 2;
    public static int Game => 3;

    public static int PreviousScene { get; private set; }
    public static string ActiveSceneName => SceneManager.GetActiveScene().name;
    public static int ActiveScene => SceneManager.GetActiveScene().buildIndex;
    
    public static bool IntroScene => SceneManager.GetActiveScene().buildIndex == Intro;
    public static bool MainMenuScene => SceneManager.GetActiveScene().buildIndex == MainMenu;
    public static bool CharacterSelectScene => SceneManager.GetActiveScene().buildIndex == CharacterSelect;
    public static bool GameScene => SceneManager.GetActiveScene().buildIndex == Game;
    

    /// <summary>
    ///     Loads the scene with the specified build index.
    /// </summary>
    /// <param name="buildIndex"></param>
    public static void LoadScene(int buildIndex)
    {
        PreviousScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(ClampBuildIndex(buildIndex));
    }

    /// <summary>
    ///     Reloads the currently active scene.
    /// </summary>
    public static void ReloadScene() {
        if (Application.isPlaying) SceneManager.LoadScene(ClampBuildIndex(SceneManager.GetActiveScene().buildIndex));
    }

    /// <summary>
    /// This extension method allows a MonoBehaviour to reload the current scene after a specified delay.
    /// </summary>
    /// <param name="delay">The delay in seconds before the current scene is reloaded.</param>
    public static void ReloadScene(float delay)
    {
        CoroutineHelper.StartCoroutine(ReloadSceneRoutine());
        
        return;  // Define the coroutine
        IEnumerator ReloadSceneRoutine()
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(delay);

            // Reload the current scene
            ReloadScene();
        }
    }

    /// <summary>
    ///     Loads the next scene according to build index order.
    /// </summary>
    public static void LoadNextScene()
    {
        PreviousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(ClampBuildIndex(SceneManager.GetActiveScene().buildIndex + 1));
    }


    /// <summary>
    /// This extension method allows a MonoBehaviour to load the next scene after a specified delay.
    /// </summary>
    /// <param name="delay">The delay in seconds before the next scene is loaded.</param>
    public static void LoadNextScene(float delay)
    {
        CoroutineHelper.StartCoroutine(LoadNextSceneRoutine());
        
        return;  // Define the coroutine
        IEnumerator LoadNextSceneRoutine()
        {
            // Wait for the specified delay
            yield return new WaitForSeconds(delay);

            // Load the next scene
            LoadNextScene();
        }
    }

    /// <summary>
    ///     Loads the previously loaded scene.
    /// </summary>
    public static void LoadPreviousScene() => SceneManager.LoadScene(ClampBuildIndex(PreviousScene));

    /// <summary>
    ///     Asynchronously loads the scene with the specified build index.
    /// </summary>
    /// <param name="scene"></param>
    /// <returns></returns>
    public static IEnumerator LoadSceneAsync(int scene)
    {
        // Pause the game
        Time.timeScale = 0f;

        // Load the scene asynchronously
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);

        // Wait for the scene to finish loading
        while (asyncOperation is { isDone: false }) yield return null;

        // Resume the game
        Time.timeScale = 1f;
    }

    /// <summary>
    ///     Overload of LoadSceneAsync that loads the *next* scene according to the specified build index.
    /// </summary>
    public static IEnumerator LoadNextSceneAsync()
    {
        // Load the next scene asynchronously
        yield return LoadSceneAsync(ClampBuildIndex(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /// <summary>
    ///     If the buildIndex is outside the range of build indexes, return 0.
    /// </summary>
    /// <param name="buildIndex"></param>
    /// <returns></returns>
    static int ClampBuildIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("BuildIndex invalid/unavailable. Loading scene with an index of 0...\nYou are probably missing a scene in the build settings.");
            buildIndex = 0;
        }

        return buildIndex;
    }

    /// <summary>
    ///     Stops playmode if used in the editor, or quits the application if in a build.
    /// </summary>
    public static void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}