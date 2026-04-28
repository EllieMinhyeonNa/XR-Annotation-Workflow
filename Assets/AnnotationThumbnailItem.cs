using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Individual thumbnail item in the history panel
/// Shows preview image, delete button
/// </summary>
public class AnnotationThumbnailItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RawImage thumbnailImage;     // Shows the screenshot
    [SerializeField] private Button viewButton;           // Click to view full size
    [SerializeField] private Button deleteButton;         // Red X button
    [SerializeField] private TextMeshProUGUI nameText;    // Optional: show annotation name

    private AnnotationData annotationData;

    /// <summary>
    /// Setup this thumbnail with annotation data
    /// </summary>
    public void Setup(AnnotationData annotation)
    {
        this.annotationData = annotation;

        // Set thumbnail image
        if (thumbnailImage != null && annotation.screenshot != null)
        {
            thumbnailImage.texture = annotation.screenshot;
        }

        // Set name if text field exists
        if (nameText != null)
        {
            nameText.text = annotation.displayName;
        }

        // Setup view button
        if (viewButton != null)
        {
            viewButton.onClick.RemoveAllListeners();
            viewButton.onClick.AddListener(OnViewClicked);
        }

        // Setup delete button
        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeleteClicked);
        }
    }

    /// <summary>
    /// Called when user clicks the thumbnail to view it
    /// </summary>
    private void OnViewClicked()
    {
        if (AnnotationManager.Instance != null && annotationData != null)
        {
            AnnotationManager.Instance.ViewAnnotation(annotationData);
        }
    }

    /// <summary>
    /// Called when user clicks the delete (X) button
    /// </summary>
    public void OnDeleteClicked()
    {
        if (AnnotationManager.Instance != null && annotationData != null)
        {
            AnnotationManager.Instance.DeleteAnnotation(annotationData);
        }
    }
}
