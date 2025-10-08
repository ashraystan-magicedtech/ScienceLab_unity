using UnityEngine;

namespace ScienceLabScene
{
    /// <summary>
    /// Simple scene setup script to initialize the lab without script reference issues
    /// </summary>
    public class SceneSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public bool setupOnStart = true;
        
        void Start()
        {
            if (setupOnStart)
            {
                SetupScene();
            }
        }
        
        /// <summary>
        /// Set up the complete scene
        /// </summary>
        [ContextMenu("Setup Scene")]
        public void SetupScene()
        {
            Debug.Log("Setting up Science Lab Scene...");
            
            // Add LabEquipmentGenerator if not present
            LabEquipmentGenerator generator = FindFirstObjectByType<LabEquipmentGenerator>();
            if (generator == null)
            {
                generator = gameObject.AddComponent<LabEquipmentGenerator>();
                generator.generateOnStart = true;
                generator.clearExistingObjects = true;
                generator.tabletop = transform;
                Debug.Log("Added LabEquipmentGenerator component");
            }
            
            // Add ScienceLabController if not present
            ScienceLabController controller = FindFirstObjectByType<ScienceLabController>();
            if (controller == null)
            {
                controller = gameObject.AddComponent<ScienceLabController>();
                controller.tabletop = transform;
                controller.enableTutorial = true;
                controller.enableHints = true;
                Debug.Log("Added ScienceLabController component");
            }
            
            // Add SimpleUIHelper if not present
            SimpleUIHelper uiHelper = FindFirstObjectByType<SimpleUIHelper>();
            if (uiHelper == null)
            {
                GameObject uiGO = new GameObject("SimpleUI");
                uiHelper = uiGO.AddComponent<SimpleUIHelper>();
                uiHelper.labController = controller;
                Debug.Log("Added SimpleUIHelper component");
            }
            
            // Add ScienceLabUI if not present
            ScienceLabUI scienceLabUI = FindFirstObjectByType<ScienceLabUI>();
            if (scienceLabUI == null)
            {
                GameObject scienceUIGO = new GameObject("ScienceLabUI");
                scienceLabUI = scienceUIGO.AddComponent<ScienceLabUI>();
                scienceLabUI.labController = controller;
                Debug.Log("Added ScienceLabUI component");
            }
            
            // Update controller references
            if (controller != null)
            {
                if (controller.uiHelper == null)
                    controller.uiHelper = uiHelper;
                if (controller.scienceLabUI == null)
                    controller.scienceLabUI = scienceLabUI;
            }
            
            Debug.Log("Scene setup complete!");
        }
        
        /// <summary>
        /// Generate equipment manually
        /// </summary>
        [ContextMenu("Generate Equipment")]
        public void GenerateEquipment()
        {
            LabEquipmentGenerator generator = GetComponent<LabEquipmentGenerator>();
            if (generator == null)
            {
                generator = gameObject.AddComponent<LabEquipmentGenerator>();
                generator.tabletop = transform;
            }
            
            generator.GenerateAllEquipment();
        }
        
        /// <summary>
        /// Clear all equipment
        /// </summary>
        [ContextMenu("Clear Equipment")]
        public void ClearEquipment()
        {
            LabEquipmentGenerator generator = GetComponent<LabEquipmentGenerator>();
            if (generator != null)
            {
                generator.ClearAllEquipment();
            }
        }
    }
}
