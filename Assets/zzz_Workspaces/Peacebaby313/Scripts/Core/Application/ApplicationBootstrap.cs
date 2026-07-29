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

    [Header("Settings Configuration")]
    [SerializeField]
    private SettingsDefaultsData settingsDefaults;

    [SerializeField]
    private string settingsFileName =
        "rescuers2d_settings.json";

    [Header("Audio Configuration")]
    [SerializeField]
    private AudioMixerConfigurationData
        audioMixerConfiguration;

    [SerializeField]
    private UIAudioProfileData
        uiAudioProfile;



    [Header("Audio Services")]
    [SerializeField]
    private MusicJukebox musicJukebox;

    [SerializeField]
    private SfxPlayer sfxPlayer;

    [Header("Runtime Components")]
    [SerializeField]
    private SceneLoadService sceneLoadService;

    public SceneLoadService SceneLoader =>
        sceneLoadService;
    public MusicJukebox MusicJukebox =>
        musicJukebox;

    public SfxPlayer SfxPlayer =>
        sfxPlayer;

    public LevelCodeService LevelCodes
    {
        get;
        private set;
    }

    public SettingsService SettingsService
    {
        get;
        private set;
    }

    public GraphicsSettingsService GraphicsService
    {
        get;
        private set;
    }

    public AudioSettingsService AudioService
    {
        get;
        private set;
    }

    public UIAudioService UIAudio
    {
        get;
        private set;
    }

    public AudioMixerConfigurationData
        AudioMixerConfiguration =>
            audioMixerConfiguration;

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

        ResolveComponents();
        InitializeServices();
    }

    private void Start()
    {
        if (!IsSingletonInstance ||
            !IsInitialized)
        {
            return;
        }
        AudioService?.ApplyCurrentSettings();
    }

    private void ResolveComponents()
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

        if (!ValidateConfiguration())
            return;

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

        SettingsService =
            new SettingsService(
                settingsFileName,
                settingsDefaults);

        GraphicsService =
            new GraphicsSettingsService(
                SettingsService);

        GraphicsService.Initialize();

        AudioService =
            new AudioSettingsService(
                SettingsService,
                audioMixerConfiguration);

        AudioService.Initialize();

        UIAudio =
            new UIAudioService(
                sfxPlayer,
                uiAudioProfile);

        UIAudio.Initialize();

        if (!GraphicsService.IsInitialized)
        {
            Debug.LogError(
                "[BOOTSTRAP] Graphics service initialization failed.");

            return;
        }

        if (!AudioService.IsInitialized)
        {
            Debug.LogError(
                "[BOOTSTRAP] Audio service initialization failed.");

            return;
        }

        if (!UIAudio.IsInitialized)
        {
            Debug.LogError(
                "[BOOTSTRAP] UI Audio service initialization failed.");

            return;
        }

        SettingsService.Initialize();

        if (!SettingsService.IsInitialized)
        {
            Debug.LogError(
                "[BOOTSTRAP] Settings initialization failed.");

            return;
        }

        IsInitialized =
            true;

        Debug.Log(
            "[BOOTSTRAP] Application services initialized.");

        OnInitialized?.Invoke();
    }

    private bool ValidateConfiguration()
    {
        bool valid =
            true;

        if (sceneLoadService == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] SceneLoadService is missing.");

            valid =
                false;
        }

        if (levelCodeCatalog == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] LevelCodeCatalogData is missing.");

            valid =
                false;
        }

        if (passwordTokenSet == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] PasswordTokenSetData is missing.");

            valid =
                false;
        }

        if (settingsDefaults == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] SettingsDefaultsData is missing.");

            valid =
                false;
        }

        if (audioMixerConfiguration == null ||
            !audioMixerConfiguration.IsConfigured)
        {
            Debug.LogError(
                "[BOOTSTRAP] AudioMixerConfigurationData " +
                "is missing or invalid.");

            valid =
                false;
        }

        if (uiAudioProfile == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] UIAudioProfileData is missing.");

            valid =
                false;
        }


        if (musicJukebox == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] MusicJukebox is missing.");

            valid =
                false;
        }

        if (sfxPlayer == null)
        {
            Debug.LogError(
                "[BOOTSTRAP] SfxPlayer is missing.");

            valid =
                false;
        }




        return valid;
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

        if (!CanLoadScene(
                firstLevel.SceneName))
        {
            feedback =
                $"Level '{firstLevel.DisplayName}' " +
                "is unavailable.";

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

        if (!CanLoadScene(
                matchingLevel.SceneName))
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
        nextLevel =
            null;

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

        if (!CanLoadScene(
                sceneName))
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

    private bool CanLoadScene(
        string sceneName)
    {
        if (string.IsNullOrWhiteSpace(
                sceneName))
        {
            Debug.LogError(
                "[BOOTSTRAP] Requested scene name is empty.");

            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(
                sceneName))
        {
            Debug.LogError(
                $"[BOOTSTRAP] Scene '{sceneName}' is not enabled " +
                "in the active Build Profile.");

            return false;
        }

        return true;
    }

    [ContextMenu("Debug/Apply Current Audio Settings")]
    private void DebugApplyCurrentAudioSettings()
    {
        AudioService?.ApplyCurrentSettings();
    }

    [ContextMenu("Debug/Apply Current Graphics Settings")]
    private void DebugApplyCurrentGraphicsSettings()
    {
        GraphicsService?.ApplyCurrentSettings(
            allowFullscreenTransition: true);
    }

    [ContextMenu("Debug/Reset Settings To Defaults")]
    private void DebugResetSettingsToDefaults()
    {
        SettingsService?.ResetToDefaults(
            saveImmediately: true);
    }

    [ContextMenu("Debug/Delete Settings File")]
    private void DebugDeleteSettingsFile()
    {
        SettingsService?.DeleteSettingsFileAndReset();
    }

    [ContextMenu("Debug/Log Settings File Path")]
    private void DebugLogSettingsFilePath()
    {
        if (SettingsService == null)
            return;

        Debug.Log(
            $"[BOOTSTRAP] Settings file path:\n" +
            $"{SettingsService.SettingsFilePath}");
    }

    protected override void OnDestroy()
    {
        if (IsSingletonInstance)
        {
            GraphicsService?.Dispose();
            AudioService?.Dispose();
        }

        base.OnDestroy();
    }
}

//----- ApplicationBootstrap.cs END -----