using UnityEngine;
using System.Collections;

namespace ScienceLabScene
{
    /// <summary>
    /// Controls the water bottle behavior including pouring animation and liquid management
    /// </summary>
    public class WaterBottleController : MonoBehaviour
    {
        [Header("Bottle Properties")]
        public float waterCapacity = 500f;
        public float currentWater = 500f;
        public float pourRate = 50f; // ml per second
        public Color waterColor = Color.blue;
        
        [Header("Animation")]
        public Animator bottleAnimator;
        public string pourAnimationName = "PourAnimation";
        public Transform pourPoint;
        public float pourHeight = 0.5f;
        
        [Header("Visual Effects")]
        public GameObject waterStreamPrefab;
        public ParticleSystem pourParticles;
        public LineRenderer waterStream;
        
        [Header("Audio")]
        public AudioClip pourStartSound;
        public AudioClip pourLoopSound;
        public AudioClip pourEndSound;
        
        [Header("Interaction")]
        public LayerMask cupLayerMask = -1;
        public float maxPourDistance = 2f;
        
        private AudioSource audioSource;
        private bool isPouring = false;
        private bool canPour = true;
        private Coroutine pourCoroutine;
        private GameObject currentWaterStream;
        private CupInteraction targetCup;
        
        void Start()
        {
            // Get or add components
            if (bottleAnimator == null)
                bottleAnimator = GetComponent<Animator>();
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.7f;
            }
            
            // Set up pour point if not assigned
            if (pourPoint == null)
            {
                GameObject pourPointGO = new GameObject("PourPoint");
                pourPointGO.transform.SetParent(transform);
                pourPointGO.transform.localPosition = new Vector3(0, 0.3f, 0.1f);
                pourPoint = pourPointGO.transform;
            }
            
            // Initialize water stream
            if (waterStream == null)
                waterStream = GetComponent<LineRenderer>();
            
            if (waterStream != null)
            {
                waterStream.enabled = false;
                waterStream.startWidth = 0.02f;
                waterStream.endWidth = 0.01f;
                waterStream.material = CreateWaterMaterial();
            }
            
            // Set up interaction
            if (GetComponent<Collider>() == null)
            {
                var collider = gameObject.AddComponent<MeshCollider>();
                collider.convex = true;
            }
        }
        
