//----- SettingsService.cs START -----

using System;
using System.IO;
using UnityEngine;

public sealed class SettingsService
{
    public event Action<SettingsData>
        OnSettingsLoaded;

    public event Action<SettingsData>
        OnSettingsChanged;

    public event Action<SettingsData>
        OnSettingsSaved;

    public event Action<SettingsData>
        OnSettingsReset;

    public event Action<AudioSettingsData>
        OnAudioSettingsChanged;

    public event Action<GraphicsSettingsData>
        OnGraphicsSettingsChanged;

    private readonly string settingsFilePath;
    private readonly string temporaryFilePath;
    private readonly SettingsDefaultsData defaults;

    public SettingsData CurrentData
    {
        get;
        private set;
    }

    public string SettingsFilePath =>
        settingsFilePath;

    public bool HasSettingsFile =>
        File.Exists(
            settingsFilePath);

    public bool IsInitialized
    {
        get;
        private set;
    }

    public SettingsService(
        string settingsFileName,
        SettingsDefaultsData defaults)
    {
        if (string.IsNullOrWhiteSpace(
                settingsFileName))
        {
            settingsFileName =
                "rescuers2d_settings.json";
        }

        this.defaults =
            defaults;

        settingsFilePath =
            Path.Combine(
                Application.persistentDataPath,
                settingsFileName.Trim());

        temporaryFilePath =
            settingsFilePath +
            ".tmp";
    }

    public void Initialize()
    {
        if (IsInitialized)
            return;

        if (defaults == null)
        {
            Debug.LogError(
                "[SETTINGS] Settings defaults are missing.");

            return;
        }

        if (TryLoad(
                out SettingsData loadedData))
        {
            loadedData.Sanitize(
                defaults);

            CurrentData =
                loadedData;

            IsInitialized =
                true;

            Debug.Log(
                $"[SETTINGS] Loaded settings from:\n" +
                $"{settingsFilePath}");

            OnSettingsLoaded?.Invoke(
                CurrentData);

            return;
        }

        CurrentData =
            defaults.CreateRuntimeData();

        IsInitialized =
            true;

        if (!SaveCurrent())
        {
            Debug.LogWarning(
                "[SETTINGS] Defaults were created in memory, " +
                "but the settings file could not be written.");
        }

        OnSettingsLoaded?.Invoke(
            CurrentData);
    }

    public void NotifySettingsChanged(
        bool saveImmediately = false)
    {
        if (!PrepareCurrentData())
            return;

        OnSettingsChanged?.Invoke(
            CurrentData);

        OnAudioSettingsChanged?.Invoke(
            CurrentData.Audio);

        OnGraphicsSettingsChanged?.Invoke(
            CurrentData.Graphics);

        if (saveImmediately)
        {
            SaveCurrent();
        }
    }

    public void NotifyAudioSettingsChanged(
        bool saveImmediately = false)
    {
        if (!PrepareCurrentData())
            return;

        OnSettingsChanged?.Invoke(
            CurrentData);

        OnAudioSettingsChanged?.Invoke(
            CurrentData.Audio);

        if (saveImmediately)
        {
            SaveCurrent();
        }
    }

    public void NotifyGraphicsSettingsChanged(
        bool saveImmediately = false)
    {
        if (!PrepareCurrentData())
            return;

        OnSettingsChanged?.Invoke(
            CurrentData);

        OnGraphicsSettingsChanged?.Invoke(
            CurrentData.Graphics);

        if (saveImmediately)
        {
            SaveCurrent();
        }
    }

