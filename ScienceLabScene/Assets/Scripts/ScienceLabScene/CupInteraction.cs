using UnityEngine;
using UnityEngine.Events;

namespace ScienceLabScene
{
    /// <summary>
    /// Handles interaction with laboratory cups including selection, highlighting, and content management
    /// </summary>
    public class CupInteraction : MonoBehaviour
    {
        [Header("Cup Properties")]
        public string cupLabel = "A";
        public float maxCapacity = 100f;
        public float currentAmount = 0f;
        public Color liquidColor = Color.blue;
        
        [Header("Visual Feedback")]
        public Material normalMaterial;
        public Material highlightMaterial;
        public GameObject liquidPrefab;
        public Transform liquidContainer;
        
        [Header("Audio")]
        public AudioClip pourSound;
        public AudioClip selectSound;
        
        [Header("Events")]
        public UnityEvent<string> OnCupSelected;
        public UnityEvent<float> OnLiquidAdded;
        public UnityEvent<float> OnLiquidRemoved;
        
        private Renderer cupRenderer;
        private AudioSource audioSource;
        private GameObject currentLiquid;
        private bool isSelected = false;
        private bool isHighlighted = false;
        
        // Static reference to currently selected cup
        private static CupInteraction selectedCup;
        
        void Start()
        {
            // Initialize UnityEvents
            if (OnCupSelected == null)
                OnCupSelected = new UnityEvent<string>();
            if (OnLiquidAdded == null)
                OnLiquidAdded = new UnityEvent<float>();
            if (OnLiquidRemoved == null)
                OnLiquidRemoved = new UnityEvent<float>();
            
            // Get components
            cupRenderer = GetComponent<Renderer>();
            audioSource = GetComponent<AudioSource>();
            
            // Initialize audio source if not present
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.5f;
            }
            
            // Store original material
            if (normalMaterial == null)
                normalMaterial = cupRenderer.material;
            
            // Initialize liquid container
            if (liquidContainer == null)
                liquidContainer = transform;
            
            // Set up collider for interaction
            if (GetComponent<Collider>() == null)
            {
                var collider = gameObject.AddComponent<MeshCollider>();
                collider.convex = true;
                collider.isTrigger = false;
            }
        }
        
        void OnMouseEnter()
        {
            if (!isSelected)
            {
                SetHighlight(true);
            }
        }
        
        void OnMouseExit()
        {
            if (!isSelected)
            {
                SetHighlight(false);
            }
        }
        
        void OnMouseDown()
        {
            SelectCup();
        }
        
        /// <summary>
        /// Select this cup and deselect others
        /// </summary>
        public void SelectCup()
        {
            // Deselect previous cup
            if (selectedCup != null && selectedCup != this)
            {
                selectedCup.DeselectCup();
            }
            
            // Select this cup
            selectedCup = this;
            isSelected = true;
            SetHighlight(true);
            
            // Play select sound
            if (selectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(selectSound);
            }
            
            // Trigger event
            OnCupSelected?.Invoke(cupLabel);
            
            Debug.Log($"Cup {cupLabel} selected");
        }
        
        /// <summary>
        /// Deselect this cup
        /// </summary>
        public void DeselectCup()
        {
            isSelected = false;
            SetHighlight(false);
            
            if (selectedCup == this)
                selectedCup = null;
        }
        
        /// <summary>
        /// Set visual highlight state
        /// </summary>
        /// <param name="highlight">Whether to highlight the cup</param>
        private void SetHighlight(bool highlight)
        {
            isHighlighted = highlight;
            
            if (cupRenderer != null)
            {
                if (highlight && highlightMaterial != null)
                {
                    cupRenderer.material = highlightMaterial;
                }
                else if (normalMaterial != null)
                {
                    cupRenderer.material = normalMaterial;
                }
            }
        }
        
