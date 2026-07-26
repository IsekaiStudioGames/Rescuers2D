using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum InventorySlotFeedbackState
{
    Normal,
    Transferable,
    Blocked
}

[DisallowMultipleComponent]
public sealed class InventoryElementView : MonoBehaviour
{
    [Header("Slot Presentation")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text stackCountText;
    [SerializeField] private Outline selectionOutline;

    [Header("Slot Colors")]
    [SerializeField]
    private Color emptyColor =
        new Color32(58, 58, 58, 255);

    [SerializeField]
    private Color occupiedColor =
        new Color32(74, 74, 74, 255);

    [SerializeField]
    private Color transferableColor =
        new Color32(46, 107, 60, 255);

    [SerializeField]
    private Color blockedColor =
        new Color32(122, 48, 48, 255);

    private bool occupied;
    private InventorySlotFeedbackState feedbackState =
        InventorySlotFeedbackState.Normal;

    private void Awake()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        SetEmpty();
    }

    public void SetEmpty()
    {
        occupied = false;
        feedbackState = InventorySlotFeedbackState.Normal;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (stackCountText != null)
        {
            stackCountText.text = string.Empty;
        }

        SetSelected(false);
        RefreshBackground();
    }

    public void SetItem(Sprite itemSprite, int stackCount)
    {
        if (!ValidateReferences())
        {
            return;
        }

        occupied = true;
        feedbackState = InventorySlotFeedbackState.Normal;

        itemIcon.sprite = itemSprite;
        itemIcon.enabled = itemSprite != null;

        stackCountText.text = stackCount > 1
            ? stackCount.ToString()
            : string.Empty;

        RefreshBackground();
    }

    public void SetSelected(bool selected)
    {
        if (selectionOutline != null)
        {
            selectionOutline.enabled = selected;
        }
    }

    public void SetFeedbackState(
        InventorySlotFeedbackState newState)
    {
        feedbackState = newState;
        RefreshBackground();
    }

    private void RefreshBackground()
    {
        if (backgroundImage == null)
        {
            return;
        }

        backgroundImage.color = feedbackState switch
        {
            InventorySlotFeedbackState.Transferable =>
                transferableColor,

            InventorySlotFeedbackState.Blocked =>
                blockedColor,

            _ => occupied
                ? occupiedColor
                : emptyColor
        };
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (backgroundImage == null)
        {
            Debug.LogError(
                $"{nameof(InventoryElementView)} on '{name}' is missing its Background Image reference.",
                this);

            valid = false;
        }

        if (itemIcon == null)
        {
            Debug.LogError(
                $"{nameof(InventoryElementView)} on '{name}' is missing its Item Icon reference.",
                this);

            valid = false;
        }

        if (stackCountText == null)
        {
            Debug.LogError(
                $"{nameof(InventoryElementView)} on '{name}' is missing its Stack Count Text reference.",
                this);

            valid = false;
        }

        if (selectionOutline == null)
        {
            Debug.LogError(
                $"{nameof(InventoryElementView)} on '{name}' is missing its Selection Outline reference.",
                this);

            valid = false;
        }

        return valid;
    }

    private void TryAutoAssignReferences()
    {
        backgroundImage = GetComponent<Image>();
        selectionOutline = GetComponent<Outline>();

        Transform iconTransform = transform.Find("ItemIcon");

        if (iconTransform != null)
        {
            itemIcon = iconTransform.GetComponent<Image>();
        }

        Transform countTransform =
            transform.Find("StackCountText");

        if (countTransform != null)
        {
            stackCountText =
                countTransform.GetComponent<TMP_Text>();
        }
    }

    private void Reset()
    {
        TryAutoAssignReferences();
    }

    private void OnValidate()
    {
        TryAutoAssignReferences();

        if (!Application.isPlaying &&
            backgroundImage != null)
        {
            backgroundImage.color = emptyColor;
        }
    }
}