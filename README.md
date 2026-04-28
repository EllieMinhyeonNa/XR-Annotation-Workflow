# XR Annotation Workflow

A Unity-based VR application for Meta Quest 3 that allows users to draw 3D annotations in space using hand tracking gestures and save them as screenshots.

## Features

### Current Implementation

- **Hand Gesture Drawing**: Draw in 3D space using three-finger pinch gesture (thumb + index + middle finger)
- **Touch-Based UI Interaction**: Poke buttons with index finger using simple collision detection
- **Save Annotations**: Capture screenshots (1920x1080) with annotation overlays on white background
- **History Panel**: Horizontal panel displaying up to 3 most recent annotation thumbnails
  - Automatic thumbnail generation with 16:9 aspect ratio (100x56)
  - Rounded corners on thumbnails with masked screenshot display
  - Filename labels below each thumbnail
  - Individual delete buttons (red X) on each thumbnail
- **Delete Annotations**: Clear current drawings or individual saved annotations
- **Persistent Storage**: Annotations saved to device storage and automatically loaded on app restart
- **Dual Hand Support**: Draw with either left or right hand
- **Hand Visualization**: See 3D hand models in VR for natural interaction
- **White Background**: Clean white background for clear screenshot captures (instead of passthrough)

## System Architecture

### Core Components

#### Hand Tracking & Interaction
- **HandColliderSetup.cs**: Creates collision sphere on index finger tip for button interaction
  - Tracks finger position in real-time using XR Hands API
  - Enables simple collision-based UI interaction without complex XR Poke Interactor setup

#### Button System
- **SimpleTouchButton.cs**: Universal touch button component for VR
  - Works with both UI Images and 3D renderers
  - Provides visual color feedback on press
  - Uses Unity Events for flexible action binding

#### Annotation Management
- **AnnotationManager.cs**: Core system for saving and managing annotations
  - Captures screenshots from main camera (1920x1080)
  - Saves PNG files to persistent data path
  - Manages annotation data and events
  - Loads saved annotations from disk on startup
  - Supports clearing all annotations (for testing/debugging)

- **AnnotationData.cs**: Data structure for annotation metadata
  - Stores screenshot texture, timestamp, and file path
  - Generates unique IDs for each annotation
  - Tracks display names for user customization

- **AnnotationHistoryPanel.cs**: History panel management
  - Displays up to 3 most recent annotation thumbnails
  - Dynamically creates thumbnail instances from prefab
  - Horizontal layout with automatic spacing and centering
  - Auto-refreshes when annotations are added or removed

- **AnnotationThumbnailItem.cs**: Individual thumbnail component
  - Displays screenshot preview with 16:9 aspect ratio
  - Shows filename label
  - Individual delete button with VR poke support
  - Click to view full annotation (connects to viewer)

- **ThumbnailItem1.prefab**: Reusable thumbnail prefab
  - Rounded background with mask for screenshot clipping
  - Raw Image for screenshot display
  - TextMeshPro label for filename
  - Delete button with Box Collider for VR interaction

- **AnnotationViewer.cs**: Full-screen annotation viewer
  - Large image display
  - Editable name field with keyboard support
  - Delete and close functionality
  - (UI implementation in progress)

#### Drawing System
- **ClearDrawingsByDestroyChildren.cs**: Utility for clearing drawn strokes
  - Removes all stroke GameObjects from designated containers
  - Supports multiple stroke containers (left/right hand)

## File Structure

```
Assets/
├── HandColliderSetup.cs                    # Index finger collision detection
├── SimpleTouchButton.cs                    # Touch-based button interaction
├── AnnotationManager.cs                    # Main annotation system
├── AnnotationData.cs                       # Annotation data structure
├── AnnotationHistoryPanel.cs               # History panel (3 thumbnails max)
├── AnnotationThumbnailItem.cs              # Individual thumbnail component
├── AnnotationViewer.cs                     # Full-screen viewer (UI in progress)
├── AnnotationUIController.cs               # UI management (future use)
├── ThumbnailItem1.prefab                   # Thumbnail prefab template
├── ClearDrawingsByDestroyChildren.cs       # Drawing cleanup utility
└── SavedAnnotationPreview.cs               # In-VR screenshot preview (legacy)

Scenes/
└── Annotation Workflow.unity               # Main VR scene

README.md                                   # Project overview
SETUP_GUIDE.md                              # Detailed setup instructions
```

