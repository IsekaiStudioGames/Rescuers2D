//----- ApplicationBootstrap.cs START -----

using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
[RequireComponent(typeof(SceneLoadService))]
public sealed class ApplicationBootstrap : MonoBehaviour
{
    public static ApplicationBootstrap Instance { get; private set; }

    public event Action OnInitialized;

    [Header("Save Configuration")]
    [SerializeField]
    private string saveFileName =
        "rescuers2d_save.json";

    [Header("Runtime Services")]
    [SerializeField]
    private SceneLoadService sceneLoadService;

    public SaveService SaveService { get; private set; }

    public SceneLoadService SceneLoader =>
        sceneLoadService;

    public bool IsInitialized { get; private set; }

    public bool CanContinue
    {
        get
        {
            if (!IsInitialized ||
                SaveService == null ||
                SaveService.CurrentData == null)
            {
                return false;
            }

            SaveData data =
                SaveService.CurrentData;

            if (!data.HasStartedGame ||
                string.IsNullOrWhiteSpace(data.LastSceneName))
            {
                return false;
            }

            return Application.CanStreamedLevelBeLoaded(
                data.LastSceneName);
        }
    }

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Debug.LogWarning(
                "[BOOTSTRAP] Duplicate ApplicationBootstrap destroyed.");

            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        if (sceneLoadService == null)
        {
            sceneLoadService =
                GetComponent<SceneLoadService>();
        }

        InitializeServices();
    }

    private void InitializeServices()
    {
        if (IsInitialized)
            return;

        SaveService =
            new SaveService(saveFileName);

        SaveService.Initialize();

        IsInitialized = true;

        Debug.Log(
            "[BOOTSTRAP] Application services initialized.");

        OnInitialized?.Invoke();
    }

    
    // Creates a fresh save and loads the configured
    // first playable scene.
    
    public bool TryStartNewGame(
        string firstSceneName)
    {
        if (!IsInitialized)
        {
            Debug.LogError(
                "[BOOTSTRAP] Cannot start a new game " +
                "before initialization.");

            return false;
        }

        if (!CanLoadScene(firstSceneName))
            return false;

        SaveData newData =
            SaveService.CreateNewGame(
                firstSceneName);

        if (newData == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] New Game was cancelled " +
                "because save creation failed.");

            return false;
        }

        sceneLoadService.LoadScene(
            firstSceneName);

        return true;
    }

    
    /// Loads the last resumable scene stored in save data.
    
    public bool TryContinueGame()
    {
        if (!CanContinue)
        {
            Debug.LogWarning(
                "[BOOTSTRAP] Continue requested, " +
                "but no valid resumable save exists.");

            return false;
        }

        sceneLoadService.LoadScene(
            SaveService.CurrentData.LastSceneName);

        return true;
    }

    
    // Records a destination in save data before loading it.
    // This can be reused by later mission-transition systems.
    
    public bool TryLoadAndRecordScene(
        string sceneName)
    {
        if (!IsInitialized)
            return false;

        if (!CanLoadScene(sceneName))
            return false;

        if (!SaveService.UpdateLastScene(sceneName))
            return false;

        sceneLoadService.LoadScene(sceneName);

        return true;
    }

    
    // Deletes current save data.
    // A confirmation menu can call this later.
    
    public void DeleteSaveData()
    {
        if (!IsInitialized ||
            SaveService == null)
        {
            return;
        }

        SaveService.DeleteSave();
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
                $"[BOOTSTRAP] Scene '{sceneName}' cannot be loaded. " +
                "Confirm that it is enabled in the " +
                "active Build Profile Scene List.");

            return false;
        }

        return true;
    }

    [ContextMenu("Debug/Delete Save Data")]
    private void DebugDeleteSaveData()
    {
        DeleteSaveData();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

//----- ApplicationBootstrap.cs END -----