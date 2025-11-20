using System.Collections;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    [Header("Crumble Settings")]
    public float crumbleDelay = 3f;      // Time before platform disappears
    public float respawnDelay = 3f;      // Time before platform reappears
    public bool respawnPlatform = true;  // Should it respawn?
    
    [Header("Visual Feedback")]
    public bool shakeBeforeCrumble = true;
    public float shakeIntensity = 0.1f;
    public bool fadeOut = true;
    public Color warningColor = Color.red;
    
    private Vector3 originalPosition;
    private Color originalColor;
    private bool isCrumbling = false;
    private Collider platformCollider;
    private Renderer platformRenderer;
    private Material platformMaterial;

    void Start()
    {
        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<Renderer>();
        
        originalPosition = transform.position;
        
        // Store original material color
        if (platformRenderer != null)
        {
            // Create a unique material instance to avoid affecting other platforms
            platformMaterial = new Material(platformRenderer.material);
            platformRenderer.material = platformMaterial;
            originalColor = platformMaterial.color;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if player stepped on platform
        if (collision.gameObject.CompareTag("Player") && !isCrumbling)
        {
            StartCoroutine(CrumbleSequence());
        }
    }

    private IEnumerator CrumbleSequence()
    {
        isCrumbling = true;
        
        // Visual warning phase
        if (shakeBeforeCrumble || fadeOut)
        {
            float timer = 0f;
            
            while (timer < crumbleDelay)
            {
                float progress = timer / crumbleDelay;
                
                // Shake effect
                if (shakeBeforeCrumble)
                {
                    // Shake intensity increases as it gets closer to crumbling
                    float currentShake = shakeIntensity * progress;
                    transform.position = originalPosition + new Vector3(
                        Random.Range(-currentShake, currentShake),
                        Random.Range(-currentShake, currentShake),
                        Random.Range(-currentShake, currentShake)
                    );
                }
                
                // Fade/color change effect
                if (fadeOut && platformMaterial != null)
                {
                    // Lerp between original color and warning color
                    platformMaterial.color = Color.Lerp(originalColor, warningColor, progress);
                    
                    // Optional: Also fade alpha
                    Color currentColor = platformMaterial.color;
                    currentColor.a = Mathf.Lerp(1f, 0.3f, progress);
                    platformMaterial.color = currentColor;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Reset position
            transform.position = originalPosition;
        }
        else
        {
            // Just wait without effects
            yield return new WaitForSeconds(crumbleDelay);
        }
        
        // Make platform untouchable
        DisablePlatform();
        
        // Wait before respawning
        yield return new WaitForSeconds(respawnDelay);
        
        // Respawn platform if enabled
        if (respawnPlatform)
        {
            EnablePlatform();
        }
    }

    private void DisablePlatform()
    {
        // Disable collision
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }
        
        // Make invisible or transparent
        if (platformRenderer != null && platformMaterial != null)
        {
            Color invisible = platformMaterial.color;
            invisible.a = 0f; // Fully transparent
            platformMaterial.color = invisible;
        }
    }

    private void EnablePlatform()
    {
        // Reset position
        transform.position = originalPosition;
        
        // Re-enable collision
        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }
        
        // Restore visibility and color
        if (platformRenderer != null && platformMaterial != null)
        {
            platformMaterial.color = originalColor;
        }
        
        isCrumbling = false;
    }

    // Optional: Draw gizmo to visualize crumbling platforms
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}