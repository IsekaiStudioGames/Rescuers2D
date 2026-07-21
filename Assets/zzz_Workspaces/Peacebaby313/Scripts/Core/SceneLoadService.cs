//----- SceneLoadService.cs START -----

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoadService : MonoBehaviour
{
    public bool IsLoading { get; private set; }


    // Starts loading a scene by name.
    // Useful for UI buttons and other systems that do not need
    // to wait for the loading operation directly.

    public void LoadScene(string sceneName)
    {
        if (IsLoading)
        {
            Debug.LogWarning(
                $"[SCENE LOAD] Ignored request for '{sceneName}' " +
                "because another scene is already loading.");

            return;
        }

        StartCoroutine(LoadSceneRoutine(sceneName));
    }


   // Loads a scene asynchronously and allows another coroutine
   // to wait until loading is complete.

    public IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (IsLoading)
            yield break;
        //fallback check in case LoadScene is not used and this method is called directly
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "[SCENE LOAD] Scene name is empty.");

            yield break;
        }
        // Check if the scene can be loaded before starting the load operation
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[SCENE LOAD] Scene '{sceneName}' cannot be loaded. " +
                "Confirm that the scene exists and is enabled in the " +
                "active Build Profile Scene List.");

            yield break;
        }

        IsLoading = true;

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                sceneName,
                LoadSceneMode.Single);


        // Note: LoadSceneMode.Single will unload the current scene and load the new one.
        if (loadOperation == null)
        {
            Debug.LogError(
                $"[SCENE LOAD] Unity could not create a load operation " +
                $"for scene '{sceneName}'.");

            IsLoading = false;
            yield break;
        }

        while (!loadOperation.isDone)
            yield return null;
    }
}

//----- SceneLoadService.cs END -----