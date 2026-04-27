using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Right-side panel showing thumbnails of saved annotations
/// </summary>
public class AnnotationHistoryPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject thumbnailPrefab;  // Prefab for each thumbnail item
    [SerializeField] private Transform thumbnailContainer; // ScrollView content area
    [SerializeField] private Button closeButton;

    private Dictionary<AnnotationData, GameObject> thumbnailObjects = new Dictionary<AnnotationData, GameObject>();

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        // Start hidden
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show or hide the panel
    /// </summary>
    public void TogglePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    /// <summary>
    /// Show the panel
    /// </summary>
    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide the panel
    /// </summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Add a new annotation thumbnail to the panel
    /// </summary>
    public void AddAnnotationThumbnail(AnnotationData annotation)
    {
        if (thumbnailPrefab == null || thumbnailContainer == null)
        {
            Debug.LogError("AnnotationHistoryPanel: thumbnailPrefab or thumbnailContainer not assigned");
            return;
        }

        // Create thumbnail object
        GameObject thumbnailObj = Instantiate(thumbnailPrefab, thumbnailContainer);
        thumbnailObjects[annotation] = thumbnailObj;

        // Set up thumbnail
        var thumbnailItem = thumbnailObj.GetComponent<AnnotationThumbnailItem>();
        if (thumbnailItem != null)
        {
            thumbnailItem.Setup(annotation);
        }
        else
        {
            Debug.LogWarning("Thumbnail prefab missing AnnotationThumbnailItem component");
        }
    }

    /// <summary>
    /// Remove an annotation thumbnail from the panel
    /// </summary>
    public void RemoveAnnotationThumbnail(AnnotationData annotation)
    {
        if (thumbnailObjects.TryGetValue(annotation, out GameObject thumbnailObj))
        {
            Destroy(thumbnailObj);
            thumbnailObjects.Remove(annotation);
        }
    }

    /// <summary>
    /// Refresh all thumbnails (e.g., after loading from disk)
    /// </summary>
    public void RefreshThumbnails()
    {
        // Clear existing thumbnails
        ClearAllThumbnails();

        // Get all annotations from manager
        if (AnnotationManager.Instance != null)
        {
            var annotations = AnnotationManager.Instance.GetAllAnnotations();
            foreach (var annotation in annotations)
            {
                AddAnnotationThumbnail(annotation);
            }
        }
    }

    /// <summary>
    /// Clear all thumbnails
    /// </summary>
    private void ClearAllThumbnails()
    {
        foreach (var thumbnailObj in thumbnailObjects.Values)
        {
            if (thumbnailObj != null)
                Destroy(thumbnailObj);
        }
        thumbnailObjects.Clear();
    }
}
