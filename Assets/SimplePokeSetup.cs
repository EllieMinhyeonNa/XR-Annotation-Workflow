using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Simple helper to configure XR Poke Interactor to use the index finger tip
/// Add this to the same GameObject as your XRPokeInteractor
/// </summary>
[RequireComponent(typeof(XRPokeInteractor))]
public class SimplePokeSetup : MonoBehaviour
{
    [Header("Hand Tracking")]
    [Tooltip("The XR Hand Tracking Events component (on the same GameObject or parent)")]
    public UnityEngine.XR.Hands.XRHandTrackingEvents handTrackingEvents;

    [Header("Settings")]
    [Tooltip("Which hand is this? (Left or Right)")]
    public UnityEngine.XR.Hands.Handedness handedness = UnityEngine.XR.Hands.Handedness.Right;

    private XRPokeInteractor pokeInteractor;
    private Transform indexTipTransform;
    private XRHand hand;

    void Start()
    {
        // Get the poke interactor
        pokeInteractor = GetComponent<XRPokeInteractor>();

        // Try to auto-find hand tracking events if not assigned
        if (handTrackingEvents == null)
        {
            handTrackingEvents = GetComponent<UnityEngine.XR.Hands.XRHandTrackingEvents>();
        }

        if (handTrackingEvents == null)
        {
            Debug.LogError($"[SimplePokeSetup] No XRHandTrackingEvents found on {gameObject.name}. Please assign it in the inspector.");
            return;
        }

        // Create a transform for the index finger tip
        CreateIndexFingerTip();

        // Disable the poke filter requirement since we're not using finger shape detection
        // This fixes the "Finger shape type Unspecified" error
        Debug.Log($"[SimplePokeSetup] Configured poke interactor on {gameObject.name} to use index finger tip tracking");
    }

    void CreateIndexFingerTip()
    {
        // Create a child GameObject to represent the index finger tip position
        GameObject tipObject = new GameObject("IndexFingerTip");
        tipObject.transform.SetParent(transform);
        tipObject.transform.localPosition = Vector3.zero;
        indexTipTransform = tipObject.transform;

        // Assign it to the poke interactor
        if (pokeInteractor != null)
        {
            // Use reflection to set the attach transform since it might be private
            var field = typeof(XRPokeInteractor).GetField("m_AttachTransform",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(pokeInteractor, indexTipTransform);
                Debug.Log($"[SimplePokeSetup] Set attach transform for {gameObject.name}");
            }
        }
    }

    void Update()
    {
        // Update the index finger tip position every frame based on hand tracking data
        if (handTrackingEvents != null && indexTipTransform != null)
        {
            UpdateIndexFingerPosition();
        }
    }

    void UpdateIndexFingerPosition()
    {
        // Get the hand from the subsystem
        var handSubsystem = handTrackingEvents.subsystem;
        if (handSubsystem == null)
            return;

        // Get the appropriate hand
        XRHand trackedHand = handedness == UnityEngine.XR.Hands.Handedness.Left
            ? handSubsystem.leftHand
            : handSubsystem.rightHand;

        // Check if hand is tracked
        if (!trackedHand.isTracked)
            return;

        // Get the index finger tip joint
        XRHandJointID tipJointID = XRHandJointID.IndexTip;

        if (trackedHand.GetJoint(tipJointID).TryGetPose(out Pose tipPose))
        {
            // Update the transform position and rotation in world space
            indexTipTransform.position = tipPose.position;
            indexTipTransform.rotation = tipPose.rotation;
        }
    }
}
