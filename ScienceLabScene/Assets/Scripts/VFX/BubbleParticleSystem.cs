using UnityEngine;

namespace ScienceLabScene.VFX
{
    /// <summary>
    /// Controller for bubble particle effects in liquid containers
    /// Manages bubble emission based on liquid properties and interactions
    /// </summary>
    public class BubbleParticleSystem : MonoBehaviour
    {
        [Header("Particle System References")]
        public ParticleSystem bubbleParticles;
        public ParticleSystem steamParticles; // Optional steam effect
        
        [Header("Bubble Properties")]
        [Range(0f, 50f)]
        public float emissionRate = 10f;
        [Range(0.002f, 0.01f)]
        public float minBubbleSize = 0.002f; // 0.2cm
        [Range(0.005f, 0.01f)]
        public float maxBubbleSize = 0.01f;  // 1cm
        [Range(0.5f, 1f)]
        public float bubbleAlpha = 0.7f;
        
        [Header("Emission Settings")]
        public bool emitFromSurface = true;
        public float surfaceOffset = 0.01f;
        public Vector3 emissionAreaSize = new Vector3(0.1f, 0.01f, 0.1f);
        
        [Header("Liquid Properties")]
        public float liquidLevel = 0f; // 0-1 range
        public float liquidTemperature = 20f; // Celsius
        public bool isReacting = false;
        public LiquidType liquidType = LiquidType.Water;
        
        [Header("Interaction Triggers")]
        public bool bubbleOnStir = true;
        public bool bubbleOnPour = true;
        public bool bubbleOnHeat = true;
        public float stirBubbleMultiplier = 2f;
        public float heatBubbleMultiplier = 1.5f;
        
        private ParticleSystem.EmissionModule emission;
        private ParticleSystem.ShapeModule shape;
        private ParticleSystem.MainModule main;
        private ParticleSystem.SizeOverLifetimeModule sizeOverLifetime;
        private ParticleSystem.ColorOverLifetimeModule colorOverLifetime;
        
        private float baseEmissionRate;
        private bool isInitialized = false;
        private Transform liquidSurface;
        
        public enum LiquidType
        {
            Water,
            Oil,
            Acid,
            Base,
            Soap,
            Carbonated
        }
        
        void Start()
        {
            InitializeParticleSystem();
        }
        
        void Update()
        {
            UpdateEmissionPosition();
            UpdateEmissionRate();
        }
        
        /// <summary>
        /// Initialize the particle system with optimized settings
        /// </summary>
        void InitializeParticleSystem()
        {
            if (bubbleParticles == null)
            {
                CreateDefaultParticleSystem();
            }
            
            // Get particle system modules
            main = bubbleParticles.main;
            emission = bubbleParticles.emission;
            shape = bubbleParticles.shape;
            sizeOverLifetime = bubbleParticles.sizeOverLifetime;
            colorOverLifetime = bubbleParticles.colorOverLifetime;
            
            // Configure main module
            main.startLifetime = new ParticleSystem.MinMaxCurve(2f, 4f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.15f);
            main.startSize = new ParticleSystem.MinMaxCurve(minBubbleSize, maxBubbleSize);
            main.startColor = new Color(1f, 1f, 1f, bubbleAlpha);
            main.maxParticles = 100;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = -0.1f; // Slight upward force
            
            // Configure emission
            emission.rateOverTime = emissionRate;
            baseEmissionRate = emissionRate;
            
            // Configure shape for surface emission
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = emissionAreaSize;
            
            // Configure size over lifetime (bubbles grow slightly as they rise)
            sizeOverLifetime.enabled = true;
            AnimationCurve sizeCurve = new AnimationCurve();
            sizeCurve.AddKey(0f, 0.8f);  // Start smaller
            sizeCurve.AddKey(0.5f, 1f);  // Grow to full size
            sizeCurve.AddKey(1f, 1.2f);  // Slightly larger before popping
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
            
            // Configure color/alpha fade
            colorOverLifetime.enabled = true;
            Gradient colorGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
            
            colorKeys[0] = new GradientColorKey(Color.white, 0f);
            colorKeys[1] = new GradientColorKey(Color.white, 1f);
            
            alphaKeys[0] = new GradientAlphaKey(0f, 0f);      // Fade in
            alphaKeys[1] = new GradientAlphaKey(bubbleAlpha, 0.2f); // Full alpha
            alphaKeys[2] = new GradientAlphaKey(0f, 1f);      // Fade out
            
            colorGradient.SetKeys(colorKeys, alphaKeys);
            colorOverLifetime.color = colorGradient;
            
            // Add velocity over lifetime for realistic bubble movement
            var velocityOverLifetime = bubbleParticles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
            
            // Add slight random movement
            AnimationCurve randomMovement = AnimationCurve.Linear(0f, 0f, 1f, 0.02f);
            velocityOverLifetime.x = new ParticleSystem.MinMaxCurve(-0.01f, randomMovement);
            velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(-0.01f, randomMovement);
            
            // Configure collision for realistic bubble behavior
            var collision = bubbleParticles.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.dampen = 0.5f;
            collision.bounce = 0.3f;
            collision.lifetimeLoss = 0.1f;
            
            isInitialized = true;
            
            Debug.Log("Bubble particle system initialized");
        }
        