        /// <summary>
        /// Add liquid to the cup
        /// </summary>
        /// <param name="amount">Amount to add</param>
        /// <param name="color">Color of the liquid</param>
        public void AddLiquid(float amount, Color color)
        {
            if (currentAmount >= maxCapacity)
            {
                Debug.Log($"Cup {cupLabel} is full!");
                return;
            }
            
            float actualAmount = Mathf.Min(amount, maxCapacity - currentAmount);
            currentAmount += actualAmount;
            liquidColor = Color.Lerp(liquidColor, color, actualAmount / currentAmount);
            
            UpdateLiquidVisual();
            
            // Play pour sound
            if (pourSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pourSound);
            }
            
            // Trigger event
            OnLiquidAdded?.Invoke(actualAmount);
            
            Debug.Log($"Added {actualAmount}ml to Cup {cupLabel}. Total: {currentAmount}ml");
        }
        
        /// <summary>
        /// Remove liquid from the cup
        /// </summary>
        /// <param name="amount">Amount to remove</param>
        public float RemoveLiquid(float amount)
        {
            float actualAmount = Mathf.Min(amount, currentAmount);
            currentAmount -= actualAmount;
            
            UpdateLiquidVisual();
            
            // Trigger event
            OnLiquidRemoved?.Invoke(actualAmount);
            
            Debug.Log($"Removed {actualAmount}ml from Cup {cupLabel}. Remaining: {currentAmount}ml");
            
            return actualAmount;
        }
        
        /// <summary>
        /// Empty the cup completely
        /// </summary>
        public void EmptyCup()
        {
            float removedAmount = currentAmount;
            currentAmount = 0f;
            UpdateLiquidVisual();
            
            OnLiquidRemoved?.Invoke(removedAmount);
            
            Debug.Log($"Cup {cupLabel} emptied. Removed {removedAmount}ml");
        }
        
        /// <summary>
        /// Update the visual representation of liquid in the cup
        /// </summary>
        private void UpdateLiquidVisual()
        {
            if (currentAmount <= 0f)
            {
                // No liquid - hide visual
                if (currentLiquid != null)
                {
                    currentLiquid.SetActive(false);
                }
                return;
            }
            
            // Create liquid visual if it doesn't exist
            if (currentLiquid == null && liquidPrefab != null)
            {
                currentLiquid = Instantiate(liquidPrefab, liquidContainer);
                currentLiquid.transform.localPosition = Vector3.zero;
            }
            
            if (currentLiquid != null)
            {
                currentLiquid.SetActive(true);
                
                // Scale liquid based on amount
                float fillPercentage = currentAmount / maxCapacity;
                Vector3 scale = currentLiquid.transform.localScale;
                scale.y = fillPercentage;
                currentLiquid.transform.localScale = scale;
                
                // Update liquid color
                var renderer = currentLiquid.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = liquidColor;
                }
                
                // Position liquid at bottom of cup
                Vector3 position = currentLiquid.transform.localPosition;
                position.y = -0.15f + (fillPercentage * 0.15f); // Adjust based on cup height
                currentLiquid.transform.localPosition = position;
            }
        }
        
        /// <summary>
        /// Get the current selected cup
        /// </summary>
        /// <returns>Currently selected cup or null</returns>
        public static CupInteraction GetSelectedCup()
        {
            return selectedCup;
        }
        
        /// <summary>
        /// Check if this cup is currently selected
        /// </summary>
        /// <returns>True if selected</returns>
        public bool IsSelected()
        {
            return isSelected;
        }
        
        /// <summary>
        /// Get current liquid amount
        /// </summary>
        /// <returns>Current amount in ml</returns>
        public float GetCurrentAmount()
        {
            return currentAmount;
        }
        
        /// <summary>
        /// Get remaining capacity
        /// </summary>
        /// <returns>Remaining capacity in ml</returns>
        public float GetRemainingCapacity()
        {
            return maxCapacity - currentAmount;
        }
        
        /// <summary>
        /// Check if cup is full
        /// </summary>
        /// <returns>True if full</returns>
        public bool IsFull()
        {
            return currentAmount >= maxCapacity;
        }
        
        /// <summary>
        /// Check if cup is empty
        /// </summary>
        /// <returns>True if empty</returns>
        public bool IsEmpty()
        {
            return currentAmount <= 0f;
        }
    }
}
