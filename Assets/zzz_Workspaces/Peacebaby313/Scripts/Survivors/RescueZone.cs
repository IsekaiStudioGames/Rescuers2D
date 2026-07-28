//----- RescueZone.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class RescueZone : MonoBehaviour
{
    public event Action OnAllTargetsSafe;
    public event Action<int, int> OnProgressChanged;

    [Header("Discovery")]
    [Tooltip(
        "When enabled, the zone finds all active required " +
        "RescueTarget components when the scene begins.")]
    [SerializeField]
    private bool discoverRequiredTargetsOnStart = true;

    [Tooltip(
        "Optional explicit target list. This can be used instead " +
        "of automatic scene discovery.")]
    [SerializeField]
    private RescueTarget[] requiredTargets;

    [Header("Debug")]
    [SerializeField]
    private bool logTargetChanges = true;

    private readonly HashSet<RescueTarget> targetsInside =
        new HashSet<RescueTarget>();

    private readonly HashSet<RescueTarget> requiredTargetSet =
        new HashSet<RescueTarget>();

    public int RequiredTargetCount =>
        requiredTargetSet.Count;

    public int SafeTargetCount
    {
        get
        {
            int count = 0;

            foreach (RescueTarget target in requiredTargetSet)
            {
                if (target != null &&
                    targetsInside.Contains(target))
                {
                    count++;
                }
            }

            return count;
        }
    }

    public bool AreAllRequiredTargetsSafe =>
        RequiredTargetCount > 0 &&
        SafeTargetCount >= RequiredTargetCount;

    private bool completionReported;

    private void Awake()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();

        if (!zoneCollider.isTrigger)
        {
            Debug.LogWarning(
                "[RESCUE ZONE] The Rescue Zone collider must be " +
                "configured as a trigger. It has been corrected.",
                this);

            zoneCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        RefreshRequiredTargets();
        EvaluateRescueState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RescueTarget target =
            other.GetComponentInParent<RescueTarget>();

        if (target == null ||
            !target.RequiredForMission ||
            !requiredTargetSet.Contains(target))
        {
            return;
        }

        if (!targetsInside.Add(target))
        {
            return;
        }

        if (logTargetChanges)
        {
            Debug.Log(
                $"[RESCUE ZONE] {target.DisplayName} entered " +
                $"the Rescue Camp. {SafeTargetCount}/" +
                $"{RequiredTargetCount} safe.",
                target);
        }

        EvaluateRescueState();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RescueTarget target =
            other.GetComponentInParent<RescueTarget>();

        if (target == null ||
            !targetsInside.Remove(target))
        {
            return;
        }

        if (logTargetChanges)
        {
            Debug.Log(
                $"[RESCUE ZONE] {target.DisplayName} left " +
                $"the Rescue Camp. {SafeTargetCount}/" +
                $"{RequiredTargetCount} safe.",
                target);
        }

        completionReported = false;
        NotifyProgressChanged();
    }

    [ContextMenu("Refresh Required Targets")]
    public void RefreshRequiredTargets()
    {
        UnsubscribeFromTargets();

        requiredTargetSet.Clear();
        targetsInside.RemoveWhere(target => target == null);

        if (discoverRequiredTargetsOnStart)
        {
            RescueTarget[] sceneTargets =
                FindObjectsByType<RescueTarget>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None);

            foreach (RescueTarget target in sceneTargets)
            {
                AddRequiredTarget(target);
            }
        }
        else if (requiredTargets != null)
        {
            foreach (RescueTarget target in requiredTargets)
            {
                AddRequiredTarget(target);
            }
        }

        completionReported = false;
        SubscribeToTargets();
        NotifyProgressChanged();

        if (requiredTargetSet.Count == 0)
        {
            Debug.LogWarning(
                "[RESCUE ZONE] No required rescue targets were found.",
                this);
        }
    }

    [ContextMenu("Evaluate Rescue State")]
    public void EvaluateRescueState()
    {
        NotifyProgressChanged();

        if (!AreAllRequiredTargetsSafe ||
            completionReported)
        {
            return;
        }

        completionReported = true;

        Debug.Log(
            $"[RESCUE ZONE] All {RequiredTargetCount} required " +
            "mission targets are safe.",
            this);

        OnAllTargetsSafe?.Invoke();
    }

    private void AddRequiredTarget(RescueTarget target)
    {
        if (target == null ||
            !target.RequiredForMission)
        {
            return;
        }

        requiredTargetSet.Add(target);
    }

    private void NotifyProgressChanged()
    {
        OnProgressChanged?.Invoke(
            SafeTargetCount,
            RequiredTargetCount);
    }

    private void HandleRequirementChanged(
        RescueTarget changedTarget)
    {
        RefreshRequiredTargets();
        EvaluateRescueState();
    }

    private void SubscribeToTargets()
    {
        foreach (RescueTarget target in requiredTargetSet)
        {
            if (target != null)
            {
                target.OnRequirementChanged +=
                    HandleRequirementChanged;
            }
        }
    }

    private void UnsubscribeFromTargets()
    {
        foreach (RescueTarget target in requiredTargetSet)
        {
            if (target != null)
            {
                target.OnRequirementChanged -=
                    HandleRequirementChanged;
            }
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromTargets();
    }

    private void OnValidate()
    {
        Collider2D zoneCollider = GetComponent<Collider2D>();

        if (zoneCollider != null &&
            !zoneCollider.isTrigger)
        {
            Debug.LogWarning(
                "[RESCUE ZONE] Its Collider2D should have " +
                "Is Trigger enabled.",
                this);
        }
    }
}

//----- RescueZone.cs END -----