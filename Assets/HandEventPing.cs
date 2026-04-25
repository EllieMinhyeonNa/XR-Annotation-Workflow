using UnityEngine;
using UnityEngine.XR.Hands;

public class HandEventPing : MonoBehaviour
{
    public XRHandTrackingEvents rightHandEvents;

    void OnEnable()
    {
        rightHandEvents.trackingAcquired.AddListener(() =>
        {
            Debug.Log("RIGHT tracking acquired");
        });

        rightHandEvents.trackingLost.AddListener(() =>
        {
            Debug.Log("RIGHT tracking lost");
        });
    }

    void OnDisable()
    {
        rightHandEvents.trackingAcquired.RemoveAllListeners();
        rightHandEvents.trackingLost.RemoveAllListeners();
    }

    // 👇 여기 그냥 추가하면 됨
    void Start()
    {
        Debug.Log("HandEventPing START");
    }

    void Update()
    {
        // 필요하면 로그 찍기
    }
}