using UnityEngine;
using UnityEngine.XR.Hands;

public class TriplePinchGate : MonoBehaviour
{
    [SerializeField] XRHandTrackingEvents tracking;
    [SerializeField] float pinchDist = 0.02f; // 2cm 정도부터 튜닝

    public bool IsTriplePinching { get; private set; }

    void OnEnable()
    {
        if (tracking != null)
            tracking.jointsUpdated.AddListener(OnJointsUpdated);
    }

    void OnDisable()
    {
        if (tracking != null)
            tracking.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        var hand = args.hand;

        if (!TryGetTip(hand, XRHandJointID.ThumbTip, out var thumb)) return;
        if (!TryGetTip(hand, XRHandJointID.IndexTip, out var index)) return;
        if (!TryGetTip(hand, XRHandJointID.MiddleTip, out var middle)) return;

        bool thumbIndex = Vector3.Distance(thumb, index) < pinchDist;
        bool thumbMiddle = Vector3.Distance(thumb, middle) < pinchDist;

        IsTriplePinching = thumbIndex && thumbMiddle;

        if (IsTriplePinching)
        {
            Debug.Log("TRIPLE PINCH DETECTED");
        }
    }

    bool TryGetTip(XRHand hand, XRHandJointID id, out Vector3 pos)
    {
        var j = hand.GetJoint(id);
        if (!j.TryGetPose(out var pose))
        {
            pos = default;
            return false;
        }
        pos = pose.position;
        return true;
    }
}