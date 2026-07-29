//----- LevelConfigurationProvider.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class LevelConfigurationProvider
    : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField]
    private LevelConfigurationData levelConfiguration;

    public LevelConfigurationData Configuration =>
        levelConfiguration;

    public bool HasConfiguration =>
        levelConfiguration != null;

    public bool TryGetConfiguration(
        out LevelConfigurationData configuration)
    {
        configuration =
            levelConfiguration;

        return configuration != null;
    }

    private void Awake()
    {
        if (levelConfiguration != null)
            return;

        Debug.LogWarning(
            $"[LEVEL CONFIGURATION] '{name}' has no " +
            "LevelConfigurationData assigned.",
            this);
    }
}

//----- LevelConfigurationProvider.cs END -----