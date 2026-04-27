using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows a preview of the last saved annotation in VR
/// Attach this to a UI panel with a RawImage component
/// </summary>
public class SavedAnnotationPreview : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private RawImage previewImage;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 3f; // How long to show the preview

    private GameObject previewPanel;

    void Start()
    {
        // Subscribe to save events
        if (AnnotationManager.Instance != null)
        {
            AnnotationManager.Instance.OnAnnotationSaved += ShowPreview;
        }

        // Get or create preview panel
        previewPanel = gameObject;

        // Start hidden
        previewPanel.SetActive(false);
    }

    void ShowPreview(AnnotationData annotation)
    {
        if (previewImage == null)
        {
            Debug.LogError("[SavedAnnotationPreview] No RawImage assigned!");
            return;
        }

        // Set the screenshot texture
        previewImage.texture = annotation.screenshot;

        // Show the panel
        previewPanel.SetActive(true);

        Debug.Log($"[SavedAnnotationPreview] Showing preview for {annotation.displayName}");

        // Auto-hide after delay
        Invoke(nameof(HidePreview), displayDuration);
    }

    void HidePreview()
    {
        previewPanel.SetActive(false);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (AnnotationManager.Instance != null)
        {
            AnnotationManager.Instance.OnAnnotationSaved -= ShowPreview;
        }
    }
}
