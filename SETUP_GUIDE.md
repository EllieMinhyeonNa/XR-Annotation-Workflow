# XR Annotation Workflow - Hand Gesture Mode Setup Guide

## Overview
This guide will help you set up the hand gesture annotation workflow system in Unity.

**Note:** This is for HAND GESTURE mode only. Controller mode will be added later.

---

## Scripts Created

### Core System
1. **AnnotationData.cs** - Data structure for annotations
2. **AnnotationManager.cs** - Main manager (screenshot, save, load, delete)
3. **AnnotationUIController.cs** - Controls Save/Delete buttons and counter

### UI Components
4. **AnnotationHistoryPanel.cs** - Right-side panel with thumbnails
5. **AnnotationThumbnailItem.cs** - Individual thumbnail in history
6. **AnnotationViewer.cs** - Full-screen viewer with rename/delete

### Optional (Not Used)
7. **FistClearSimple.cs** - Fist gesture clear (not needed - use Delete button instead)

---

## Unity Setup Steps

### 1. Setup Stroke Organization

1. Create empty GameObject: `StrokeContainer_Right` (for right hand strokes)
2. Create empty GameObject: `StrokeContainer_Left` (for left hand strokes, if using)
3. These will organize your drawn strokes

### 2. Update DrawWhileThreePinch Components

For each DrawWhileThreePinch component (right and left hand):
1. Find the component in your scene
2. In Inspector, find the new **Stroke Parent** field
3. Assign the corresponding StrokeContainer:
   - Right hand → `StrokeContainer_Right`
   - Left hand → `StrokeContainer_Left`

### 3. Setup ClearDrawingsByDestroyChildren

1. Find or create GameObject with `ClearDrawingsByDestroyChildren` component
2. Configure in Inspector:
   - **Stroke Roots**: Set size to 2 (or 1 if only using one hand)
   - Element 0: Assign `StrokeContainer_Right`
   - Element 1: Assign `StrokeContainer_Left` (if using both hands)
   - **Enable Debug Logs**: Keep checked for testing

### 4. Create AnnotationManager GameObject

1. Create empty GameObject: `AnnotationManager`
2. Add component: `AnnotationManager.cs`
3. Configure in Inspector:
   - **Capture Camera**: Assign your Main Camera
   - **Screenshot Width**: 1920 (or desired resolution)
   - **Screenshot Height**: 1080
   - **Clear Drawings**: Assign your ClearDrawingsByDestroyChildren object

### 2. Create UI Canvas (if not exists)

1. Create UI Canvas (Screen Space - Camera for VR)
2. Set Canvas camera to Main Camera
3. Add Canvas Scaler component

### 3. Create Main UI Panel (Save/Delete Buttons)

Create a panel at the bottom with:
- **Delete Button** (dark brown)
- **Save Button** (dark brown)
- **History Button** with counter text (shows "0 ≡")

#### Setup:
1. Create GameObject: `MainUI`
2. Add component: `AnnotationUIController.cs`
3. Assign in Inspector:
   - **Save Button**: Your Save button
   - **Delete Button**: Your Delete button
   - **History Button**: Your counter button
   - **Counter Text**: TextMeshPro showing the number
   - **History Panel**: (assign after creating history panel)

### 4. Create History Panel (Right Side)

1. Create Panel: `HistoryPanel`
2. Add `AnnotationHistoryPanel.cs` component
3. Inside HistoryPanel, create:
   - **ScrollView** with vertical layout
   - **Content** area (this will hold thumbnails)
   - **Close Button** (X button)

4. Configure AnnotationHistoryPanel:
   - **Thumbnail Prefab**: (create next)
   - **Thumbnail Container**: Assign the ScrollView's Content
   - **Close Button**: Assign X button

### 5. Create Thumbnail Prefab

1. Create new GameObject: `ThumbnailItem`
2. Add these UI elements as children:
   - **RawImage** - Shows screenshot thumbnail
   - **Button** - Click to view (covers whole thumbnail)
   - **Delete Button** - Small red X button in corner
   - Optional: **TextMeshPro** - Shows annotation name

