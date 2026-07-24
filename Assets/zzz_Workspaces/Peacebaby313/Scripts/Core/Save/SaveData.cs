//----- SaveData.cs START -----

using System;
using UnityEngine;

[Serializable]
public sealed class SaveData
{
    [SerializeField] private int saveVersion = 1;
    [SerializeField] private bool hasStartedGame;
    [SerializeField] private string lastSceneName = string.Empty;
    [SerializeField] private string lastSaveUtc = string.Empty;

    public int SaveVersion => saveVersion;
    public bool HasStartedGame => hasStartedGame;
    public string LastSceneName => lastSceneName;
    public string LastSaveUtc => lastSaveUtc;

    
    // Creates an empty runtime save state.
    // This does not create a file on disk.

    public static SaveData CreateEmpty()
    {
        return new SaveData();
    }

    
    // Creates initialized data for a new game.
    
    public static SaveData CreateNewGame(string firstSceneName)
    {
        SaveData data = new SaveData();

        data.BeginNewGame(firstSceneName);

        return data;
    }

    
    // Resets this data for a new game and records
    // the first playable scene.
    
    public void BeginNewGame(string firstSceneName)
    {
        hasStartedGame = true;
        lastSceneName = firstSceneName?.Trim() ?? string.Empty;

        UpdateTimestamp();
    }

    
    // Records the most recent resumable scene.
    
    public void SetLastScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning(
                "[SAVE DATA] Ignored an empty last-scene value.");

            return;
        }

        hasStartedGame = true;
        lastSceneName = sceneName.Trim();

        UpdateTimestamp();
    }

    private void UpdateTimestamp()
    {
        lastSaveUtc =
            DateTime.UtcNow.ToString("O");
    }
}

//----- SaveData.cs END -----