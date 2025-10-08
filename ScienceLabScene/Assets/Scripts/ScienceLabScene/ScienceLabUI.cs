using UnityEngine;
using System.Collections.Generic;

namespace ScienceLabScene
{
    /// <summary>
    /// Comprehensive UI system for the Science Lab with panels, notifications, and guidance
    /// </summary>
    public class ScienceLabUI : MonoBehaviour
    {
        [Header("UI Settings")]
        public bool showUI = true;
        public int fontSize = 18;
        public int titleFontSize = 22;
        public Color panelColor = new Color(0.05f, 0.05f, 0.05f, 0.9f);
        public Color textColor = Color.white;
        public Color highlightColor = new Color(0.3f, 0.8f, 1f, 1f); // Light blue
        public Color buttonColor = new Color(0.2f, 0.6f, 1f, 1f); // Blue
        public Color successColor = new Color(0.2f, 0.8f, 0.2f, 1f); // Green
        
        [Header("Responsive Layout")]
        public float panelMargin = 20f;
        public float panelPadding = 15f;
        
        [Header("References")]
        public ScienceLabController labController;
        
        // UI Styles
        private GUIStyle panelStyle;
        private GUIStyle textStyle;
        private GUIStyle titleStyle;
        private GUIStyle buttonStyle;
        private GUIStyle highlightStyle;
        
        // UI State
        private string currentInstruction = "Welcome to the Science Lab!";
        private string currentStatus = "Ready to begin experiments";
        private List<string> actionHistory = new List<string>();
        private Dictionary<string, float> experimentData = new Dictionary<string, float>();
        private bool showWelcomeScreen = true;
        
        // Notification system
        private string notification = "";
        private float notificationTimer = 0f;
        private float notificationDuration = 3f;
        
        // Responsive layout
        private Rect mainPanelRect;
        private Rect instructionPanelRect;
        private Rect actionPanelRect;
        private Rect controlPanelRect;
        
        void Start()
        {
            // Find lab controller if not assigned
            if (labController == null)
                labController = FindFirstObjectByType<ScienceLabController>();
            
            // Initialize experiment data
            InitializeExperimentData();
            
            // Set initial instruction
            SetInstruction("Click on cups (A-H) to select them. Use the water bottle to pour liquid.");
        }
        
        void Update()
        {
            // Update notification timer
            if (notificationTimer > 0)
            {
                notificationTimer -= Time.deltaTime;
                if (notificationTimer <= 0)
                {
                    notification = "";
                }
            }
            
            // Update responsive layout
            UpdateResponsiveLayout();
        }
        
        void OnGUI()
        {
            if (!showUI) return;
            
            // Setup styles if needed
            if (panelStyle == null)
                SetupGUIStyles();
            
            // Show welcome screen or main UI
            if (showWelcomeScreen)
            {
                DrawWelcomeScreen();
            }
            else
            {
                DrawMainUI();
            }
            
            // Always draw notifications on top
            DrawNotification();
        }
        
        /// <summary>
        /// Setup GUI styles
        /// </summary>
        private void SetupGUIStyles()
        {
            if (GUI.skin == null) return;
            
            // Panel style
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = CreateColorTexture(panelColor);
            panelStyle.padding = new RectOffset((int)panelPadding, (int)panelPadding, (int)panelPadding, (int)panelPadding);
            
            // Text style
            textStyle = new GUIStyle(GUI.skin.label);
            textStyle.fontSize = fontSize;
            textStyle.normal.textColor = textColor;
            textStyle.wordWrap = true;
            textStyle.alignment = TextAnchor.MiddleLeft;
            
            // Title style
            titleStyle = new GUIStyle(textStyle);
            titleStyle.fontSize = titleFontSize;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = highlightColor;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            
            // Button style
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = fontSize;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.normal.background = CreateColorTexture(buttonColor);
            buttonStyle.hover.background = CreateColorTexture(new Color(buttonColor.r + 0.1f, buttonColor.g + 0.1f, buttonColor.b + 0.1f, buttonColor.a));
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.white;
            buttonStyle.padding = new RectOffset(20, 20, 10, 10);
            
            // Highlight style
            highlightStyle = new GUIStyle(textStyle);
            highlightStyle.normal.textColor = successColor;
            highlightStyle.fontStyle = FontStyle.Bold;
        }
        
