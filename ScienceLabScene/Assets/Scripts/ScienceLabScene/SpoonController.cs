using UnityEngine;
using System.Collections;

namespace ScienceLabScene
{
    /// <summary>
    /// Controls the spoon behavior including stirring animation and mixing functionality
    /// </summary>
    public class SpoonController : MonoBehaviour
    {
        [Header("Stirring Properties")]
        public float stirRadius = 0.08f;
        public float stirSpeed = 2f;
        public int stirRevolutions = 3;
        public Transform stirCenter;
        
        [Header("Animation")]
        public Animator spoonAnimator;
        public string stirAnimationName = "StirAnimation";
        
        [Header("Visual Effects")]
        public ParticleSystem stirParticles;
        public GameObject rippleEffect;
        
        [Header("Audio")]
        public AudioClip stirSound;
        public AudioClip mixingSound;
        
        [Header("Interaction")]
        public LayerMask cupLayerMask = -1;
        public float detectionRadius = 0.2f;
        
        private AudioSource audioSource;
        private bool isStirring = false;
        private bool canStir = true;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Coroutine stirCoroutine;
        private CupInteraction targetCup;
        
        void Start()
        {
            // Store original transform
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            
            // Get or add components
            if (spoonAnimator == null)
                spoonAnimator = GetComponent<Animator>();
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.6f;
            }
            
            // Set up stir center if not assigned
            if (stirCenter == null)
            {
                // Default to mixing cup position
                GameObject mixingCup = GameObject.Find("Cup_MIX");
                if (mixingCup != null)
                {
                    stirCenter = mixingCup.transform;
                }
                else
                {
                    // Create default stir center
                    GameObject stirCenterGO = new GameObject("StirCenter");
                    stirCenterGO.transform.position = new Vector3(0, 0.5f, 0.25f);
                    stirCenter = stirCenterGO.transform;
                }
            }
            
            // Set up interaction
            if (GetComponent<Collider>() == null)
            {
                var collider = gameObject.AddComponent<CapsuleCollider>();
                collider.isTrigger = false;
            }
        }
        
