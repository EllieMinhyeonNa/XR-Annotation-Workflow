using UnityEngine;

/// <summary>
/// Clears all drawing strokes by destroying children of specified root transforms
/// </summary>
public class ClearDrawingsByDestroyChildren : MonoBehaviour
{
    [Header("Stroke Organization")]
    [SerializeField] private Transform[] strokeRoots;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    /// <summary>
    /// Clear all strokes under the specified roots
    /// </summary>
    public void ClearAll()
    {
        int totalCleared = 0;

        foreach (var root in strokeRoots)
        {
            if (!root)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("ClearDrawingsByDestroyChildren: Null stroke root!");
                continue;
            }

            int childCount = root.childCount;

            // Destroy all children (strokes)
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(root.GetChild(i).gameObject);
                totalCleared++;
            }
        }

        if (enableDebugLogs)
            Debug.Log($"ClearDrawingsByDestroyChildren: Cleared {totalCleared} stroke(s)");
    }

    /// <summary>
    /// Get total number of strokes currently drawn
    /// </summary>
    public int GetStrokeCount()
    {
        int count = 0;
        foreach (var root in strokeRoots)
        {
            if (root != null)
                count += root.childCount;
        }
        return count;
    }

    // Validation in editor
    void OnValidate()
    {
        if (strokeRoots == null || strokeRoots.Length == 0)
        {
            Debug.LogWarning("ClearDrawingsByDestroyChildren: No stroke roots assigned!");
        }
    }
}