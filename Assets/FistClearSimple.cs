using UnityEngine;

public class FistClearSimple : MonoBehaviour
{
    [SerializeField] private DetectGesture fistDetector; // DetectGestures_Fist_Right에 있는 DetectGesture
    [SerializeField] private ClearDrawingsByDestroyChildren clearTarget;

    bool was = false;

    void Update()
    {
        if (!fistDetector || !clearTarget) return;

        bool now = fistDetector.IsDetected;

        // 주먹이 "처음" 감지되는 순간에만 1번 삭제
        if (now && !was)
            clearTarget.ClearAll();

        was = now;
    }
}