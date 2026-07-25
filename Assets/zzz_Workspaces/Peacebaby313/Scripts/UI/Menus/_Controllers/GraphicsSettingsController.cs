//----- GraphicsSettingsController.cs START -----

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class GraphicsSettingsController
    : MonoBehaviour
{
    [Header("Graphics Controls")]
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;

    [SerializeField]
    private TMP_Dropdown displayModeDropdown;

    [SerializeField]
    private Toggle vSyncToggle;

    [SerializeField]
    private TMP_Dropdown qualityDropdown;

    [SerializeField]
    private TMP_Dropdown targetFrameRateDropdown;

    [Header("Actions")]
    [SerializeField]
    private Button applyButton;

    [SerializeField]
    private Button revertButton;

    [SerializeField]
    private Button resetGraphicsButton;

    [Header("Feedback")]
    [SerializeField]
    private TMP_Text statusText;

    [SerializeField]
    private TMP_Text targetFrameRateHintText;

    [Header("Target Frame Rate Options")]
    [SerializeField]
    private int[] configuredTargetFrameRates =
    {
        -1,
        30,
        60,
        120,
        144,
        165,
        240
    };

    private readonly List<GraphicsResolutionOption>
        resolutionOptions =
            new List<GraphicsResolutionOption>();

    private readonly List<GraphicsDisplayModeOption>
        displayModeOptions =
            new List<GraphicsDisplayModeOption>();

    private readonly List<int>
        targetFrameRateOptions =
            new List<int>();

    private ApplicationBootstrap bootstrap;
    private SettingsService settingsService;
    private GraphicsSettingsService graphicsService;

    private bool controlsReady;
    private bool refreshingControls;

    public bool HasPendingChanges =>
        controlsReady &&
        DetectPendingChanges();

    private void Awake()
    {
        AddListeners();
    }

    public void RefreshFromCurrentSettings(
        bool updateStatus = true)
    {
        if (!ResolveServices())
        {
            SetStatus(
                "Graphics services are unavailable.");

            return;
        }

        GraphicsSettingsData currentGraphics =
            settingsService
                .CurrentData
                .Graphics;

        graphicsService.RefreshAvailableOptions(
            currentGraphics.ResolutionWidth,
            currentGraphics.ResolutionHeight);

        CopyRuntimeOptions();

        BuildTargetFrameRateOptions(
            currentGraphics.TargetFrameRate);

        refreshingControls = true;

        PopulateResolutionDropdown();
        PopulateDisplayModeDropdown();
        PopulateQualityDropdown();
        PopulateTargetFrameRateDropdown();

        SetCurrentControlValues(
            currentGraphics);

        refreshingControls = false;
        controlsReady = true;

        UpdateVSyncPresentation();
        UpdateActionButtons();

        if (updateStatus)
        {
            SetStatus(
                "Adjust graphics settings, then choose Apply.");
        }
    }

    public void ApplyChanges()
    {
        if (!ResolveServices() ||
            !controlsReady)
        {
            SetStatus(
                "Graphics services are unavailable.");

            return;
        }

        if (!TryGetSelectedResolution(
                out GraphicsResolutionOption resolution))
        {
            SetStatus(
                "No valid resolution is selected.");

            return;
        }

        if (!TryGetSelectedDisplayMode(
                out GraphicsDisplayModeOption displayMode))
        {
            SetStatus(
                "No valid display mode is selected.");

            return;
        }

        GraphicsSettingsData graphicsData =
            settingsService
                .CurrentData
                .Graphics;

        graphicsData.SetResolution(
            resolution.Width,
            resolution.Height);

        graphicsData.SetFullscreenMode(
            displayMode.Mode);

        graphicsData.SetVSyncCount(
            vSyncToggle != null &&
            vSyncToggle.isOn
                ? 1
                : 0);

        graphicsData.SetQualityLevel(
            GetSelectedQualityLevel(
                graphicsData.QualityLevel));

        graphicsData.SetTargetFrameRate(
            GetSelectedTargetFrameRate(
                graphicsData.TargetFrameRate));

        // GraphicsSettingsService listens to this event
        // and applies the data immediately.
        settingsService.NotifyGraphicsSettingsChanged(
            saveImmediately: false);

        bool saveSucceeded =
            settingsService.SaveCurrent();

        RefreshFromCurrentSettings(
            updateStatus: false);

        SetStatus(
            saveSucceeded
                ? "Graphics settings applied and saved."
                : "Graphics settings applied, but could not be saved.");
    }

    public void RevertPendingChanges(
        bool showFeedback = true)
    {
        if (!controlsReady)
            return;

        RefreshFromCurrentSettings(
            updateStatus: false);

        if (showFeedback)
        {
            SetStatus(
                "Pending graphics changes discarded.");
        }
    }

    public void ResetGraphicsToDefaults()
    {
        if (!ResolveServices())
        {
            SetStatus(
                "Graphics services are unavailable.");

            return;
        }

        bool resetSucceeded =
            settingsService.ResetGraphicsToDefaults(
                saveImmediately: true);

        RefreshFromCurrentSettings(
            updateStatus: false);

        SetStatus(
            resetSucceeded
                ? "Graphics settings reset to defaults."
                : "Graphics settings could not be reset.");
    }

    private bool ResolveServices()
    {
        if (bootstrap == null)
        {
            bootstrap =
                ApplicationBootstrap.Instance;
        }

        if (bootstrap == null ||
            !bootstrap.IsInitialized)
        {
            return false;
        }

        settingsService =
            bootstrap.SettingsService;

        graphicsService =
            bootstrap.GraphicsService;

        return
            settingsService != null &&
            settingsService.IsInitialized &&
            settingsService.CurrentData != null &&
            settingsService.CurrentData.Graphics != null &&
            graphicsService != null &&
            graphicsService.IsInitialized;
    }

    private void CopyRuntimeOptions()
    {
        resolutionOptions.Clear();

        foreach (GraphicsResolutionOption option in
                 graphicsService.ResolutionOptions)
        {
            resolutionOptions.Add(
                option);
        }

        displayModeOptions.Clear();

        foreach (GraphicsDisplayModeOption option in
                 graphicsService.DisplayModeOptions)
        {
            displayModeOptions.Add(
                option);
        }
    }

    private void BuildTargetFrameRateOptions(
        int currentTargetFrameRate)
    {
        targetFrameRateOptions.Clear();

        AddTargetFrameRateOption(
            -1);

        if (configuredTargetFrameRates != null)
        {
            foreach (int frameRate in
                     configuredTargetFrameRates)
            {
                AddTargetFrameRateOption(
                    frameRate);
            }
        }

        AddTargetFrameRateOption(
            currentTargetFrameRate);

        targetFrameRateOptions.Sort(
            CompareFrameRates);

        if (targetFrameRateOptions.Remove(-1))
        {
            targetFrameRateOptions.Insert(
                0,
                -1);
        }
    }

    private void AddTargetFrameRateOption(
        int frameRate)
    {
        frameRate =
            Mathf.Max(
                -1,
                frameRate);

        if (!targetFrameRateOptions.Contains(
                frameRate))
        {
            targetFrameRateOptions.Add(
                frameRate);
        }
    }

    private static int CompareFrameRates(
        int left,
        int right)
    {
        if (left == -1 &&
            right != -1)
        {
            return -1;
        }

        if (right == -1 &&
            left != -1)
        {
            return 1;
        }

        return left.CompareTo(
            right);
    }

    private void PopulateResolutionDropdown()
    {
        if (resolutionDropdown == null)
            return;

        resolutionDropdown.ClearOptions();

        List<string> labels =
            new List<string>();

        foreach (GraphicsResolutionOption option in
                 resolutionOptions)
        {
            labels.Add(
                option.Label);
        }

        resolutionDropdown.AddOptions(
            labels);
    }

    private void PopulateDisplayModeDropdown()
    {
        if (displayModeDropdown == null)
            return;

        displayModeDropdown.ClearOptions();

        List<string> labels =
            new List<string>();

        foreach (GraphicsDisplayModeOption option in
                 displayModeOptions)
        {
            labels.Add(
                option.Label);
        }

        displayModeDropdown.AddOptions(
            labels);
    }

    private void PopulateQualityDropdown()
    {
        if (qualityDropdown == null)
            return;

        qualityDropdown.ClearOptions();

        string[] qualityNames =
            QualitySettings.names;

        List<string> labels =
            new List<string>();

        if (qualityNames != null)
        {
            labels.AddRange(
                qualityNames);
        }

        if (labels.Count == 0)
        {
            labels.Add(
                "Default");
        }

        qualityDropdown.AddOptions(
            labels);
    }

    private void PopulateTargetFrameRateDropdown()
    {
        if (targetFrameRateDropdown == null)
            return;

        targetFrameRateDropdown.ClearOptions();

        List<string> labels =
            new List<string>();

        foreach (int frameRate in
                 targetFrameRateOptions)
        {
            labels.Add(
                frameRate < 0
                    ? "Unlimited"
                    : $"{frameRate} FPS");
        }

        targetFrameRateDropdown.AddOptions(
            labels);
    }

    private void SetCurrentControlValues(
        GraphicsSettingsData graphicsData)
    {
        if (resolutionDropdown != null)
        {
            int resolutionIndex =
                graphicsService.FindResolutionIndex(
                    graphicsData.ResolutionWidth,
                    graphicsData.ResolutionHeight);

            resolutionDropdown.SetValueWithoutNotify(
                Mathf.Clamp(
                    resolutionIndex,
                    0,
                    Mathf.Max(
                        0,
                        resolutionOptions.Count - 1)));

            resolutionDropdown.RefreshShownValue();
        }

        if (displayModeDropdown != null)
        {
            int displayModeIndex =
                graphicsService.FindDisplayModeIndex(
                    graphicsData.FullscreenMode);

            displayModeDropdown.SetValueWithoutNotify(
                Mathf.Clamp(
                    displayModeIndex,
                    0,
                    Mathf.Max(
                        0,
                        displayModeOptions.Count - 1)));

            displayModeDropdown.RefreshShownValue();
        }

        if (vSyncToggle != null)
        {
            vSyncToggle.SetIsOnWithoutNotify(
                graphicsData.VSyncCount > 0);
        }

        if (qualityDropdown != null)
        {
            int maximumQualityIndex =
                Mathf.Max(
                    0,
                    qualityDropdown.options.Count - 1);

            qualityDropdown.SetValueWithoutNotify(
                Mathf.Clamp(
                    graphicsData.QualityLevel,
                    0,
                    maximumQualityIndex));

            qualityDropdown.RefreshShownValue();
        }

        if (targetFrameRateDropdown != null)
        {
            int frameRateIndex =
                targetFrameRateOptions.IndexOf(
                    graphicsData.TargetFrameRate);

            if (frameRateIndex < 0)
            {
                frameRateIndex = 0;
            }

            targetFrameRateDropdown.SetValueWithoutNotify(
                frameRateIndex);

            targetFrameRateDropdown.RefreshShownValue();
        }
    }

    private bool TryGetSelectedResolution(
        out GraphicsResolutionOption resolution)
    {
        resolution = null;

        if (resolutionOptions.Count == 0)
            return false;

        int selectedIndex =
            resolutionDropdown == null
                ? 0
                : resolutionDropdown.value;

        if (selectedIndex < 0 ||
            selectedIndex >= resolutionOptions.Count)
        {
            return false;
        }

        resolution =
            resolutionOptions[selectedIndex];

        return true;
    }

    private bool TryGetSelectedDisplayMode(
        out GraphicsDisplayModeOption displayMode)
    {
        displayMode = null;

        if (displayModeOptions.Count == 0)
            return false;

        int selectedIndex =
            displayModeDropdown == null
                ? 0
                : displayModeDropdown.value;

        if (selectedIndex < 0 ||
            selectedIndex >= displayModeOptions.Count)
        {
            return false;
        }

        displayMode =
            displayModeOptions[selectedIndex];

        return true;
    }

    private int GetSelectedQualityLevel(
        int fallbackValue)
    {
        if (qualityDropdown == null ||
            qualityDropdown.options.Count == 0)
        {
            return fallbackValue;
        }

        return qualityDropdown.value;
    }

    private int GetSelectedTargetFrameRate(
        int fallbackValue)
    {
        if (targetFrameRateDropdown == null ||
            targetFrameRateOptions.Count == 0)
        {
            return fallbackValue;
        }

        int selectedIndex =
            targetFrameRateDropdown.value;

        if (selectedIndex < 0 ||
            selectedIndex >= targetFrameRateOptions.Count)
        {
            return fallbackValue;
        }

        return
            targetFrameRateOptions[selectedIndex];
    }

    private bool DetectPendingChanges()
    {
        if (!ResolveServices())
            return false;

        GraphicsSettingsData currentGraphics =
            settingsService
                .CurrentData
                .Graphics;

        if (TryGetSelectedResolution(
                out GraphicsResolutionOption resolution))
        {
            if (resolution.Width !=
                    currentGraphics.ResolutionWidth ||
                resolution.Height !=
                    currentGraphics.ResolutionHeight)
            {
                return true;
            }
        }

        if (TryGetSelectedDisplayMode(
                out GraphicsDisplayModeOption displayMode))
        {
            if (displayMode.Mode !=
                currentGraphics.FullscreenMode)
            {
                return true;
            }
        }

        bool selectedVSync =
            vSyncToggle != null &&
            vSyncToggle.isOn;

        bool currentVSync =
            currentGraphics.VSyncCount > 0;

        if (selectedVSync != currentVSync)
            return true;

        if (GetSelectedQualityLevel(
                currentGraphics.QualityLevel) !=
            currentGraphics.QualityLevel)
        {
            return true;
        }

        if (GetSelectedTargetFrameRate(
                currentGraphics.TargetFrameRate) !=
            currentGraphics.TargetFrameRate)
        {
            return true;
        }

        return false;
    }

    private void HandleControlChanged(
        int unusedValue)
    {
        HandleAnyControlChanged();
    }

    private void HandleVSyncChanged(
        bool unusedValue)
    {
        HandleAnyControlChanged();
    }

    private void HandleAnyControlChanged()
    {
        if (refreshingControls)
            return;

        UpdateVSyncPresentation();
        UpdateActionButtons();

        SetStatus(
            HasPendingChanges
                ? "Graphics changes are waiting to be applied."
                : "Graphics settings match the saved values.");
    }

    private void UpdateVSyncPresentation()
    {
        bool vSyncEnabled =
            vSyncToggle != null &&
            vSyncToggle.isOn;

        if (targetFrameRateDropdown != null)
        {
            targetFrameRateDropdown.interactable =
                !vSyncEnabled;
        }

        if (targetFrameRateHintText != null)
        {
            targetFrameRateHintText.text =
                vSyncEnabled
                    ? "Target FPS is ignored while VSync is enabled."
                    : "Target FPS is used while VSync is disabled.";
        }
    }

    private void UpdateActionButtons()
    {
        bool hasPendingChanges =
            HasPendingChanges;

        if (applyButton != null)
        {
            applyButton.interactable =
                hasPendingChanges;
        }

        if (revertButton != null)
        {
            revertButton.interactable =
                hasPendingChanges;
        }

        if (resetGraphicsButton != null)
        {
            resetGraphicsButton.interactable =
                controlsReady;
        }
    }

    private void AddListeners()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(
                HandleControlChanged);
        }

        if (displayModeDropdown != null)
        {
            displayModeDropdown.onValueChanged.AddListener(
                HandleControlChanged);
        }

        if (vSyncToggle != null)
        {
            vSyncToggle.onValueChanged.AddListener(
                HandleVSyncChanged);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.AddListener(
                HandleControlChanged);
        }

        if (targetFrameRateDropdown != null)
        {
            targetFrameRateDropdown.onValueChanged.AddListener(
                HandleControlChanged);
        }

        if (applyButton != null)
        {
            applyButton.onClick.AddListener(
                ApplyChanges);
        }

        if (revertButton != null)
        {
            revertButton.onClick.AddListener(
                RevertPendingChangesFromButton);
        }

        if (resetGraphicsButton != null)
        {
            resetGraphicsButton.onClick.AddListener(
                ResetGraphicsToDefaults);
        }
    }

    private void RemoveListeners()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(
                HandleControlChanged);
        }

        if (displayModeDropdown != null)
        {
            displayModeDropdown.onValueChanged.RemoveListener(
                HandleControlChanged);
        }

        if (vSyncToggle != null)
        {
            vSyncToggle.onValueChanged.RemoveListener(
                HandleVSyncChanged);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.RemoveListener(
                HandleControlChanged);
        }

        if (targetFrameRateDropdown != null)
        {
            targetFrameRateDropdown.onValueChanged.RemoveListener(
                HandleControlChanged);
        }

        if (applyButton != null)
        {
            applyButton.onClick.RemoveListener(
                ApplyChanges);
        }

        if (revertButton != null)
        {
            revertButton.onClick.RemoveListener(
                RevertPendingChangesFromButton);
        }

        if (resetGraphicsButton != null)
        {
            resetGraphicsButton.onClick.RemoveListener(
                ResetGraphicsToDefaults);
        }
    }

    private void RevertPendingChangesFromButton()
    {
        RevertPendingChanges(
            showFeedback: true);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text =
                message;
        }
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
}

//----- GraphicsSettingsController.cs END -----