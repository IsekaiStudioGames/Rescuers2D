//----- LevelConfigurationData.cs START -----

using System;
using UnityEngine;

[Serializable]
public sealed class RescuerBriefingEntry
{
    [Header("Identity")]
    [SerializeField]
    private string characterName;

    [SerializeField]
    private Sprite characterPortrait;

    [Header("Briefing Description")]
    [TextArea(2, 5)]
    [SerializeField]
    private string abilityDescription;

    public string CharacterName => characterName;
    public Sprite CharacterPortrait => characterPortrait;
    public string AbilityDescription => abilityDescription;
}

[CreateAssetMenu(
    fileName = "LevelConfiguration_",
    menuName = "Rescuers2D/Levels/Level Configuration")]
public sealed class LevelConfigurationData : ScriptableObject
{
    private const string VictimNameToken = "{victim}";

    [Header("Level Identity")]
    [Tooltip(
        "Permanent internal identifier used by save and progression systems.")]
    [SerializeField]
    private string levelId;

    [Tooltip(
        "Player-facing level number, such as 0-0 or 1-2.")]
    [SerializeField]
    private string levelNumber;

    [Tooltip(
        "Player-facing level title, such as Bomb Shelter.")]
    [SerializeField]
    private string levelName;

    [SerializeField]
    private string sceneName;

    [Header("Briefing")]
    [TextArea(2, 4)]
    [Tooltip(
        "Short objective. Use {victim} to insert the assigned " +
        "victim prefab's name.")]
    [SerializeField]
    private string levelGoal;

    [TextArea(3, 8)]
    [SerializeField]
    private string levelOverview;

    [Min(1f)]
    [SerializeField]
    private float briefingDuration = 30f;

    [Header("Level Victim")]
    [Tooltip(
        "Victim prefab used by this level. Its prefab name can be " +
        "inserted into the goal with the {victim} token.")]
    [SerializeField]
    private GameObject victimPrefab;

    [Header("Gameplay Timer")]
    [Min(1f)]
    [SerializeField]
    private float levelTimeLimit = 300f;

    [Header("Progression")]
    [SerializeField]
    private string currentLevelPassword;

    [SerializeField]
    private string nextLevelPassword;

    [SerializeField]
    private string nextSceneName;

    [Header("Rescuer Team")]
    [SerializeField]
    private RescuerBriefingEntry[] rescuers =
        new RescuerBriefingEntry[3];

    public string LevelId => levelId;
    public string LevelNumber => levelNumber;
    public string LevelName => levelName;
    public string SceneName => sceneName;

    public string LevelGoal => BuildLevelGoal();
    public string LevelOverview => levelOverview;

    public GameObject VictimPrefab => victimPrefab;

    public string VictimName =>
        victimPrefab != null
            ? victimPrefab.name
            : "the survivor";

    public float BriefingDuration => briefingDuration;
    public float LevelTimeLimit => levelTimeLimit;
    public string CurrentLevelPassword => currentLevelPassword;
    public string NextLevelPassword => nextLevelPassword;
    public string NextSceneName => nextSceneName;
    public RescuerBriefingEntry[] Rescuers => rescuers;

    public bool TryGetRescuer(
        int index,
        out RescuerBriefingEntry rescuer)
    {
        rescuer = null;

        if (rescuers == null ||
            index < 0 ||
            index >= rescuers.Length)
        {
            return false;
        }

        rescuer = rescuers[index];
        return rescuer != null;
    }

    private string BuildLevelGoal()
    {
        if (string.IsNullOrWhiteSpace(levelGoal))
        {
            return string.Empty;
        }

        return levelGoal.Replace(
            VictimNameToken,
            VictimName);
    }

    private void OnValidate()
    {
        briefingDuration =
            Mathf.Max(1f, briefingDuration);

        levelTimeLimit =
            Mathf.Max(1f, levelTimeLimit);

        if (rescuers == null || rescuers.Length != 3)
        {
            Debug.LogWarning(
                $"[LEVEL CONFIGURATION] {name} should contain " +
                "exactly three rescuer briefing entries.",
                this);
        }

        if (!string.IsNullOrWhiteSpace(levelGoal) &&
            levelGoal.Contains(VictimNameToken) &&
            victimPrefab == null)
        {
            Debug.LogWarning(
                $"[LEVEL CONFIGURATION] {name} uses the " +
                $"{VictimNameToken} token but has no victim prefab.",
                this);
        }
    }
}

//----- LevelConfigurationData.cs END -----