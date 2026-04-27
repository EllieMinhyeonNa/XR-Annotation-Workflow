using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Super simple button that triggers when hand touches it
/// Works with both UI Images and 3D renderers
/// </summary>
public class SimpleTouchButton : MonoBehaviour
{
    [Header("Button Action")]
    public UnityEvent onButtonPressed;

    [Header("Visual Feedback (Optional)")]
    public Color pressedColor = Color.green;
    public Color normalColor = Color.white;

    private Image buttonImage;        // For UI buttons
    private Renderer buttonRenderer;  // For 3D buttons
    private bool isPressed = false;

    void Start()
    {
        // Try to get UI Image component first
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
            Debug.Log($"[SimpleTouchButton] Using UI Image for {gameObject.name}");
        }
        else
        {
            // Fall back to Renderer for 3D objects
            buttonRenderer = GetComponent<Renderer>();
            if (buttonRenderer != null)
            {
                buttonRenderer.material.color = normalColor;
                Debug.Log($"[SimpleTouchButton] Using Renderer for {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"[SimpleTouchButton] No Image or Renderer found on {gameObject.name}. Color feedback won't work.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if a hand touched this button
        if (other.gameObject.name.Contains("Hand") || other.gameObject.name.Contains("Index"))
        {
            PressButton();
        }
    }

    void PressButton()
    {
        if (isPressed) return; // Prevent multiple triggers

        isPressed = true;
        Debug.Log($"[SimpleTouchButton] Button {gameObject.name} pressed!");

        // Visual feedback - try UI Image first, then Renderer
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
        }
        else if (buttonRenderer != null)
        {
            buttonRenderer.material.color = pressedColor;
        }

        // Trigger the action
        onButtonPressed?.Invoke();

        // Reset after a short delay
        Invoke(nameof(ResetButton), 0.5f);
    }

    void ResetButton()
    {
        isPressed = false;

        // Reset color - try UI Image first, then Renderer
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
        }
        else if (buttonRenderer != null)
        {
            buttonRenderer.material.color = normalColor;
        }
    }
}
