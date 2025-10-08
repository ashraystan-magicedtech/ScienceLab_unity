# Unity Science Lab Tabletop Scene

A realtime-ready Unity educational scene for science lab simulations featuring a complete tabletop setup with laboratory equipment.

## Features

- **Tabletop**: Clean laminate/wood surface optimized for realtime rendering
- **Laboratory Cups**: 8 transparent cups (A-H) arranged in semi-circle + 1 mixing cup
- **Equipment**: Plastic spoon, water bottle with pour animation, hand lens
- **Lighting**: Soft studio lighting setup with controlled shadows
- **Performance**: Optimized for 60fps on mid-range hardware

## Technical Specifications

- **Rendering**: PBR materials with full texture sets
- **Textures**: Albedo, Normal, Roughness, Metallic, Opacity maps
- **LODs**: High-poly and low-poly versions for performance scaling
- **Animations**: Water pouring and spoon stirring animations
- **Export Formats**: Unity Package, FBX, GLTF

## File Structure

```
ScienceLabScene/
├── Models/
│   ├── High_LOD/
│   └── Low_LOD/
├── Textures/
│   ├── Albedo/
│   ├── Normal/
│   ├── Roughness/
│   ├── Metallic/
│   └── Opacity/
├── Animations/
├── Materials/
├── Prefabs/
└── Scenes/
```

## Usage

1. Import the Unity package or individual assets
2. Drag the ScienceLabTable prefab into your scene
3. Configure lighting using the provided lighting setup
4. Customize cup labels and materials as needed
 
## Performance Notes

- High LOD: ~2000-5000 triangles per object
- Low LOD: ~500-1500 triangles per object
- Texture resolution: 1024x1024 for main objects, 512x512 for small items
- Optimized for Unity's Universal Render Pipeline (URP)
