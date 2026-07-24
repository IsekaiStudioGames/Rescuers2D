//----- ApplicationBootstrap.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(SceneLoadService))]
public sealed class ApplicationBootstrap
    : Singleton<ApplicationBootstrap>
{
    public event Action OnInitialized;

    [Header("Level Code Configuration")]
    [SerializeField]
    private LevelCodeCatalogData levelCodeCatalog;

    [SerializeField]
    private PasswordTokenSetData passwordTokenSet;

    [Header("Runtime Services")]
    [SerializeField]
    private SceneLoadService sceneLoadService;

    public SceneLoadService SceneLoader =>
        sceneLoadService;

    public LevelCodeService LevelCodes
    {
        get;
        private set;
    }

    public bool IsInitialized
    {
        get;
        private set;
    }

    public bool CanStartNewGame
    {
        get
        {
            if (!IsInitialized ||
                LevelCodes == null ||
                !LevelCodes.TryGetFirstLevel(
                    out LevelCodeEntry firstLevel))
            {
                return false;
            }

            return CanLoadScene(
                firstLevel.SceneName);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (!IsSingletonInstance)
            return;

        ResolveServices();
        InitializeServices();
    }

    private void ResolveServices()
    {
        if (sceneLoadService == null)
        {
            sceneLoadService =
                GetComponent<SceneLoadService>();
        }

        if (sceneLoadService == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] SceneLoadService is missing.");
        }
    }

    private void InitializeServices()
    {
        if (IsInitialized)
            return;

        if (sceneLoadService == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] Cannot initialize because " +
                "SceneLoadService is missing.");

            return;
        }

        if (levelCodeCatalog == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] LevelCodeCatalogData is missing.");

            return;
        }

        if (passwordTokenSet == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] PasswordTokenSetData is missing.");

            return;
        }

        LevelCodes =
            new LevelCodeService(
                levelCodeCatalog,
                passwordTokenSet);

        if (!LevelCodes.IsReady)
        {
            Debug.LogError(
                "[BOOTSTRAP] Level-code validation failed.");

            return;
        }

        IsInitialized = true;

        Debug.Log(
            "[BOOTSTRAP] Application services initialized.");

        OnInitialized?.Invoke();
    }

    public bool TryStartNewGame(
        out string feedback)
    {
        feedback =
            "Unable to start a new game.";

        if (!IsInitialized ||
            LevelCodes == null)
        {
            feedback =
                "Startup services are not ready.";

            return false;
        }

        if (!LevelCodes.TryGetFirstLevel(
                out LevelCodeEntry firstLevel))
        {
            feedback =
                "No first level is configured.";

            return false;
        }

        if (!CanLoadScene(firstLevel.SceneName))
        {
            feedback =
                $"Level '{firstLevel.DisplayName}' is unavailable.";

            return false;
        }

        feedback =
            $"Loading {firstLevel.DisplayName}...";

        sceneLoadService.LoadScene(
            firstLevel.SceneName);

        return true;
    }

    public bool TryLoadLevelByPassword(
        IReadOnlyList<string> submittedTokenIds,
        out string feedback)
    {
        feedback =
            "Invalid password.";

        if (!IsInitialized ||
            LevelCodes == null)
        {
            feedback =
                "Startup services are not ready.";

            return false;
        }

        if (!LevelCodes.TryResolvePassword(
                submittedTokenIds,
                out LevelCodeEntry matchingLevel))
        {
            feedback =
                "Invalid password. Check the sequence and try again.";

            return false;
        }

        if (!CanLoadScene(matchingLevel.SceneName))
        {
            feedback =
                $"Password recognized, but " +
                $"'{matchingLevel.DisplayName}' is unavailable.";

            return false;
        }

        feedback =
            $"Loading {matchingLevel.DisplayName}...";

        sceneLoadService.LoadScene(
            matchingLevel.SceneName);

        return true;
    }

    public bool TryGetNextLevel(
        string currentSceneName,
        out LevelCodeEntry nextLevel)
    {
        nextLevel = null;

        return IsInitialized &&
               LevelCodes != null &&
               LevelCodes.TryGetNextLevel(
                   currentSceneName,
                   out nextLevel);
    }

    public bool TryLoadScene(
        string sceneName,
        out string feedback)
    {
        feedback =
            "Unable to load scene.";

        if (!IsInitialized)
        {
            feedback =
                "Startup services are not ready.";

            return false;
        }

        if (!CanLoadScene(sceneName))
        {
            feedback =
                $"Scene '{sceneName}' is unavailable.";

            return false;
        }

        feedback =
            $"Loading {sceneName}...";

        sceneLoadService.LoadScene(
            sceneName);

        return true;
    }

    private bool CanLoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError(
                "[BOOTSTRAP] Requested scene name is empty.");

            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"[BOOTSTRAP] Scene '{sceneName}' is not enabled " +
                "in the active Build Profile.");

            return false;
        }

        return true;
    }
}

//----- ApplicationBootstrap.cs END -----