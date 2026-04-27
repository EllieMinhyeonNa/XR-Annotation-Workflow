# XR Annotation Workflow - Setup Guide

## Overview
This guide covers the complete setup for the hand gesture-based annotation workflow system in Unity for Meta Quest 3.

**Current Version:** Hand Gesture Mode (White Background)
**Future Version:** Controller Mode (Separate APK - to be created later)

---

## Project Structure

### Core Scripts

**Input & Interaction:**
- `HandColliderSetup.cs` - Creates collision sphere on index finger for button poking
- `SimpleTouchButton.cs` - Touch-based button system for VR

**Annotation System:**
- `AnnotationManager.cs` - Core system for saving, loading, and managing annotations
- `AnnotationData.cs` - Data structure for annotation metadata
- `SavedAnnotationPreview.cs` - Shows saved screenshot preview in VR

**History & Viewing (In Progress):**
- `AnnotationHistoryPanel.cs` - Horizontal scrollable gallery of saved annotations
- `AnnotationThumbnailItem.cs` - Individual thumbnail with delete button
- `AnnotationUIController.cs` - Manages UI buttons and counters
- `AnnotationViewer.cs` - Full-screen annotation viewer (future)

**Drawing System:**
- `DrawWhileThreePinch.cs` - Three-finger pinch drawing (already existed)
- `ClearDrawingsByDestroyChildren.cs` - Clears drawn strokes

---

## Complete Setup Steps

### Part 1: Drawing System Setup

#### 1. Create Stroke Containers
1. In Hierarchy, create empty GameObject: **StrokeContainer_Right**
2. Create empty GameObject: **StrokeContainer_Left**
3. These organize drawn strokes by hand

#### 2. Connect Drawing Systems
For each DrawWhileThreePinch component:
1. Find DrawingSystem_R and DrawingSystem_L in Hierarchy
2. In Inspector, assign **Stroke Parent**:
   - Right hand → StrokeContainer_Right
   - Left hand → StrokeContainer_Left

#### 3. Setup Clear Manager
1. Create empty GameObject: **ClearDrawingsManager**
2. Add component: `ClearDrawingsByDestroyChildren`
3. Configure:
   - Stroke Roots: Size 2
   - Element 0: StrokeContainer_Right
   - Element 1: StrokeContainer_Left

---

### Part 2: Button Interaction System

#### 4. Setup Hand Colliders
On **Right Hand Tracking**:
1. Add component: `HandColliderSetup`
2. Configure:
   - Handedness: Right
   - Collider Radius: 0.015

On **Left Hand Tracking**:
1. Add component: `HandColliderSetup`
2. Configure:
   - Handedness: Left
   - Collider Radius: 0.015

#### 5. Setup Delete Button
On **Button (Delete)**:
1. Add component: `Box Collider`
   - Is Trigger: ✅ Checked
2. Add component: `SimpleTouchButton`
3. Configure SimpleTouchButton:
   - Normal Color: Your button's normal color
   - Pressed Color: Bright color (green/white)
   - On Button Pressed:
     - Click +
     - Drag AnnotationManager
     - Select: AnnotationManager → DeleteCurrentDrawings()

#### 6. Setup Save Button
On **Button (Save)**:
1. Add component: `Box Collider`
   - Is Trigger: ✅ Checked
2. Add component: `SimpleTouchButton`
3. Configure SimpleTouchButton:
   - Normal Color: Your button's normal color
   - Pressed Color: Bright color (green/white)
   - On Button Pressed:
     - Click +
     - Drag AnnotationManager
     - Select: AnnotationManager → SaveAnnotation()

---

### Part 3: Annotation Manager Setup

#### 7. Create Annotation Manager
1. Create empty GameObject: **AnnotationManager**
2. Add component: `AnnotationManager`
3. Configure:
   - Capture Camera: Main Camera
   - Screenshot Width: 1920
   - Screenshot Height: 1080
   - Save Folder Name: "Annotations"
   - Clear Drawings: ClearDrawingsManager

---

### Part 4: Visual Settings

#### 8. Setup White Background
On **Main Camera**:
1. Disable **AR Camera Manager** component (uncheck it)
2. In Camera component:
   - Clear Flags: Solid Color
   - Background: White (RGB: 255, 255, 255)

#### 9. Setup Hand Visualization
1. Find or create **Hand Visualizer** GameObject
2. Add component: `HandVisualizer` (from XR Hands samples)
3. Configure:
   - Meta Quest Left Hand Mesh: Assign hand mesh
   - Meta Quest Right Hand Mesh: Assign hand mesh
   - Hand Mesh Material: HandsDefaultMaterial (or custom)
   - Draw Meshes: ✅ Checked

---

### Part 5: Save Preview (Optional but Recommended)

#### 10. Create Save Preview Panel
1. Under Canvas, create UI → Panel: **SavePreviewPanel**
2. Position in front of user (Y: 1.8, Z: 1.5)
3. Add child: UI → Raw Image: **PreviewImage**
4. Configure PreviewImage to fill panel (anchor stretch)

#### 11. Add Preview Script
On **SavePreviewPanel**:
1. Add component: `SavedAnnotationPreview`
2. Configure:
   - Preview Image: Drag PreviewImage
   - Display Duration: 3 seconds

