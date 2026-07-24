//----- SaveService.cs START -----

using System;
using System.IO;
using UnityEngine;

public sealed class SaveService
{
    private readonly string saveFilePath;
    private readonly string temporaryFilePath;

    public SaveData CurrentData { get; private set; }

    public string SaveFilePath => saveFilePath;

    public bool HasSaveFile =>
        File.Exists(saveFilePath);

    public SaveService(string saveFileName)
    {
        if (string.IsNullOrWhiteSpace(saveFileName))
            saveFileName = "rescuers2d_save.json";

        saveFilePath =
            Path.Combine(
                Application.persistentDataPath,
                saveFileName);

        temporaryFilePath =
            saveFilePath + ".tmp";
    }

    
    // Attempts to load existing data.
    // If no valid save exists, creates empty runtime data.
    
    public void Initialize()
    {
        if (TryLoad(out SaveData loadedData))
        {
            CurrentData = loadedData;

            Debug.Log(
                $"[SAVE] Loaded save data from:\n{saveFilePath}");

            return;
        }

        CurrentData = SaveData.CreateEmpty();

        Debug.Log(
            $"[SAVE] No valid save found. " +
            $"Created empty runtime data.\n" +
            $"Save path:\n{saveFilePath}");
    }

    
    // Creates and writes a fresh New Game save.
    
    public SaveData CreateNewGame(string firstSceneName)
    {
        SaveData newData =
            SaveData.CreateNewGame(firstSceneName);

        if (!Save(newData))
            return null;

        return CurrentData;
    }

    
    // Serializes the supplied SaveData to JSON.
    // A temporary file is written first to reduce the chance
    // of leaving a partially written save.
    
    public bool Save(SaveData data)
    {
        if (data == null)
        {
            Debug.LogError(
                "[SAVE] Cannot save null data.");

            return false;
        }

        try
        {
            string json =
                JsonUtility.ToJson(
                    data,
                    prettyPrint: true);

            string directory =
                Path.GetDirectoryName(saveFilePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(
                temporaryFilePath,
                json);

            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);

            File.Move(
                temporaryFilePath,
                saveFilePath);

            CurrentData = data;

            Debug.Log(
                $"[SAVE] Save completed:\n{saveFilePath}");

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"[SAVE] Failed to write save data.\n" +
                $"{exception.Message}");

            return false;
        }
        finally
        {
            if (File.Exists(temporaryFilePath))
            {
                try
                {
                    File.Delete(temporaryFilePath);
                }
                catch
                {
                    // Cleanup failure is non-fatal.
                }
            }
        }
    }

    
    // Attempts to read and deserialize the current save file.
    
    public bool TryLoad(out SaveData loadedData)
    {
        loadedData = null;

        if (!File.Exists(saveFilePath))
            return false;

        try
        {
            string json =
                File.ReadAllText(saveFilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning(
                    "[SAVE] Save file was empty.");

                PreserveCorruptSave();
                return false;
            }

            loadedData =
                JsonUtility.FromJson<SaveData>(json);

            if (loadedData == null)
            {
                Debug.LogWarning(
                    "[SAVE] Save file could not be deserialized.");

                PreserveCorruptSave();
                return false;
            }

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"[SAVE] Failed to load save data.\n" +
                $"{exception.Message}");

            PreserveCorruptSave();
            return false;
        }
    }

    
    // Updates the current resumable scene and writes the save.
    
    public bool UpdateLastScene(string sceneName)
    {
        if (CurrentData == null)
            CurrentData = SaveData.CreateEmpty();

        CurrentData.SetLastScene(sceneName);

        return Save(CurrentData);
    }

    
    // Deletes the current save file and resets runtime data.
    
    public bool DeleteSave()
    {
        try
        {
            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);

            if (File.Exists(temporaryFilePath))
                File.Delete(temporaryFilePath);

            CurrentData = SaveData.CreateEmpty();

            Debug.Log(
                "[SAVE] Save data deleted.");

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"[SAVE] Failed to delete save data.\n" +
                $"{exception.Message}");

            return false;
        }
    }

    
    // Renames invalid save data instead of silently deleting it.
    
    private void PreserveCorruptSave()
    {
        if (!File.Exists(saveFilePath))
            return;

        try
        {
            string timestamp =
                DateTime.UtcNow.ToString(
                    "yyyyMMdd_HHmmss");

            string corruptPath =
                saveFilePath +
                $".corrupt_{timestamp}";

            File.Move(
                saveFilePath,
                corruptPath);

            Debug.LogWarning(
                $"[SAVE] Preserved invalid save as:\n{corruptPath}");
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"[SAVE] Could not preserve invalid save.\n" +
                $"{exception.Message}");
        }
    }
}

//----- SaveService.cs END -----