## How It Works

### 1. Hand Tracking & Collision
```
Quest 3 Hand Tracking
        ↓
XRHandTrackingEvents (Unity XR Hands)
        ↓
HandColliderSetup
  └─ Creates invisible sphere collider on index finger tip
  └─ Updates position every frame to follow finger
```

### 2. Button Interaction
```
Index Finger Collider touches Button Box Collider
        ↓
SimpleTouchButton.OnTriggerEnter()
        ↓
Visual feedback (color change)
        ↓
Invokes assigned Unity Event
        ↓
Calls AnnotationManager function (Save/Delete)
```

### 3. Save Workflow
```
User draws annotations → Pokes Save button
        ↓
AnnotationManager.SaveAnnotation()
        ↓
Captures screenshot from camera
        ↓
Encodes as PNG and saves to disk
        ↓
Shows preview in VR for 3 seconds
        ↓
Annotations remain visible until deleted
```

### 4. Delete Workflow
```
User pokes Delete button
        ↓
AnnotationManager.DeleteCurrentDrawings()
        ↓
ClearDrawingsByDestroyChildren.ClearDrawings()
        ↓
All stroke GameObjects destroyed
```

## Save Location

Annotations are saved to:
```
Quest 3: /sdcard/Android/data/com.UnityTechnologies.com.unity.template.../files/Annotations/
Format: [unique-id].png
```

Access via:
- Meta Quest Developer Hub → File Manager
- USB file transfer (requires Android File Transfer on Mac)
- ADB: `adb pull /sdcard/Android/data/[app-name]/files/Annotations/`

## Setup Requirements

### Unity Version
- Unity 6.3 LTS (6000.3.81f)

### Required Packages
- XR Hands (1.7.3+)
- XR Interaction Toolkit (3.3.1+)
- OpenXR Plugin
- Meta XR SDK (recommended)

### Target Platform
- Meta Quest 3
- Android build target
- Hand tracking enabled

## Current Status

### Working Features ✅
- Three-finger pinch drawing gesture
- Index finger button poking
- Save button with screenshot capture (1920x1080)
- Delete button with immediate clear (clears current drawings)
- Delete all saved annotations button (for testing)
- Visual button feedback (color change)
- History panel with 3 most recent thumbnails
- Individual delete buttons on each thumbnail (red X)
- Thumbnail generation with rounded corners
- Filename labels on thumbnails
- Persistent storage (saves/loads annotations)
- Automatic thumbnail limit (keeps only 3 most recent)
- Dual hand support (left/right)
- White background for clear screenshots
- Hand mesh visualization in VR

### In Progress 🚧
- Annotation viewer UI (full-screen display)
- Annotation renaming with keyboard
- Click thumbnail to view full annotation

### Future Enhancements 💡
- Controller support version (separate APK with trigger-based drawing)
- Multiple finger support for button interaction
- Haptic feedback on button press
- Sound effects for save/delete actions
- Line color picker and customization
- Annotation metadata (timestamps, tags)
- Share/export functionality
- Undo/redo for drawings

## Development Notes

### Design Decisions

**Why collision-based buttons instead of XR Poke Interactor?**
- Simpler implementation with fewer components
- No dependency on XR Hand Skeleton Driver
- Avoids "Finger shape type Unspecified" errors
- More reliable and easier to debug
- Faster iteration and development

**Why keep annotations after save?**
- Allows users to continue adding to existing annotations
- Explicit delete action gives user control
- More flexible workflow

**Why index finger only for poking?**
- Prevents accidental button presses from other fingers
- More precise interaction
- Standard UI interaction paradigm

## Troubleshooting

**Buttons not responding:**
- Verify Box Collider has "Is Trigger" enabled
- Check HandColliderSetup is on both hand tracking objects
- Ensure SimpleTouchButton is on the button
- Verify button has Box Collider component

**Screenshots are black:**
- Passthrough cannot be captured in screenshots
- Switch to white/colored background for visible screenshots
- Or add 3D environment to scene

**Collider size issues:**
- Increase Box Collider Z size for easier poking
- Adjust HandColliderSetup collider radius if needed

## License

[Add your license here]

## Credits

Developed by Ellie Minhyeon Na
Built with Unity and Meta Quest SDK
