using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Full-screen viewer for a single annotation
/// Allows viewing, renaming, and deleting
/// </summary>
public class AnnotationViewer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RawImage annotationImage;       // Main large image display
    [SerializeField] private TMP_InputField nameInputField;  // Editable name field
    [SerializeField] private Button editNameButton;          // Pencil icon to edit name
    [SerializeField] private Button deleteButton;            // Delete button
    [SerializeField] private Button closeButton;             // Close button
    [SerializeField] private GameObject keyboardPanel;       // Virtual keyboard (optional)

    private AnnotationData currentAnnotation;
    private bool isEditingName = false;

    void Start()
    {
        // Setup button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseViewer);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(OnDeleteClicked);

        if (editNameButton != null)
            editNameButton.onClick.AddListener(OnEditNameClicked);

        if (nameInputField != null)
        {
            nameInputField.onEndEdit.AddListener(OnNameEditFinished);
            nameInputField.interactable = false; // Start as non-editable
        }

        // Hide keyboard initially
        if (keyboardPanel != null)
            keyboardPanel.SetActive(false);

        // Start hidden
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Show annotation in viewer
    /// </summary>
    public void ShowAnnotation(AnnotationData annotation)
    {
        if (annotation == null)
        {
            Debug.LogWarning("Attempted to show null annotation");
            return;
        }

        currentAnnotation = annotation;

        // Show annotation image
        if (annotationImage != null && annotation.screenshot != null)
        {
            annotationImage.texture = annotation.screenshot;
        }

        // Show annotation name
        if (nameInputField != null)
        {
            nameInputField.text = annotation.displayName;
            nameInputField.interactable = false;
        }

        // Show the viewer panel
        gameObject.SetActive(true);

        Debug.Log($"Viewing annotation: {annotation.displayName}");
    }

    /// <summary>
    /// Close the viewer
    /// </summary>
    public void CloseViewer()
    {
        // Save any pending name changes
        if (isEditingName)
        {
            SaveNameChange();
        }

        currentAnnotation = null;
        gameObject.SetActive(false);

        // Hide keyboard
        if (keyboardPanel != null)
            keyboardPanel.SetActive(false);
    }

    /// <summary>
    /// Called when edit name button (pencil icon) is clicked
    /// </summary>
    private void OnEditNameClicked()
    {
        if (nameInputField == null) return;

        isEditingName = !isEditingName;

        if (isEditingName)
        {
            // Enable editing
            nameInputField.interactable = true;
            nameInputField.ActivateInputField();
            nameInputField.Select();

            // Show virtual keyboard if available
            if (keyboardPanel != null)
                keyboardPanel.SetActive(true);
        }
        else
        {
            // Disable editing and save
            SaveNameChange();
        }
    }

    /// <summary>
    /// Called when user finishes editing name (hits enter/return)
    /// </summary>
    private void OnNameEditFinished(string newName)
    {
        if (isEditingName)
        {
            SaveNameChange();
        }
    }

    /// <summary>
    /// Save the name change
    /// </summary>
    private void SaveNameChange()
    {
        if (currentAnnotation == null || nameInputField == null) return;

        string newName = nameInputField.text.Trim();

        if (!string.IsNullOrEmpty(newName) && newName != currentAnnotation.displayName)
        {
            // Update annotation name through manager
            if (AnnotationManager.Instance != null)
            {
                AnnotationManager.Instance.RenameAnnotation(currentAnnotation, newName);
                Debug.Log($"Annotation renamed to: {newName}");
            }
        }

        // Disable editing mode
        isEditingName = false;
        nameInputField.interactable = false;

        // Hide keyboard
        if (keyboardPanel != null)
            keyboardPanel.SetActive(false);
    }

    /// <summary>
    /// Called when delete button is clicked
    /// </summary>
    private void OnDeleteClicked()
    {
        if (currentAnnotation == null) return;

        // Delete through manager
        if (AnnotationManager.Instance != null)
        {
            AnnotationManager.Instance.DeleteAnnotation(currentAnnotation);
        }

        // Close viewer
        CloseViewer();
    }
}