        /// <summary>
        /// Create default particle system if none assigned
        /// </summary>
        void CreateDefaultParticleSystem()
        {
            GameObject particleGO = new GameObject("BubbleParticles");
            particleGO.transform.SetParent(transform);
            particleGO.transform.localPosition = Vector3.zero;
            
            bubbleParticles = particleGO.AddComponent<ParticleSystem>();
            
            // Set default material (will be replaced with bubble sprite material)
            var renderer = bubbleParticles.GetComponent<ParticleSystemRenderer>();
            renderer.material = CreateDefaultBubbleMaterial();
        }
        
        /// <summary>
        /// Create default bubble material
        /// </summary>
        Material CreateDefaultBubbleMaterial()
        {
            Material bubbleMat = new Material(Shader.Find("Universal Render Pipeline/Particles/Lit"));
            bubbleMat.name = "DefaultBubbleMaterial";
            
            // Configure for transparent bubbles
            bubbleMat.SetFloat("_Surface", 1f); // Transparent
            bubbleMat.SetFloat("_Blend", 0f);   // Alpha blend
            bubbleMat.SetColor("_BaseColor", new Color(0.9f, 0.95f, 1f, 0.7f));
            bubbleMat.SetFloat("_Smoothness", 0.9f);
            bubbleMat.SetFloat("_Metallic", 0f);
            
            return bubbleMat;
        }
        
        /// <summary>
        /// Update emission position based on liquid surface
        /// </summary>
        void UpdateEmissionPosition()
        {
            if (!emitFromSurface || !isInitialized) return;
            
            // Calculate surface position based on liquid level
            Vector3 surfacePosition = transform.position;
            surfacePosition.y += (liquidLevel * 0.3f) + surfaceOffset; // Adjust based on container height
            
            // Update particle system position
            bubbleParticles.transform.position = surfacePosition;
        }
        
        /// <summary>
        /// Update emission rate based on liquid properties and interactions
        /// </summary>
        void UpdateEmissionRate()
        {
            if (!isInitialized) return;
            
            float currentRate = baseEmissionRate;
            
            // Modify rate based on liquid type
            switch (liquidType)
            {
                case LiquidType.Carbonated:
                    currentRate *= 3f;
                    break;
                case LiquidType.Soap:
                    currentRate *= 2f;
                    break;
                case LiquidType.Acid:
                case LiquidType.Base:
                    if (isReacting)
                        currentRate *= 2.5f;
                    break;
            }
            
            // Modify rate based on temperature
            if (liquidTemperature > 60f) // Hot liquid
            {
                currentRate *= heatBubbleMultiplier;
            }
            
            // Apply rate with smooth transition
            float targetRate = liquidLevel > 0.1f ? currentRate : 0f;
            emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, targetRate, Time.deltaTime * 2f);
        }
        
