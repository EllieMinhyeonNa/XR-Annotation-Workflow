using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Hands.Samples.GestureSample;

public class DetectGesture : MonoBehaviour
{
    [SerializeField] private XRHandTrackingEvents handTrackingEvents;
    [SerializeField] private XRHandShape[] handShapes;
    [SerializeField] private float gestureDetectionInterval = 0.02f;
    [SerializeField] private float minimumDetectionThreshold = 0.82f;
    [SerializeField] private HandShapeCompletenessCalculator handShapeCompletenessCalculator;

    [Header("Hard Stop (Release)")]
    [SerializeField] private float thumbIndexReleaseDistance = 0.02f; // 0.03~0.06에서 튜닝

    private float timeOfLastConditionCheck;

    public bool IsDetected { get; private set; }
    public float LastScore { get; private set; }

    void OnEnable()  => handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
    void OnDisable() => handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        // 0) 트래킹 끊기면 즉시 false
        if (!handTrackingEvents.handIsTracked)
        {
            IsDetected = false;
            LastScore = 0f;
            return;
        }

        // 1) HARD STOP: 이미 감지 중일 때, 엄지-검지 벌어지면 즉시 false (interval 무시)
        if (IsDetected)
        {
            if (eventArgs.hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumbTipPose) &&
                eventArgs.hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexTipPose))
            {
                float d = Vector3.Distance(thumbTipPose.position, indexTipPose.position);
                if (d > thumbIndexReleaseDistance)
                {
                    IsDetected = false;
                    LastScore = 0f;
                    return;
                }
            }
        }

        // 2) interval gate (감지 계산만 주기적으로)
        if (Time.time - timeOfLastConditionCheck < gestureDetectionInterval)
            return;

        bool anyDetected = false;
        float bestScore = 0f;

        foreach (var handShape in handShapes)
        {
            handShapeCompletenessCalculator.TryCalculateHandShapeCompletenessScore(
                eventArgs.hand,
                handShape,
                out float completenessScore
            );

            bool detected = completenessScore >= minimumDetectionThreshold;

            if (detected)
            {
                anyDetected = true;
                if (completenessScore > bestScore) bestScore = completenessScore;

                Debug.Log($"Hand Gesture Detected: {handShape.name} | Score: {completenessScore}");
            }
        }

        IsDetected = anyDetected;
        LastScore = bestScore;
        timeOfLastConditionCheck = Time.time;
    }
}