#### 12. Connect to Annotation Manager
On **AnnotationManager**:
- (No specific field needed - SavedAnnotationPreview subscribes to events automatically)

#### 13. Start Hidden
- Uncheck SavePreviewPanel in Inspector (starts disabled)

---

### Part 6: History Panel (In Progress)

#### 14. Create History Panel Structure
1. Under Canvas, create UI → Panel: **HistoryPanel**
2. Position horizontally centered at top
3. Add child: UI → Scroll View
4. Configure Scroll View:
   - Horizontal: ✅ Checked
   - Vertical: ❌ Unchecked
   - Delete Scrollbar Vertical

#### 15. Setup Content Layout
On **Content** (under Scroll View → Viewport):
1. Add component: `Horizontal Layout Group`
   - Padding: 10 all sides
   - Spacing: 15
   - Child Alignment: Middle Center
2. Add component: `Content Size Fitter`
   - Horizontal Fit: Preferred Size
   - Vertical Fit: Unconstrained

#### 16. Create Thumbnail Prefab
1. Under Content, create UI → Image: **ThumbnailItem**
2. Set size: 180x180
3. Add component: `Button`
4. Add child: UI → Raw Image (for screenshot)
5. Add child: UI → Text (for filename)
6. Add child: UI → Button (small, for delete ❌)
7. Add component: `AnnotationThumbnailItem`
8. Drag ThumbnailItem to Project folder (make prefab)
9. Delete from Hierarchy

#### 17. Connect History Panel
On **HistoryPanel**:
1. Add component: `AnnotationHistoryPanel`
2. Configure:
   - Content Transform: Content (from Scroll View)
   - Thumbnail Prefab: ThumbnailItem prefab

On **AnnotationManager**:
- History Panel: Drag HistoryPanel

#### 18. Setup History Button
On **Button (History)**:
1. Add component: `SimpleTouchButton` (if not already)
2. Configure On Button Pressed:
   - Drag HistoryPanel
   - Select: GameObject → SetActive(bool)
   - Check the checkbox ✅

#### 19. Start History Panel Hidden
- Uncheck HistoryPanel in Inspector (starts disabled)

---

## Testing Checklist

### Hand Tracking & Drawing
- [ ] Can see hand meshes in VR
- [ ] Three-finger pinch creates pencil visual
- [ ] Drawing creates white lines in space
- [ ] Both hands can draw independently

### Button Interaction
- [ ] Index finger can poke Save button
- [ ] Index finger can poke Delete button
- [ ] Buttons change color when poked
- [ ] Button colliders are positioned correctly

### Save Workflow
- [ ] Poke Save button captures screenshot
- [ ] Preview panel appears showing saved image
- [ ] Preview disappears after 3 seconds
- [ ] Annotations remain visible after save
- [ ] Files saved to Quest storage

### Delete Workflow
- [ ] Poke Delete button clears all drawings
- [ ] Strokes disappear immediately
- [ ] Can draw again after deleting

### History Panel (When Completed)
- [ ] History button shows saved thumbnails
- [ ] Thumbnails scroll horizontally
- [ ] Each thumbnail shows correct screenshot
- [ ] Delete buttons work on individual items
- [ ] Close button hides panel

---

## File Locations

### Saved Annotations
**Quest 3:**
```
/sdcard/Android/data/com.UnityTechnologies.com.unity.template.../files/Annotations/
```

**Access via:**
- Meta Quest Developer Hub → File Manager
- USB connection with Android File Transfer (Mac)
- ADB commands

---

## Troubleshooting

### Buttons Not Responding
- Verify Box Collider has "Is Trigger" enabled
- Check HandColliderSetup on both hands
- Verify SimpleTouchButton component on buttons
- Test collider size (increase Z if needed)

### Screenshots Are Black
- Verify AR Camera Manager is disabled
- Check Camera background is set to Solid Color: White
- Confirm Main Camera is assigned in AnnotationManager

### Hands Not Visible
- Check Hand Visualizer component is enabled
- Verify "Draw Meshes" is checked
- Confirm hand mesh materials are assigned
- Check hand tracking is enabled in Quest settings

### Drawing Not Working
- Verify Stroke Parent is assigned
- Check three-finger pinch gesture detection
- Confirm LineRenderer prefab is assigned
- Test in Quest (hand tracking doesn't work in Unity Editor)

---

## Next Steps

### Completing History Panel
1. Finish thumbnail prefab design
2. Add delete functionality per thumbnail
3. Add close button to history panel
4. Test horizontal scrolling

### Future Controller Version
1. Duplicate scene
2. Replace HandColliderSetup with XR Ray Interactor
3. Replace DrawWhileThreePinch with controller trigger drawing
4. Build separate APK for comparison

---

## Build Settings

**Platform:** Android
**Texture Compression:** ASTC
**Minimum API Level:** Android 10.0 (API 29)
**Target Devices:** Meta Quest 3
**XR Plugin:** OpenXR
**Hand Tracking:** Enabled

---

## Credits

Developed for MADE Program - Spring 2026
Built with Unity XR Hands and XR Interaction Toolkit
Meta Quest 3 SDK
