using UnityEngine;

namespace ScienceLabScene
{
    /// <summary>
    /// Generates all lab equipment using Unity primitives - no external tools required
    /// </summary>
    public class LabEquipmentGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        public bool generateOnStart = true;
        public bool clearExistingObjects = true;
        
        [Header("Positioning")]
        public Transform tabletop;
        public float cupRadius = 1.2f;
        public float tabletopHeight = 0.1f;
        
        [Header("Materials")]
        public Material glassMaterial;
        public Material plasticMaterial;
        public Material metalMaterial;
        public Material labelMaterial;
        
        void Start()
        {
            if (generateOnStart)
            {
                GenerateAllEquipment();
            }
        }
        
        /// <summary>
        /// Generate all lab equipment
        /// </summary>
        [ContextMenu("Generate All Equipment")]
        public void GenerateAllEquipment()
        {
            Debug.Log("Generating lab equipment using Unity primitives...");
            
            if (clearExistingObjects)
            {
                ClearAllEquipment();
            }
            
            CreateMaterials();
            CreateCups();
            CreateMixingCup();
            CreateWaterBottle();
            CreateSpoon();
            CreateMagnifyingGlass();
            
            Debug.Log("Lab equipment generation complete!");
        }
        
        /// <summary>
        /// Clear all generated equipment
        /// </summary>
        [ContextMenu("Clear All Equipment")]
        public void ClearAllEquipment()
        {
            // Find and destroy all generated objects
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Cup_") || obj.name.Contains("Water_Bottle") || 
                    obj.name.Contains("Spoon") || obj.name.Contains("Hand_Lens") ||
                    obj.name.Contains("Label_") || obj.name.Contains("Mixing_Cup"))
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }
            }
            
            Debug.Log("Cleared all lab equipment");
        }
        
        /// <summary>
        /// Regenerate just the magnifying glass with improved design
        /// </summary>
        [ContextMenu("Regenerate Magnifying Glass")]
        public void RegenerateMagnifyingGlass()
        {
            // Clear existing magnifying glass
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("Hand_Lens"))
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }
            }
            
            // Create new improved magnifying glass
            CreateMagnifyingGlass();
            Debug.Log("Regenerated magnifying glass with improved lens design");
        }
        /// <summary>
        /// Create materials if not assigned
        /// </summary>
        private void CreateMaterials()
        {
            if (glassMaterial == null)
            {
                glassMaterial = new Material(Shader.Find("Standard"));
                glassMaterial.name = "Glass_Material";
                glassMaterial.color = new Color(0.9f, 0.9f, 1f, 0.3f);
                glassMaterial.SetFloat("_Mode", 3); // Transparent
                glassMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                glassMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                glassMaterial.SetInt("_ZWrite", 0);
                glassMaterial.DisableKeyword("_ALPHATEST_ON");
                glassMaterial.EnableKeyword("_ALPHABLEND_ON");
                glassMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                glassMaterial.renderQueue = 3000;
                glassMaterial.SetFloat("_Metallic", 0f);
                glassMaterial.SetFloat("_Glossiness", 0.9f);
            }
            
            if (plasticMaterial == null)
            {
                plasticMaterial = new Material(Shader.Find("Standard"));
                plasticMaterial.name = "Plastic_Material";
                plasticMaterial.color = Color.white;
                plasticMaterial.SetFloat("_Metallic", 0f);
                plasticMaterial.SetFloat("_Glossiness", 0.4f);
            }
            
            if (metalMaterial == null)
            {
                metalMaterial = new Material(Shader.Find("Standard"));
                metalMaterial.name = "Metal_Material";
                metalMaterial.color = new Color(0.7f, 0.7f, 0.7f);
                metalMaterial.SetFloat("_Metallic", 0.8f);
                metalMaterial.SetFloat("_Glossiness", 0.8f);
            }
            
            if (labelMaterial == null)
            {
                labelMaterial = new Material(Shader.Find("Standard"));
                labelMaterial.name = "Label_Material";
                labelMaterial.color = Color.black;
                labelMaterial.SetFloat("_Metallic", 0f);
                labelMaterial.SetFloat("_Glossiness", 0.1f);
            }
        }
        
        /// <summary>
        /// Create cups A-H in semi-circle
        /// </summary>
        private void CreateCups()
        {
            string[] labels = { "A", "B", "C", "D", "E", "F", "G", "H" };
            
            for (int i = 0; i < labels.Length; i++)
            {
                float angle = Mathf.PI * (i / 7.0f); // Semi-circle
                float x = cupRadius * Mathf.Cos(angle);
                float y = cupRadius * Mathf.Sin(angle) - 0.5f;
                float z = tabletopHeight + 0.15f; // Cup height/2
                
                Vector3 position = new Vector3(x, y, z);
                CreateCup(position, labels[i]);
            }
        }
        
        /// <summary>
        /// Create individual cup with clean design
        /// </summary>
        private GameObject CreateCup(Vector3 position, string label)
        {
            // Create cup body (simple cylinder)
            GameObject cup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cup.name = $"Cup_{label}";
            cup.transform.position = position;
            cup.transform.localScale = new Vector3(0.3f, 0.15f, 0.3f); // Clean proportions
            
            // Apply clean glass material
            Material cleanGlass = new Material(Shader.Find("Standard"));
            cleanGlass.color = new Color(0.9f, 0.95f, 1f, 0.3f);
            cleanGlass.SetFloat("_Mode", 3); // Transparent
            cleanGlass.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            cleanGlass.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            cleanGlass.SetInt("_ZWrite", 0);
            cleanGlass.EnableKeyword("_ALPHABLEND_ON");
            cleanGlass.renderQueue = 3000;
            cleanGlass.SetFloat("_Metallic", 0f);
            cleanGlass.SetFloat("_Glossiness", 0.8f);
            
            cup.GetComponent<Renderer>().material = cleanGlass;
            
            // Add CupInteraction script
            CupInteraction cupScript = cup.AddComponent<CupInteraction>();
            cupScript.cupLabel = label;
            cupScript.maxCapacity = 200f; // 200ml
            
            // Create simple label
            CreateLabel(position + Vector3.back * 0.25f + Vector3.up * 0.05f, label);
            
            return cup;
        }
        
        /// <summary>
        /// Create mixing cup in center
        /// </summary>
        private void CreateMixingCup()
        {
            Vector3 position = new Vector3(0, 0.5f, tabletopHeight + 0.15f);
            GameObject mixingCup = CreateCup(position, "MIX");
            mixingCup.transform.localScale = new Vector3(0.4f, 0.2f, 0.4f); // Larger mixing cup
            
            // Update capacity for larger cup
            CupInteraction cupScript = mixingCup.GetComponent<CupInteraction>();
            if (cupScript != null)
            {
                cupScript.maxCapacity = 500f; // 500ml
            }
        }
        
        /// <summary>
        /// Create clean water bottle
        /// </summary>
        private void CreateWaterBottle()
        {
            Vector3 position = new Vector3(-0.8f, 0, tabletopHeight + 0.35f);
            
            // Bottle body (simple cylinder)
            GameObject bottle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bottle.name = "Water_Bottle";
            bottle.transform.position = position;
            bottle.transform.localScale = new Vector3(0.24f, 0.3f, 0.24f); // Clean proportions
            
            // Create clean bottle material (transparent blue)
            Material bottleMaterial = new Material(Shader.Find("Standard"));
            bottleMaterial.color = new Color(0.8f, 0.9f, 1f, 0.7f);
            bottleMaterial.SetFloat("_Mode", 3); // Transparent
            bottleMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            bottleMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            bottleMaterial.SetInt("_ZWrite", 0);
            bottleMaterial.EnableKeyword("_ALPHABLEND_ON");
            bottleMaterial.renderQueue = 3000;
            bottleMaterial.SetFloat("_Metallic", 0f);
            bottleMaterial.SetFloat("_Glossiness", 0.6f);
            
            bottle.GetComponent<Renderer>().material = bottleMaterial;
            
            // Bottle cap (simple)
            GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cap.name = "Bottle_Cap";
            cap.transform.position = position + Vector3.up * 0.35f;
            cap.transform.localScale = new Vector3(0.16f, 0.025f, 0.16f);
            cap.GetComponent<Renderer>().material = plasticMaterial;
            cap.transform.SetParent(bottle.transform);
            
            // Add WaterBottleController script
            WaterBottleController bottleScript = bottle.AddComponent<WaterBottleController>();
            bottleScript.waterCapacity = 500f;
            bottleScript.currentWater = 500f;
            
            // Set pour point
            GameObject pourPoint = new GameObject("PourPoint");
            pourPoint.transform.SetParent(bottle.transform);
            pourPoint.transform.localPosition = new Vector3(0, 0.3f, 0.1f);
            bottleScript.pourPoint = pourPoint.transform;
        }
        
        /// <summary>
        /// Create simple spoon with proper alignment
        /// </summary>
        private void CreateSpoon()
        {
            Vector3 position = new Vector3(0.3f, 0.3f, tabletopHeight + 0.12f);
            
            // Spoon handle (simple cylinder)
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Spoon";
            handle.transform.position = position;
            handle.transform.localScale = new Vector3(0.016f, 0.2f, 0.016f); // Clean proportions
            handle.GetComponent<Renderer>().material = metalMaterial;
            
            // Spoon bowl (simple sphere)
            GameObject bowl = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bowl.name = "Spoon_Bowl";
            bowl.transform.position = position + Vector3.forward * 0.2f;
            bowl.transform.localScale = new Vector3(0.08f, 0.12f, 0.024f); // Clean flattened sphere
            bowl.GetComponent<Renderer>().material = metalMaterial;
            bowl.transform.SetParent(handle.transform);
            
            // Add SpoonController script
            SpoonController spoonScript = handle.AddComponent<SpoonController>();
            spoonScript.stirRadius = 0.08f;
            spoonScript.stirSpeed = 2f;
        }
        
        /// <summary>
        /// Create magnifying glass with proper lens
        /// </summary>
        private void CreateMagnifyingGlass()
        {
            Vector3 position = new Vector3(1.2f, -0.8f, tabletopHeight + 0.15f);
            
            // Handle
            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Hand_Lens";
            handle.transform.position = position;
            handle.transform.localScale = new Vector3(0.04f, 0.15f, 0.04f);
            handle.transform.rotation = Quaternion.Euler(0, 0, 45f);
            handle.GetComponent<Renderer>().material = metalMaterial;
            
            // Lens frame (outer ring)
            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            frame.name = "Lens_Frame";
            frame.transform.position = position + new Vector3(0.2f, 0.2f, 0.05f);
            frame.transform.localScale = new Vector3(0.26f, 0.015f, 0.26f);
            frame.GetComponent<Renderer>().material = metalMaterial;
            frame.transform.SetParent(handle.transform);
            
            // Lens glass (more visible)
            GameObject lens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lens.name = "Lens_Glass";
            lens.transform.position = frame.transform.position;
            lens.transform.localScale = new Vector3(0.22f, 0.01f, 0.22f); // Made thicker
            
            // Create better lens material
            Material lensMaterial = new Material(Shader.Find("Standard"));
            lensMaterial.color = new Color(0.9f, 0.95f, 1f, 0.3f); // More visible blue tint
            
            // Set up transparency
            lensMaterial.SetFloat("_Mode", 3); // Transparent
            lensMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lensMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lensMaterial.SetInt("_ZWrite", 0);
            lensMaterial.EnableKeyword("_ALPHABLEND_ON");
            lensMaterial.renderQueue = 3000;
            
            // Glass properties
            lensMaterial.SetFloat("_Metallic", 0f);
            lensMaterial.SetFloat("_Glossiness", 0.98f); // Very glossy like glass
            
            lens.GetComponent<Renderer>().material = lensMaterial;
            lens.transform.SetParent(handle.transform);
            
            // Add inner lens highlight (makes it more visible)
            GameObject lensHighlight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lensHighlight.name = "Lens_Highlight";
            lensHighlight.transform.position = lens.transform.position + Vector3.up * 0.005f;
            lensHighlight.transform.localScale = new Vector3(0.18f, 0.002f, 0.18f);
            
            Material highlightMaterial = new Material(Shader.Find("Standard"));
            highlightMaterial.color = new Color(1f, 1f, 1f, 0.6f);
            highlightMaterial.SetFloat("_Mode", 3);
            highlightMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            highlightMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            highlightMaterial.SetInt("_ZWrite", 0);
            highlightMaterial.EnableKeyword("_ALPHABLEND_ON");
            highlightMaterial.renderQueue = 3001;
            highlightMaterial.SetFloat("_Metallic", 0f);
            highlightMaterial.SetFloat("_Glossiness", 1f);
            
            lensHighlight.GetComponent<Renderer>().material = highlightMaterial;
            lensHighlight.transform.SetParent(handle.transform);
            
            // Add collider for interaction
            BoxCollider lensCollider = handle.AddComponent<BoxCollider>();
            lensCollider.size = new Vector3(0.3f, 0.3f, 0.1f);
            lensCollider.center = new Vector3(0.2f, 0.2f, 0.05f);
            
            // Add MagnifyingGlass script
            MagnifyingGlass lensScript = handle.AddComponent<MagnifyingGlass>();
            lensScript.magnificationPower = 2f;
            lensScript.lensRadius = 0.12f;
            lensScript.lensGlass = lens.transform;
            
            Debug.Log("Created magnifying glass with visible lens");
        }
        
        /// <summary>
        /// Create text label
        /// </summary>
        private void CreateLabel(Vector3 position, string text)
        {
            GameObject labelObj = new GameObject($"Label_{text}");
            labelObj.transform.position = position;
            
            // Add TextMesh component
            TextMesh textMesh = labelObj.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = 20;
            textMesh.characterSize = 0.1f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
            
            // Rotate to face camera
            labelObj.transform.rotation = Quaternion.Euler(90f, 0, 0);
            
            // Apply label material
            MeshRenderer labelRenderer = labelObj.GetComponent<MeshRenderer>();
            labelRenderer.material = labelMaterial;
        }
        
        /// <summary>
        /// Create professional label with background
        /// </summary>
        private void CreateProfessionalLabel(Vector3 position, string text)
        {
            // Create label background
            GameObject labelBg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            labelBg.name = $"Label_Background_{text}";
            labelBg.transform.position = position;
            labelBg.transform.localScale = new Vector3(0.15f, 0.001f, 0.08f);
            labelBg.transform.rotation = Quaternion.Euler(90f, 0, 0);
            
            // White background material
            Material bgMaterial = new Material(Shader.Find("Standard"));
            bgMaterial.color = Color.white;
            bgMaterial.SetFloat("_Metallic", 0f);
            bgMaterial.SetFloat("_Glossiness", 0.1f);
            labelBg.GetComponent<Renderer>().material = bgMaterial;
            
            // Create text
            GameObject labelObj = new GameObject($"Label_Text_{text}");
            labelObj.transform.position = position + Vector3.up * 0.002f;
            labelObj.transform.SetParent(labelBg.transform);
            
            // Add TextMesh component
            TextMesh textMesh = labelObj.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = 25;
            textMesh.characterSize = 0.08f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.color = Color.black;
            textMesh.fontStyle = FontStyle.Bold;
            
            // Rotate to face camera
            labelObj.transform.rotation = Quaternion.Euler(90f, 0, 0);
            
            // Apply label material
            MeshRenderer labelRenderer = labelObj.GetComponent<MeshRenderer>();
            labelRenderer.material = labelMaterial;
        }
        
        /// <summary>
        /// Regenerate all equipment
        /// </summary>
        [ContextMenu("Regenerate Equipment")]
        public void RegenerateEquipment()
        {
            ClearAllEquipment();
            GenerateAllEquipment();
        }
    }
}
