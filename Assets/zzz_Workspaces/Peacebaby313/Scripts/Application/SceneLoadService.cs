//----- SceneLoadService.cs START -----

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoadService : MonoBehaviour
{
    public bool IsLoading { get; private set; }

   
    // Starts loading a scene without requiring the caller
    // to wait for the operation directly.
   
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
    // to wait for completion.

    public IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (IsLoading)
            yield break;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "[SCENE LOAD] Scene name is empty.");

            yield break;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[SCENE LOAD] Scene '{sceneName}' cannot be loaded. " +
                "Confirm that it exists and is enabled in the " +
                "active Build Profile Scene List.");

            yield break;
        }

        IsLoading = true;

        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(
                sceneName,
                LoadSceneMode.Single);

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

        IsLoading = false;
    }
}

//----- SceneLoadService.cs END -----