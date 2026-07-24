//----- PasswordTokenSetData.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PasswordTokenSet_New",
    menuName = "Rescuers2D/Level Codes/Password Token Set")]
public sealed class PasswordTokenSetData : ScriptableObject
    {
        [SerializeField]
        private PasswordTokenDefinition[] tokens =
            Array.Empty<PasswordTokenDefinition>();

        public IReadOnlyList<PasswordTokenDefinition> Tokens =>
            tokens;

        public int Count =>
            tokens?.Length ?? 0;

        public bool TryGetTokenById(
            string tokenId,
            out PasswordTokenDefinition token)
        {
            token = null;

            string normalizedId =
                NormalizeTokenId(tokenId);

            if (string.IsNullOrEmpty(normalizedId) ||
                tokens == null)
            {
                return false;
            }

            foreach (PasswordTokenDefinition candidate in tokens)
            {
                if (candidate == null)
                    continue;

                string candidateId =
                    NormalizeTokenId(candidate.TokenId);

                if (candidateId == normalizedId)
                {
                    token = candidate;
                    return true;
                }
            }

            return false;
        }

        public int GetTokenIndex(string tokenId)
        {
            string normalizedId =
                NormalizeTokenId(tokenId);

            if (string.IsNullOrEmpty(normalizedId) ||
                tokens == null)
            {
                return -1;
            }

            for (int index = 0;
                 index < tokens.Length;
                 index++)
            {
                PasswordTokenDefinition candidate =
                    tokens[index];

                if (candidate == null)
                    continue;

                string candidateId =
                    NormalizeTokenId(candidate.TokenId);

                if (candidateId == normalizedId)
                    return index;
            }

            return -1;
        }

        public PasswordTokenDefinition GetTokenAtWrappedIndex(
            int index)
        {
            if (tokens == null ||
                tokens.Length == 0)
            {
                return null;
            }

            int wrappedIndex =
                ((index % tokens.Length) +
                 tokens.Length) %
                tokens.Length;

            return tokens[wrappedIndex];
        }

        public static string NormalizeTokenId(string tokenId)
        {
            if (string.IsNullOrWhiteSpace(tokenId))
                return string.Empty;

            return tokenId
                .Trim()
                .ToUpperInvariant();
        }

        [ContextMenu("Populate English Alphabet")]
        private void PopulateEnglishAlphabet()
        {
            tokens =
                new PasswordTokenDefinition[26];

            for (int index = 0;
                 index < tokens.Length;
                 index++)
            {
                char letter =
                    (char)('A' + index);

                string value =
                    letter.ToString();

                tokens[index] =
                    new PasswordTokenDefinition(
                        value,
                        value,
                        null);
            }

            Debug.Log(
                "[PASSWORD TOKENS] Populated A-Z.");
        }

        [ContextMenu("Validate Token Set")]
        private void ValidateTokenSet()
        {
            if (tokens == null ||
                tokens.Length == 0)
            {
                Debug.LogError(
                    "[PASSWORD TOKENS] Token set is empty.");

                return;
            }

            HashSet<string> usedIds =
                new HashSet<string>();

            bool valid = true;

            foreach (PasswordTokenDefinition token in tokens)
            {
                if (token == null)
                {
                    Debug.LogError(
                        "[PASSWORD TOKENS] Null token found.");

                    valid = false;
                    continue;
                }

                string normalizedId =
                    NormalizeTokenId(token.TokenId);

                if (string.IsNullOrEmpty(normalizedId))
                {
                    Debug.LogError(
                        "[PASSWORD TOKENS] Empty token ID.");

                    valid = false;
                    continue;
                }

                if (!usedIds.Add(normalizedId))
                {
                    Debug.LogError(
                        $"[PASSWORD TOKENS] Duplicate token ID " +
                        $"'{normalizedId}'.");

                    valid = false;
                }
            }

            if (valid)
            {
                Debug.Log(
                    $"[PASSWORD TOKENS] Validation passed. " +
                    $"{tokens.Length} tokens available.");
            }
        }
}

[Serializable]
public sealed class PasswordTokenDefinition
{
    [SerializeField] private string tokenId;
    [SerializeField] private string displayText;
    [SerializeField] private Sprite sprite;

    public string TokenId =>
        tokenId;

    public string DisplayText =>
        string.IsNullOrWhiteSpace(displayText)
            ? tokenId
            : displayText;

    public Sprite Sprite =>
        sprite;

    public PasswordTokenDefinition(
        string tokenId,
        string displayText,
        Sprite sprite)
    {
        this.tokenId = tokenId;
        this.displayText = displayText;
        this.sprite = sprite;
    }
}

//----- PasswordTokenSetData.cs END -----