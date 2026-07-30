//----- GraphicsSettingsService.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GraphicsSettingsService
    : IDisposable
{
    private const int MinimumResolutionWidth = 640;
    private const int MinimumResolutionHeight = 360;

    private readonly SettingsService settingsService;

    private readonly List<GraphicsResolutionOption>
        resolutionOptions =
            new List<GraphicsResolutionOption>();

    private readonly List<GraphicsDisplayModeOption>
        displayModeOptions =
            new List<GraphicsDisplayModeOption>();

    public IReadOnlyList<GraphicsResolutionOption>
        ResolutionOptions =>
            resolutionOptions;

    public IReadOnlyList<GraphicsDisplayModeOption>
        DisplayModeOptions =>
            displayModeOptions;

    public bool IsInitialized
    {
        get;
        private set;
    }

    public GraphicsSettingsService(
        SettingsService settingsService)
    {
        this.settingsService =
            settingsService;
    }

    public void Initialize()
    {
        if (IsInitialized)
            return;

        if (settingsService == null)
        {
            Debug.LogError(
                "[GRAPHICS] SettingsService is missing.");

            return;
        }

        settingsService.OnSettingsLoaded +=
            HandleSettingsLoaded;

        settingsService.OnGraphicsSettingsChanged +=
            HandleGraphicsSettingsChanged;

        IsInitialized = true;

        if (settingsService.IsInitialized &&
            settingsService.CurrentData != null)
        {
            Apply(
                settingsService
                    .CurrentData
                    .Graphics,
                allowFullscreenTransition: false);
        }
    }

    public void Dispose()
    {
        if (!IsInitialized ||
            settingsService == null)
        {
            return;
        }

        settingsService.OnSettingsLoaded -=
            HandleSettingsLoaded;

        settingsService.OnSettingsChanged -=
            HandleSettingsChanged;

        IsInitialized = false;
    }

    public void RefreshAvailableOptions(
        int preferredWidth = 0,
        int preferredHeight = 0)
    {
        BuildResolutionOptions(
            preferredWidth,
            preferredHeight);

        BuildDisplayModeOptions();
    }

    public bool ApplyCurrentSettings(
        bool allowFullscreenTransition = true)
    {
        if (!IsInitialized ||
            settingsService == null ||
            settingsService.CurrentData == null)
        {
            return false;
        }

        return Apply(
            settingsService
                .CurrentData
                .Graphics,
            allowFullscreenTransition);
    }

    public bool Apply(
        GraphicsSettingsData graphicsData,
        bool allowFullscreenTransition = true)
    {
        if (graphicsData == null)
        {
            Debug.LogError(
                "[GRAPHICS] Cannot apply null graphics data.");

            return false;
        }

        RefreshAvailableOptions(
            graphicsData.ResolutionWidth,
            graphicsData.ResolutionHeight);

        GraphicsResolutionOption resolution =
            FindClosestResolution(
                graphicsData.ResolutionWidth,
                graphicsData.ResolutionHeight);

        FullScreenMode fullscreenMode =
            NormalizeDisplayMode(
                graphicsData.FullscreenMode);

#if UNITY_WEBGL && !UNITY_EDITOR
        // Browser fullscreen transitions should originate
        // from the player's Apply-button interaction.
        if (!allowFullscreenTransition)
        {
            fullscreenMode =
                Screen.fullScreenMode;
        }
#endif

        int qualityLevel =
            NormalizeQualityLevel(
                graphicsData.QualityLevel);

        int vSyncCount =
            Mathf.Clamp(
                graphicsData.VSyncCount,
                0,
                4);

        int targetFrameRate =
            Mathf.Max(
                -1,
                graphicsData.TargetFrameRate);

        // Apply quality before VSync because a quality
        // profile can contain its own VSync value.
        if (QualitySettings.names != null &&
            QualitySettings.names.Length > 0)
        {
            QualitySettings.SetQualityLevel(
                qualityLevel,
                applyExpensiveChanges: true);
        }

        QualitySettings.vSyncCount =
            vSyncCount;

        Application.targetFrameRate =
            targetFrameRate;



        graphicsData.SetQualityLevel(
            qualityLevel);

        graphicsData.SetVSyncCount(
            vSyncCount);

        graphicsData.SetTargetFrameRate(
            targetFrameRate);

    #if UNITY_WEBGL && !UNITY_EDITOR
            // The browser/itch embed owns the WebGL canvas size.
            // Calling Screen.SetResolution here can desynchronize
            // Unity's render surface from the HTML canvas and input.
            graphicsData.SetResolution(
                Screen.width,
                Screen.height);

            graphicsData.SetFullscreenMode(
                Screen.fullScreenMode);
    #else
            graphicsData.SetResolution(
                resolution.Width,
                resolution.Height);

            graphicsData.SetFullscreenMode(
                fullscreenMode);

            Screen.SetResolution(
                resolution.Width,
                resolution.Height,
                fullscreenMode);
    #endif

        Debug.Log(
            "[GRAPHICS] Applied graphics settings:\n" +
            $"Resolution: {resolution.Label}\n" +
            $"Display Mode: " +
            $"{GetDisplayModeLabel(fullscreenMode)}\n" +
            $"VSync Count: {vSyncCount}\n" +
            $"Quality Level: {qualityLevel}\n" +
            $"Target Frame Rate: {targetFrameRate}");

        return true;
    }

    public int FindResolutionIndex(
        int width,
        int height)
    {
        for (int index = 0;
             index < resolutionOptions.Count;
             index++)
        {
            GraphicsResolutionOption option =
                resolutionOptions[index];

            if (option.Width == width &&
                option.Height == height)
            {
                return index;
            }
        }

        GraphicsResolutionOption closest =
            FindClosestResolution(
                width,
                height);

        return resolutionOptions.IndexOf(
            closest);
    }

    public int FindDisplayModeIndex(
        FullScreenMode fullscreenMode)
    {
        FullScreenMode normalizedMode =
            NormalizeDisplayMode(
                fullscreenMode);

        for (int index = 0;
             index < displayModeOptions.Count;
             index++)
        {
            if (displayModeOptions[index].Mode ==
                normalizedMode)
            {
                return index;
            }
        }

        return 0;
    }

    public static string GetDisplayModeLabel(
        FullScreenMode fullscreenMode)
    {
        switch (fullscreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                return "Borderless Fullscreen";

            case FullScreenMode.ExclusiveFullScreen:
                return "Exclusive Fullscreen";

            case FullScreenMode.Windowed:
                return "Windowed";

            case FullScreenMode.MaximizedWindow:
                return "Maximized Window";

            default:
                return fullscreenMode.ToString();
        }
    }

    private void HandleSettingsLoaded(
        SettingsData settingsData)
    {
        if (settingsData == null)
            return;

        Apply(
            settingsData.Graphics,
            allowFullscreenTransition: false);
    }

    private void HandleSettingsChanged(
        SettingsData settingsData)
    {
        if (settingsData == null)
            return;

        Apply(
            settingsData.Graphics,
            allowFullscreenTransition: true);
    }
    private void HandleGraphicsSettingsChanged(
    GraphicsSettingsData graphicsData)
    {
        if (graphicsData == null)
            return;

        Apply(
            graphicsData,
            allowFullscreenTransition: true);
    }

    private void BuildResolutionOptions(
        int preferredWidth,
        int preferredHeight)
    {
        resolutionOptions.Clear();

        Resolution[] supportedResolutions =
            Screen.resolutions;

        bool hasSupportedResolutionList =
            supportedResolutions != null &&
            supportedResolutions.Length > 0;

        if (hasSupportedResolutionList)
        {
            foreach (Resolution resolution in
                     supportedResolutions)
            {
                AddResolutionOption(
                    resolution.width,
                    resolution.height);
            }
        }
        else
        {
            // Fallback list for platforms or environments
            // that do not expose Screen.resolutions.
            AddResolutionOption(960, 540);
            AddResolutionOption(1280, 720);
            AddResolutionOption(1600, 900);
            AddResolutionOption(1920, 1080);
            AddResolutionOption(2560, 1440);

            AddResolutionOption(
                preferredWidth,
                preferredHeight);
        }

        AddResolutionOption(
            Screen.width,
            Screen.height);

        if (resolutionOptions.Count == 0)
        {
            AddResolutionOption(
                1920,
                1080);
        }

        resolutionOptions.Sort(
            CompareResolutionsDescending);
    }

    private void AddResolutionOption(
        int width,
        int height)
    {
        if (width < MinimumResolutionWidth ||
            height < MinimumResolutionHeight)
        {
            return;
        }

        foreach (GraphicsResolutionOption option in
                 resolutionOptions)
        {
            if (option.Width == width &&
                option.Height == height)
            {
                return;
            }
        }

        resolutionOptions.Add(
            new GraphicsResolutionOption(
                width,
                height));
    }

    private static int CompareResolutionsDescending(
        GraphicsResolutionOption left,
        GraphicsResolutionOption right)
    {
        int pixelComparison =
            right.PixelCount.CompareTo(
                left.PixelCount);

        if (pixelComparison != 0)
            return pixelComparison;

        int widthComparison =
            right.Width.CompareTo(
                left.Width);

        if (widthComparison != 0)
            return widthComparison;

        return right.Height.CompareTo(
            left.Height);
    }

    private void BuildDisplayModeOptions()
    {
        displayModeOptions.Clear();

        displayModeOptions.Add(
            new GraphicsDisplayModeOption(
                FullScreenMode.FullScreenWindow,
                "Borderless Fullscreen"));

        displayModeOptions.Add(
            new GraphicsDisplayModeOption(
                FullScreenMode.Windowed,
                "Windowed"));

        if (Application.platform ==
            RuntimePlatform.WindowsPlayer)
        {
            displayModeOptions.Add(
                new GraphicsDisplayModeOption(
                    FullScreenMode.ExclusiveFullScreen,
                    "Exclusive Fullscreen"));
        }
    }

    private GraphicsResolutionOption
        FindClosestResolution(
            int requestedWidth,
            int requestedHeight)
    {
        if (resolutionOptions.Count == 0)
        {
            return new GraphicsResolutionOption(
                Mathf.Max(
                    MinimumResolutionWidth,
                    requestedWidth),
                Mathf.Max(
                    MinimumResolutionHeight,
                    requestedHeight));
        }

        foreach (GraphicsResolutionOption option in
                 resolutionOptions)
        {
            if (option.Width == requestedWidth &&
                option.Height == requestedHeight)
            {
                return option;
            }
        }

        GraphicsResolutionOption closest =
            resolutionOptions[0];

        long closestDistance =
            GetResolutionDistance(
                closest,
                requestedWidth,
                requestedHeight);

        for (int index = 1;
             index < resolutionOptions.Count;
             index++)
        {
            GraphicsResolutionOption candidate =
                resolutionOptions[index];

            long candidateDistance =
                GetResolutionDistance(
                    candidate,
                    requestedWidth,
                    requestedHeight);

            if (candidateDistance <
                closestDistance)
            {
                closest =
                    candidate;

                closestDistance =
                    candidateDistance;
            }
        }

        return closest;
    }

    private static long GetResolutionDistance(
        GraphicsResolutionOption option,
        int requestedWidth,
        int requestedHeight)
    {
        long widthDifference =
            option.Width - requestedWidth;

        long heightDifference =
            option.Height - requestedHeight;

        return
            widthDifference * widthDifference +
            heightDifference * heightDifference;
    }

    private FullScreenMode NormalizeDisplayMode(
        FullScreenMode requestedMode)
    {
        foreach (GraphicsDisplayModeOption option in
                 displayModeOptions)
        {
            if (option.Mode == requestedMode)
                return requestedMode;
        }

        foreach (GraphicsDisplayModeOption option in
                 displayModeOptions)
        {
            if (option.Mode ==
                FullScreenMode.FullScreenWindow)
            {
                return
                    FullScreenMode.FullScreenWindow;
            }
        }

        return FullScreenMode.Windowed;
    }

    private static int NormalizeQualityLevel(
        int requestedQualityLevel)
    {
        int qualityCount =
            QualitySettings.names == null
                ? 0
                : QualitySettings.names.Length;

        if (qualityCount <= 0)
            return 0;

        return Mathf.Clamp(
            requestedQualityLevel,
            0,
            qualityCount - 1);
    }
}

public sealed class GraphicsResolutionOption
{
    public int Width
    {
        get;
    }

    public int Height
    {
        get;
    }

    public long PixelCount =>
        (long)Width * Height;

    public string Label =>
        $"{Width} x {Height}";

    public GraphicsResolutionOption(
        int width,
        int height)
    {
        Width =
            width;

        Height =
            height;
    }
}

public sealed class GraphicsDisplayModeOption
{
    public FullScreenMode Mode
    {
        get;
    }

    public string Label
    {
        get;
    }

    public GraphicsDisplayModeOption(
        FullScreenMode mode,
        string label)
    {
        Mode =
            mode;

        Label =
            label;
    }
}

//----- GraphicsSettingsService.cs END -----