using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Core manager for the annotation system
/// Handles saving, loading, and managing annotations
/// </summary>
public class AnnotationManager : MonoBehaviour
{
    [Header("Screenshot Settings")]
    [SerializeField] private Camera captureCamera;
    [SerializeField] private int screenshotWidth = 1920;
    [SerializeField] private int screenshotHeight = 1080;

    [Header("Storage")]
    [SerializeField] private string saveFolderName = "Annotations";

    [Header("References")]
    [SerializeField] private ClearDrawingsByDestroyChildren clearDrawings;

    [Header("UI References (assign these)")]
    [SerializeField] private AnnotationHistoryPanel historyPanel;
    [SerializeField] private AnnotationViewer annotationViewer;

    // List of all saved annotations
    private List<AnnotationData> savedAnnotations = new List<AnnotationData>();

    // Singleton for easy access
    public static AnnotationManager Instance { get; private set; }

    // Events
    public System.Action<AnnotationData> OnAnnotationSaved;
    public System.Action<AnnotationData> OnAnnotationDeleted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Use main camera if not assigned
        if (captureCamera == null)
            captureCamera = Camera.main;
    }

    void Start()
    {
        // Load existing annotations from disk on startup
        LoadAnnotationsFromDisk();
    }

    /// <summary>
    /// Capture current view and save as annotation
    /// </summary>
    public void SaveAnnotation()
    {
        StartCoroutine(CaptureAndSave());
    }

    private IEnumerator CaptureAndSave()
    {
        // Wait until end of frame to capture
        yield return new WaitForEndOfFrame();

        // Create screenshot
        Texture2D screenshot = CaptureScreenshot();

        if (screenshot == null)
        {
            Debug.LogError("Failed to capture screenshot");
            yield break;
        }

        // Create annotation data
        AnnotationData annotation = new AnnotationData(screenshot);

        // Save to disk
        string filePath = SaveScreenshotToDisk(screenshot, annotation.id);
        annotation.filePath = filePath;

        // Add to list
        savedAnnotations.Add(annotation);

        // Notify listeners
        OnAnnotationSaved?.Invoke(annotation);

        // Update UI
        if (historyPanel != null)
            historyPanel.AddAnnotationThumbnail(annotation);

        Debug.Log($"Annotation saved: {annotation.displayName} at {filePath}");
    }

    /// <summary>
    /// Delete all current drawings (like pressing Delete button)
    /// </summary>
    public void DeleteCurrentDrawings()
    {
        if (clearDrawings != null)
        {
            clearDrawings.ClearAll();
            Debug.Log("Current drawings cleared");
        }
    }

    /// <summary>
    /// Delete a saved annotation
    /// </summary>
    public void DeleteAnnotation(AnnotationData annotation)
    {
        if (!savedAnnotations.Contains(annotation))
            return;

        // Delete file from disk
        if (!string.IsNullOrEmpty(annotation.filePath) && File.Exists(annotation.filePath))
        {
            File.Delete(annotation.filePath);
        }

        // Remove from list
        savedAnnotations.Remove(annotation);

        // Notify listeners
        OnAnnotationDeleted?.Invoke(annotation);

        // Update UI
        if (historyPanel != null)
            historyPanel.RemoveAnnotationThumbnail(annotation);

        Debug.Log($"Annotation deleted: {annotation.displayName}");
    }

    /// <summary>
    /// Rename an annotation
    /// </summary>
    public void RenameAnnotation(AnnotationData annotation, string newName)
    {
        if (!savedAnnotations.Contains(annotation))
            return;

        annotation.displayName = newName;

        // Update UI if needed
        if (historyPanel != null)
            historyPanel.RefreshThumbnails();

        Debug.Log($"Annotation renamed to: {newName}");
    }

    /// <summary>
    /// Get all saved annotations
    /// </summary>
    public List<AnnotationData> GetAllAnnotations()
    {
        return new List<AnnotationData>(savedAnnotations);
    }

    /// <summary>
    /// Get count of saved annotations
    /// </summary>
    public int GetAnnotationCount()
    {
        return savedAnnotations.Count;
    }

    /// <summary>
    /// Capture screenshot from camera
    /// </summary>
    private Texture2D CaptureScreenshot()
    {
        if (captureCamera == null)
        {
            Debug.LogError("Capture camera not assigned!");
            return null;
        }

        // Create render texture
        RenderTexture rt = new RenderTexture(screenshotWidth, screenshotHeight, 24);
        RenderTexture currentRT = RenderTexture.active;

        // Render camera view to texture
        captureCamera.targetTexture = rt;
        captureCamera.Render();

        // Read pixels
        RenderTexture.active = rt;
        Texture2D screenshot = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
        screenshot.Apply();

        // Cleanup
        captureCamera.targetTexture = null;
        RenderTexture.active = currentRT;
        Destroy(rt);

        return screenshot;
    }

    /// <summary>
    /// Save screenshot to disk as PNG
    /// </summary>
    private string SaveScreenshotToDisk(Texture2D texture, string id)
    {
        // Get persistent data path
        string folderPath = Path.Combine(Application.persistentDataPath, saveFolderName);

        // Create folder if it doesn't exist
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Create file path
        string fileName = $"{id}.png";
        string filePath = Path.Combine(folderPath, fileName);

        // Encode and save
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        return filePath;
    }

    /// <summary>
    /// Load annotations from disk on startup
    /// </summary>
    private void LoadAnnotationsFromDisk()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, saveFolderName);

        if (!Directory.Exists(folderPath))
            return;

        string[] files = Directory.GetFiles(folderPath, "*.png");

        foreach (string filePath in files)
        {
            // Load texture
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            // Create annotation data
            string id = Path.GetFileNameWithoutExtension(filePath);
            AnnotationData annotation = new AnnotationData(texture)
            {
                id = id,
                filePath = filePath
            };

            savedAnnotations.Add(annotation);
        }

        Debug.Log($"Loaded {savedAnnotations.Count} annotations from disk");

        // Refresh UI
        if (historyPanel != null)
            historyPanel.RefreshThumbnails();
    }

    /// <summary>
    /// Open annotation viewer for a specific annotation
    /// </summary>
    public void ViewAnnotation(AnnotationData annotation)
    {
        if (annotationViewer != null)
        {
            annotationViewer.ShowAnnotation(annotation);
        }
    }
}
