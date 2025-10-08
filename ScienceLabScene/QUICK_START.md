# Quick Start Guide - Science Lab Scene

## 🚀 **Fixing the "Missing Script" Error**

The "missing script" error occurs because Unity needs to regenerate the script references. Here's how to fix it:

### **Step 1: Fix Script References**
1. **Open Unity** and load the ScienceLabScene project
2. **In the Hierarchy**, select the `ScienceLabTable` GameObject
3. **In the Inspector**, you'll see a "Missing Script" component
4. **Click the circular icon** next to the missing script field
5. **Search for "ScienceLabController"** and select it
6. **Click Apply**

### **Step 2: Fix the Blue Scene**
The scene appears blue because there's no skybox and limited lighting:

1. **Window → Rendering → Lighting**
2. **In Environment tab**:
   - Set **Skybox Material** to `Default-Skybox` (or any skybox)
   - Set **Sun Source** to the Directional Light in your scene
3. **Click "Generate Lighting"** at the bottom

### **Step 3: Alternative Quick Fix**
If the above doesn't work:

1. **Delete the ScienceLabTable GameObject**
2. **Create → Empty GameObject** and name it "ScienceLabController"
3. **Add Component → ScienceLabController** script
4. **Add Component → Simple UI Helper** script
5. **Create → 3D Object → Cube** for a basic tabletop
6. **Scale the cube** to (3, 0.1, 2) to make it look like a table

## 🎯 **Expected Results**

After fixing:
- ✅ No "missing script" errors
- ✅ White tabletop cube visible in scene
- ✅ UI overlay with "Science Lab Ready" text
- ✅ Reset button and volume slider
- ✅ FPS counter in top-right

## 🔧 **Testing the Scene**

1. **Press Play** in Unity
2. **You should see**:
   - A flat white tabletop
   - UI text saying "Science Lab Ready"
   - A Reset button
   - A volume slider
   - FPS counter
3. **Press 'R'** to test reset functionality
4. **Press 'T'** to toggle tutorial
5. **Press 'H'** to toggle hints

## 🎨 **Adding Visual Content**

To make the scene more interesting:

1. **Create → 3D Object → Cylinder** (for cups)
2. **Create → 3D Object → Capsule** (for bottles)
3. **Position them** on the tabletop
4. **Add the respective controller scripts**:
   - `CupInteraction.cs` for cups
   - `WaterBottleController.cs` for bottles
   - `SpoonController.cs` for spoons
   - `MagnifyingGlass.cs` for magnifying glass

## 🐛 **Common Issues**

**Issue**: Still seeing blue screen
**Solution**: 
- Check camera position (should be at 0,1,-2)
- Add a skybox material
- Increase directional light intensity

**Issue**: UI not showing
**Solution**:
- Check that SimpleUIHelper script is attached
- Verify the script is enabled
- Check console for any script errors

**Issue**: Scripts not compiling
**Solution**:
- Check that all scripts are in the correct folders
- Verify namespace declarations match
- Clear Unity's script cache (Assets → Reimport All)

## 📁 **Correct File Structure**

Ensure your project structure looks like this:
```
Assets/
├── Scripts/
│   ├── ScienceLabScene/
│   │   ├── ScienceLabController.cs
│   │   ├── SimpleUIHelper.cs
│   │   ├── CupInteraction.cs
│   │   ├── WaterBottleController.cs
│   │   ├── SpoonController.cs
│   │   └── MagnifyingGlass.cs
│   └── VFX/
│       └── BubbleParticleSystem.cs
└── Scenes/
    └── ScienceLabScene.unity
```

Once you follow these steps, your scene should work properly with no errors!
