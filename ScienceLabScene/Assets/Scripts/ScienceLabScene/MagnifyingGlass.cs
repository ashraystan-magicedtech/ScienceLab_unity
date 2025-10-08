using UnityEngine;

namespace ScienceLabScene
{
    /// <summary>
    /// Controls the magnifying glass functionality with realistic magnification effects
    /// </summary>
    public class MagnifyingGlass : MonoBehaviour
    {
        [Header("Magnification Properties")]
        public float magnificationPower = 2f;
        public float lensRadius = 0.12f;
        public LayerMask magnificationLayers = -1;
        
        [Header("Visual Components")]
        public Camera magnificationCamera;
        public RenderTexture magnificationTexture;
        public Material lensMaterial;
        public Transform lensGlass;
        
        [Header("Movement")]
        public float moveSpeed = 2f;
        public float rotationSpeed = 90f;
        public bool followMouse = true;
        public Transform targetTransform;
        
        [Header("Effects")]
        public Light lensLight;
        public ParticleSystem glintEffect;
        public AudioClip pickupSound;
        public AudioClip putdownSound;
        
        [Header("Interaction")]
        public KeyCode activationKey = KeyCode.M;
        public bool isActive = false;
        
        private AudioSource audioSource;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Camera mainCamera;
        private bool isBeingUsed = false;
        private Vector3 targetPosition;
        
        void Start()
        {
            // Store original transform
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            
            // Get main camera
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindFirstObjectByType<Camera>();
            
            // Get or add audio source
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.5f;
            }
            
            // Set up magnification camera
            SetupMagnificationCamera();
            
            // Set up lens material
            SetupLensMaterial();
            
            // Initialize in inactive state
            SetActive(false);
        }
        
