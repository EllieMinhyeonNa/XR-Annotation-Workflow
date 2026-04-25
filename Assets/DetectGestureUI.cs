using UnityEngine;
using TMPro;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Hands.Samples.GestureSample;

public class DetectGestureUI : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRHandTrackingEvents handTrackingEvents;
    [SerializeField] private XRHandShape[] handShapes;
    [SerializeField] private float gestureDetectionInterval = 0.1f;
    [SerializeField] private float minimumDetectionThreshold = 0.9f;
    [SerializeField] private HandShapeCompletenessCalculator handShapeCompletenessCalculator;

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private float messageHoldSeconds = 3f;

    private float timeOfLastConditionCheck;
    private float lastMessageTime;

    void Start()
    {
        // ✅ UI가 제대로 보이는지 강제 테스트용
        SetText("HELLO VR UI");
    }

    void OnEnable()
    {
        if (handTrackingEvents != null)
            handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
    }

    void OnDisable()
    {
        if (handTrackingEvents != null)
            handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    void Update()
    {
        // 메시지를 잠깐 보여주고 지우고 싶으면 사용
        if (statusText != null && Time.time - lastMessageTime > messageHoldSeconds)
            statusText.text = "";
    }

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        if (Time.time - timeOfLastConditionCheck < gestureDetectionInterval)
            return;

        // 안전장치: 참조가 빠지면 UI로 바로 알려주기
        if (handShapeCompletenessCalculator == null)
        {
            SetText("❌ Missing: HandShapeCompletenessCalculator");
            timeOfLastConditionCheck = Time.time;
            return;
        }

        if (handShapes == null || handShapes.Length == 0)
        {
            SetText("❌ HandShapes is empty");
            timeOfLastConditionCheck = Time.time;
            return;
        }

        // 추적 상태
        if (handTrackingEvents == null || !handTrackingEvents.handIsTracked)
        {
            SetText("Hand: NOT TRACKED");
            timeOfLastConditionCheck = Time.time;
            return;
        }

        // 제스처 검사
        foreach (var handShape in handShapes)
        {
            if (handShape == null) continue;

            handShapeCompletenessCalculator.TryCalculateHandShapeCompletenessScore(
                eventArgs.hand,
                handShape,
                out float score
            );

            if (score >= minimumDetectionThreshold)
            {
                string msg = $"✅ Detected: {handShape.name}\nScore: {score:0.00}";
                SetText(msg);
                Debug.Log($"Hand Gesture Detected: {handShape.name} | Score: {score}");
                break; // 여러 개 뜨면 보기 힘드니까 1개만 표시
            }
        }

        timeOfLastConditionCheck = Time.time;
    }

    void SetText(string msg)
    {
        if (statusText == null) return;
        statusText.text = msg;
        lastMessageTime = Time.time;
    }
}