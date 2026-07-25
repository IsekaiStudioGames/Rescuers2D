//----- UIAudioFeedback.cs START -----

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class UIAudioFeedback
    : MonoBehaviour,
      ISelectHandler,
      IPointerEnterHandler
{
    [Header("Navigation")]
    [SerializeField]
    private bool playNavigateOnSelect = true;

    [SerializeField]
    private bool playNavigateOnPointerEnter = true;

    [Header("Activation")]
    [SerializeField]
    private UIAudioCue buttonCue =
        UIAudioCue.Submit;

    [SerializeField]
    private bool playValueChangedCue = true;

    private Selectable selectable;

    private Button button;
    private TMP_Dropdown dropdown;
    private Toggle toggle;
    private Slider slider;

    private int lastNavigationFrame = -1;

    private void Awake()
    {
        ResolveSupportedControl();

        if (selectable == null)
        {
            Debug.LogWarning(
                $"[UI AUDIO] UIAudioFeedback on '{gameObject.name}' " +
                "does not have a supported UI control. " +
                "Attach it directly to a Button, Slider, Toggle, " +
                "or TMP_Dropdown.");

            enabled = false;

            return;
        }

        AddListeners();
    }

    public void OnSelect(
        BaseEventData eventData)
    {
        if (!playNavigateOnSelect ||
            !CanPlayFeedback())
        {
            return;
        }

        PlayNavigationOncePerFrame();
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (!playNavigateOnPointerEnter ||
            !CanPlayFeedback())
        {
            return;
        }

        PlayNavigationOncePerFrame();
    }

    private void ResolveSupportedControl()
    {
        button =
            GetComponent<Button>();

        dropdown =
            GetComponent<TMP_Dropdown>();

        toggle =
            GetComponent<Toggle>();

        slider =
            GetComponent<Slider>();

        if (button != null)
        {
            selectable = button;
            return;
        }

        if (dropdown != null)
        {
            selectable = dropdown;
            return;
        }

        if (toggle != null)
        {
            selectable = toggle;
            return;
        }

        if (slider != null)
        {
            selectable = slider;
        }
    }

    private bool CanPlayFeedback()
    {
        return selectable != null &&
               selectable.IsActive() &&
               selectable.IsInteractable();
    }

    private void AddListeners()
    {
        if (button != null)
        {
            button.onClick.AddListener(
                HandleButtonActivated);
        }

        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(
                HandleIntegerValueChanged);
        }

        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(
                HandleBooleanValueChanged);
        }

        if (slider != null)
        {
            slider.onValueChanged.AddListener(
                HandleFloatValueChanged);
        }
    }

    private void RemoveListeners()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(
                HandleButtonActivated);
        }

        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(
                HandleIntegerValueChanged);
        }

        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(
                HandleBooleanValueChanged);
        }

        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(
                HandleFloatValueChanged);
        }
    }

    private void HandleButtonActivated()
    {
        GetUIAudioService()
            ?.Play(
                buttonCue);
    }

    private void HandleIntegerValueChanged(
        int unusedValue)
    {
        PlayValueChanged();
    }

    private void HandleBooleanValueChanged(
        bool unusedValue)
    {
        PlayValueChanged();
    }

    private void HandleFloatValueChanged(
        float unusedValue)
    {
        PlayValueChanged();
    }

    private void PlayValueChanged()
    {
        if (!playValueChangedCue ||
            !CanPlayFeedback())
        {
            return;
        }

        GetUIAudioService()
            ?.PlayValueChanged();
    }

    private void PlayNavigationOncePerFrame()
    {
        if (lastNavigationFrame ==
            Time.frameCount)
        {
            return;
        }

        lastNavigationFrame =
            Time.frameCount;

        GetUIAudioService()
            ?.PlayNavigate();
    }

    private static UIAudioService
        GetUIAudioService()
    {
        ApplicationBootstrap bootstrap =
            ApplicationBootstrap.Instance;

        if (bootstrap == null ||
            !bootstrap.IsInitialized)
        {
            return null;
        }

        return bootstrap.UIAudio;
    }

    private void OnDestroy()
    {
        RemoveListeners();
    }
}

//----- UIAudioFeedback.cs END -----