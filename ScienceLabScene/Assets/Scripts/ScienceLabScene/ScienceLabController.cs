using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

namespace ScienceLabScene
{
    /// <summary>
    /// Main controller for the Science Lab scene, managing all interactions and educational features
    /// </summary>
    public class ScienceLabController : MonoBehaviour
    {
        [Header("Scene References")]
        public Transform tabletop;
        public Transform cupsParent;
        public WaterBottleController waterBottle;
        public SpoonController spoon;
        public MagnifyingGlass magnifyingGlass;
        
        [Header("Educational Features")]
        public bool enableTutorial = true;
        public bool enableHints = true;
        public float hintDelay = 5f;
        [Header("Performance")]
        public LODGroup[] lodGroups;
        public bool enableOcclusion = true;
        public int targetFrameRate = 60;
        
        [Header("UI")]
        public SimpleUIHelper uiHelper;
        public ScienceLabUI scienceLabUI;
        
        [Header("Audio")]
        public AudioSource backgroundMusic;
        public AudioClip[] ambientSounds;
        [Header("Events")]
        public UnityEvent OnSceneReady;
        public UnityEvent OnExperimentComplete;
        public UnityEvent<string> OnInstructionChanged;
        
        private List<CupInteraction> allCups = new List<CupInteraction>();
        private Dictionary<string, float> experimentData = new Dictionary<string, float>();
        private bool sceneInitialized = false;
        private Coroutine tutorialCoroutine;
        private int currentTutorialStep = 0;
        private string lastSelectedCup = "";
        
        // Tutorial steps
        private readonly string[] tutorialSteps = {
            "Welcome to the Science Lab! Click on any cup to select it.",
            "Great! Now try clicking and dragging the water bottle to pour water.",
            "Excellent! You can use the spoon to stir the liquid in cups.",
            "Press 'M' or click the magnifying glass to examine objects closely.",
            "Try mixing different amounts in various cups to see what happens!",
            "Experiment complete! Use the reset button to start over."
        };
        
        void Start()
        {
            InitializeScene();
        }
        
        void Update()
        {
            // Performance monitoring
            if (Time.unscaledTime % 1f < Time.unscaledDeltaTime)
            {
                MonitorPerformance();
            }
            
            // Handle global input
            HandleGlobalInput();
        }
        
        /// <summary>
        /// Initialize the entire scene
        /// </summary>
        private void InitializeScene()
        {
            Debug.Log("Initializing Science Lab Scene...");
            
            // Set target frame rate
            Application.targetFrameRate = targetFrameRate;
            
            // Find and register all cups
            FindAllCups();
            
            // Set up component references
            SetupComponentReferences();
            
            // Configure performance settings
            ConfigurePerformance();
            
            // Set up UI
            SetupUI();
            
            // Set up audio
            SetupAudio();
            
            // Register event listeners
            RegisterEventListeners();
            
            // Start tutorial if enabled
            if (enableTutorial)
            {
                StartTutorial();
            }
            
            sceneInitialized = true;
            OnSceneReady?.Invoke();
            
            Debug.Log("Science Lab Scene initialized successfully!");
        }
        
        /// <summary>
        /// Find and register all cup interactions in the scene
        /// </summary>
        private void FindAllCups()
        {
            CupInteraction[] cups = FindObjectsByType<CupInteraction>(FindObjectsSortMode.None);
            allCups.AddRange(cups);
            
            Debug.Log($"Found {allCups.Count} cups in the scene");
        }
        
        /// <summary>
        /// Set up references to key components
        /// </summary>
        private void SetupComponentReferences()
        {
            // Find components if not assigned
            if (waterBottle == null)
                waterBottle = FindFirstObjectByType<WaterBottleController>();
            
            if (spoon == null)
                spoon = FindFirstObjectByType<SpoonController>();
            
            if (magnifyingGlass == null)
                magnifyingGlass = FindFirstObjectByType<MagnifyingGlass>();
            
            // Find LOD groups
            if (lodGroups == null || lodGroups.Length == 0)
                lodGroups = FindObjectsByType<LODGroup>(FindObjectsSortMode.None);
        }
        
        /// <summary>
        /// Configure performance settings
        /// </summary>
        private void ConfigurePerformance()
        {
            // Set up LOD bias
            QualitySettings.lodBias = 1.5f;
            
            // Configure occlusion culling
            if (enableOcclusion)
            {
                Camera.main.useOcclusionCulling = true;
            }
            
            // Set up batching
            QualitySettings.maxQueuedFrames = 2;
            
            Debug.Log("Performance settings configured");
        }
        
