//----- LevelCodeCatalogData.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelCodeCatalog_New",
    menuName = "Rescuers2D/Level Codes/Level Code Catalog")]
public sealed class LevelCodeCatalogData : ScriptableObject
{
    [SerializeField, Min(1)]
    private int passwordLength = 8;

    [SerializeField]
    private LevelCodeEntry[] levels =
        Array.Empty<LevelCodeEntry>();

    public int PasswordLength =>
        passwordLength;

    public IReadOnlyList<LevelCodeEntry> Levels =>
        levels;

    public bool TryGetFirstLevel(
        out LevelCodeEntry firstLevel)
    {
        firstLevel = null;

        if (levels == null ||
            levels.Length == 0)
        {
            return false;
        }

        int lowestOrder =
            int.MaxValue;

        foreach (LevelCodeEntry entry in levels)
        {
            if (entry == null)
                continue;

            if (entry.LevelOrder < lowestOrder)
            {
                lowestOrder =
                    entry.LevelOrder;

                firstLevel =
                    entry;
            }
        }

        return firstLevel != null;
    }

    public bool TryFindBySceneName(
        string sceneName,
        out LevelCodeEntry matchingLevel)
    {
        matchingLevel = null;

        if (string.IsNullOrWhiteSpace(sceneName) ||
            levels == null)
        {
            return false;
        }

        foreach (LevelCodeEntry entry in levels)
        {
            if (entry == null)
                continue;

            if (string.Equals(
                    entry.SceneName,
                    sceneName,
                    StringComparison.OrdinalIgnoreCase))
            {
                matchingLevel = entry;
                return true;
            }
        }

        return false;
    }

    public bool TryGetNextLevel(
        string currentSceneName,
        out LevelCodeEntry nextLevel)
    {
        nextLevel = null;

        if (!TryFindBySceneName(
                currentSceneName,
                out LevelCodeEntry currentLevel))
        {
            return false;
        }

        int nearestHigherOrder =
            int.MaxValue;

        foreach (LevelCodeEntry candidate in levels)
        {
            if (candidate == null ||
                candidate.LevelOrder <= currentLevel.LevelOrder)
            {
                continue;
            }

            if (candidate.LevelOrder < nearestHigherOrder)
            {
                nearestHigherOrder =
                    candidate.LevelOrder;

                nextLevel =
                    candidate;
            }
        }

        return nextLevel != null;
    }

    private void OnValidate()
    {
        passwordLength =
            Mathf.Max(1, passwordLength);

        if (levels == null)
            return;

        foreach (LevelCodeEntry entry in levels)
        {
            entry?.SyncTemporaryLetterCode(
                passwordLength);
        }
    }

    [ContextMenu("Validate Catalog")]
    private void ValidateCatalog()
    {
        if (levels == null ||
            levels.Length == 0)
        {
            Debug.LogError(
                "[LEVEL CODES] Catalog is empty.");

            return;
        }

        HashSet<int> usedOrders =
            new HashSet<int>();

        HashSet<string> usedMissionIds =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        HashSet<string> usedSceneNames =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        HashSet<string> usedPasswords =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        bool valid = true;

        foreach (LevelCodeEntry entry in levels)
        {
            if (entry == null)
            {
                Debug.LogError(
                    "[LEVEL CODES] Null level entry found.");

                valid = false;
                continue;
            }

            if (!usedOrders.Add(entry.LevelOrder))
            {
                Debug.LogError(
                    $"[LEVEL CODES] Duplicate level order " +
                    $"'{entry.LevelOrder}'.");

                valid = false;
            }

            if (string.IsNullOrWhiteSpace(entry.MissionId))
            {
                Debug.LogError(
                    "[LEVEL CODES] Mission ID cannot be empty.");

                valid = false;
            }
            else if (!usedMissionIds.Add(entry.MissionId))
            {
                Debug.LogError(
                    $"[LEVEL CODES] Duplicate mission ID " +
                    $"'{entry.MissionId}'.");

                valid = false;
            }

            if (string.IsNullOrWhiteSpace(entry.SceneName))
            {
                Debug.LogError(
                    $"[LEVEL CODES] Scene name is missing for " +
                    $"'{entry.MissionId}'.");

                valid = false;
            }
            else if (!usedSceneNames.Add(entry.SceneName))
            {
                Debug.LogError(
                    $"[LEVEL CODES] Duplicate scene name " +
                    $"'{entry.SceneName}'.");

                valid = false;
            }

            if (entry.PasswordTokenIds.Count != passwordLength)
            {
                Debug.LogError(
                    $"[LEVEL CODES] '{entry.MissionId}' must contain " +
                    $"exactly {passwordLength} tokens.");

                valid = false;
                continue;
            }

            string passwordKey =
                entry.GetNormalizedPasswordKey();

            if (!usedPasswords.Add(passwordKey))
            {
                Debug.LogError(
                    $"[LEVEL CODES] Duplicate password detected " +
                    $"for '{entry.MissionId}'.");

                valid = false;
            }
        }

        if (valid)
        {
            Debug.Log(
                $"[LEVEL CODES] Validation passed. " +
                $"{levels.Length} levels configured.");
        }
    }
}

[Serializable]
public sealed class LevelCodeEntry
{
    [Header("Mission Identity")]
    [SerializeField] private string missionId;
    [SerializeField] private string sceneName;
    [SerializeField] private string displayName;
    [SerializeField, Min(0)] private int levelOrder;

    [Header("Temporary Letter Authoring")]
    [Tooltip(
        "Temporary eight-character helper. " +
        "Clear this before manually assigning sprite-token IDs.")]
    [SerializeField]
    private string temporaryLetterCode =
        "FIRSTLVL";

    [Header("Runtime Password Tokens")]
    [SerializeField]
    private string[] passwordTokenIds =
        Array.Empty<string>();

    public string MissionId =>
        missionId;

    public string SceneName =>
        sceneName;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? missionId
            : displayName;

    public int LevelOrder =>
        levelOrder;

    public IReadOnlyList<string> PasswordTokenIds =>
        passwordTokenIds;

    public string GetPasswordDebugString()
    {
        if (passwordTokenIds == null)
            return string.Empty;

        return string.Join(
            string.Empty,
            passwordTokenIds);
    }

    public string GetNormalizedPasswordKey()
    {
        if (passwordTokenIds == null)
            return string.Empty;

        string[] normalizedTokens =
            new string[passwordTokenIds.Length];

        for (int index = 0;
             index < passwordTokenIds.Length;
             index++)
        {
            normalizedTokens[index] =
                PasswordTokenSetData.NormalizeTokenId(
                    passwordTokenIds[index]);
        }

        return string.Join(
            "|",
            normalizedTokens);
    }

    internal void SyncTemporaryLetterCode(
        int expectedLength)
    {
        if (string.IsNullOrWhiteSpace(
                temporaryLetterCode))
        {
            return;
        }

        string normalized =
            temporaryLetterCode
                .Trim()
                .Replace(" ", string.Empty)
                .ToUpperInvariant();

        if (normalized.Length != expectedLength)
            return;

        passwordTokenIds =
            new string[expectedLength];

        for (int index = 0;
             index < expectedLength;
             index++)
        {
            passwordTokenIds[index] =
                normalized[index].ToString();
        }
    }
}

//----- LevelCodeCatalogData.cs END -----