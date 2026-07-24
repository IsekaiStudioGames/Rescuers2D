//----- PasswordSlotUI.cs START -----

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PasswordSlotUI : MonoBehaviour
{
    [Header("Token Presentation")]
    [SerializeField] private TMP_Text tokenText;
    [SerializeField] private Image tokenImage;

    [Header("Selection")]
    [SerializeField] private GameObject selectedVisual;

    [Header("Empty State")]
    [SerializeField] private string emptyText = "-";

    [Header("Presentation Rules")]
    [SerializeField]
    private bool showTextWhenSpriteAvailable;

    public void SetToken(
        PasswordTokenDefinition token)
    {
        bool hasToken =
            token != null;

        bool hasSprite =
            hasToken &&
            token.Sprite != null;

        if (tokenImage != null)
        {
            tokenImage.sprite =
                hasSprite
                    ? token.Sprite
                    : null;

            // Handles cases where the whole child object
            // was disabled in the prefab.
            tokenImage.gameObject.SetActive(
                hasSprite);

            tokenImage.enabled =
                hasSprite;

            if (hasSprite)
            {
                tokenImage.preserveAspect = true;

                tokenImage.rectTransform.localScale =
                    Vector3.one;

                Color imageColor =
                    tokenImage.color;

                imageColor.a = 1f;

                tokenImage.color =
                    imageColor;

                tokenImage.SetAllDirty();
            }
        }

        if (tokenText != null)
        {
            tokenText.gameObject.SetActive(
                !hasSprite ||
                showTextWhenSpriteAvailable);

            tokenText.enabled =
                !hasSprite ||
                showTextWhenSpriteAvailable;

            tokenText.text =
                hasToken
                    ? token.DisplayText
                    : emptyText;
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectedVisual != null)
        {
            selectedVisual.SetActive(
                selected);
        }
    }
}

//----- PasswordSlotUI.cs END -----