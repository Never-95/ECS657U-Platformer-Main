using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip[] footstepSounds;
    public AudioSource audioSource;
    
    [Header("Footstep Timing")]
    public float baseStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    public float minimumSpeed = 0.5f;  // Minimum speed to trigger footsteps
    
    [Header("Volume")]
    public float footstepVolume = 0.5f;
    
    private PlayerController playerController;
    private Rigidbody rb;
    private float stepTimer = 0f;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        
        // Create audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }
    
    void Update()
    {
        // Only play footsteps when grounded
        if (playerController != null && playerController.isGrounded)
        {
            // Get horizontal velocity (movement speed)
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            float currentSpeed = horizontalVelocity.magnitude;
            
            // Check if player is moving fast enough
            if (currentSpeed > minimumSpeed)
            {
                stepTimer += Time.deltaTime;
                
                // Determine step interval based on speed
                float currentInterval = baseStepInterval;
                
                // Faster steps when speed boosted or running
                if (currentSpeed > playerController.baseMoveSpeed * 1.5f)
                {
                    currentInterval = runStepInterval;
                }
                
                // Play footstep at intervals
                if (stepTimer >= currentInterval)
                {
                    PlayFootstepSound();
                    stepTimer = 0f;
                }
            }
            else
            {
                // Reset timer when standing still
                stepTimer = 0f;
            }
        }
        else
        {
            // Reset timer when in air
            stepTimer = 0f;
        }
    }
    
    private void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0 || audioSource == null) return;
        
        // Pick random footstep sound for variety
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        
        // Play sound
        audioSource.PlayOneShot(clip, footstepVolume);
    }
}