//----- LevelAudioCoordinator.cs START -----

using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-900)]
[DisallowMultipleComponent]
public sealed class LevelAudioCoordinator
    : MonoBehaviour
{
    [Header("Diagnostics")]
    [SerializeField]
    private bool logLevelAudioChanges = true;

    public LevelConfigurationData ActiveConfiguration
    {
        get;
        private set;
    }

    private ApplicationBootstrap bootstrap;
    private MusicJukebox musicJukebox;

    private bool hasStarted;
    private bool isSubscribed;

    private void Awake()
    {
        bootstrap =
            GetComponentInParent<ApplicationBootstrap>(
                includeInactive: true);

        if (bootstrap == null ||
            bootstrap != ApplicationBootstrap.Instance)
        {
            Debug.LogWarning(
                "[LEVEL AUDIO] This coordinator is not owned by " +
                "the active ApplicationBootstrap and will be disabled.",
                this);

            enabled = false;
            return;
        }

        musicJukebox =
            bootstrap.MusicJukebox;

        if (musicJukebox == null)
        {
            Debug.LogError(
                "[LEVEL AUDIO] The active ApplicationBootstrap " +
                "does not have a MusicJukebox assigned.",
                bootstrap);

            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!hasStarted)
            return;

        Subscribe();
        ApplyActiveScene();
    }

    private void Start()
    {
        hasStarted = true;

        Subscribe();
        ApplyActiveScene();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (isSubscribed)
            return;

        SceneManager.activeSceneChanged +=
            HandleActiveSceneChanged;

        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed)
            return;

        SceneManager.activeSceneChanged -=
            HandleActiveSceneChanged;

        isSubscribed = false;
    }

    private void HandleActiveSceneChanged(
        Scene previousScene,
        Scene currentScene)
    {
        ApplySceneAudio(
            currentScene);
    }

    private void ApplyActiveScene()
    {
        ApplySceneAudio(
            SceneManager.GetActiveScene());
    }

    private void ApplySceneAudio(
        Scene scene)
    {
        ActiveConfiguration =
            null;

        if (musicJukebox == null)
        {
            Debug.LogError(
                "[LEVEL AUDIO] Cannot apply level audio " +
                "because the persistent MusicJukebox is unavailable.",
                this);

            return;
        }

        if (!scene.IsValid() ||
            !scene.isLoaded)
        {
            Debug.LogWarning(
                "[LEVEL AUDIO] The requested scene is not valid " +
                "and loaded.",
                this);

            return;
        }

        LevelConfigurationProvider provider =
            FindProvider(
                scene,
                out int providerCount);

        if (providerCount > 1)
        {
            Debug.LogWarning(
                $"[LEVEL AUDIO] Scene '{scene.name}' contains " +
                $"{providerCount} active configuration providers. " +
                $"Using '{provider.name}'.",
                provider);
        }

        if (provider == null)
        {
            Log(
                $"Scene '{scene.name}' has no active " +
                "LevelConfigurationProvider. Keeping current music.");

            return;
        }

        if (!provider.TryGetConfiguration(
                out LevelConfigurationData configuration))
        {
            Debug.LogWarning(
                $"[LEVEL AUDIO] Provider '{provider.name}' has no " +
                "LevelConfigurationData. Keeping current music.",
                provider);

            return;
        }

        ActiveConfiguration =
            configuration;

        switch (configuration.MusicMode)
        {
            case LevelMusicMode.KeepCurrent:
                Log(
                    $"Scene '{scene.name}' requested Keep Current.");
                break;

            case LevelMusicMode.PlayAssignedTrack:
                PlayAssignedTrack(
                    scene,
                    configuration);
                break;

            case LevelMusicMode.StopMusic:
                StopMusic(
                    scene);
                break;

            default:
                Debug.LogWarning(
                    $"[LEVEL AUDIO] Scene '{scene.name}' has an " +
                    "unsupported music mode. Keeping current music.",
                    provider);
                break;
        }
    }

    private void PlayAssignedTrack(
        Scene scene,
        LevelConfigurationData configuration)
    {
        MusicTrackData track =
            configuration.MusicTrack;

        if (track == null)
        {
            Debug.LogWarning(
                $"[LEVEL AUDIO] Scene '{scene.name}' requested " +
                "Play Assigned Track, but no track is assigned. " +
                "Keeping current music.",
                this);

            return;
        }

        musicJukebox.Play(
            track,
            restartIfAlreadyPlaying: false);

        Log(
            $"Scene '{scene.name}' requested " +
            $"'{track.DisplayName}'.");
    }

    private void StopMusic(
        Scene scene)
    {
        if (musicJukebox.HasCurrentTrack)
        {
            musicJukebox.Stop();
        }

        Log(
            $"Scene '{scene.name}' requested silence.");
    }

    private LevelConfigurationProvider FindProvider(
        Scene scene,
        out int providerCount)
    {
        providerCount = 0;

        LevelConfigurationProvider firstProvider =
            null;

        GameObject[] rootObjects =
            scene.GetRootGameObjects();

        for (int rootIndex = 0;
             rootIndex < rootObjects.Length;
             rootIndex++)
        {
            LevelConfigurationProvider[] providers =
                rootObjects[rootIndex]
                    .GetComponentsInChildren
                        <LevelConfigurationProvider>(
                            includeInactive: false);

            for (int providerIndex = 0;
                 providerIndex < providers.Length;
                 providerIndex++)
            {
                LevelConfigurationProvider provider =
                    providers[providerIndex];

                if (provider == null ||
                    !provider.isActiveAndEnabled)
                {
                    continue;
                }

                providerCount++;

                if (firstProvider == null)
                {
                    firstProvider =
                        provider;
                }
            }
        }

        return firstProvider;
    }

    private void Log(
        string message)
    {
        if (!logLevelAudioChanges)
            return;

        Debug.Log(
            $"[LEVEL AUDIO] {message}",
            this);
    }
}

//----- LevelAudioCoordinator.cs END -----