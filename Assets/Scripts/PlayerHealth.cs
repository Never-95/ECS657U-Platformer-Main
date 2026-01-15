using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBarSlider;
    
    [Header("Oxygen Settings")]
    public float maxOxygen = 60f;
    public float currentOxygen;
    public float oxygenDepletionRate = 5f;
    public float oxygenDamageRate = 5f;
    public Slider oxygenBarSlider;
    
    [Header("Damage Settings")]
    public float damageAmount = 20f;
    private float oxygenDepletionTimer = 0f;
    private bool oxygenDepleted = false;
    public float damageCooldown = 0.5f;
    private float lastDamageTime;
    
    [Header("Audio Settings")]
    public AudioClip hitSound;              // Single hit/cry sound
    public AudioClip oxygenBurstSound;      // Sound when oxygen runs out
    public AudioClip suffocationSound;      // Continuous pain sound (looping)
    public float hitVolume = 0.7f;
    public float suffocationVolume = 0.5f;
    
    private AudioSource audioSource;
    private AudioSource suffocationAudioSource;  // Separate source for looping sound
    
    void Start()
    {
        currentHealth = maxHealth;
        currentOxygen = maxOxygen;
        UpdateHealthBar();
        UpdateOxygenBar();
        
        // Create main audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Create separate audio source for continuous suffocation sound
        suffocationAudioSource = gameObject.AddComponent<AudioSource>();
        suffocationAudioSource.playOnAwake = false;
        suffocationAudioSource.loop = true;  // This will loop continuously
        suffocationAudioSource.volume = suffocationVolume;
    }
    
    void Update()
    {
        oxygenDepletionTimer += Time.deltaTime;
        
        if (oxygenDepletionTimer >= oxygenDepletionRate)
        {
            currentOxygen -= oxygenDepletionRate;
            oxygenDepletionTimer = 0f;
            
            if (currentOxygen <= 0)
            {
                currentOxygen = 0;
                if (!oxygenDepleted)
                {
                    oxygenDepleted = true;
                    Debug.Log("Oxygen depleted! Starting to take damage.");
                    
                    // Play oxygen burst sound
                    if (oxygenBurstSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(oxygenBurstSound, 0.8f);
                    }
                    
                    // Start continuous suffocation sound
                    if (suffocationSound != null && suffocationAudioSource != null)
                    {
                        suffocationAudioSource.clip = suffocationSound;
                        suffocationAudioSource.Play();
                    }
                }
            }
            UpdateOxygenBar();
        }
        
        if (oxygenDepleted)
        {
            TakeOxygenDamage(oxygenDamageRate * Time.deltaTime);
        }
    }
    
    // Oxygen damage - no cooldown, applies every frame
    private void TakeOxygenDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    
    // Enemy/external damage - has cooldown to prevent spam
    public void TakeDamage(float amount)
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            lastDamageTime = Time.time;
            currentHealth -= amount;
            Debug.Log("Player took " + amount + " damage. Current health: " + currentHealth);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();
            
            // Play hit/cry sound
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound, hitVolume);
            }
            
            if (currentHealth <= 0f)
            {
                Die();
            }
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();
        Debug.Log("Player healed by " + amount + " points.");
    }
    
    public void RestoreOxygen(float amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);
        
        if (currentOxygen > 0f)
        {
            oxygenDepleted = false;
            
            // Stop suffocation sound when oxygen restored
            if (suffocationAudioSource != null && suffocationAudioSource.isPlaying)
            {
                suffocationAudioSource.Stop();
            }
        }
        
        UpdateOxygenBar();
        Debug.Log("Oxygen restored by " + amount + " seconds.");
    }
    
    public void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth / maxHealth;
        }
    }
    
    public void UpdateOxygenBar()
    {
        if (oxygenBarSlider != null)
        {
            oxygenBarSlider.value = currentOxygen / maxOxygen;
        }
    }
    
    public void Die()
    {
        Debug.Log("Player has died.");
        
        // Stop suffocation sound on death
        if (suffocationAudioSource != null && suffocationAudioSource.isPlaying)
        {
            suffocationAudioSource.Stop();
        }
        
        StartCoroutine(RespawnDelay());
    }
    
    private System.Collections.IEnumerator RespawnDelay()
    {
        // Disable player controls during respawn
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = false;
        }
        
        yield return new WaitForSeconds(2f);  // 2 second delay
        
        // Re-enable controls
        if (pc != null)
        {
            pc.enabled = true;
        }
        
        // Reset oxygen status
        oxygenDepleted = false;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}