        /// <summary>
        /// Update responsive layout based on screen size
        /// </summary>
        private void UpdateResponsiveLayout()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            // Make panels readable but not overlapping
            float panelWidth = 300f; // Fixed readable width
            float spacing = 25f; // More spacing to prevent overlap
            
            // Task panel (top left)
            instructionPanelRect = new Rect(panelMargin, panelMargin, 
                panelWidth, 130f); // Readable height
            
            // Stats panel (below task panel with proper spacing)
            actionPanelRect = new Rect(panelMargin, 
                instructionPanelRect.y + instructionPanelRect.height + spacing, 
                panelWidth, 180f); // Readable height
            
            // Control panel (top right, completely separate)
            float controlWidth = 150f;
            float controlHeight = 140f;
            controlPanelRect = new Rect(
                screenWidth - controlWidth - panelMargin, 
                panelMargin, // Put at top right instead of bottom
                controlWidth, controlHeight);
        }
        
        /// <summary>
        /// Draw welcome screen
        /// </summary>
        private void DrawWelcomeScreen()
        {
            // Semi-transparent overlay
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", 
                new GUIStyle { normal = { background = CreateColorTexture(new Color(0, 0, 0, 0.7f)) } });
            
            // Welcome panel - make it larger and properly centered
            float panelWidth = 600;
            float panelHeight = 500;
            Rect welcomeRect = new Rect(Screen.width / 2 - panelWidth / 2, Screen.height / 2 - panelHeight / 2, 
                panelWidth, panelHeight);
            
            GUI.Box(welcomeRect, "", panelStyle);
            
            GUILayout.BeginArea(welcomeRect);
            GUILayout.Space(30);
            
            GUILayout.Label("🧪 Science Lab Simulator", titleStyle);
            GUILayout.Space(20);
            
            GUILayout.Label("Welcome to the interactive science lab!", textStyle);
            GUILayout.Space(10);
            
            GUILayout.Label("What you can do:", textStyle);
            GUILayout.Label("• Click on cups (A-H) to select them", textStyle);
            GUILayout.Label("• Use the water bottle to pour liquid", textStyle);
            GUILayout.Label("• Stir liquids with the spoon", textStyle);
            GUILayout.Label("• Examine objects with magnifying glass (M key)", textStyle);
            
            GUILayout.Space(30);
            
            if (GUILayout.Button("🚀 Start Experiment", buttonStyle, GUILayout.Height(50)))
            {
                StartExperiment();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("❌ Exit", buttonStyle, GUILayout.Height(40)))
            {
                Application.Quit();
            }
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Draw main UI
        /// </summary>
        private void DrawMainUI()
        {
            DrawInstructionPanel();
            DrawActionPanel();
            DrawControlPanel();
        }
        
