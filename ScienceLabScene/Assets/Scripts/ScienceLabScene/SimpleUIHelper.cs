using UnityEngine;

namespace ScienceLabScene
{
    /// <summary>
    /// Simple UI helper that works without requiring Unity UI namespace
    /// Compatible with both legacy GUI and Unity UI systems
    /// </summary>
    public class SimpleUIHelper : MonoBehaviour
    {
        [Header("Text Display")]
        public string currentText = "Science Lab Ready";
        public Vector2 textPosition = new Vector2(350, 10); // Moved to center-top
        public int fontSize = 16;
        public Color textColor = Color.white;
        
        [Header("Button Settings")]
        public Vector2 buttonPosition = new Vector2(350, 50); // Moved to center
        public Vector2 buttonSize = new Vector2(120, 40); // Larger button
        public string buttonText = "Reset";
        
        [Header("Slider Settings")]
        public Vector2 sliderPosition = new Vector2(350, 90); // Moved to center
        public float sliderWidth = 200f;
        public float sliderValue = 1f;
        
        [Header("References")]
        public ScienceLabController labController;
        
        private GUIStyle textStyle;
        private GUIStyle buttonStyle;
        
        void Start()
        {
            // Find lab controller if not assigned
            if (labController == null)
                labController = FindFirstObjectByType<ScienceLabController>();
            
            // Don't set up GUI styles here - they'll be set up in OnGUI when needed
        }
        
        void OnGUI()
        {
            // Ensure styles are set up
            if (textStyle == null || buttonStyle == null)
            {
                SetupGUIStyles();
            }
            
            // Calculate responsive positions to avoid overlap with ScienceLabUI panels
            float centerX = Screen.width * 0.5f; // Center of screen
            float bottomY = Screen.height - 100f; // Bottom area
            
            // Draw instruction text (center-top, away from panels)
            if (!string.IsNullOrEmpty(currentText))
            {
                GUI.Label(new Rect(centerX - 200, 10, 400, 50), currentText, textStyle);
            }
            
            // Draw reset button (bottom center, larger size)
            float buttonWidth = 120f;
            float buttonHeight = 40f;
            if (GUI.Button(new Rect(centerX - buttonWidth/2, bottomY, buttonWidth, buttonHeight), buttonText, buttonStyle))
            {
                OnResetButtonClicked();
            }
            
            // Draw volume controls (better positioned and sized)
            float volumeLabelY = bottomY + 45f;
            float volumeSliderY = volumeLabelY + 30f;
            float sliderWidth = 200f; // Bigger slider
            
            // Create larger volume label style
            GUIStyle volumeLabelStyle = new GUIStyle(textStyle);
            volumeLabelStyle.fontSize = fontSize + 2; // Bigger text
            volumeLabelStyle.fontStyle = FontStyle.Bold;
            volumeLabelStyle.alignment = TextAnchor.MiddleCenter;
            
            // Volume label (centered above slider, bigger text)
            GUI.Label(new Rect(centerX - 40, volumeLabelY, 80, 25), "Volume:", volumeLabelStyle);
            
            // Volume slider (centered and bigger)
            float newVolume = GUI.HorizontalSlider(new Rect(centerX - sliderWidth/2, volumeSliderY, sliderWidth, 25), sliderValue, 0f, 1f);
            
            if (newVolume != sliderValue)
            {
                sliderValue = newVolume;
                OnVolumeChanged(sliderValue);
            }
            
            // Draw additional info
            DrawPerformanceInfo();
        }
        
        /// <summary>
        /// Set up GUI styles for better appearance
        /// </summary>
        private void SetupGUIStyles()
        {
            // Ensure GUI.skin is available
            if (GUI.skin == null) return;
            
            if (textStyle == null)
            {
                textStyle = new GUIStyle(GUI.skin.label);
                textStyle.fontSize = fontSize;
                textStyle.normal.textColor = textColor;
                textStyle.wordWrap = true;
            }
            
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontSize = fontSize; // Same size as text
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.normal.textColor = Color.white;
            }
        }
        
        /// <summary>
        /// Handle reset button click
        /// </summary>
        private void OnResetButtonClicked()
        {
            if (labController != null)
            {
                labController.ResetScene();
            }
            else
            {
                Debug.Log("Reset button clicked - no lab controller found");
            }
        }
        
        /// <summary>
        /// Handle volume change
        /// </summary>
        private void OnVolumeChanged(float volume)
        {
            if (labController != null)
            {
                labController.SetMasterVolume(volume);
            }
            else
            {
                AudioListener.volume = volume;
            }
        }
        
        /// <summary>
        /// Update the instruction text (called by ScienceLabController)
        /// </summary>
        /// <param name="text">New instruction text</param>
        public void SetText(string text)
        {
            currentText = text;
        }
        
        /// <summary>
        /// Draw performance information
        /// </summary>
        private void DrawPerformanceInfo()
        {
            if (Application.isPlaying && textStyle != null)
            {
                float fps = 1f / Time.unscaledDeltaTime;
                string perfInfo = $"FPS: {fps:F1}";
                
                // Move FPS counter to bottom-right, away from control panel
                GUI.Label(new Rect(Screen.width - 100, Screen.height - 25, 90, 20), perfInfo, textStyle);
            }
        }
        
        /// <summary>
        /// Show tutorial step
        /// </summary>
        public void ShowTutorialStep(string step, int stepNumber, int totalSteps)
        {
            string tutorialText = $"Step {stepNumber}/{totalSteps}: {step}";
            SetText(tutorialText);
        }
        
        /// <summary>
        /// Show experiment data
        /// </summary>
        public void ShowExperimentData(System.Collections.Generic.Dictionary<string, float> data)
        {
            if (data == null || data.Count == 0 || textStyle == null) return;
            
            float yPos = Screen.height - 150;
            GUI.Label(new Rect(10, yPos, 300, 20), "Experiment Data:", textStyle);
            
            yPos += 25;
            int count = 0;
            foreach (var kvp in data)
            {
                if (count >= 5) break; // Limit display to 5 items
                
                string dataText = $"{kvp.Key}: {kvp.Value:F2}";
                GUI.Label(new Rect(10, yPos, 300, 20), dataText, textStyle);
                yPos += 20;
                count++;
            }
        }
        
        /// <summary>
        /// Toggle UI visibility
        /// </summary>
        public void SetUIVisible(bool visible)
        {
            enabled = visible;
        }
        
        /// <summary>
        /// Update button text
        /// </summary>
        public void SetButtonText(string text)
        {
            buttonText = text;
        }
        
        /// <summary>
        /// Set text color
        /// </summary>
        public void SetTextColor(Color color)
        {
            textColor = color;
            if (textStyle != null)
            {
                textStyle.normal.textColor = textColor;
            }
        }
    }
}