        void Update()
        {
            // Handle activation input
            if (Input.GetKeyDown(activationKey))
            {
                ToggleActive();
            }
            
            // Handle mouse interaction
            HandleMouseInteraction();
            
            // Update magnifying glass behavior
            if (isActive)
            {
                UpdateMagnification();
                
                if (followMouse)
                {
                    FollowMousePosition();
                }
                else if (targetTransform != null)
                {
                    FollowTarget();
                }
            }
            
            // Smooth movement to target position
            if (isBeingUsed)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Handle mouse interaction for picking up/putting down the magnifying glass
        /// </summary>
        private void HandleMouseInteraction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == GetComponent<Collider>())
                    {
                        if (!isActive)
                        {
                            PickUp();
                        }
                        else
                        {
                            PutDown();
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Pick up the magnifying glass
        /// </summary>
        public void PickUp()
        {
            isBeingUsed = true;
            SetActive(true);
            
            // Play pickup sound
            if (pickupSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }
            
            // Enable glint effect
            if (glintEffect != null)
            {
                glintEffect.Play();
            }
            
            Debug.Log("Picked up magnifying glass");
            
            // Notify UI system
            ScienceLabUI uiSystem = FindFirstObjectByType<ScienceLabUI>();
            if (uiSystem != null)
            {
                uiSystem.OnMagnifyingGlassUsed();
            }
        }
        
        /// <summary>
        /// Put down the magnifying glass
        /// </summary>
        public void PutDown()
        {
            isBeingUsed = false;
            SetActive(false);
            
            // Return to original position
            StartCoroutine(ReturnToOriginalPosition());
            
            // Play putdown sound
            if (putdownSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(putdownSound);
            }
            
            // Stop glint effect
            if (glintEffect != null)
            {
                glintEffect.Stop();
            }
            
            Debug.Log("Put down magnifying glass");
        }
        
        /// <summary>
        /// Toggle active state
        /// </summary>
        public void ToggleActive()
        {
            if (isActive)
            {
                PutDown();
            }
            else
            {
                PickUp();
            }
        }
        
        /// <summary>
        /// Set active state
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
            
            // Enable/disable magnification camera
            if (magnificationCamera != null)
            {
                magnificationCamera.enabled = active;
            }
            
            // Enable/disable lens light
            if (lensLight != null)
            {
                lensLight.enabled = active;
            }
            
            // Update lens material
            if (lensMaterial != null)
            {
                lensMaterial.SetFloat("_Magnification", active ? magnificationPower : 1f);
            }
        }
        
        /// <summary>
        /// Follow mouse position in world space
        /// </summary>
        private void FollowMousePosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Vector3.Distance(mainCamera.transform.position, transform.position);
            
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.y = Mathf.Max(worldPos.y, 0.2f); // Keep above table
            
            targetPosition = worldPos;
            
            // Rotate to face the surface being magnified
            Vector3 lookDirection = (worldPos - mainCamera.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Follow a specific target transform
        /// </summary>
        private void FollowTarget()
        {
            if (targetTransform == null) return;
            
            Vector3 offset = Vector3.up * 0.3f; // Hover above target
            targetPosition = targetTransform.position + offset;
            
            // Look at target
            Vector3 lookDirection = (targetTransform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        /// <summary>
        /// Update magnification effect
        /// </summary>
        private void UpdateMagnification()
        {
            if (magnificationCamera == null) return;
            
            // Position magnification camera to look through the lens
            Vector3 lensPosition = lensGlass != null ? lensGlass.position : transform.position;
            Vector3 lensForward = lensGlass != null ? lensGlass.forward : transform.forward;
            
            magnificationCamera.transform.position = lensPosition - lensForward * 0.1f;
            magnificationCamera.transform.rotation = Quaternion.LookRotation(lensForward);
            
            // Adjust field of view for magnification
            float baseFOV = mainCamera.fieldOfView;
            magnificationCamera.fieldOfView = baseFOV / magnificationPower;
            
            // Update render texture on lens material
            if (lensMaterial != null && magnificationTexture != null)
            {
                lensMaterial.SetTexture("_MagnificationTex", magnificationTexture);
            }
        }
        
        /// <summary>
        /// Set up the magnification camera
        /// </summary>
        private void SetupMagnificationCamera()
        {
            if (magnificationCamera == null)
            {
                // Create magnification camera
                GameObject camGO = new GameObject("MagnificationCamera");
                camGO.transform.SetParent(transform);
                magnificationCamera = camGO.AddComponent<Camera>();
                
                // Configure camera
                magnificationCamera.enabled = false;
                magnificationCamera.cullingMask = magnificationLayers;
                magnificationCamera.clearFlags = CameraClearFlags.Color;
                magnificationCamera.backgroundColor = Color.clear;
                magnificationCamera.renderingPath = RenderingPath.Forward;
            }
            
            // Create render texture
            if (magnificationTexture == null)
            {
                magnificationTexture = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
                magnificationTexture.name = "MagnificationTexture";
                magnificationCamera.targetTexture = magnificationTexture;
            }
        }
        
        /// <summary>
        /// Set up the lens material
        /// </summary>
        private void SetupLensMaterial()
        {
            if (lensMaterial == null && lensGlass != null)
            {
                Renderer lensRenderer = lensGlass.GetComponent<Renderer>();
                if (lensRenderer != null)
                {
                    lensMaterial = lensRenderer.material;
                }
            }
            
            if (lensMaterial == null)
            {
                // Create a basic magnification material
                lensMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                lensMaterial.SetFloat("_Surface", 1f); // Transparent
                lensMaterial.SetFloat("_Blend", 0f);   // Alpha blend
                lensMaterial.SetColor("_BaseColor", new Color(0.9f, 0.9f, 1f, 0.1f));
                
                if (lensGlass != null)
                {
                    Renderer lensRenderer = lensGlass.GetComponent<Renderer>();
                    if (lensRenderer != null)
                    {
                        lensRenderer.material = lensMaterial;
                    }
                }
            }
        }
        
        /// <summary>
        /// Return to original position smoothly
        /// </summary>
        private System.Collections.IEnumerator ReturnToOriginalPosition()
        {
            float duration = 1f;
            float elapsedTime = 0f;
            
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            
            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                progress = Mathf.SmoothStep(0f, 1f, progress);
                
                transform.position = Vector3.Lerp(startPos, originalPosition, progress);
                transform.rotation = Quaternion.Slerp(startRot, originalRotation, progress);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
        
        /// <summary>
        /// Set magnification power
        /// </summary>
        public void SetMagnificationPower(float power)
        {
            magnificationPower = Mathf.Clamp(power, 1f, 10f);
            
            if (lensMaterial != null)
            {
                lensMaterial.SetFloat("_Magnification", isActive ? magnificationPower : 1f);
            }
        }
        
        /// <summary>
        /// Set target to follow
        /// </summary>
        public void SetTarget(Transform target)
        {
            targetTransform = target;
            followMouse = (target == null);
        }
        
        /// <summary>
        /// Get current magnification power
        /// </summary>
        public float GetMagnificationPower()
        {
            return magnificationPower;
        }
        
        /// <summary>
        /// Check if magnifying glass is active
        /// </summary>
        public bool IsActive()
        {
            return isActive;
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw lens radius
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, lensRadius);
            
            // Draw magnification view cone
            if (isActive && magnificationCamera != null)
            {
                Gizmos.color = Color.yellow;
                Vector3 forward = transform.forward;
                Vector3 right = transform.right * lensRadius;
                Vector3 up = transform.up * lensRadius;
                
                Gizmos.DrawRay(transform.position, forward * 0.5f);
                Gizmos.DrawRay(transform.position + forward * 0.5f, right * 0.5f);
                Gizmos.DrawRay(transform.position + forward * 0.5f, -right * 0.5f);
                Gizmos.DrawRay(transform.position + forward * 0.5f, up * 0.5f);
                Gizmos.DrawRay(transform.position + forward * 0.5f, -up * 0.5f);
            }
        }
        
        void OnDestroy()
        {
            // Clean up render texture
            if (magnificationTexture != null)
            {
                magnificationTexture.Release();
            }
        }
    }
}
