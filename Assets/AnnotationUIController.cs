using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the main annotation UI buttons (Save, Delete) and counter
/// </summary>
public class AnnotationUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button saveButton;              // "Save" button
    [SerializeField] private Button deleteButton;            // "Delete" button
    [SerializeField] private Button historyButton;           // Button with counter (e.g., "0 ≡")
    [SerializeField] private TextMeshProUGUI counterText;    // Text showing annotation count

    [Header("References")]
    [SerializeField] private AnnotationHistoryPanel historyPanel;

    void Start()
    {
        // Setup button listeners
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveClicked);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(OnDeleteClicked);

        if (historyButton != null)
            historyButton.onClick.AddListener(OnHistoryClicked);

        // Subscribe to annotation events
        if (AnnotationManager.Instance != null)
        {
            AnnotationManager.Instance.OnAnnotationSaved += OnAnnotationCountChanged;
            AnnotationManager.Instance.OnAnnotationDeleted += OnAnnotationCountChanged;
        }

        // Initial counter update
        UpdateCounter();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (AnnotationManager.Instance != null)
        {
            AnnotationManager.Instance.OnAnnotationSaved -= OnAnnotationCountChanged;
            AnnotationManager.Instance.OnAnnotationDeleted -= OnAnnotationCountChanged;
        }
    }

    /// <summary>
    /// Called when Save button is clicked
    /// </summary>
    private void OnSaveClicked()
    {
        if (AnnotationManager.Instance != null)
        {
            AnnotationManager.Instance.SaveAnnotation();
            Debug.Log("Save button clicked");
        }
    }

    /// <summary>
    /// Called when Delete button is clicked
    /// Clears all current drawings
    /// </summary>
    private void OnDeleteClicked()
    {
        if (AnnotationManager.Instance != null)
        {
            AnnotationManager.Instance.DeleteCurrentDrawings();
            Debug.Log("Delete button clicked");
        }
    }

    /// <summary>
    /// Called when history button (counter) is clicked
    /// Opens the annotation history panel
    /// </summary>
    private void OnHistoryClicked()
    {
        if (historyPanel != null)
        {
            historyPanel.TogglePanel();
            Debug.Log("History panel toggled");
        }
    }

    /// <summary>
    /// Called when annotation count changes
    /// </summary>
    private void OnAnnotationCountChanged(AnnotationData annotation)
    {
        UpdateCounter();
    }

    /// <summary>
    /// Update the counter display
    /// </summary>
    private void UpdateCounter()
    {
        int count = 0;

        if (AnnotationManager.Instance != null)
        {
            count = AnnotationManager.Instance.GetAnnotationCount();
        }

        if (counterText != null)
        {
            counterText.text = count.ToString();
        }

        Debug.Log($"Annotation count updated: {count}");
    }
}
