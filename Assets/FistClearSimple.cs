using UnityEngine;

public class FistClearSimple : MonoBehaviour
{
    [Header("Gesture Detection")]
    [SerializeField] private DetectGesture fistDetectorRight;  // Right hand fist gesture
    [SerializeField] private DetectGesture fistDetectorLeft;   // Left hand fist gesture (optional)

    [Header("Clear Target")]
    [SerializeField] private ClearDrawingsByDestroyChildren clearTarget;

    [Header("Settings")]
    [SerializeField] private bool requireBothHands = false;     // If true, both hands must be fist
    [SerializeField] private bool enableDebugLogs = true;

    private bool wasRightFist = false;
    private bool wasLeftFist = false;

    void Update()
    {
        if (!clearTarget)
        {
            if (enableDebugLogs)
                Debug.LogWarning("FistClearSimple: clearTarget not assigned!");
            return;
        }

        // Check right hand
        bool nowRightFist = fistDetectorRight != null && fistDetectorRight.IsDetected;

        // Check left hand (optional)
        bool nowLeftFist = fistDetectorLeft != null && fistDetectorLeft.IsDetected;

        // Determine if we should clear
        bool shouldClear = false;

        if (requireBothHands)
        {
            // Both hands must be fist
            bool bothFist = nowRightFist && nowLeftFist;
            bool wasBothFist = wasRightFist && wasLeftFist;

            // Trigger on first moment both become fist
            if (bothFist && !wasBothFist)
            {
                shouldClear = true;
                if (enableDebugLogs)
                    Debug.Log("FistClearSimple: Both hands fist detected! Clearing drawings...");
            }
        }
        else
        {
            // Either hand can trigger clear
            bool eitherFist = nowRightFist || nowLeftFist;
            bool wasEitherFist = wasRightFist || wasLeftFist;

            // Trigger on first moment either becomes fist
            if (eitherFist && !wasEitherFist)
            {
                shouldClear = true;
                string hand = nowRightFist ? "Right" : "Left";
                if (enableDebugLogs)
                    Debug.Log($"FistClearSimple: {hand} hand fist detected! Clearing drawings...");
            }
        }

        // Execute clear
        if (shouldClear)
        {
            clearTarget.ClearAll();
            if (enableDebugLogs)
                Debug.Log("FistClearSimple: Drawings cleared!");
        }

        // Update previous state
        wasRightFist = nowRightFist;
        wasLeftFist = nowLeftFist;
    }

    // Debug visualization in inspector
    void OnValidate()
    {
        if (fistDetectorRight == null && fistDetectorLeft == null)
        {
            Debug.LogWarning("FistClearSimple: No fist detectors assigned! Please assign at least one.");
        }

        if (clearTarget == null)
        {
            Debug.LogWarning("FistClearSimple: clearTarget not assigned!");
        }
    }
}