        /// <summary>
        /// Set up UI elements
        /// </summary>
        private void SetupUI()
        {
            // Find UI helper if not assigned
            if (uiHelper == null)
                uiHelper = FindFirstObjectByType<SimpleUIHelper>();
            
            // Create UI helper if none exists
            if (uiHelper == null)
            {
                GameObject uiGO = new GameObject("SimpleUI");
                uiHelper = uiGO.AddComponent<SimpleUIHelper>();
                uiHelper.labController = this;
            }
            
            // Initialize UI
            if (uiHelper != null)
            {
                uiHelper.SetText("Science Lab Ready");
                uiHelper.sliderValue = AudioListener.volume;
            }
        }
        
        /// <summary>
        /// Set up audio components
        /// </summary>
        private void SetupAudio()
        {
            if (backgroundMusic != null)
            {
                backgroundMusic.loop = true;
                backgroundMusic.volume = 0.3f;
                backgroundMusic.Play();
            }
            
            // Start ambient sounds
            if (ambientSounds != null && ambientSounds.Length > 0)
            {
                StartCoroutine(PlayAmbientSounds());
            }
        }
        
        /// <summary>
        /// Register event listeners for all interactive components
        /// </summary>
        private void RegisterEventListeners()
        {
            // Register cup events
            if (allCups != null)
            {
                foreach (var cup in allCups)
                {
                    if (cup != null && cup.OnCupSelected != null)
                    {
                        cup.OnCupSelected.AddListener(OnCupSelected);
                    }
                    if (cup != null && cup.OnLiquidAdded != null)
                    {
                        cup.OnLiquidAdded.AddListener(OnLiquidAdded);
                    }
                    if (cup != null && cup.OnLiquidRemoved != null)
                    {
                        cup.OnLiquidRemoved.AddListener(OnLiquidRemoved);
                    }
                }
            }
        }
        
        /// <summary>
        /// Handle global input
        /// </summary>
        private void HandleGlobalInput()
        {
            // Reset scene with R key
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetScene();
            }
            
            // Toggle tutorial with T key
            if (Input.GetKeyDown(KeyCode.T))
            {
                ToggleTutorial();
            }
            
            // Toggle hints with H key
            if (Input.GetKeyDown(KeyCode.H))
            {
                enableHints = !enableHints;
                UpdateInstruction(enableHints ? "Hints enabled" : "Hints disabled");
            }
        }
        
        /// <summary>
        /// Start the tutorial sequence
        /// </summary>
        public void StartTutorial()
        {
            if (tutorialCoroutine != null)
            {
                StopCoroutine(tutorialCoroutine);
            }
            
            currentTutorialStep = 0;
            tutorialCoroutine = StartCoroutine(TutorialSequence());
        }
        
        /// <summary>
        /// Tutorial sequence coroutine
        /// </summary>
        private IEnumerator TutorialSequence()
        {
            while (currentTutorialStep < tutorialSteps.Length)
            {
                UpdateInstruction(tutorialSteps[currentTutorialStep]);
                
                // Wait for user action or timeout
                yield return new WaitForSeconds(hintDelay);
                
                // Auto-advance tutorial (in a real implementation, this would wait for specific actions)
                currentTutorialStep++;
            }
            
            UpdateInstruction("Tutorial complete! Experiment freely.");
        }
        
        /// <summary>
        /// Toggle tutorial on/off
        /// </summary>
        public void ToggleTutorial()
        {
            enableTutorial = !enableTutorial;
            
            if (enableTutorial)
            {
                StartTutorial();
            }
            else
            {
                if (tutorialCoroutine != null)
                {
                    StopCoroutine(tutorialCoroutine);
                    tutorialCoroutine = null;
                }
                UpdateInstruction("Tutorial disabled");
            }
        }
        
        /// <summary>
        /// Reset the entire scene to initial state
        /// </summary>
        public void ResetScene()
        {
            Debug.Log("Resetting Science Lab Scene...");
            
            // Reset all cups
            foreach (var cup in allCups)
            {
                cup.EmptyCup();
                cup.DeselectCup();
            }
            
            // Reset water bottle
            if (waterBottle != null)
            {
                waterBottle.RefillBottle();
                waterBottle.SetPouringEnabled(true);
            }
            
            // Reset spoon
            if (spoon != null)
            {
                spoon.SetStirringEnabled(true);
            }
            
            // Reset magnifying glass
            if (magnifyingGlass != null)
            {
                magnifyingGlass.SetActive(false);
            }
            
            // Clear experiment data
            experimentData.Clear();
            
            // Restart tutorial if enabled
            if (enableTutorial)
            {
                StartTutorial();
            }
            else
            {
                UpdateInstruction("Scene reset - ready for new experiment");
            }
            
            Debug.Log("Scene reset complete");
        }
        