3. Add component: `AnnotationThumbnailItem.cs`
4. Assign in Inspector:
   - **Thumbnail Image**: The RawImage
   - **View Button**: The main button
   - **Delete Button**: The red X button
   - **Name Text**: (optional) TextMeshPro

5. **Save as Prefab** in Assets folder

### 6. Create Annotation Viewer (Full Screen)

1. Create Panel: `AnnotationViewer` (full screen, initially disabled)
2. Add component: `AnnotationViewer.cs`
3. Add these UI children:
   - **RawImage** (large, centered) - Shows full annotation
   - **TMP InputField** - Shows/edits annotation name
   - **Edit Button** (pencil icon) - Enables name editing
   - **Delete Button** - Deletes this annotation
   - **Close Button** - Closes viewer

4. Configure AnnotationViewer:
   - **Annotation Image**: The large RawImage
   - **Name Input Field**: The TMP InputField
   - **Edit Name Button**: Pencil button
   - **Delete Button**: Delete button
   - **Close Button**: Close button
   - **Keyboard Panel**: (optional) Virtual keyboard

### 7. Connect Everything to AnnotationManager

Go back to AnnotationManager Inspector:
- **History Panel**: Assign your HistoryPanel object
- **Annotation Viewer**: Assign your AnnotationViewer object

Also in HistoryPanel:
- **Thumbnail Prefab**: Assign the ThumbnailItem prefab you created

### 8. (Optional) FistClearSimple

**Note:** You don't need this! Just use the Delete button instead.

If you want fist gesture as an alternative way to clear:
1. Add `FistClearSimple` component to a GameObject
2. Assign fist detector and clear target

---

## Hand Gesture Workflow Testing

### 1. Draw with Three-Finger Pinch
1. Make three-finger pinch gesture (thumb + index + middle)
2. Pencil should appear
3. Move hand to draw in 3D space
4. Release pinch to stop drawing

### 2. Save Annotation
1. Draw some annotations
2. **Tap Save button** (or use three-finger pinch gesture if configured)
3. Screenshot captured
4. Counter increments (0 → 1)
5. Check console: "Annotation saved: capture_..."

### 3. Delete Current Drawings
1. Draw some annotations
2. **Tap Delete button**
3. All current drawings cleared
4. Counter stays the same (only clears unsaved drawings)

### 4. View History Panel
1. **Tap the counter button** (shows "1 ≡")
2. Right panel opens with thumbnails
3. See your saved annotation(s)

### 5. View Full Annotation
1. In history panel, **tap a thumbnail**
2. Full-screen viewer opens
3. See large view of annotation

### 6. Rename Annotation
1. In viewer, **tap pencil icon** next to name
2. Virtual keyboard appears (if configured)
3. Type new name
4. Press enter or tap outside to save

### 7. Delete Saved Annotation
1. Option A: In viewer, **tap Delete button**
2. Option B: In history panel, **tap red X on thumbnail**
3. Annotation deleted from history
4. Counter decrements

---

## File Locations

Annotations are saved to:
```
Application.persistentDataPath/Annotations/
```

On Quest 2/3:
```
/sdcard/Android/data/[YourAppPackageName]/files/Annotations/
```

Each annotation is saved as:
```
[GUID].png
```

---

## Troubleshooting

### Save button does nothing
- Check AnnotationManager has Main Camera assigned
- Check Console for errors
- Verify AnnotationUIController is connected to AnnotationManager

### Thumbnails don't appear
- Check ThumbnailPrefab is assigned in HistoryPanel
- Check ThumbnailContainer is the ScrollView's Content
- Open Console and look for "Loaded X annotations" message

### Fist gesture doesn't work
- Check FistDetector is assigned (DetectGesture component)
- Check that DetectGesture has XRHandShape for fist gesture assigned
- Enable Debug Logs and watch Console
- Try making a tight fist (all fingers closed)
- Check minimumDetectionThreshold in DetectGesture (try lowering it)

### Counter doesn't update
- Check AnnotationUIController has counterText assigned
- Check AnnotationManager events are being fired (add Debug.Logs)

---

## Next Steps

1. Create UI elements in Unity based on your mockups
2. Test the hand gesture workflow thoroughly
3. Adjust colors, sizes, positions to match your design
4. Debug and refine gesture detection if needed

---

Last Updated: 2026-04-25