        /// <summary>
        /// Trigger bubble burst when stirring
        /// </summary>
        public void OnStirring(bool isStirring)
        {
            if (!bubbleOnStir || !isInitialized) return;
            
            if (isStirring)
            {
                // Increase emission rate during stirring
                emission.rateOverTime = baseEmissionRate * stirBubbleMultiplier;
                
                // Add burst of bubbles
                bubbleParticles.Emit(5);
            }
            else
            {
                // Return to normal rate
                emission.rateOverTime = baseEmissionRate;
            }
        }
        
        /// <summary>
        /// Trigger bubbles when pouring liquid
        /// </summary>
        public void OnPouring(bool isPouring)
        {
            if (!bubbleOnPour || !isInitialized) return;
            
            if (isPouring)
            {
                // Create splash bubbles
                bubbleParticles.Emit(10);
            }
        }
        
        /// <summary>
        /// Set liquid properties
        /// </summary>
        public void SetLiquidProperties(float level, LiquidType type, float temperature = 20f)
        {
            liquidLevel = Mathf.Clamp01(level);
            liquidType = type;
            liquidTemperature = temperature;
            
            // Update bubble appearance based on liquid type
            UpdateBubbleAppearance();
        }
        
        /// <summary>
        /// Update bubble appearance based on liquid type
        /// </summary>
        void UpdateBubbleAppearance()
        {
            if (!isInitialized) return;
            
            Color bubbleColor = Color.white;
            float bubbleSize = 1f;
            
            switch (liquidType)
            {
                case LiquidType.Soap:
                    bubbleColor = new Color(1f, 1f, 1f, 0.9f);
                    bubbleSize = 1.5f; // Larger soap bubbles
                    break;
                case LiquidType.Acid:
                    bubbleColor = new Color(1f, 0.9f, 0.8f, 0.6f);
                    break;
                case LiquidType.Base:
                    bubbleColor = new Color(0.8f, 0.9f, 1f, 0.6f);
                    break;
                case LiquidType.Oil:
                    bubbleColor = new Color(1f, 1f, 0.8f, 0.4f);
                    bubbleSize = 0.7f; // Smaller oil bubbles
                    break;
                case LiquidType.Carbonated:
                    bubbleColor = new Color(1f, 1f, 1f, 0.8f);
                    bubbleSize = 0.5f; // Small CO2 bubbles
                    break;
            }
            
            main.startColor = bubbleColor;
            main.startSize = new ParticleSystem.MinMaxCurve(
                minBubbleSize * bubbleSize, 
                maxBubbleSize * bubbleSize
            );
        }
        
        /// <summary>
        /// Start chemical reaction bubbling
        /// </summary>
        public void StartReaction(float intensity = 1f)
        {
            isReacting = true;
            
            if (isInitialized)
            {
                emission.rateOverTime = baseEmissionRate * intensity * 3f;
                bubbleParticles.Emit((int)(20 * intensity));
            }
        }
        
        /// <summary>
        /// Stop chemical reaction bubbling
        /// </summary>
        public void StopReaction()
        {
            isReacting = false;
            
            if (isInitialized)
            {
                emission.rateOverTime = baseEmissionRate;
            }
        }
        
        /// <summary>
        /// Enable or disable bubble emission
        /// </summary>
        public void SetEmissionEnabled(bool enabled)
        {
            if (isInitialized)
            {
                emission.enabled = enabled;
            }
        }
        
        /// <summary>
        /// Get current emission rate
        /// </summary>
        public float GetCurrentEmissionRate()
        {
            return isInitialized ? emission.rateOverTime.constant : 0f;
        }
        
        void OnDrawGizmosSelected()
        {
            // Draw emission area
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.up * surfaceOffset, emissionAreaSize);
            
            // Draw liquid surface level
            if (liquidLevel > 0)
            {
                Gizmos.color = Color.blue;
                Vector3 surfacePos = Vector3.up * (liquidLevel * 0.3f);
                Gizmos.DrawWireCube(surfacePos, new Vector3(0.2f, 0.01f, 0.2f));
            }
        }
    }
}
