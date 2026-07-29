//----- RescueTarget.cs START -----

using System;
using UnityEngine;

public enum RescueTargetType
{
    Rescuer,
    Civilian
}

[DisallowMultipleComponent]
public sealed class RescueTarget : MonoBehaviour
{
    public event Action<RescueTarget> OnRequirementChanged;

    [Header("Rescue Identity")]
    [SerializeField]
    private RescueTargetType targetType =
        RescueTargetType.Rescuer;

    [Tooltip(
        "Player-facing name used by rescue and debugging systems.")]
    [SerializeField]
    private string displayName;

    [Tooltip(
        "Required targets must reach the Rescue Camp before " +
        "the mission can be completed.")]
    [SerializeField]
    private bool requiredForMission = true;

    public RescueTargetType TargetType => targetType;

    public string DisplayName =>
        !string.IsNullOrWhiteSpace(displayName)
            ? displayName
            : gameObject.name;

    public bool RequiredForMission => requiredForMission;

    public void SetRequiredForMission(bool isRequired)
    {
        if (requiredForMission == isRequired)
        {
            return;
        }

        requiredForMission = isRequired;
        OnRequirementChanged?.Invoke(this);
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = gameObject.name;
        }
    }
}

//----- RescueTarget.cs END -----