        /// <summary>
        /// Monitor performance and adjust settings if needed
        /// </summary>
        private void MonitorPerformance()
        {
            float currentFPS = 1f / Time.unscaledDeltaTime;
            
            if (currentFPS < targetFrameRate * 0.8f) // If FPS drops below 80% of target
            {
                // Reduce quality settings
                AdjustQualityForPerformance();
            }
        }
        
        /// <summary>
        /// Adjust quality settings for better performance
        /// </summary>
        private void AdjustQualityForPerformance()
        {
            // Reduce LOD bias
            QualitySettings.lodBias = Mathf.Max(0.5f, QualitySettings.lodBias - 0.1f);
            
            // Reduce shadow distance
            QualitySettings.shadowDistance = Mathf.Max(20f, QualitySettings.shadowDistance - 5f);
            
            Debug.Log("Adjusted quality settings for better performance");
        }
        
        /// <summary>
        /// Update instruction text
        /// </summary>
        private void UpdateInstruction(string instruction)
        {
            if (uiHelper != null)
            {
                uiHelper.SetText(instruction);
            }
            
            OnInstructionChanged?.Invoke(instruction);
            Debug.Log($"Instruction: {instruction}");
        }
        
        /// <summary>
        /// Set master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            AudioListener.volume = Mathf.Clamp01(volume);
        }
        
        /// <summary>
        /// Play ambient sounds randomly
        /// </summary>
        private IEnumerator PlayAmbientSounds()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(10f, 30f));
                
                if (ambientSounds != null && ambientSounds.Length > 0)
                {
                    AudioClip randomSound = ambientSounds[Random.Range(0, ambientSounds.Length)];
                    AudioSource.PlayClipAtPoint(randomSound, Camera.main.transform.position, 0.3f);
                }
            }
        }
        
        // Event handlers
        private void OnCupSelected(string cupLabel)
        {
            lastSelectedCup = cupLabel; // Track the selected cup
            experimentData[$"cup_{cupLabel}_selected"] = Time.time;
            
            // Update new UI system
            if (scienceLabUI != null)
            {
                scienceLabUI.OnCupSelected(cupLabel);
            }
            
            if (enableHints)
            {
                UpdateInstruction($"Cup {cupLabel} selected. Try pouring water or using the spoon!");
            }
            
            Debug.Log($"Cup {cupLabel} selected");
        }
        
        private void OnLiquidAdded(float amount)
        {
            // Update new UI system
            if (scienceLabUI != null)
            {
                // Use the last selected cup or default to "Cup"
                string cupName = !string.IsNullOrEmpty(lastSelectedCup) ? $"Cup {lastSelectedCup}" : "Cup";
                scienceLabUI.OnLiquidPoured(cupName, amount);
            }
            
            if (enableHints)
            {
                UpdateInstruction($"Added {amount:F1}ml of liquid. Use the spoon to mix!");
            }
            
            Debug.Log($"Added {amount:F1}ml of liquid");
        }
        
        private void OnLiquidRemoved(float amount)
        {
            if (enableHints)
            {
                UpdateInstruction($"Removed {amount:F1}ml of liquid.");
            }
            
            Debug.Log($"Removed {amount:F1}ml of liquid");
        }
        
        /// <summary>
        /// Get experiment data for analysis
        /// </summary>
        public Dictionary<string, float> GetExperimentData()
        {
            return new Dictionary<string, float>(experimentData);
        }
        
        /// <summary>
        /// Check if scene is ready
        /// </summary>
        public bool IsSceneReady()
        {
            return sceneInitialized;
        }
        
        /// <summary>
        /// Get all cups in the scene
        /// </summary>
        public List<CupInteraction> GetAllCups()
        {
            return new List<CupInteraction>(allCups);
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            // Handle application pause/resume
            if (pauseStatus)
            {
                if (backgroundMusic != null)
                    backgroundMusic.Pause();
            }
            else
            {
                if (backgroundMusic != null)
                    backgroundMusic.UnPause();
            }
        }
    }
}
