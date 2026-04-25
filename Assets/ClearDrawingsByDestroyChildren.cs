using UnityEngine;

public class ClearDrawingsByDestroyChildren : MonoBehaviour
{
    [SerializeField] private Transform[] strokeRoots;

    public void ClearAll()
    {
        foreach (var root in strokeRoots)
        {
            if (!root) continue;
            for (int i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }
    }
}