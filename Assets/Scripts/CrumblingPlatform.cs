using System.Collections;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    [Header("Crumble Settings")]
    public float crumbleDelay = 3f;
    public float respawnDelay = 3f;
    public bool respawnPlatform = true;
    
    [Header("Visual Feedback")]
    public bool shakeBeforeCrumble = true;
    public float shakeIntensity = 0.1f;
    public bool fadeOut = true;
    public Color warningColor = Color.red;
    
    [Header("Audio")]
    public AudioClip crackSound;        // When player steps on (warning)
    public AudioClip crumbleSound;      // When platform breaks
    public float crackVolume = 0.6f;
    public float crumbleVolume = 0.8f;
    
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
        
        if (platformRenderer != null)
        {
            platformMaterial = new Material(platformRenderer.material);
            platformRenderer.material = platformMaterial;
            originalColor = platformMaterial.color;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isCrumbling)
        {
            // Play crack/warning sound immediately
            if (crackSound != null)
            {
                AudioSource.PlayClipAtPoint(crackSound, transform.position, crackVolume);
            }
            
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
                    platformMaterial.color = Color.Lerp(originalColor, warningColor, progress);
                    
                    Color currentColor = platformMaterial.color;
                    currentColor.a = Mathf.Lerp(1f, 0.3f, progress);
                    platformMaterial.color = currentColor;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            transform.position = originalPosition;
        }
        else
        {
            yield return new WaitForSeconds(crumbleDelay);
        }
        
        // Play crumble sound when it breaks
        if (crumbleSound != null)
        {
            AudioSource.PlayClipAtPoint(crumbleSound, transform.position, crumbleVolume);
        }
        
        // Make platform untouchable
        DisablePlatform();
        
        yield return new WaitForSeconds(respawnDelay);
        
        if (respawnPlatform)
        {
            EnablePlatform();
        }
    }
    
    private void DisablePlatform()
    {
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
        }
        
        if (platformRenderer != null && platformMaterial != null)
        {
            Color invisible = platformMaterial.color;
            invisible.a = 0f;
            platformMaterial.color = invisible;
        }
    }
    
    private void EnablePlatform()
    {
        transform.position = originalPosition;
        
        if (platformCollider != null)
        {
            platformCollider.enabled = true;
        }
        
        if (platformRenderer != null && platformMaterial != null)
        {
            platformMaterial.color = originalColor;
        }
        
        isCrumbling = false;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}