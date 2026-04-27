using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>
/// Adds a simple sphere collider to the index finger tip for touching buttons
/// Add this to your Right Hand Tracking and Left Hand Tracking objects
/// </summary>
public class HandColliderSetup : MonoBehaviour
{
    [Header("Settings")]
    public Handedness handedness = Handedness.Right;
    public float colliderRadius = 0.02f; // 2cm sphere on fingertip

    private GameObject fingerTipCollider;
    private SphereCollider sphereCollider;
    private UnityEngine.XR.Hands.XRHandTrackingEvents handTracking;

    void Start()
    {
        // Get hand tracking
        handTracking = GetComponent<UnityEngine.XR.Hands.XRHandTrackingEvents>();

        if (handTracking == null)
        {
            Debug.LogError($"[HandColliderSetup] No XRHandTrackingEvents found on {gameObject.name}");
            return;
        }

        // Create finger tip collider
        CreateFingerCollider();
    }

    void CreateFingerCollider()
    {
        fingerTipCollider = new GameObject($"{handedness}IndexFingerCollider");
        fingerTipCollider.transform.SetParent(transform);
        fingerTipCollider.layer = gameObject.layer;

        // Add sphere collider
        sphereCollider = fingerTipCollider.AddComponent<SphereCollider>();
        sphereCollider.radius = colliderRadius;
        sphereCollider.isTrigger = true;

        // Add rigidbody (required for trigger detection)
        Rigidbody rb = fingerTipCollider.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Debug.Log($"[HandColliderSetup] Created finger collider for {handedness} hand");
    }

    void Update()
    {
        if (fingerTipCollider == null || handTracking == null)
            return;

        UpdateFingerPosition();
    }

    void UpdateFingerPosition()
    {
        var subsystem = handTracking.subsystem;
        if (subsystem == null)
            return;

        XRHand hand = handedness == Handedness.Left ? subsystem.leftHand : subsystem.rightHand;

        if (!hand.isTracked)
            return;

        // Get index finger tip position
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose tipPose))
        {
            fingerTipCollider.transform.position = tipPose.position;
            fingerTipCollider.transform.rotation = tipPose.rotation;
        }
    }
}
