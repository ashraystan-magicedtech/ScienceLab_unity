# Unity Science Lab Scene - Complete Setup Guide

## Overview
This guide provides step-by-step instructions for importing and setting up the Science Lab tabletop scene in Unity, optimized for educational simulations and 60fps performance on mid-range hardware.

## Prerequisites
- Unity 2022.3 LTS or newer
- Universal Render Pipeline (URP) package
- Input System package (optional, for enhanced input handling)

## Quick Start

### 1. Generate Assets
Run the batch script to generate all assets:
```bash
cd ScienceLabScene
generate_all.bat
```

### 2. Import to Unity
1. Create a new Unity project with URP template
2. Copy the `Unity_Export` folder contents to your `Assets` folder
3. Import the provided C# scripts to `Assets/Scripts/ScienceLab/`

## Detailed Setup

### Asset Import Settings

#### Models (FBX Files)
```
Import Settings:
- Scale Factor: 1
- Convert Units: ✓
- Import BlendShapes: ✓
- Import Visibility: ✓
- Import Cameras: ✗
- Import Lights: ✗
- Preserve Hierarchy: ✓

Rig:
- Animation Type: Generic
- Avatar Definition: Create From This Model
- Optimize Game Objects: ✓

Animation:
- Import Animation: ✓
- Bake Animations: ✓
- Resample Curves: ✓
- Anim. Compression: Keyframe Reduction
```

#### Textures
```
Albedo Maps:
- Texture Type: Default
- sRGB: ✓
- Alpha Source: Input Texture Alpha
- Non-Power of 2: None
- Max Size: 1024
- Compression: Normal Quality
- Use Crunch Compression: ✓

Normal Maps:
- Texture Type: Normal Map
- sRGB: ✗
- Create from Grayscale: ✗
- Filtering: Trilinear
- Max Size: 1024

Metallic/Roughness Maps:
- Texture Type: Default
- sRGB: ✗
- Alpha Source: Input Texture Alpha
- Max Size: 1024
- Compression: High Quality
```

### Material Setup

#### Glass Material (Cups)
```csharp
Shader: Universal Render Pipeline/Lit
Rendering Mode: Transparent
Blend Mode: Alpha

Base Map: Glass_Albedo
Metallic: 0
Smoothness: 0.95
Normal Map: Glass_Normal
Emission: None

Surface Options:
- Surface Type: Transparent
- Blending Mode: Alpha
- Preserve Specular Lighting: ✓
- Receive Shadows: ✓
```

#### Wood Material (Tabletop)
```csharp
Shader: Universal Render Pipeline/Lit
Rendering Mode: Opaque

Base Map: Wood_Albedo
Metallic: 0
Smoothness: 0.4
Normal Map: Wood_Normal
Occlusion Map: Wood_AO

Surface Options:
- Surface Type: Opaque
- Alpha Clipping: ✗
- Receive Shadows: ✓
- Cast Shadows: On
```

#### Plastic Material (Spoon)
```csharp
Shader: Universal Render Pipeline/Lit
Rendering Mode: Opaque

Base Map: Plastic_Albedo
Metallic: 0
Smoothness: 0.7
Normal Map: Plastic_Normal

Surface Options:
- Surface Type: Opaque
- Receive Shadows: ✓
- Cast Shadows: On
```

#### Metal Material (Hand Lens)
```csharp
Shader: Universal Render Pipeline/Lit
Rendering Mode: Opaque

Base Map: Metal_Albedo
Metallic: 0.9
Smoothness: 0.8
Normal Map: Metal_Normal

Surface Options:
- Surface Type: Opaque
- Receive Shadows: ✓
- Cast Shadows: On
```

### Prefab Hierarchy

Create the following hierarchy in your scene:

```
ScienceLabTable (Empty GameObject)
├── Tabletop (MeshRenderer + MeshFilter + BoxCollider)
├── Cups (Empty GameObject)
│   ├── Cup_A (MeshRenderer + MeshFilter + MeshCollider + CupInteraction)
│   ├── Cup_B (MeshRenderer + MeshFilter + MeshCollider + CupInteraction)
│   ├── ... (Cup_C through Cup_H)
│   └── Cup_MIX (MeshRenderer + MeshFilter + MeshCollider + CupInteraction)
├── Equipment (Empty GameObject)
│   ├── Spoon (MeshRenderer + MeshFilter + CapsuleCollider + SpoonController + Animator)
│   ├── WaterBottle (MeshRenderer + MeshFilter + MeshCollider + WaterBottleController + Animator)
│   └── HandLens (MeshRenderer + MeshFilter + SphereCollider + MagnifyingGlass)
└── Lighting (Empty GameObject)
    ├── KeyLight (Light - Directional)
    ├── FillLight (Light - Area)
    └── RimLight (Light - Spot)
```

### Script Configuration

#### CupInteraction Script
```csharp
// Attach to each cup GameObject
Cup Label: A, B, C, D, E, F, G, H, or MIX
Max Capacity: 100
Current Amount: 0
Liquid Color: Blue (default)

// Materials
Normal Material: Glass_Material
Highlight Material: Glass_Highlight_Material

// Audio
Pour Sound: PourSound.wav
Select Sound: SelectSound.wav
```

#### WaterBottleController Script
```csharp
// Attach to water bottle GameObject
Water Capacity: 500
Current Water: 500
Pour Rate: 50 (ml/second)
Water Color: Blue

// Animation
Bottle Animator: WaterBottle_Animator
Pour Animation Name: "PourAnimation"

// Audio
Pour Start Sound: PourStart.wav
Pour Loop Sound: PourLoop.wav
Pour End Sound: PourEnd.wav
```

#### SpoonController Script
```csharp
// Attach to spoon GameObject
Stir Radius: 0.08
Stir Speed: 2
Stir Revolutions: 3

// Animation
Spoon Animator: Spoon_Animator
Stir Animation Name: "StirAnimation"

// Audio
Stir Sound: StirSound.wav
Mixing Sound: MixingSound.wav
```

#### MagnifyingGlass Script
```csharp
// Attach to hand lens GameObject
Magnification Power: 2
Lens Radius: 0.12
Move Speed: 2
Rotation Speed: 90

// Controls
Activation Key: M
Follow Mouse: ✓

// Audio
Pickup Sound: PickupSound.wav
Putdown Sound: PutdownSound.wav
```

#### ScienceLabController Script
```csharp
// Attach to ScienceLabTable root GameObject
// Assign all references through inspector

Enable Tutorial: ✓
Enable Hints: ✓
Hint Delay: 5 seconds
Target Frame Rate: 60
```

### LOD Setup

For each model, create LOD Groups:

```csharp
LOD 0 (0-50 units): High poly model
LOD 1 (50-100 units): Low poly model
LOD 2 (100+ units): Culled
```

### Lighting Configuration

#### URP Renderer Settings
```
Rendering Path: Forward
Depth Texture: ✓
Opaque Texture: ✓
Opaque Downsampling: None
Terrain Holes: ✓

Shadows:
- Max Distance: 50
- Cascade Count: 2
- Depth Bias: 1
- Normal Bias: 1
- Soft Shadows: ✓
```

#### Light Settings
```csharp
Key Light (Directional):
- Intensity: 1.5
- Color: Warm White (255, 242, 224)
- Shadows: Soft Shadows
- Shadow Strength: 0.8

Fill Light (Area):
- Intensity: 0.8
- Color: Cool White (224, 242, 255)
- Range: 10
- Shadows: No Shadows

Rim Light (Spot):
- Intensity: 1.2
- Color: White
- Range: 8
- Spot Angle: 45°
- Shadows: Hard Shadows
```

### Performance Optimization

#### Quality Settings
Texture Quality: Half Res
Anisotropic Textures: Per Texture
Anti Aliasing: 2x Multi Sampling
Soft Particles: 
Realtime Reflection Probes: 
Billboards Face Camera Position: 
Resolution Scaling Fixed DPI Factor: 1
- Input System 1.7.0
- Windows 10/11
- Mid-range hardware (GTX 1060 / RX 580 equivalent)

This setup guide ensures optimal performance and educational value for your science lab simulation!
