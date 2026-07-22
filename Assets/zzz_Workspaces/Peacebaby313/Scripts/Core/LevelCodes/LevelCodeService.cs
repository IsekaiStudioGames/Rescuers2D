//----- LevelCodeService.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class LevelCodeService
{
    private readonly LevelCodeCatalogData catalog;
    private readonly PasswordTokenSetData tokenSet;

    public LevelCodeCatalogData Catalog =>
        catalog;

    public PasswordTokenSetData TokenSet =>
        tokenSet;

    public int PasswordLength =>
        catalog?.PasswordLength ?? 0;

    public bool IsReady { get; }

    public LevelCodeService(
        LevelCodeCatalogData catalog,
        PasswordTokenSetData tokenSet)
    {
        this.catalog = catalog;
        this.tokenSet = tokenSet;

        IsReady =
            ValidateConfiguration();
    }

    public bool TryGetFirstLevel(
        out LevelCodeEntry firstLevel)
    {
        firstLevel = null;

        return IsReady &&
               catalog.TryGetFirstLevel(
                   out firstLevel);
    }

    public bool TryGetNextLevel(
        string currentSceneName,
        out LevelCodeEntry nextLevel)
    {
        nextLevel = null;

        return IsReady &&
               catalog.TryGetNextLevel(
                   currentSceneName,
                   out nextLevel);
    }

    public bool TryResolvePassword(
        IReadOnlyList<string> submittedTokenIds,
        out LevelCodeEntry matchingLevel)
    {
        matchingLevel = null;

        if (!IsReady ||
            submittedTokenIds == null ||
            submittedTokenIds.Count != PasswordLength)
        {
            return false;
        }

        foreach (LevelCodeEntry entry in catalog.Levels)
        {
            if (entry == null ||
                entry.PasswordTokenIds.Count != PasswordLength)
            {
                continue;
            }

            bool matches = true;

            for (int index = 0;
                 index < PasswordLength;
                 index++)
            {
                string submitted =
                    PasswordTokenSetData.NormalizeTokenId(
                        submittedTokenIds[index]);

                string configured =
                    PasswordTokenSetData.NormalizeTokenId(
                        entry.PasswordTokenIds[index]);

                if (submitted != configured)
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                matchingLevel = entry;
                return true;
            }
        }

        return false;
    }

    public int GetTokenIndex(string tokenId)
    {
        if (!IsReady)
            return -1;

        return tokenSet.GetTokenIndex(
            tokenId);
    }

    public PasswordTokenDefinition GetTokenAtWrappedIndex(
        int index)
    {
        if (!IsReady)
            return null;

        return tokenSet.GetTokenAtWrappedIndex(
            index);
    }

    private bool ValidateConfiguration()
    {
        bool valid = true;

        if (catalog == null)
        {
            Debug.LogError(
                "[LEVEL CODES] Catalog is missing.");

            valid = false;
        }

        if (tokenSet == null)
        {
            Debug.LogError(
                "[LEVEL CODES] Token set is missing.");

            valid = false;
        }

        if (!valid)
            return false;

        if (catalog.PasswordLength <= 0)
        {
            Debug.LogError(
                "[LEVEL CODES] Password length must be positive.");

            valid = false;
        }

        if (catalog.Levels == null ||
            catalog.Levels.Count == 0)
        {
            Debug.LogError(
                "[LEVEL CODES] Catalog contains no levels.");

            valid = false;
        }

        if (tokenSet.Count == 0)
        {
            Debug.LogError(
                "[LEVEL CODES] Token set is empty.");

            valid = false;
        }

        if (!valid)
            return false;

        HashSet<string> passwordKeys =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (LevelCodeEntry entry in catalog.Levels)
        {
            if (entry == null)
            {
                Debug.LogError(
                    "[LEVEL CODES] Null catalog entry found.");

                valid = false;
                continue;
            }

            if (entry.PasswordTokenIds.Count !=
                catalog.PasswordLength)
            {
                Debug.LogError(
                    $"[LEVEL CODES] '{entry.MissionId}' must contain " +
                    $"{catalog.PasswordLength} tokens.");

                valid = false;
                continue;
            }

            List<string> normalizedTokens =
                new List<string>();

            foreach (string tokenId in entry.PasswordTokenIds)
            {
                string normalizedId =
                    PasswordTokenSetData.NormalizeTokenId(
                        tokenId);

                normalizedTokens.Add(
                    normalizedId);

                if (!tokenSet.TryGetTokenById(
                        normalizedId,
                        out _))
                {
                    Debug.LogError(
                        $"[LEVEL CODES] '{entry.MissionId}' references " +
                        $"missing token '{normalizedId}'.");

                    valid = false;
                }
            }

            string passwordKey =
                string.Join(
                    "|",
                    normalizedTokens);

            if (!passwordKeys.Add(passwordKey))
            {
                Debug.LogError(
                    $"[LEVEL CODES] Duplicate password found for " +
                    $"'{entry.MissionId}'.");

                valid = false;
            }

            if (!Application.CanStreamedLevelBeLoaded(
                    entry.SceneName))
            {
                Debug.LogWarning(
                    $"[LEVEL CODES] Scene '{entry.SceneName}' for " +
                    $"'{entry.MissionId}' is not enabled in the " +
                    "active Build Profile.");
            }
        }

        if (valid)
        {
            Debug.Log(
                "[LEVEL CODES] Runtime configuration validated.");
        }

        return valid;
    }
}

//----- LevelCodeService.cs END -----