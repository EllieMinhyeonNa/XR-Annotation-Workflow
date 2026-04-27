using System;
using UnityEngine;

/// <summary>
/// Represents a single saved annotation with its metadata
/// </summary>
[Serializable]
public class AnnotationData
{
    public string id;               // Unique identifier
    public string displayName;       // User-editable name
    public Texture2D screenshot;     // The captured image
    public DateTime timestamp;       // When it was created
    public string filePath;          // Path where screenshot is saved

    public AnnotationData()
    {
        id = Guid.NewGuid().ToString();
        timestamp = DateTime.Now;
        displayName = $"capture_{timestamp:yyyyMMdd_HHmmss}";
    }

    public AnnotationData(Texture2D screenshot) : this()
    {
        this.screenshot = screenshot;
    }
}