    public bool SaveCurrent()
    {
        if (!IsInitialized ||
            CurrentData == null)
        {
            Debug.LogError(
                "[SETTINGS] Cannot save before initialization.");

            return false;
        }

        CurrentData.Sanitize(
            defaults);

        CurrentData.MarkSaved();

        try
        {
            string json =
                JsonUtility.ToJson(
                    CurrentData,
                    prettyPrint: true);

            string directory =
                Path.GetDirectoryName(
                    settingsFilePath);

            if (!string.IsNullOrWhiteSpace(
                    directory))
            {
                Directory.CreateDirectory(
                    directory);
            }

            File.WriteAllText(
                temporaryFilePath,
                json);

            if (File.Exists(
                    settingsFilePath))
            {
                File.Delete(
                    settingsFilePath);
            }

            File.Move(
                temporaryFilePath,
                settingsFilePath);

            Debug.Log(
                $"[SETTINGS] Settings saved:\n" +
                $"{settingsFilePath}");

            OnSettingsSaved?.Invoke(
                CurrentData);

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[SETTINGS] Failed to save settings.\n" +
                exception.Message);

            return false;
        }
        finally
        {
            DeleteTemporaryFile();
        }
    }

    public bool ResetToDefaults(
        bool saveImmediately = true)
    {
        if (!IsInitialized ||
            defaults == null)
        {
            return false;
        }

        CurrentData =
            defaults.CreateRuntimeData();

        OnSettingsChanged?.Invoke(
            CurrentData);

        OnAudioSettingsChanged?.Invoke(
            CurrentData.Audio);

        OnGraphicsSettingsChanged?.Invoke(
            CurrentData.Graphics);

        OnSettingsReset?.Invoke(
            CurrentData);

        if (!saveImmediately)
            return true;

        return SaveCurrent();
    }

    public bool ResetAudioToDefaults(
        bool saveImmediately = true)
    {
        if (!PrepareCurrentData())
            return false;

        CurrentData
            .Audio
            .ResetToDefaults(
                defaults);

        CurrentData.Sanitize(
            defaults);

        OnSettingsChanged?.Invoke(
            CurrentData);

        OnAudioSettingsChanged?.Invoke(
            CurrentData.Audio);

        if (!saveImmediately)
            return true;

        return SaveCurrent();
    }

    public bool ResetGraphicsToDefaults(
        bool saveImmediately = true)
    {
        if (!PrepareCurrentData())
            return false;

        CurrentData
            .Graphics
            .ResetToDefaults(
                defaults);

        CurrentData.Sanitize(
            defaults);

        OnSettingsChanged?.Invoke(
            CurrentData);

        OnGraphicsSettingsChanged?.Invoke(
            CurrentData.Graphics);

        if (!saveImmediately)
            return true;

        return SaveCurrent();
    }

    public bool DeleteSettingsFileAndReset()
    {
        try
        {
            if (File.Exists(
                    settingsFilePath))
            {
                File.Delete(
                    settingsFilePath);
            }

            DeleteTemporaryFile();

            CurrentData =
                defaults.CreateRuntimeData();

            IsInitialized =
                true;

            OnSettingsChanged?.Invoke(
                CurrentData);

            OnAudioSettingsChanged?.Invoke(
                CurrentData.Audio);

            OnGraphicsSettingsChanged?.Invoke(
                CurrentData.Graphics);

            OnSettingsReset?.Invoke(
                CurrentData);

            Debug.Log(
                "[SETTINGS] Settings file deleted. " +
                "Runtime defaults restored.");

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[SETTINGS] Failed to delete settings.\n" +
                exception.Message);

            return false;
        }
    }

    private bool PrepareCurrentData()
    {
        if (!IsInitialized ||
            defaults == null ||
            CurrentData == null)
        {
            return false;
        }

        CurrentData.Sanitize(
            defaults);

        return true;
    }

    private bool TryLoad(
        out SettingsData loadedData)
    {
        loadedData =
            null;

        if (!File.Exists(
                settingsFilePath))
        {
            return false;
        }

        try
        {
            string json =
                File.ReadAllText(
                    settingsFilePath);

            if (string.IsNullOrWhiteSpace(
                    json))
            {
                Debug.LogWarning(
                    "[SETTINGS] Settings file was empty.");

                PreserveCorruptFile();

                return false;
            }

            loadedData =
                JsonUtility.FromJson<SettingsData>(
                    json);

            if (loadedData == null)
            {
                Debug.LogWarning(
                    "[SETTINGS] Settings file could not " +
                    "be deserialized.");

                PreserveCorruptFile();

                return false;
            }

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[SETTINGS] Failed to load settings.\n" +
                exception.Message);

            PreserveCorruptFile();

            return false;
        }
    }

    private void PreserveCorruptFile()
    {
        if (!File.Exists(
                settingsFilePath))
        {
            return;
        }

        try
        {
            string timestamp =
                DateTime.UtcNow.ToString(
                    "yyyyMMdd_HHmmss_fff");

            string corruptPath =
                settingsFilePath +
                $".corrupt_{timestamp}";

            File.Move(
                settingsFilePath,
                corruptPath);

            Debug.LogWarning(
                $"[SETTINGS] Preserved invalid settings as:\n" +
                $"{corruptPath}");
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "[SETTINGS] Could not preserve invalid settings.\n" +
                exception.Message);
        }
    }

    private void DeleteTemporaryFile()
    {
        if (!File.Exists(
                temporaryFilePath))
        {
            return;
        }

        try
        {
            File.Delete(
                temporaryFilePath);
        }
        catch
        {
            // Cleanup failure is non-fatal.
        }
    }
}

//----- SettingsService.cs END -----