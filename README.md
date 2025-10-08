# 🧪 Science Lab Simulator - Unity 3D

A comprehensive interactive science laboratory simulation built with Unity 3D, featuring realistic lab equipment, interactive experiments, and a modern UI system for educational and experimental purposes.

## 🎯 Overview

The Science Lab Simulator provides an immersive virtual laboratory environment where users can:
- Conduct interactive experiments with realistic lab equipment
- Mix liquids in various beakers and observe reactions
- Use laboratory tools like spoons, water bottles, and magnifying glasses
- Track experiment progress with real-time UI feedback
- Learn laboratory procedures in a safe, virtual environment

## ✨ Features

### 🧪 Interactive Lab Equipment
- **8 Labeled Beakers (A-H)**: Individual glass beakers with 200ml capacity each
- **Central Mixing Cup**: Large 500ml beaker for combining experiments
- **Water Bottle**: Interactive bottle for pouring liquids into beakers
- **Laboratory Spoon**: Metallic spoon for stirring liquids
- **Magnifying Glass**: Examination tool for detailed observation
- **Professional Materials**: Realistic glass, metal, and plastic materials

### 📊 Advanced UI System
- **Task Panel**: Step-by-step instructions and current objectives
- **Stats Panel**: Real-time experiment statistics and progress tracking
- **Control Panel**: Reset, new experiment, and menu navigation
- **Live Feedback**: Instant notifications and status updates
- **Responsive Design**: Adapts to different screen sizes

### 🎮 Interactive Controls
- **Click to Select**: Choose beakers and equipment
- **Drag to Pour**: Intuitive liquid pouring mechanics
- **Stir Animation**: Realistic stirring interactions
- **Magnify Mode**: Press 'M' for detailed examination
- **Audio Feedback**: Ambient sounds and interaction audio

## 🚀 Getting Started

### Prerequisites
- Unity 2021.3 LTS or higher
- Windows, macOS, or Linux
- Minimum 4GB RAM
- DirectX 11 compatible graphics card

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/ashraystan-magicedtech/ScienceLab_unity.git
   cd ScienceLab_unity
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Open" and select the `ScienceLabScene` folder
   - Wait for Unity to import all assets

3. **Run the Simulation**
   - Open the `ScienceLabScene` scene
   - Press the Play button in Unity Editor
   - Or build and run as standalone application

## 🎮 How to Use

### Basic Operations

1. **Starting an Experiment**
   - Click the "📋 New Experiment" button
   - Read the task instructions in the top-left panel
   - Follow the step-by-step guidance

2. **Selecting Equipment**
   - Click on any beaker (A-H) to select it
   - Selected beaker will be highlighted
   - Status panel shows current selection

3. **Pouring Liquids**
   - Click and drag the water bottle toward a selected beaker
   - Watch the liquid pour animation
   - Volume is tracked in the stats panel

4. **Stirring Solutions**
   - Click on the spoon after adding liquid
   - Spoon will animate stirring motion
   - Stir count is recorded in statistics

5. **Examining Results**
   - Press 'M' key or click magnifying glass
   - Get detailed view of experiment results
   - Magnification uses are tracked

### UI Panels Guide

#### 📋 Task Panel (Top-Left)
- Shows current experiment instructions
- Updates based on your actions
- Displays experiment status

#### 📊 Stats Panel (Middle-Left)  
- **Experiments Performed**: Total liquid pours
- **Times Stirred**: Stirring action count
- **Magnifications**: Examination tool usage
- **Recent Actions**: Last 3 actions with details

#### 🎮 Control Panel (Top-Right)
- **Reset Lab**: Clear all experiments and restart
- **New Experiment**: Begin fresh experiment sequence
- **Main Menu**: Return to welcome screen

### Controls Reference

| Action | Control | Description |
|--------|---------|-------------|
| Select Beaker | Left Click | Choose a beaker (A-H) for interaction |
| Pour Liquid | Click & Drag | Drag water bottle to pour into selected beaker |
| Stir Solution | Click Spoon | Stir liquid in the last selected beaker |
| Magnify | Press 'M' | Activate magnifying glass for examination |
| Reset Lab | Click Reset | Clear all experiments and restart |
| Adjust Volume | Volume Slider | Control audio volume (bottom center) |

## 🏗️ Project Structure

```
ScienceLabScene/
├── Assets/
│   ├── Scripts/
│   │   └── ScienceLabScene/
│   │       ├── ScienceLabController.cs      # Main lab controller
│   │       ├── ScienceLabUI.cs              # Modern UI system
│   │       ├── LabEquipmentGenerator.cs     # Equipment creation
│   │       ├── CupInteraction.cs            # Beaker interactions
│   │       ├── WaterBottleController.cs     # Bottle mechanics
│   │       ├── SpoonController.cs           # Stirring system
│   │       ├── MagnifyingGlass.cs           # Examination tool
│   │       └── SimpleUIHelper.cs            # Legacy UI support
│   ├── Materials/                           # Material assets
│   ├── Models/                             # 3D model assets
│   ├── Audio/                              # Sound effects
│   └── Prefabs/                            # Reusable components
├── Unity_Scripts/                          # Additional scripts
└── README.md                               # This file
```

## 🔧 Technical Details

### Architecture
- **Modular Design**: Each component is self-contained and reusable
- **Event-Driven**: Uses Unity Events for loose coupling
- **Responsive UI**: Adapts to different screen resolutions
- **Performance Optimized**: Efficient rendering and memory usage

### Key Components
- **ScienceLabController**: Central hub managing all lab interactions
- **ScienceLabUI**: Modern UI system with real-time updates
- **LabEquipmentGenerator**: Procedural generation of lab equipment
- **Interactive Scripts**: Individual controllers for each piece of equipment

### Materials & Rendering
- **Realistic Glass**: Transparent materials with proper alpha blending
- **Metallic Surfaces**: PBR materials for spoons and metal components
- **Optimized Shaders**: Standard Unity shaders for compatibility

## 🎨 Customization

### Adding New Equipment
1. Create new GameObject with appropriate mesh
2. Add interaction script inheriting from base classes
3. Register with ScienceLabController
4. Update UI system for new interactions

### Modifying Experiments
1. Edit experiment sequences in ScienceLabController
2. Update instruction text in UI system
3. Add new tracking parameters to stats system

### UI Customization
1. Modify panel layouts in ScienceLabUI.cs
2. Adjust colors and fonts in GUI styles
3. Add new panels following existing patterns

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines
- Follow Unity coding conventions
- Comment complex interactions
- Test on multiple screen resolutions
- Ensure backward compatibility

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Unity Technologies for the excellent game engine
- Contributors and testers who helped improve the simulation
- Educational institutions using this for science learning

## 📞 Support

For questions, issues, or suggestions:
- Open an issue on GitHub
- Contact the development team
- Check the documentation in the `UI_SYSTEM_GUIDE.md`

---

**Made with ❤️ for science education and interactive learning**