        /// <summary>
        /// Draw instruction panel
        /// </summary>
        private void DrawInstructionPanel()
        {
            GUI.Box(instructionPanelRect, "", panelStyle);
            
            GUILayout.BeginArea(instructionPanelRect);
            GUILayout.Space(8);
            
            // Normal title
            GUILayout.Label("📋 Current Task", titleStyle);
            GUILayout.Space(8);
            
            // Readable text
            GUIStyle readableTextStyle = new GUIStyle(textStyle);
            readableTextStyle.fontSize = fontSize - 1;
            readableTextStyle.wordWrap = true;
            
            GUILayout.Label(currentInstruction, readableTextStyle);
            GUILayout.Space(8);
            
            GUIStyle readableHighlightStyle = new GUIStyle(highlightStyle);
            readableHighlightStyle.fontSize = fontSize - 1;
                
            GUILayout.Label($"Status: {currentStatus}", readableHighlightStyle);
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Draw action panel
        /// </summary>
        private void DrawActionPanel()
        {
            GUI.Box(actionPanelRect, "", panelStyle);
            
            GUILayout.BeginArea(actionPanelRect);
            GUILayout.Space(8);
            
            // Normal title
            GUILayout.Label("📊 Experiment Stats", titleStyle);
            GUILayout.Space(8);
            
            // Readable text style
            GUIStyle readableTextStyle = new GUIStyle(textStyle);
            readableTextStyle.fontSize = fontSize - 1;
            
            GUIStyle readableHighlightStyle = new GUIStyle(highlightStyle);
            readableHighlightStyle.fontSize = fontSize - 1;
            
            // Show experiment data in readable format
            GUILayout.Label($"🧪 Experiments Performed: {experimentData.GetValueOrDefault("Total Pours", 0)}", readableTextStyle);
            GUILayout.Label($"🥄 Times Stirred: {experimentData.GetValueOrDefault("Stir Count", 0)}", readableTextStyle);
            GUILayout.Label($"🔍 Magnifications: {experimentData.GetValueOrDefault("Magnify Uses", 0)}", readableTextStyle);
            
            GUILayout.Space(8);
            
            // Show recent actions (last 3)
            if (actionHistory.Count > 0)
            {
                GUILayout.Label("Recent Actions:", readableHighlightStyle);
                int startIndex = Mathf.Max(0, actionHistory.Count - 3);
                for (int i = startIndex; i < actionHistory.Count; i++)
                {
                    // Keep action text readable
                    string shortAction = actionHistory[i];
                    if (shortAction.Length > 35)
                        shortAction = shortAction.Substring(0, 32) + "...";
                    GUILayout.Label($"• {shortAction}", readableTextStyle);
                }
            }
            else
            {
                GUILayout.Label("No experiments performed yet.", readableTextStyle);
                GUILayout.Label("Click on a cup to get started!", readableHighlightStyle);
            }
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Draw control panel
        /// </summary>
        private void DrawControlPanel()
        {
            GUI.Box(controlPanelRect, "", panelStyle);
            
            GUILayout.BeginArea(controlPanelRect);
            GUILayout.Space(8);
            
            // Readable button style
            GUIStyle readableButtonStyle = new GUIStyle(buttonStyle);
            readableButtonStyle.fontSize = fontSize - 1;
            
            if (GUILayout.Button("🔄 Reset Lab", readableButtonStyle, GUILayout.Height(35)))
            {
                ResetLab();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("📋 New Experiment", readableButtonStyle, GUILayout.Height(35)))
            {
                NewExperiment();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("❌ Main Menu", readableButtonStyle, GUILayout.Height(35)))
            {
                showWelcomeScreen = true;
            }
            
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Draw notification popup
        /// </summary>
        private void DrawNotification()
        {
            if (!string.IsNullOrEmpty(notification) && notificationTimer > 0)
            {
                float alpha = Mathf.Clamp01(notificationTimer / notificationDuration);
                
                // Notification background
                Rect notifRect = new Rect(Screen.width / 2 - 200, 80, 400, 60);
                GUI.Box(notifRect, "", new GUIStyle(panelStyle) 
                { 
                    normal = { background = CreateColorTexture(new Color(successColor.r, successColor.g, successColor.b, alpha * 0.9f)) }
                });
                
                // Notification text
                GUIStyle notifStyle = new GUIStyle(titleStyle);
                notifStyle.normal.textColor = new Color(1f, 1f, 1f, alpha);
                notifStyle.alignment = TextAnchor.MiddleCenter;
                
                GUI.Label(notifRect, notification, notifStyle);
            }
        }
        
        /// <summary>
        /// Start experiment
        /// </summary>
        private void StartExperiment()
        {
            showWelcomeScreen = false;
            SetInstruction("Select a cup (A-H) by clicking on it to begin your experiment.");
            SetStatus("Experiment started - Ready to begin!");
            ShowNotification("🚀 Experiment Started!");
        }
        
        /// <summary>
        /// New experiment
        /// </summary>
        private void NewExperiment()
        {
            ClearHistory();
            InitializeExperimentData();
            SetInstruction("Select a cup (A-H) by clicking on it to begin your experiment.");
            SetStatus("Ready for new experiment!");
            ShowNotification("📋 New Experiment Started!");
        }
        
        /// <summary>
        /// Set current instruction text
        /// </summary>
        public void SetInstruction(string instruction)
        {
            currentInstruction = instruction;
        }
        
        /// <summary>
        /// Set current status text
        /// </summary>
        public void SetStatus(string status)
        {
            currentStatus = status;
        }
        
        /// <summary>
        /// Add an action to the history
        /// </summary>
        public void AddAction(string action)
        {
            actionHistory.Add($"{System.DateTime.Now:HH:mm:ss} - {action}");
            
            // Keep only last 10 actions
            if (actionHistory.Count > 10)
            {
                actionHistory.RemoveAt(0);
            }
        }
        
        /// <summary>
        /// Show notification popup
        /// </summary>
        public void ShowNotification(string message)
        {
            notification = message;
            notificationTimer = notificationDuration;
        }
        
        /// <summary>
        /// Update experiment data
        /// </summary>
        public void UpdateExperimentData(string key, float value)
        {
            experimentData[key] = value;
        }
        
        /// <summary>
        /// Handle cup selection
        /// </summary>
        public void OnCupSelected(string cupName)
        {
            AddAction($"Selected Cup {cupName}");
            SetStatus($"Cup {cupName} is now selected");
            SetInstruction("Pour water into the selected cup or select another cup to compare.");
            ShowNotification($"Cup {cupName} Selected!");
            
            UpdateExperimentData("Selected Cup", cupName[0] - 'A' + 1);
        }
        
        /// <summary>
        /// Handle liquid operations
        /// </summary>
        public void OnLiquidPoured(string cupName, float amount)
        {
            AddAction($"Poured {amount:F1}ml into {cupName}");
            SetStatus($"Added liquid to {cupName}");
            SetInstruction("Use the spoon to stir the liquid or select another cup.");
            ShowNotification($"💧 Liquid Added to {cupName}!");
            
            UpdateExperimentData($"{cupName} Volume", amount);
            UpdateExperimentData("Total Pours", experimentData.GetValueOrDefault("Total Pours", 0) + 1);
        }
        
        /// <summary>
        /// Handle stirring action
        /// </summary>
        public void OnLiquidStirred(string cupName)
        {
            AddAction($"Stirred liquid in Cup {cupName}");
            SetStatus($"Stirring Cup {cupName}");
            SetInstruction("Liquid is being mixed. You can use the magnifying glass to examine it closely.");
            ShowNotification($"Stirring Cup {cupName}!");
            
            UpdateExperimentData("Stir Count", experimentData.GetValueOrDefault("Stir Count", 0) + 1);
        }
        
        /// <summary>
        /// Handle magnifying glass usage
        /// </summary>
        public void OnMagnifyingGlassUsed()
        {
            AddAction("Used magnifying glass");
            SetStatus("Examining with magnifying glass");
            SetInstruction("Move the magnifying glass around to examine different objects closely.");
            ShowNotification("Magnifying Glass Activated!");
            
            UpdateExperimentData("Magnify Uses", experimentData.GetValueOrDefault("Magnify Uses", 0) + 1);
        }
        
        /// <summary>
        /// Reset the lab
        /// </summary>
        private void ResetLab()
        {
            if (labController != null)
            {
                labController.ResetScene();
            }
            
            AddAction("Lab reset");
            SetStatus("Lab has been reset");
            SetInstruction("Lab is ready for new experiments. Start by selecting a cup.");
            ShowNotification("Lab Reset!");
            
            InitializeExperimentData();
        }
        
        /// <summary>
        /// Clear action history
        /// </summary>
        private void ClearHistory()
        {
            actionHistory.Clear();
            SetStatus("Action history cleared");
            ShowNotification("📝 History Cleared!");
        }
        
        /// <summary>
        /// Initialize experiment data
        /// </summary>
        private void InitializeExperimentData()
        {
            experimentData.Clear();
            experimentData["Total Pours"] = 0;
            experimentData["Stir Count"] = 0;
            experimentData["Magnify Uses"] = 0;
            experimentData["Selected Cup"] = 0;
        }
        
        /// <summary>
        /// Create a colored texture for GUI backgrounds
        /// </summary>
        private Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
        
        /// <summary>
        /// Toggle UI visibility
        /// </summary>
        public void ToggleUI()
        {
            showUI = !showUI;
        }
    }
}