        void Update()
        {
            // Handle input for stirring
            if (Input.GetMouseButtonDown(0) && canStir && !isStirring)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == GetComponent<Collider>())
                    {
                        StartStirring();
                    }
                }
            }
            
            // Detect nearby cups
            DetectNearbyCup();
        }
        
        /// <summary>
        /// Start the stirring animation and process
        /// </summary>
        public void StartStirring()
        {
            if (!canStir || isStirring)
            {
                Debug.Log("Cannot stir: already stirring or stirring disabled");
                return;
            }
            
            // Check if there's a cup to stir
            if (targetCup == null || targetCup.IsEmpty())
            {
                Debug.Log("No liquid to stir");
                return;
            }
            
            isStirring = true;
            
            // Play animation
            if (spoonAnimator != null)
            {
                spoonAnimator.SetBool("IsStirring", true);
                spoonAnimator.Play(stirAnimationName);
            }
            
            // Start stirring coroutine
            stirCoroutine = StartCoroutine(StirProcess());
            
            // Play sound
            if (stirSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(stirSound);
            }
            
            Debug.Log("Started stirring");
            
            // Notify UI system
            ScienceLabUI uiSystem = FindFirstObjectByType<ScienceLabUI>();
            if (uiSystem != null && targetCup != null)
            {
                uiSystem.OnLiquidStirred(targetCup.cupLabel);
            }
        }
        
        /// <summary>
        /// Stop the stirring process
        /// </summary>
        public void StopStirring()
        {
            if (!isStirring) return;
            
            isStirring = false;
            
            // Stop animation
            if (spoonAnimator != null)
            {
                spoonAnimator.SetBool("IsStirring", false);
            }
            
            // Stop stirring coroutine
            if (stirCoroutine != null)
            {
                StopCoroutine(stirCoroutine);
                stirCoroutine = null;
            }
            
            // Return to original position
            StartCoroutine(ReturnToOriginalPosition());
            
            // Stop particles
            if (stirParticles != null)
            {
                stirParticles.Stop();
            }
            
            // Stop audio
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            Debug.Log("Stopped stirring");
        }
        
        /// <summary>
        /// Main stirring process coroutine
        /// </summary>
        private IEnumerator StirProcess()
        {
            if (targetCup == null) yield break;
            
            // Move to stirring position
            Vector3 stirPosition = stirCenter.position;
            stirPosition.y = targetCup.transform.position.y + 0.25f; // Above liquid level
            
            yield return StartCoroutine(MoveToPosition(stirPosition, 0.5f));
            
            // Start particles
            if (stirParticles != null)
            {
                stirParticles.Play();
            }
            
            // Start mixing sound
            if (mixingSound != null && audioSource != null)
            {
                audioSource.clip = mixingSound;
                audioSource.loop = true;
                audioSource.Play();
            }
            
            // Perform circular stirring motion
            float totalTime = stirRevolutions / stirSpeed;
            float elapsedTime = 0f;
            
            Vector3 centerPos = stirPosition;
            
            while (elapsedTime < totalTime && isStirring)
            {
                float progress = elapsedTime / totalTime;
                float angle = progress * stirRevolutions * 2f * Mathf.PI;
                
                // Calculate circular position
                Vector3 offset = new Vector3(
                    stirRadius * Mathf.Cos(angle),
                    0f,
                    stirRadius * Mathf.Sin(angle)
                );
                
                transform.position = centerPos + offset;
                
                // Rotate spoon to follow the stirring motion
                Vector3 direction = offset.normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, 0.5f);
                }
                
                // Create ripple effect periodically
                if (rippleEffect != null && Mathf.Sin(angle * 4f) > 0.9f)
                {
                    CreateRippleEffect(transform.position);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Stop mixing sound
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }
            
            // Trigger mixing effect on the cup
            if (targetCup != null)
            {
                TriggerMixingEffect();
            }
            
            // Auto-stop stirring
            StopStirring();
        }
        
        /// <summary>
        /// Move spoon to a specific position smoothly
        /// </summary>
        private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                progress = Mathf.SmoothStep(0f, 1f, progress); // Smooth interpolation
                
                transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.position = targetPosition;
        }
        
        /// <summary>
        /// Return spoon to original position
        /// </summary>
        private IEnumerator ReturnToOriginalPosition()
        {
            yield return StartCoroutine(MoveToPosition(originalPosition, 1f));
            
            // Return to original rotation
            float elapsedTime = 0f;
            float duration = 0.5f;
            Quaternion startRotation = transform.rotation;
            
            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                progress = Mathf.SmoothStep(0f, 1f, progress);
                
                transform.rotation = Quaternion.Slerp(startRotation, originalRotation, progress);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.rotation = originalRotation;
        }
        
        /// <summary>
        /// Detect nearby cups for stirring
        /// </summary>
        private void DetectNearbyCup()
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRadius, cupLayerMask);
            
            CupInteraction nearestCup = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var collider in nearbyColliders)
            {
                CupInteraction cup = collider.GetComponent<CupInteraction>();
                if (cup != null && !cup.IsEmpty())
                {
                    float distance = Vector3.Distance(transform.position, cup.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestCup = cup;
                    }
                }
            }
            
            if (nearestCup != targetCup)
            {
                targetCup = nearestCup;
                if (targetCup != null)
                {
                    Debug.Log($"Spoon near cup {targetCup.cupLabel}");
                }
            }
        }
        
        /// <summary>
        /// Create ripple effect in the liquid
        /// </summary>
        private void CreateRippleEffect(Vector3 position)
        {
            if (rippleEffect != null)
            {
                GameObject ripple = Instantiate(rippleEffect, position, Quaternion.identity);
                Destroy(ripple, 2f); // Clean up after 2 seconds
            }
        }
        
        /// <summary>
        /// Trigger mixing effect on the target cup
        /// </summary>
        private void TriggerMixingEffect()
        {
            if (targetCup == null) return;
            
            // Could trigger color mixing, temperature changes, chemical reactions, etc.
            Debug.Log($"Mixed contents of cup {targetCup.cupLabel}");
            
            // Example: Slightly change liquid color to show mixing
            // This would be expanded based on the specific educational simulation needs
        }
        
        /// <summary>
        /// Set stirring parameters
        /// </summary>
        public void SetStirringParameters(float radius, float speed, int revolutions)
        {
            stirRadius = Mathf.Max(0.01f, radius);
            stirSpeed = Mathf.Max(0.1f, speed);
            stirRevolutions = Mathf.Max(1, revolutions);
        }
        
        /// <summary>
        /// Enable or disable stirring capability
        /// </summary>
        public void SetStirringEnabled(bool enabled)
        {
            canStir = enabled;
            
            if (!enabled && isStirring)
            {
                StopStirring();
            }
        }
        
        /// <summary>
        /// Check if currently stirring
        /// </summary>
        public bool IsStirring()
        {
            return isStirring;
        }
        
        /// <summary>
        /// Get the target cup for stirring
        /// </summary>
        public CupInteraction GetTargetCup()
        {
            return targetCup;
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            // Draw stirring radius
            if (stirCenter != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(stirCenter.position, stirRadius);
            }
        }
    }
}
