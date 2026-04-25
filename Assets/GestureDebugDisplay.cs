using UnityEngine;

public class GestureDebugDisplay : MonoBehaviour
{
    public DetectGesture rightDetector;
    public DetectGesture leftDetector;

    bool prevRight;
    bool prevLeft;

    void Update()
    {
        if (rightDetector != null && rightDetector.IsDetected != prevRight)
        {
            Debug.Log($"[Right] IsDetected -> {rightDetector.IsDetected} | Score: {rightDetector.LastScore:F2}");
            prevRight = rightDetector.IsDetected;
        }

        if (leftDetector != null && leftDetector.IsDetected != prevLeft)
        {
            Debug.Log($"[Left] IsDetected -> {leftDetector.IsDetected} | Score: {leftDetector.LastScore:F2}");
            prevLeft = leftDetector.IsDetected;
        }
    }
}