        void Update()
        {
            // Handle input for pouring
            if (Input.GetMouseButtonDown(0) && canPour && !isPouring)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == GetComponent<Collider>())
                    {
                        StartPouring();
                    }
                }
            }
            
            if (Input.GetMouseButtonUp(0) && isPouring)
            {
                StopPouring();
            }
            
            // Update target cup detection while pouring
            if (isPouring)
            {
                DetectTargetCup();
            }
        }
        
        /// <summary>
        /// Start the pouring animation and process
        /// </summary>
        public void StartPouring()
        {
            if (!canPour || currentWater <= 0f)
            {
                Debug.Log("Cannot pour: bottle empty or pouring disabled");
                return;
            }
            
            isPouring = true;
            
            // Play animation
            if (bottleAnimator != null)
            {
                bottleAnimator.SetBool("IsPour", true);
                bottleAnimator.Play(pourAnimationName);
            }
            
            // Start pour coroutine
            pourCoroutine = StartCoroutine(PourProcess());
            
            // Play sound
            if (pourStartSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pourStartSound);
            }
            
            Debug.Log("Started pouring water");
        }
        
        /// <summary>
        /// Stop the pouring process
        /// </summary>
        public void StopPouring()
        {
            if (!isPouring) return;
            
            isPouring = false;
            
            // Stop animation
            if (bottleAnimator != null)
            {
                bottleAnimator.SetBool("IsPour", false);
            }
            
            // Stop pour coroutine
            if (pourCoroutine != null)
            {
                StopCoroutine(pourCoroutine);
                pourCoroutine = null;
            }
            
            // Hide water stream
            HideWaterStream();
            
            // Stop particles
            if (pourParticles != null)
            {
                pourParticles.Stop();
            }
            
            // Play end sound
            if (pourEndSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pourEndSound);
            }
            
            targetCup = null;
            
            Debug.Log("Stopped pouring water");
        }
        
        /// <summary>
        /// Main pouring process coroutine
        /// </summary>
        private IEnumerator PourProcess()
        {
            // Start particles
            if (pourParticles != null)
            {
                pourParticles.Play();
            }
            
            // Play loop sound
            if (pourLoopSound != null && audioSource != null)
            {
                audioSource.clip = pourLoopSound;
                audioSource.loop = true;
                audioSource.Play();
            }
            
            while (isPouring && currentWater > 0f)
            {
                float deltaTime = Time.deltaTime;
                float pourAmount = pourRate * deltaTime;
                
                // Limit pour amount by remaining water
                pourAmount = Mathf.Min(pourAmount, currentWater);
                
                // Remove water from bottle
                currentWater -= pourAmount;
                
                // Show water stream
                ShowWaterStream();
                
                // Pour into target cup if detected
                if (targetCup != null)
                {
                    targetCup.AddLiquid(pourAmount, waterColor);
                }
                
                // Check if bottle is empty
                if (currentWater <= 0f)
                {
                    Debug.Log("Bottle is now empty");
                    break;
                }
                
                yield return null;
            }
            
            // Stop loop sound
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }
            
            // Auto-stop if empty
            if (currentWater <= 0f)
            {
                StopPouring();
            }
        }
        
        /// <summary>
        /// Detect which cup is under the pour point
        /// </summary>
        private void DetectTargetCup()
        {
            Vector3 pourPosition = pourPoint.position;
            Vector3 downDirection = Vector3.down;
            
            RaycastHit hit;
            if (Physics.Raycast(pourPosition, downDirection, out hit, maxPourDistance, cupLayerMask))
            {
                CupInteraction cup = hit.collider.GetComponent<CupInteraction>();
                if (cup != null && cup != targetCup)
                {
                    targetCup = cup;
                    Debug.Log($"Pouring into cup {cup.cupLabel}");
                }
            }
            else
            {
                if (targetCup != null)
                {
                    Debug.Log("No longer pouring into cup");
                    targetCup = null;
                }
            }
        }
        
        /// <summary>
        /// Show the water stream visual effect
        /// </summary>
        private void ShowWaterStream()
        {
            if (waterStream != null)
            {
                waterStream.enabled = true;
                
                Vector3 startPos = pourPoint.position;
                Vector3 endPos = startPos + Vector3.down * pourHeight;
                
                // If there's a target cup, aim for it
                if (targetCup != null)
                {
                    endPos = targetCup.transform.position + Vector3.up * 0.1f;
                }
                
                waterStream.positionCount = 2;
                waterStream.SetPosition(0, startPos);
                waterStream.SetPosition(1, endPos);
            }
            
            // Create water stream prefab if available
            if (waterStreamPrefab != null && currentWaterStream == null)
            {
                currentWaterStream = Instantiate(waterStreamPrefab, pourPoint.position, Quaternion.identity);
                currentWaterStream.transform.SetParent(pourPoint);
            }
        }
        
        /// <summary>
        /// Hide the water stream visual effect
        /// </summary>
        private void HideWaterStream()
        {
            if (waterStream != null)
            {
                waterStream.enabled = false;
            }
            
            if (currentWaterStream != null)
            {
                Destroy(currentWaterStream);
                currentWaterStream = null;
            }
        }
        
        /// <summary>
        /// Create a material for the water stream
        /// </summary>
        private Material CreateWaterMaterial()
        {
            Material waterMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            waterMat.color = waterColor;
            waterMat.SetFloat("_Metallic", 0f);
            waterMat.SetFloat("_Smoothness", 0.9f);
            waterMat.SetFloat("_Surface", 1f); // Transparent
            waterMat.SetFloat("_Blend", 0f);   // Alpha blend
            
            return waterMat;
        }
        
        /// <summary>
        /// Refill the bottle to full capacity
        /// </summary>
        public void RefillBottle()
        {
            currentWater = waterCapacity;
            Debug.Log("Bottle refilled");
        }
        
        /// <summary>
        /// Set the pour rate
        /// </summary>
        /// <param name="rate">New pour rate in ml/second</param>
        public void SetPourRate(float rate)
        {
            pourRate = Mathf.Max(0f, rate);
        }
        
        /// <summary>
        /// Enable or disable pouring capability
        /// </summary>
        /// <param name="enabled">Whether pouring is enabled</param>
        public void SetPouringEnabled(bool enabled)
        {
            canPour = enabled;
            
            if (!enabled && isPouring)
            {
                StopPouring();
            }
        }
        
        /// <summary>
        /// Get current water amount
        /// </summary>
        /// <returns>Current water in ml</returns>
        public float GetCurrentWater()
        {
            return currentWater;
        }
        
        /// <summary>
        /// Get water capacity
        /// </summary>
        /// <returns>Total capacity in ml</returns>
        public float GetCapacity()
        {
            return waterCapacity;
        }
        
        /// <summary>
        /// Check if bottle is empty
        /// </summary>
        /// <returns>True if empty</returns>
        public bool IsEmpty()
        {
            return currentWater <= 0f;
        }
        
        /// <summary>
        /// Check if currently pouring
        /// </summary>
        /// <returns>True if pouring</returns>
        public bool IsPouring()
        {
            return isPouring;
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw pour detection ray
            if (pourPoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(pourPoint.position, Vector3.down * maxPourDistance);
            }
        }
    }
}
